Imports System.ComponentModel

Partial Public Class Project
    Partial Public Class Track
        Partial Public Class Layer
            <Serializable()>
            Public Class ElementMarquee
                Inherits ElementText

                Private lines() As String = {}
                Private textRenderers As List(Of TextRenderer) = New List(Of TextRenderer)

                Public Enum DirectionConstants
                    Left
                    Right
                    Up
                    Down
                End Enum

                <CategoryAttribute("Marquee")>
                Public Property Speed As Integer = 120
                <CategoryAttribute("Marquee")>
                Public Property Direction As DirectionConstants
                <CategoryAttribute("Marquee")>
                Public Property SeparationBetweenStrings As Integer = 80

                Private textBitmap As Bitmap = Nothing

                Public Sub New(ByVal layer As Layer)
                    MyBase.New(layer)
                    AddHandler MyBase.TextChanged, Sub(sender As ElementText)
                                                       textRenderers.Clear()
                                                       lines = MyBase.Text.Split(vbLf)
                                                       For i As Integer = 0 To lines.Length - 1
                                                           lines(i) = lines(i).Replace(vbCr, "")
                                                           textRenderers.Add(New TextRenderer())
                                                       Next
                                                   End Sub
                End Sub

                Public Sub New(layer As Layer, text As String)
                    MyBase.New(layer)
                    Me.Text = text
                End Sub

                Public Overrides ReadOnly Property Type As IElement.TypeConstants
                    Get
                        Return IElement.TypeConstants.Marquee
                    End Get
                End Property

                Public Overrides ReadOnly Property Name As String
                    Get
                        Return "Marquee"
                    End Get
                End Property

                Public Overrides ReadOnly Property IsValid As Boolean
                    Get
                        Return True
                    End Get
                End Property

                Public Overrides Function ToXML() As XElement
                    Dim xml = MyBase.ToXML()
                    xml.<propietary>(0).Add(<speed><%= Me.Speed %></speed>)
                    xml.<propietary>(0).Add(<directionConstants><%= Me.Direction %></directionConstants>)
                    Return xml
                End Function

                Public Overrides Sub SetFromXML(xml As XElement)
                    MyBase.SetFromXML(xml)

                    Dim dc As DirectionConstants = DirectionConstants.Left
                    If [Enum].TryParse(xml.<propietary>.<directionConstants>.Value, dc) Then Me.Direction = dc

                    Me.Speed = xml.<propietary>.<speed>.Value
                End Sub

                Public Overrides Sub ResetRenderer()
                    For Each renderer In textRenderers
                        renderer.ClearCache()
                    Next
                End Sub

                Public Overrides Function Render(g As Graphics, r As Rectangle, m As Drawing2D.Matrix) As KeyFrame
                    Dim bk As KeyFrame = MyBase.RenderBackground(g, m)

                    Dim originalClip As RectangleF
                    Dim p As Point = Point.Empty
                    Dim rp As Point
                    Using stringFormat As StringFormat = New StringFormat(StringFormat.GenericDefault)
                        stringFormat.FormatFlags = StringFormatFlags.LineLimit Or StringFormatFlags.NoWrap

                        Select Case Direction
                            Case DirectionConstants.Left
                                p.X = bk.Bounds.Right - bk.Padding.Right
                            Case DirectionConstants.Right
                                p.X = bk.Bounds.Left + bk.Padding.Left
                            Case DirectionConstants.Up
                                p.Y = bk.Bounds.Bottom - bk.Padding.Bottom
                            Case DirectionConstants.Down
                                p.Y = bk.Bounds.Top - bk.Padding.Top
                        End Select

                        originalClip = g.ClipBounds
                        Dim newClip As Rectangle = bk.Bounds
                        newClip.X += bk.Padding.Left
                        newClip.Y += bk.Padding.Top
                        newClip.Width -= bk.Padding.Horizontal
                        newClip.Height -= bk.Padding.Vertical
                        g.SetClip(newClip)

                        If lines.Length > 0 Then
                            Dim offset As Integer = Speed * ((MyBase.Layer.Track.CursorTime - MyBase.InitialKeyFrame.Time).TotalSeconds)
                            Using b As Brush = bk.ForeColor.ToBrush()
                                Do
                                    For lineIndex As Integer = 0 To lines.Length - 1
                                        Dim bmp As Bitmap = textRenderers(lineIndex).Render(lines(lineIndex), bk)
                                        Dim ap As Point = AlignContent(bk.Bounds, bk.Padding, textRenderers(lineIndex).TextSize, bk.ContentAlignment).Location

                                        Select Case Direction
                                            Case DirectionConstants.Left
                                                rp = New Point(p.X - offset, ap.Y)
                                            Case DirectionConstants.Right
                                                rp = New Point(p.X + offset, ap.Y)
                                            Case DirectionConstants.Up
                                                rp = New Point(ap.X, p.Y - offset)
                                            Case DirectionConstants.Down
                                                rp = New Point(ap.X, p.Y + offset)
                                        End Select

                                        If bmp IsNot Nothing Then g.DrawImageUnscaled(bmp, rp)

                                        Select Case Direction
                                            Case DirectionConstants.Left
                                                If rp.X >= newClip.Right + textRenderers(lineIndex).TextSize.Width Then Exit Do
                                                p.X += (SeparationBetweenStrings + textRenderers(lineIndex).TextSize.Width)
                                            Case DirectionConstants.Right
                                                If rp.X <= newClip.Left - textRenderers(lineIndex).TextSize.Width Then Exit Do
                                                p.X -= (SeparationBetweenStrings + textRenderers(lineIndex).TextSize.Width)
                                            Case DirectionConstants.Up
                                                If rp.Y >= newClip.Bottom + textRenderers(lineIndex).TextSize.Height Then Exit Do
                                                p.Y += (SeparationBetweenStrings + textRenderers(lineIndex).TextSize.Height)
                                            Case DirectionConstants.Down
                                                If rp.Y <= newClip.Top - textRenderers(lineIndex).TextSize.Height Then Exit Do
                                                p.Y -= (SeparationBetweenStrings + textRenderers(lineIndex).TextSize.Height)
                                        End Select
                                    Next
                                Loop
                            End Using
                        End If
                    End Using

                    g.SetClip(originalClip)

                    Return bk
                End Function
            End Class
        End Class
    End Class
End Class
