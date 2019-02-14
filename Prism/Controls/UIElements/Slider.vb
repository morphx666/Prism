<ComponentModel.Designer(GetType(SliderDesigner))>
Public Class Slider
    Public Enum HighlightStyleConstants
        None
        Solid
        Gradient
    End Enum

    Public Enum OrientationConstants
        Horitzontal
        Vertical
    End Enum

    Public Enum HighlightModeConstants
        Full
        ToKnob
    End Enum

    Public Enum TickStyleConstants
        NoTicks
        TopLeft
        BottomRight
        Both
    End Enum

    Public Enum TextAlignConstants
        TopLeft
        TopCenter
        TopRight
        BottomLeft
        BottomCenter
        BottomRight
    End Enum

    Private mValue As Integer = 50
    Private mMax As Integer = 100
    Private mMin As Integer = 0
    Private mSmallChange As Integer = 1
    Private mLargeChange As Integer = 10
    Private mOrientation As OrientationConstants = OrientationConstants.Horitzontal
    Private mTextAlign As TextAlignConstants = TextAlignConstants.TopLeft
    Private mHighlightColor As Color = Color.FromKnownColor(KnownColor.Highlight)
    Private mHighlightColorEnd As Color = Color.White
    Private mHighlightMode As HighlightModeConstants = HighlightModeConstants.Full
    Private mHighlightStyle As HighlightStyleConstants = HighlightStyleConstants.None
    Private mKnobVisible As Boolean = True
    Private mTextVisible As Boolean = True
    Private mTickFrequency As Integer = 10
    Private mTickStyle As TickStyleConstants = TickStyleConstants.BottomRight
    Private mText As String = "Slider"
    Private mGrooveSize As Integer = 6

    Private textSize As Size = Size.Empty
    Private knobSize As Size = Size.Empty
    Private grooveRect As Rectangle = Rectangle.Empty
    Private knobRect As Rectangle = Rectangle.Empty

    Private isScrolling As Boolean
    Private mouseRect As Rectangle

    Public Event ValueChanged(sender As Slider, value As Integer)
    Public Event Scrolling(sender As Slider)

    Public Property GrooveSize As Integer
        Get
            Return mGrooveSize
        End Get
        Set(value As Integer)
            mGrooveSize = value
            UpdateUI()
        End Set
    End Property

    Public Property TickStyle As TickStyleConstants
        Get
            Return mTickStyle
        End Get
        Set(value As TickStyleConstants)
            mTickStyle = value
            UpdateUI()
        End Set
    End Property

    Public Overrides Property Text As String
        Get
            Return mText
        End Get
        Set(value As String)
            mText = value
            UpdateUI()
            MyBase.OnTextChanged(New EventArgs())
        End Set
    End Property

    Public Property TickFrequency As Integer
        Get
            Return mTickFrequency
        End Get
        Set(value As Integer)
            mTickFrequency = value
            UpdateUI()
        End Set
    End Property

    Public Property TextVisible As Boolean
        Get
            Return mTextVisible
        End Get
        Set(value As Boolean)
            mTextVisible = value
            UpdateUI()
        End Set
    End Property

    Public Property KnobVisible As Boolean
        Get
            Return mKnobVisible
        End Get
        Set(value As Boolean)
            mKnobVisible = value
            UpdateUI()
        End Set
    End Property

    Public Property SmallChange As Integer
        Get
            Return mSmallChange
        End Get
        Set(value As Integer)
            mSmallChange = value
        End Set
    End Property

    Public Property LargeChange As Integer
        Get
            Return mLargeChange
        End Get
        Set(value As Integer)
            mLargeChange = value
        End Set
    End Property

    Public Property HighlightMode As HighlightModeConstants
        Get
            Return mHighlightMode
        End Get
        Set(value As HighlightModeConstants)
            mHighlightMode = value
            UpdateUI()
        End Set
    End Property

    Public Property HighlightColor As Color
        Get
            Return mHighlightColor
        End Get
        Set(value As Color)
            mHighlightColor = value
            UpdateUI()
        End Set
    End Property

    Public Property HighlightColorEnd As Color
        Get
            Return mHighlightColorEnd
        End Get
        Set(value As Color)
            mHighlightColorEnd = value
            UpdateUI()
        End Set
    End Property

    Public Property TextAlign As TextAlignConstants
        Get
            Return mTextAlign
        End Get
        Set(value As TextAlignConstants)
            mTextAlign = value
            UpdateUI()
        End Set
    End Property

    Public Property Orientation As OrientationConstants
        Get
            Return mOrientation
        End Get
        Set(value As OrientationConstants)
            If mOrientation <> value Then
                mOrientation = value
                Me.Size = New Size(Me.Height, Me.Width)
                UpdateUI()
            End If
        End Set
    End Property

    Public Property HighlightStyle As HighlightStyleConstants
        Get
            Return mHighlightStyle
        End Get
        Set(value As HighlightStyleConstants)
            mHighlightStyle = value
            UpdateUI()
        End Set
    End Property

    Public Property Value As Integer
        Get
            Return mValue
        End Get
        Set(value As Integer)
            Dim newValue As Integer = Math.Min(Math.Max(value, mMin), mMax)
            If mValue <> newValue Then
                mValue = newValue
                UpdateUI()

                RaiseEvent ValueChanged(Me, newValue)
            End If
        End Set
    End Property

    Public Property Min As Integer
        Get
            Return mMin
        End Get
        Set(value As Integer)
            mMin = value
            UpdateUI()
        End Set
    End Property

    Public Property Max As Integer
        Get
            Return mMax
        End Get
        Set(value As Integer)
            mMax = value
            UpdateUI()
        End Set
    End Property

    Private Sub Slider_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.SetStyle(ControlStyles.UserPaint, True)
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        Me.SetStyle(ControlStyles.Selectable, True)
    End Sub

    Protected Overrides Sub OnFontChanged(ByVal e As EventArgs)
        MyBase.OnFontChanged(e)
        UpdateUI()
    End Sub

    Private Sub UpdateUI()
        textSize = Size.Empty
        If mTextVisible Then textSize = Windows.Forms.TextRenderer.MeasureText(mText, Font)

        knobSize = Size.Empty
        If mKnobVisible Then knobSize = My.Resources.knob_sel.Size

        Select Case mOrientation
            Case OrientationConstants.Horitzontal
                Me.Height = textSize.Height + Math.Max(knobSize.Height, mGrooveSize + 12) + Me.Padding.Vertical + 2
            Case OrientationConstants.Vertical
                Me.Width = Math.Max(Math.Max(textSize.Width, knobSize.Width), mGrooveSize + 12) + Me.Padding.Horizontal + 2
        End Select

        Me.Invalidate()
    End Sub

    Private Sub Slider_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        mouseRect = New Rectangle(e.Location, New Size(1, 1))
        If knobRect.IntersectsWith(mouseRect) Then
            isScrolling = (e.Button = Windows.Forms.MouseButtons.Left)
        End If
    End Sub

    Private Sub Slider_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If isScrolling Then
            Dim oldValue As Integer = mValue
            Dim newValue As Integer
            Select Case mOrientation
                Case OrientationConstants.Horitzontal
                    newValue = mMin + (e.X - knobRect.Width / 2) / grooveRect.Width * mMax
                Case OrientationConstants.Vertical
                    newValue = mMin + (e.Y - knobRect.Height / 2) / grooveRect.Width * mMax
            End Select

            If oldValue <> newValue Then
                Value = newValue
                RaiseEvent ValueChanged(Me, newValue)
                RaiseEvent Scrolling(Me)
            End If
        End If
    End Sub

    Private Sub Slider_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        isScrolling = False
    End Sub

    Private Sub Slider_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        Dim g As Graphics = e.Graphics
        Dim r As Rectangle = Me.DisplayRectangle
        r.Width -= 1
        r.Height -= 1

        Dim offsetH As Integer = 0
        Dim offsetV As Integer = 0
        Dim textIsAtTop As Boolean = False

        If mTextVisible Then
            Dim tr As Rectangle = New Rectangle(New Point(0, 0), textSize)
            Select Case mTextAlign
                Case TextAlignConstants.TopLeft
                    tr.X = Me.Padding.Left
                    offsetV = tr.Height + 2
                    textIsAtTop = True
                Case TextAlignConstants.TopCenter
                    tr.X = (r.Width - textSize.Width) / 2
                    offsetV = tr.Height + 2
                    textIsAtTop = True
                Case TextAlignConstants.TopRight
                    tr.X = r.Width - Me.Padding.Right - textSize.Width
                    offsetV = tr.Height + 2
                    textIsAtTop = True

                Case TextAlignConstants.BottomLeft
                    tr.Y = Me.Height - Me.Padding.Bottom - textSize.Height
                    offsetH = tr.Height + 2
                Case TextAlignConstants.BottomCenter
                    tr.X = (r.Width - textSize.Width) / 2
                    tr.Y = Me.Height - Me.Padding.Bottom - textSize.Height
                    offsetH = tr.Height + 2
                Case TextAlignConstants.BottomRight
                    tr.X = r.Width - Me.Padding.Right - textSize.Width
                    tr.Y = Me.Height - Me.Padding.Bottom - textSize.Height
                    offsetH = tr.Height + 2
            End Select

            Using b As SolidBrush = New SolidBrush(Me.ForeColor)
                g.DrawString(mText, Font, b, tr)
            End Using
        End If

        Select Case mOrientation
            Case OrientationConstants.Horitzontal
                grooveRect = New Rectangle(r.X + Me.Padding.Left + knobSize.Width / 2,
                                            textSize.Height + 6,
                                            r.Width - Me.Padding.Horizontal - knobSize.Width,
                                            mGrooveSize)
            Case OrientationConstants.Vertical
                grooveRect = New Rectangle(r.X + (r.Width - mGrooveSize) / 2,
                                            r.Y + Me.Padding.Top + knobSize.Height / 2 + offsetV,
                                            mGrooveSize,
                                            r.Height - Me.Padding.Vertical - knobSize.Height * 2)
        End Select

        If mKnobVisible Then
            If mHighlightStyle <> HighlightStyleConstants.None Then
                Dim hr As Rectangle = grooveRect

                Select Case mHighlightMode
                    Case HighlightModeConstants.Full
                    Case HighlightModeConstants.ToKnob
                        Select Case mOrientation
                            Case OrientationConstants.Horitzontal
                                hr.Width = ValueToPercentage() * hr.Width
                            Case OrientationConstants.Vertical
                                hr.Height = ValueToPercentage() * hr.Height
                        End Select
                End Select

                Select Case mHighlightStyle
                    Case HighlightStyleConstants.Solid
                        Using b As SolidBrush = New SolidBrush(mHighlightColor)
                            g.FillRectangle(b, hr)
                        End Using
                    Case HighlightStyleConstants.Gradient
                        Using b As Drawing2D.LinearGradientBrush = New Drawing2D.LinearGradientBrush(grooveRect.Location, New Point(grooveRect.Right, grooveRect.Bottom), mHighlightColor, mHighlightColorEnd)
                            g.FillRectangle(b, hr)
                        End Using
                End Select
            End If

            ControlPaint.DrawBorder3D(g, grooveRect, Border3DStyle.SunkenOuter)

            If mTickStyle <> TickStyleConstants.NoTicks Then
                Dim tr As Rectangle

                Select Case mOrientation
                    Case OrientationConstants.Horitzontal
                        tr = New Rectangle(grooveRect.Location, New Size(2, 4))
                    Case OrientationConstants.Vertical
                        tr = New Rectangle(grooveRect.Location, New Size(4, 2))
                End Select

                For i As Integer = mMin To mMax Step mTickFrequency
                    Select Case mOrientation
                        Case OrientationConstants.Horitzontal
                            tr.X = grooveRect.X + ValueToPercentage(i) * grooveRect.Width
                            If mTickStyle = TickStyleConstants.Both OrElse mTickStyle = TickStyleConstants.TopLeft Then
                                tr.Y = grooveRect.Y - 4
                                ControlPaint.DrawBorder3D(g, tr, Border3DStyle.SunkenOuter)
                            End If
                            If mTickStyle = TickStyleConstants.Both OrElse mTickStyle = TickStyleConstants.BottomRight Then
                                tr.Y = grooveRect.Bottom + 1
                                ControlPaint.DrawBorder3D(g, tr, Border3DStyle.SunkenOuter)
                            End If
                        Case OrientationConstants.Vertical
                            tr.Y = grooveRect.Y + ValueToPercentage(i) * grooveRect.Height
                            If mTickStyle = TickStyleConstants.Both OrElse mTickStyle = TickStyleConstants.TopLeft Then
                                tr.X = grooveRect.X - 4
                                ControlPaint.DrawBorder3D(g, tr, Border3DStyle.SunkenOuter)
                            End If
                            If mTickStyle = TickStyleConstants.Both OrElse mTickStyle = TickStyleConstants.BottomRight Then
                                tr.X = grooveRect.Right + 1
                                ControlPaint.DrawBorder3D(g, tr, Border3DStyle.SunkenOuter)
                            End If
                    End Select
                Next
            End If

            Select Case mOrientation
                Case OrientationConstants.Horitzontal
                    knobRect = New Rectangle(grooveRect.X + ValueToPercentage() * grooveRect.Width - knobSize.Width / 2,
                                                                       grooveRect.Y + (grooveRect.Height - knobSize.Height) / 2,
                                                                       knobSize.Width,
                                                                       knobSize.Height)
                Case OrientationConstants.Vertical
                    knobRect = New Rectangle(grooveRect.X + (grooveRect.Width - knobSize.Width) / 2,
                                                                       grooveRect.Y + ValueToPercentage() * grooveRect.Height - knobSize.Height / 2 + 2,
                                                                       knobSize.Width,
                                                                       knobSize.Height)
            End Select
            g.DrawImage(My.Resources.knob_unsel, knobRect)
        End If
    End Sub

    Private Function ValueToPercentage(Optional value As Integer = Integer.MinValue) As Double
        If value = Integer.MinValue Then value = mValue
        Return (value - mMin) / (mMax - mMin)
    End Function
End Class

Public Class SliderDesigner
    Inherits Design.ControlDesigner

    Public Overrides ReadOnly Property SelectionRules As Design.SelectionRules
        Get
            Dim slider As Slider = CType(MyBase.Control, Slider)
            Dim sr As Design.SelectionRules = Design.SelectionRules.Moveable Or Design.SelectionRules.Visible

            Select Case slider.Orientation
                Case Slider.OrientationConstants.Horitzontal
                    sr = sr Or Design.SelectionRules.LeftSizeable Or Design.SelectionRules.RightSizeable
                Case Slider.OrientationConstants.Vertical
                    sr = sr Or Design.SelectionRules.TopSizeable Or Design.SelectionRules.BottomSizeable
            End Select

            Return sr
        End Get
    End Property
End Class
