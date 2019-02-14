Imports System.Drawing.Drawing2D

Public Class ColorWheel
    Private Enum OverElementConstants
        None
        ColorWheel
        Luma
        Alpha
    End Enum

    Private mQuality As Integer = 4
    Private mSelColor As New ColorSpace.HLSRGB(Color.Blue)

    Private colorWheelCursor As Point = Point.Empty
    Private lumaCursor As Single = 0.5
    Private alphaCursor As Integer = 0

    Private overElement As OverElementConstants = OverElementConstants.None
    Private mainRect As Rectangle
    Private mouseIsDown As Boolean

    Private colorWheelRect As Rectangle
    Private lumaRect As Rectangle
    Private alphaRect As Rectangle

    Private Const ctrlsSize As Integer = 22
    Private Const ctrlsSpacing As Integer = 8

    Public Event ColorChanged()

    Public Property Color As Color
        Get
            Return mSelColor.Color
        End Get
        Set(value As Color)
            mSelColor.Color = value
            colorWheelCursor = New Point(hue2x(mSelColor.Hue), sat2y(mSelColor.Saturation))
            lumaCursor = mSelColor.Luminance
            alphaCursor = mSelColor.Alpha

            UpdateSliders()
            UpdateElements()
        End Set
    End Property

    Private Sub ColorEditor_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.SetStyle(ControlStyles.UserPaint, True)
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        Me.SetStyle(ControlStyles.Selectable, True)

        Color = Color.GreenYellow
    End Sub

    Private Sub UpdateElements()
        mainRect = Me.DisplayRectangle
        mainRect.Width -= 1
        mainRect.Height = SliderRed.Top - ctrlsSpacing

        Me.Invalidate()
        RaiseEvent ColorChanged()
    End Sub

    Private Sub DrawElements(g As Graphics)
        Dim colorWheel As New ColorSpace.HLSRGB()
        Dim colorLuma As New ColorSpace.HLSRGB(x2Hue(colorWheelCursor.X), 0.5!, y2Sat(colorWheelCursor.Y))
        Dim hue As Single = colorLuma.Hue

        Dim pixel As Rectangle

        For y As Integer = 0 To colorWheelRect.Height - 1 Step mQuality
            For x As Integer = 0 To colorWheelRect.Width - 1 Step mQuality
                colorWheel.HLS = New ColorSpace.HLSRGB.HueLumSat(x2Hue(x), lumaCursor, y2Sat(y))
                Using b As SolidBrush = New SolidBrush(colorWheel.Color)
                    pixel = New Rectangle(x + colorWheelRect.X, y + colorWheelRect.Y, mQuality, mQuality)
                    pixel.Intersect(colorWheelRect)
                    g.FillRectangle(b, pixel)
                End Using
            Next

            colorLuma.HLS = New ColorSpace.HLSRGB.HueLumSat(hue, y2Lum(y), 1)
            Using b As SolidBrush = New SolidBrush(colorLuma.Color)
                pixel = New Rectangle(lumaRect.X, y + lumaRect.Y, lumaRect.Width, mQuality)
                pixel.Intersect(lumaRect)
                g.FillRectangle(b, pixel)
            End Using
        Next

        DrawTransparency(g, alphaRect, False)
        Dim c As New ColorSpace.HLSRGB(x2Hue(colorWheelCursor.X), 0.5!, y2Sat(colorWheelCursor.Y))
        Using gb As LinearGradientBrush = New LinearGradientBrush(alphaRect, c.Color, Color.FromArgb(0, Color.White), LinearGradientMode.Horizontal)
            g.FillRectangle(gb, alphaRect)
        End Using

        g.DrawRectangle(Pens.Gray, colorWheelRect)
        g.DrawRectangle(Pens.Gray, lumaRect)
        g.DrawRectangle(Pens.Gray, alphaRect)
    End Sub

    Private Function X2Hue(x As Integer) As Single
        Return CSng(x / colorWheelRect.Width * 360)
    End Function

    Private Function Hue2x(hue As Single) As Integer
        Return CInt(hue * colorWheelRect.Width / 360)
    End Function

    Private Function Y2Sat(y As Integer) As Single
        Return CSng(1 - y / colorWheelRect.Height)
    End Function

    Private Function Sat2y(sat As Single) As Integer
        Return CInt((1 - sat) * colorWheelRect.Height)
    End Function

    Private Function Y2Lum(y As Integer) As Single
        Return CSng(1 - y / lumaRect.Height)
    End Function

    Private Function Lum2y(lum As Single) As Integer
        Return CInt((1 - lum) * lumaRect.Height)
    End Function

    Private Function X2Alpha(x As Integer) As Byte
        Return CByte(255 - x / alphaRect.Width * 255)
    End Function

    Private Function Alpha2x(alpha As Byte) As Integer
        Return CInt(alphaRect.Width - alpha * alphaRect.Width / 255)
    End Function

    Public Shared Sub DrawTransparency(g As Graphics, r As Rectangle, hasBorder As Boolean)
        Dim flag As Boolean = False
        Dim lastFlag As Boolean = False
        Dim b As SolidBrush
        Dim gr As Rectangle
        Dim offset As Integer = If(hasBorder, 1, 0)

        For x As Integer = r.Left + offset To r.Right - offset - 1 Step 4
            For y As Integer = r.Top + offset To r.Bottom - offset - 1 Step 4
                b = New SolidBrush(If(flag, Color.White, Color.Gray))

                gr = New Rectangle(x, y, 4, 4)
                gr.Intersect(r)
                g.FillRectangle(b, gr)

                b.Dispose()
                flag = Not flag
            Next
            If lastFlag = flag Then
                flag = Not flag
                lastFlag = flag
            End If
        Next
    End Sub

    Private Sub SetColorFromCursors()
        mSelColor = New ColorSpace.HLSRGB(x2Hue(colorWheelCursor.X), lumaCursor, y2Sat(colorWheelCursor.Y)) With {.Alpha = alphaCursor}

        UpdateSliders()
        UpdateElements()
    End Sub

    Private Sub ColorEditor_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        Cursor.Hide()
        mouseIsDown = True
        DetectMouseAction(e.Location)
        UpdateCursors(e.Location)
    End Sub

    Private Sub ColorEditor_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        Static lastP As Point = Point.Empty

        If lastP = e.Location Then Exit Sub
        lastP = e.Location

        If mouseIsDown Then
            UpdateCursors(e.Location)
        Else
            DetectMouseAction(e.Location)
        End If
    End Sub

    Private Sub ColorEditor_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        Cursor.Show()
        mouseIsDown = False
        DetectMouseAction(e.Location)
    End Sub

    Private Sub UpdateCursors(p As Point)
        If mouseIsDown Then
            Select Case overElement
                Case OverElementConstants.ColorWheel
                    p.X = Math.Min(Math.Max(p.X, 0), colorWheelRect.Width - 2)
                    p.Y = Math.Min(Math.Max(p.Y, 0), colorWheelRect.Height - 2)

                    colorWheelCursor = p
                Case OverElementConstants.Luma
                    lumaCursor = y2Lum(Math.Min(Math.Max(p.Y - lumaRect.Y, 0), lumaRect.Height - 2))
                Case OverElementConstants.Alpha
                    alphaCursor = x2Alpha(Math.Min(Math.Max(p.X - alphaRect.X, 0), alphaRect.Width - 2))
            End Select

            SetColorFromCursors()
            Me.Invalidate()
            RaiseEvent ColorChanged()
        End If
    End Sub

    Private Sub DetectMouseAction(p As Point)
        Dim cursorRect As Rectangle = New Rectangle(p.X, p.Y, 1, 1)

        If colorWheelRect.IntersectsWith(cursorRect) Then
            overElement = OverElementConstants.ColorWheel
            Me.Cursor = Cursors.SizeAll
        ElseIf lumaRect.IntersectsWith(cursorRect) Then
            overElement = OverElementConstants.Luma
            Me.Cursor = Cursors.SizeNS
        ElseIf alphaRect.IntersectsWith(cursorRect) Then
            overElement = OverElementConstants.Alpha
            Me.Cursor = Cursors.SizeWE
        Else
            overElement = OverElementConstants.None
            Me.Cursor = Cursors.Default
        End If
    End Sub

    Private Sub ColorEditor_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        Dim g As Graphics = e.Graphics

        DrawElements(g)

        g.SmoothingMode = SmoothingMode.AntiAlias

        DrawColorCursor(g)
        DrawLumaCursor(g)
        DrawAlphaCursor(g)
        DrawSelColor(g)
    End Sub

    Private Sub DrawColorCursor(g As Graphics)
        Dim r As Rectangle = New Rectangle(colorWheelCursor.X - 7, colorWheelCursor.Y - 7, 15, 15)
        Dim c As Color = mSelColor.Color

        r.Offset(colorWheelRect.Location)

        Using b As SolidBrush = New SolidBrush(Color.FromArgb(128, c))
            g.FillEllipse(b, r)
        End Using
        g.DrawEllipse(Pens.Black, r)

        Dim colorTool As New ColorSpace.HLSRGB(x2Hue(colorWheelCursor.X), 0.5!, y2Sat(colorWheelCursor.Y))
        r.Offset(r.Width \ 4 + 1, r.Height \ 4 + 1)
        r.Width \= 2
        r.Height \= 2
        Using b As SolidBrush = New SolidBrush(colorTool.Color)
            g.FillEllipse(b, r)
        End Using
    End Sub

    Private Sub DrawLumaCursor(g As Graphics)
        Dim p() As Point = New Point() {New Point(0, 0), New Point(0, 0), New Point(0, 0)}
        Dim w As Integer = ctrlsSpacing \ 2
        Dim h As Integer = w

        p(1).X = lumaRect.Left - 1
        p(1).Y = lumaRect.Y + lum2y(lumaCursor) + 1

        p(0).X = p(1).X - w
        p(0).Y = p(1).Y - h

        p(2).X = p(1).X - w
        p(2).Y = p(1).Y + h

        g.FillPolygon(Brushes.Black, p)
    End Sub

    Private Sub DrawAlphaCursor(g As Graphics)
        Dim p() As Point = New Point() {New Point(0, 0), New Point(0, 0), New Point(0, 0)}
        Dim w As Integer = ctrlsSpacing \ 2
        Dim h As Integer = w

        p(0).X = alphaRect.X + alpha2x(alphaCursor)
        p(0).Y = alphaRect.Y - 1

        p(1).X = p(0).X - w
        p(1).Y = p(0).Y - h

        p(2).X = p(0).X + w
        p(2).Y = p(0).Y - h

        g.FillPolygon(Brushes.Black, p)
    End Sub

    Private Sub DrawSelColor(g As Graphics)
        Dim r As Rectangle = New Rectangle(lumaRect.X, alphaRect.Y, lumaRect.Width, alphaRect.Height)
        DrawTransparency(g, r, False)
        Using b As SolidBrush = New SolidBrush(mSelColor.Color)
            g.FillRectangle(b, r)
        End Using
        g.DrawRectangle(Pens.Gray, r)
    End Sub

    Private Sub ColorEditor_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        UpdateElements()
    End Sub

    Private Sub Slider_Scrolling(sender As Slider) Handles SliderRed.Scrolling, SliderGreen.Scrolling, SliderBlue.Scrolling
        UpdateColorTextBoxes(True)
        UpdateElements()
    End Sub

    Private Sub Slider_ValueChanged(ByVal sender As Slider, ByVal value As Integer) Handles SliderRed.ValueChanged, SliderGreen.ValueChanged, SliderBlue.ValueChanged
        UpdateColorTextBoxes(False)
    End Sub

    Private Sub UpdateSliders()
        SliderRed.Value = mSelColor.Red / 255 * SliderRed.Max
        SliderGreen.Value = mSelColor.Green / 255 * SliderGreen.Max
        SliderBlue.Value = mSelColor.Blue / 255 * SliderBlue.Max
    End Sub

    Private Sub UpdateColorTextBoxes(updateMainColor As Boolean)
        Dim red As Byte = CByte(SliderRed.Value / SliderRed.Max * 255)
        Dim green As Byte = CByte(SliderGreen.Value / SliderGreen.Max * 255)
        Dim blue As Byte = CByte(SliderBlue.Value / SliderBlue.Max * 255)

        TextBoxRed.Text = red
        TextBoxGreen.Text = green
        TextBoxBlue.Text = blue

        If updateMainColor Then Color = Color.FromArgb(mSelColor.Alpha, red, green, blue)
    End Sub

    Private Sub ColorWheel_SizeChanged(sender As Object, e As EventArgs) Handles Me.SizeChanged
        Dim margin As Integer = Me.Margin.All

        colorWheelRect = New Rectangle(margin, margin, mainRect.Width - ctrlsSize - margin * 2 - ctrlsSpacing, mainRect.Height - ctrlsSize - margin - ctrlsSpacing)
        colorWheelRect.Width -= 1
        colorWheelRect.Height -= 1

        lumaRect = New Rectangle(colorWheelRect.Right + ctrlsSpacing, margin, ctrlsSize, colorWheelRect.Height + 1)
        lumaRect.Width -= 1
        lumaRect.Height -= 1

        alphaRect = New Rectangle(colorWheelRect.Left, colorWheelRect.Bottom + ctrlsSpacing, colorWheelRect.Width + 1, ctrlsSize)
        alphaRect.Width -= 1
        alphaRect.Height -= 1
    End Sub
End Class
