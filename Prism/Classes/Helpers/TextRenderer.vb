''' <summary>
''' Renders formatted text using specialized HTML-like code.
''' </summary>
''' <remarks>
''' <list type="bullet">
''' <listheader><description>The supported tags are:</description></listheader>
''' <item><description>&lt;foreColor:AAA,RRR,GGG,BBB&gt;</description></item>
''' <item><description>&lt;backColor:AAA,RRR,GGG,BBB&gt;</description></item>
''' <item><description>&lt;font:FAMILY,SIZE,STYLE&gt;</description></item>
''' <item><description>&lt;fontSize:SIZE_PX&gt;</description></item>
''' <item><description>&lt;bold&gt;</description></item>
''' <item><description>&lt;italic&gt;</description></item>
''' <item><description>&lt;image:SOURCE,WIDTH,HEIGHT,OFFSET_X,OFFSET_Y&gt;</description></item>
''' <item><description>&lt;offset:X,Y&gt;</description></item>
''' </list>
''' </remarks>
Public Class TextRenderer
    Private selFont As List(Of Font) = New List(Of Font)
    Private selForeColor As List(Of Brush2) = New List(Of Brush2)
    Private selBackColor As List(Of Brush2) = New List(Of Brush2)
    Private selOffset As List(Of Point) = New List(Of Point)

    Private text As String
    Private right As Integer
    Private bottom As Integer
    Private offsetX As Integer
    Private offsetY As Integer
    Private lineIndex As Integer
    Private linesWidth As New Dictionary(Of Integer, Integer)
    Private mTextSize As Size
    Private stringFormat As Drawing.StringFormat = New Drawing.StringFormat(stringFormat.GenericTypographic)
    Private keyFrame As Project.Track.Layer.Element.KeyFrame
    Private cachedKeyFrame As Project.Track.Layer.Element.KeyFrame
    Private cachedCode As String

    Private rscSize As New Size(-1, -1)
    Private g As Graphics
    Private mBitmap As Bitmap = Nothing

    Public Sub New()
        stringFormat.FormatFlags = StringFormatFlags.LineLimit Or StringFormatFlags.NoWrap Or StringFormatFlags.MeasureTrailingSpaces
    End Sub

    Private Enum TokenType
        Text
        ForeColor
        BackColor
        Font
        Bold
        Italic
        FontSize
        Image
        Offset
        NewLine
        CarriageReturn
    End Enum

    Private Class Token
        Private mType As TokenType = TokenType.Text
        Private mParams As List(Of String) = New List(Of String)
        Private mContents As String = ""
        Private mIsValid As Boolean = False

        Public Sub New(code As String)
            ParseToken(code, code.ToLower())
        End Sub

        Public Shared Function ParseCode(code As String) As List(Of Token)
            Dim tokens As List(Of Token) = New List(Of Token)
            Dim buffer As String = ""

            Dim codeLower As String = code.ToLower()

            Try
                For p As Integer = 0 To code.Length - 1
                    If code.Substring(p, 1) = "<" Then
                        If buffer <> "" Then
                            tokens.Add(New Token(buffer))
                            buffer = ""
                        End If
                        Dim instruction As String = GetInstructionName(codeLower.Substring(p))
                        Dim p1 As Integer = codeLower.IndexOf("</" + instruction + ">", p) + instruction.Length + 3
                        tokens.Add(New Token(code.Substring(p, p1 - p)))
                        p = p1 - 1
                    Else
                        buffer += code.Substring(p, 1)
                    End If
                Next

                If buffer <> "" Then tokens.Add(New Token(buffer))
            Catch
            End Try

            Return tokens
        End Function

        Private Sub ParseToken(code As String, codeLower As String)
            Try
                Dim p1 As Integer
                Dim p2 As Integer

                If code.StartsWith("<") Then
                    Dim instruction As String = GetInstructionName(codeLower)
                    Select Case instruction
                        Case "forecolor" : mType = TokenType.ForeColor
                        Case "backcolor" : mType = TokenType.BackColor
                        Case "fontsize" : mType = TokenType.FontSize
                        Case "font" : mType = TokenType.Font
                        Case "bold" : mType = TokenType.Bold
                        Case "italic" : mType = TokenType.Italic
                        Case "image" : mType = TokenType.Image
                        Case "offset" : mType = TokenType.Offset
                    End Select

                    p1 = code.IndexOf(":") + 1
                    If p1 > instruction.Length Then
                        p2 = code.IndexOf(">", p1)
                        mParams = code.Substring(p1, p2 - p1).Split(",").ToList()
                    End If

                    p1 = code.IndexOf(">") + 1
                    p2 = codeLower.IndexOf("</" + instruction + ">", p1)
                    mContents = code.Substring(p1, p2 - p1)
                Else
                    mType = TokenType.Text
                    mContents = code
                End If

                'If mContents.Contains("<") Then mSubTokens = Token.ParseCode(mContents)

                mIsValid = True
            Catch
                mIsValid = False
            End Try
        End Sub

        Private Shared Function GetInstructionName(code As String) As String
            Dim p1 As Integer = code.IndexOf(":") - 1
            Dim p2 As Integer = code.IndexOf(">") - 1
            If p2 < p1 OrElse p1 = -2 Then p1 = p2
            Return code.Substring(1, p1)
        End Function

        Public ReadOnly Property Type As TokenType
            Get
                Return mType
            End Get
        End Property

        Public ReadOnly Property Params As List(Of String)
            Get
                Return mParams
            End Get
        End Property

        Public ReadOnly Property Contents As String
            Get
                Return mContents
            End Get
        End Property

        'Public ReadOnly Property SubTokens As List(Of Token)
        '    Get
        '        Return mSubTokens
        '    End Get
        'End Property
    End Class

    Public ReadOnly Property TextSize As Size
        Get
            Return mTextSize
        End Get
    End Property

    Public Sub ClearCache()
        cachedKeyFrame = Nothing
        cachedCode = ""
    End Sub

    Public Function Render(code As String, keyFrame As Project.Track.Layer.Element.KeyFrame) As Bitmap
        'If code = cachedCode AndAlso cachedKeyFrame IsNot Nothing AndAlso Project.Track.Layer.Element.KeyFrame.IsEqual(keyFrame, cachedKeyFrame, True) Then
        '    Return mBitmap
        'Else
        '    cachedKeyFrame = keyFrame
        '    cachedCode = code
        'End If

        Dim location As Point = Point.Empty
        Me.keyFrame = keyFrame

        right = 0
        bottom = 0
        offsetX = 0
        offsetY = 0

        selFont.Clear()
        selFont.Add(keyFrame.Font)
        selForeColor.Clear()
        selForeColor.Add(keyFrame.ForeColor)
        selBackColor.Clear()
        selBackColor.Add(keyFrame.BackColor)
        selOffset.Clear()
        selOffset.Add(New Point(0, 0))

        linesWidth.Clear()

        Try
            CreateResources()
            RenderCode(g, code, location, True)
            mTextSize = New Size(right - location.X, bottom - location.Y)

            CreateResources()
            g.Clear(Color.Transparent)
            RenderCode(g, code, location, False)
        Catch
        End Try

        Return mBitmap
    End Function

    Private Sub CreateResources()
        If rscSize <> mTextSize Then
            rscSize.Width = If(mTextSize.Width = 0, 1, mTextSize.Width)
            rscSize.Height = If(mTextSize.Height = 0, 1, mTextSize.Height)

            If mBitmap IsNot Nothing Then
                mBitmap.Dispose()
                g.Dispose()
            End If

            mBitmap = New Bitmap(rscSize.Width + Math.Abs(offsetX), rscSize.Height + Math.Abs(offsetY), Imaging.PixelFormat.Format32bppArgb)
            g = Graphics.FromImage(mBitmap)
            g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
        End If
    End Sub

    Private Function RenderCode(g As Graphics, code As String, p As Point, calcSize As Boolean) As Point
        Dim tokens As List(Of Token) = Token.ParseCode(code)
        Dim isFirst As Boolean = True
        Dim offset As Point = Point.Empty

        lineIndex = 0

        For Each token In tokens
            p = RenderToken(g, token, p, calcSize, isFirst)
            If token.Contents.Contains("<") Then
                p = RenderCode(g, token.Contents, p, calcSize)
            End If

            Select Case token.Type
                Case TokenType.Bold, TokenType.Italic, TokenType.Font, TokenType.FontSize
                    selFont.Remove(selFont.Last())
                Case TokenType.ForeColor
                    selForeColor.Remove(selForeColor.Last())
                Case TokenType.BackColor
                    selBackColor.Remove(selBackColor.Last())
                Case TokenType.Offset
                    'Dim lastOffset As Point = selOffset.Last()
                    'p.X -= lastOffset.X
                    'p.Y -= lastOffset.Y
                    selOffset.Remove(selOffset.Last())
            End Select

            isFirst = False
        Next

        Return p
    End Function

    Private Function RenderToken(g As Graphics, token As Token, p As Point, calcSize As Boolean, isFirst As Boolean) As Point
        Select Case token.Type
            Case TokenType.Bold
                selFont.Add(New Font(selFont.Last(), FontStyle.Bold))
            Case TokenType.Italic
                selFont.Add(New Font(selFont.Last(), FontStyle.Italic))
            Case TokenType.FontSize
                Dim size As Integer = 9
                Integer.TryParse(token.Params(0), size)

                selFont.Add(New Font(selFont.Last().FontFamily, size, selFont.Last().Style, GraphicsUnit.Pixel))
            Case TokenType.Font
                Dim style As Drawing.FontStyle = FontStyle.Regular
                [Enum].TryParse(token.Params(2), style)

                Dim size As Integer = 9
                Integer.TryParse(token.Params(1), size)

                selFont.Add(New Font(token.Params(0), size, style, GraphicsUnit.Pixel))
            Case TokenType.ForeColor
                selForeColor.Add(New Brush2(ParamsToColor(token.Params)))
            Case TokenType.BackColor
                selBackColor.Add(New Brush2(ParamsToColor(token.Params)))
            Case TokenType.Offset
                Dim oX As Integer
                Dim oY As Integer

                Integer.TryParse(token.Params(0), oX)
                Integer.TryParse(token.Params(1), oY)

                selOffset.Add(New Point(oX, oY))
        End Select

        Dim offset As Point = selOffset.Last()

        If Not token.Contents.Contains("<") Then
            Dim x As Integer = 0
            Dim parts() As String = token.Contents.Replace(vbCrLf, vbLf).Split(vbLf)
            For i = 0 To parts.Length - 1
                Dim tokenSize As Size = TextRenderer.MeasureToken(g, parts(i), selFont.Last(), stringFormat)

                If calcSize Then
                    p.X += tokenSize.Width
                Else
                    If selBackColor.Last() <> selBackColor(0) Then
                        Using b As Brush = selBackColor.Last().ToBrush()
                            g.FillRectangle(b, New Rectangle(p, tokenSize))
                        End Using
                    End If

                    Dim xPos As Integer = p.X
                    If isFirst Then
                        Select Case keyFrame.ContentAlignment
                            Case ContentAlignment.BottomLeft, ContentAlignment.MiddleLeft, ContentAlignment.TopLeft
                            Case ContentAlignment.BottomCenter, ContentAlignment.MiddleCenter, ContentAlignment.TopCenter
                                xPos = p.X + right / 2 - linesWidth(lineIndex) / 2
                            Case ContentAlignment.BottomRight, ContentAlignment.MiddleRight, ContentAlignment.TopRight
                                xPos = p.X + right - linesWidth(lineIndex)
                        End Select
                    End If
                    g.DrawString(parts(i), selFont.Last(), selForeColor.Last().ToBrush(), xPos + offset.X, p.Y + offset.Y, stringFormat)

                    p.X = xPos + tokenSize.Width
                End If

                If calcSize Then
                    right = Math.Max(right, p.X + x + offsetX)
                    bottom = Math.Max(bottom, p.Y + tokenSize.Height + offsetY)

                    offsetX = Math.Max(offsetX, offset.X)
                    offsetY = Math.Max(offsetY, offset.Y)

                    If Not linesWidth.ContainsKey(lineIndex) Then
                        linesWidth.Add(lineIndex, p.X + x)
                    Else
                        linesWidth(lineIndex) = Math.Max(linesWidth(lineIndex), p.X + x)
                    End If
                End If

                If i < parts.Length - 1 Then
                    p.X = x
                    p.Y += tokenSize.Height
                    isFirst = True
                    lineIndex += 1
                End If
            Next
        End If

        Return p
    End Function

    Private Function ParamsToColor(params As List(Of String)) As Color
        Dim a As Integer = 255
        Dim r As Integer = 255
        Dim g As Integer = 255
        Dim b As Integer = 255

        Integer.TryParse(params(0), a)
        Integer.TryParse(params(1), r)
        Integer.TryParse(params(2), g)
        Integer.TryParse(params(3), b)

        Return Color.FromArgb(a, r, g, b)
    End Function

    Private Shared Function MeasureToken(g As Graphics, text As String, font As Font, format As StringFormat) As Size
        If text = "" Then
            Dim size As Size = g.MeasureString(" ", font, 0, format).ToSize()
            size.Width = 0
            Return size
        Else
            Return g.MeasureString(text, font, 0, format).ToSize()
        End If

        'Dim rect As RectangleF = New RectangleF(0, 0, 10000, 10000)
        'Dim ranges() As CharacterRange = {New CharacterRange(0, text.Length)}
        'Dim regions() As Region = {New Region()}
        'format.SetMeasurableCharacterRanges(ranges)
        'regions = g.MeasureCharacterRanges(text, font, rect, format)
        'rect = regions(0).GetBounds(g)

        'Return New Size(Math.Ceiling(rect.Right + 1.0F), Math.Ceiling(rect.Bottom))
    End Function
End Class
