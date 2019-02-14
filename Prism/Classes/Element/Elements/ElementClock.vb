Imports System.ComponentModel

Partial Public Class Project
    Partial Public Class Track
        Partial Public Class Layer
            <Serializable()>
            Public Class ElementClock
                Inherits Element

                Private Const toRadians As Single = Math.PI / 180
                Private Const hoursToRadians As Single = 360 * toRadians / 12
                Private Const minutesToRadians As Single = 360 * toRadians / 60
                Private Const secondsToRadians As Single = 360 * toRadians / 60

                Private textRenderer As TextRenderer = New TextRenderer()

                Public Enum ModeConstants
                    Digital
                    Analog
                End Enum

                Public Enum TimeFormatConstants
                    Normal
                    NormalAMPM
                    Military
                    Custom
                End Enum

                Public Enum DateFormatConstants
                    [Short]
                    [Long]
                    Custom
                End Enum

                Public Enum ArragementConstants
                    TimeOnTop
                    TimeOnLeft
                    TimeOnRight
                    TimeOnBottom
                End Enum

                Public Sub New(ByVal layer As Layer)
                    MyBase.New(layer)
                End Sub

                Public Overrides ReadOnly Property Type As IElement.TypeConstants
                    Get
                        Return IElement.TypeConstants.Clock
                    End Get
                End Property

                Public Overrides ReadOnly Property Name As String
                    Get
                        Return "Clock"
                    End Get
                End Property

                Public Overrides ReadOnly Property IsValid As Boolean
                    Get
                        Return True
                    End Get
                End Property

                <Category("Appearance")>
                Public Property Mode As ModeConstants = ModeConstants.Digital

                <Category("Digital")>
                Public Property TimeFormat As TimeFormatConstants = TimeFormatConstants.NormalAMPM

                <Category("Digital")>
                Public Property DateFormat As DateFormatConstants = DateFormatConstants.Short

                <Category("Digital")>
                Public Property ShowTime As Boolean = True

                <Category("Digital")>
                Public Property ShowDate As Boolean = False

                <Category("Digital")>
                Public Property ShowSeconds As Boolean = False

                <Category("Digital")>
                Public Property FlashSecondsSeparator As Boolean = False

                <Category("Digital")>
                Public Property CustomTimeFormat As String = "hh:mm tt"

                <Category("Digital")>
                Public Property CustomDateFormat As String = "dd/MM/yyyy"

                <Category("Digital")>
                Public Property Arrangement As ArragementConstants = ArragementConstants.TimeOnTop

                <Category("Analog")>
                Public Property BorderColor As Color = Color.Wheat

                <Category("Analog")>
                Public Property BackgroundColor As Color = Color.LightSlateGray

                <Category("Analog")>
                Public Property HoursTicksColor As Color = Color.LightGray

                <Category("Analog")>
                Public Property HoursNeedleColor As Color = Color.White

                <Category("Analog")>
                Public Property MinutesNeedleColor As Color = Color.White

                <Category("Analog")>
                Public Property SecondsNeedleColor As Color = Color.White

                Public Overrides Function Render(g As Graphics, r As Rectangle, m As Drawing2D.Matrix) As KeyFrame
                    Dim bk As KeyFrame = MyBase.RenderBackground(g, m)

                    Select Case Mode
                        Case ModeConstants.Analog
                            RenderAnalog(g, bk)
                        Case ModeConstants.Digital
                            RenderDigital(g, bk)
                    End Select

                    Return bk
                End Function

                Private Sub RenderAnalog(g As Graphics, bk As KeyFrame)
                    Dim t As Date = My.Computer.Clock.LocalTime

                    Dim cp As Point = New Point(bk.Bounds.Width \ 2, bk.Bounds.Height \ 2)

                    Using p As Pen = New Pen(BorderColor)
                        g.DrawEllipse(p, bk.Bounds)
                    End Using

                    Using b As SolidBrush = New SolidBrush(BackgroundColor)
                        g.FillEllipse(b, bk.Bounds.Left + 4, bk.Bounds.Top + 4, bk.Bounds.Width - 8, bk.Bounds.Height - 8)
                        g.FillEllipse(Brushes.White, bk.Bounds.X + cp.X - 4, bk.Bounds.Y + cp.Y - 4, 8, 8)

                        ' Hours
                        g.FillPolygon(b, GenNeedle(bk, ((t.Hour + t.Minute / 60) Mod 12 - 3) * hoursToRadians, cp, cp.Y - 45, 4))
                        Using b1 As SolidBrush = New SolidBrush(HoursNeedleColor)
                            g.FillPolygon(b1, GenNeedle(bk, ((t.Hour + t.Minute / 60) Mod 12 - 3) * hoursToRadians, cp, cp.Y - 45, 4))
                        End Using

                        ' Minutes
                        g.FillPolygon(b, GenNeedle(bk, (t.Minute - 15) * minutesToRadians, cp, cp.Y - 18, 4))
                        Using b1 As SolidBrush = New SolidBrush(MinutesNeedleColor)
                            g.FillPolygon(b1, GenNeedle(bk, (t.Minute - 15) * minutesToRadians, cp, cp.Y - 18, 3))
                        End Using

                        ' Seconds
                        g.FillPolygon(b, GenNeedle(bk, (t.Second - 15) * secondsToRadians, cp, cp.Y - 12, 2))
                        Using b1 As SolidBrush = New SolidBrush(SecondsNeedleColor)
                            g.FillPolygon(b1, GenNeedle(bk, (t.Second - 15) * secondsToRadians, cp, cp.Y - 12, 1))
                        End Using
                    End Using

                    ' Hours' ticks

                    Using b As SolidBrush = New SolidBrush(HoursTicksColor)
                        Dim tp As Point = New Point()
                        For i As Integer = 0 To 11
                            tp.X = bk.Bounds.X + cp.X + (cp.X - 10) * Math.Cos((i Mod 12 - 3) * hoursToRadians)
                            tp.Y = bk.Bounds.Y + cp.Y + (cp.Y - 10) * Math.Sin((i Mod 12 - 3) * hoursToRadians)
                            g.FillEllipse(b, tp.X - 1, tp.Y - 1, 2, 2)
                        Next
                    End Using
                End Sub

                Private Sub RenderDigital(g As Graphics, bk As KeyFrame)
                    Dim timeText As String = ""
                    Dim dateText As String = ""

                    If ShowTime Then
                        Dim f As String = ""
                        If ShowSeconds Then
                            f = If(FlashSecondsSeparator,
                                    If(My.Computer.Clock.LocalTime.Second Mod 2 = 0, ":ss", " ss"),
                                    ":ss")
                        End If
                        Select Case TimeFormat
                            Case ElementClock.TimeFormatConstants.Custom
                                timeText = My.Computer.Clock.LocalTime.ToString(CustomTimeFormat)
                            Case ElementClock.TimeFormatConstants.Military
                                timeText = My.Computer.Clock.LocalTime.ToString("HH:mm" + f)
                            Case ElementClock.TimeFormatConstants.Normal
                                timeText = My.Computer.Clock.LocalTime.ToString("hh:mm" + f)
                            Case ElementClock.TimeFormatConstants.NormalAMPM
                                timeText = My.Computer.Clock.LocalTime.ToString("hh:mm" + f + " tt").ToLower()
                        End Select
                    End If

                    If ShowDate Then
                        Select Case DateFormat
                            Case ElementClock.DateFormatConstants.Custom
                                dateText = My.Computer.Clock.LocalTime.ToString(CustomDateFormat)
                            Case ElementClock.DateFormatConstants.Short
                                dateText = My.Computer.Clock.LocalTime.ToShortDateString()
                            Case ElementClock.DateFormatConstants.Long
                                dateText = My.Computer.Clock.LocalTime.ToLongDateString()
                        End Select
                    End If

                    Dim code As String = ""
                    Select Case Arrangement
                        Case ArragementConstants.TimeOnBottom
                            code = dateText + IIf(timeText = "", "", vbCrLf + timeText)
                        Case ArragementConstants.TimeOnLeft
                            code = timeText + IIf(dateText = "", "", " " + dateText)
                        Case ArragementConstants.TimeOnRight
                            code = dateText + IIf(timeText = "", "", " " + timeText)
                        Case ArragementConstants.TimeOnTop
                            code = timeText + IIf(dateText = "", "", vbCrLf + dateText)
                    End Select

                    Dim bmp As Bitmap = textRenderer.Render(code, bk)
                    If bmp IsNot Nothing Then
                        Dim txtRect As Rectangle = AlignContent(bk.Bounds, bk.Padding, textRenderer.TextSize, bk.ContentAlignment)
                        g.DrawImageUnscaled(bmp, txtRect.Location)
                    End If
                End Sub

                Public Overrides Function ToXML() As XElement
                    Dim xml = MyBase.ToXML()
                    xml.Add(
                            <propietary>
                                <mode><%= Me.Mode %></mode>
                                <timeFormat><%= Me.TimeFormat %></timeFormat>
                                <dateFormat><%= Me.DateFormat %></dateFormat>
                                <showTime><%= Me.ShowTime %></showTime>
                                <showDate><%= Me.ShowDate %></showDate>
                                <showSeconds><%= Me.ShowSeconds %></showSeconds>
                                <flashSecondsSeparator><%= Me.FlashSecondsSeparator %></flashSecondsSeparator>
                                <customTimeFormat><%= Me.CustomTimeFormat %></customTimeFormat>
                                <customDateFormat><%= Me.CustomDateFormat %></customDateFormat>
                                <arrangement><%= Me.Arrangement %></arrangement>

                                <analogBorderColor><%= Me.BorderColor.ToArgb() %></analogBorderColor>
                                <analogBackgroundColor><%= Me.BackgroundColor.ToArgb() %></analogBackgroundColor>
                                <analogHoursTicksColor><%= Me.HoursTicksColor.ToArgb() %></analogHoursTicksColor>
                                <analogHoursNeedleColor><%= Me.HoursNeedleColor.ToArgb() %></analogHoursNeedleColor>
                                <analogMinutesNeedleColor><%= Me.MinutesNeedleColor.ToArgb() %></analogMinutesNeedleColor>
                                <analogSecondsNeedleColor><%= Me.SecondsNeedleColor.ToArgb() %></analogSecondsNeedleColor>
                            </propietary>
                    )
                    Return xml
                End Function

                Public Overrides Sub SetFromXML(xml As XElement)
                    Dim m As ModeConstants = ModeConstants.Digital
                    If [Enum].TryParse(xml.<propietary>.<mode>.Value, m) Then Me.Mode = m

                    Dim tf As TimeFormatConstants = TimeFormatConstants.NormalAMPM
                    If [Enum].TryParse(xml.<propietary>.<timeFormat>.Value, tf) Then Me.TimeFormat = tf

                    Dim df As DateFormat = Microsoft.VisualBasic.DateFormat.ShortDate
                    If [Enum].TryParse(xml.<propietary>.<dateFormat>.Value, df) Then Me.DateFormat = df

                    Dim ar As ArragementConstants = ArragementConstants.TimeOnTop
                    If [Enum].TryParse(xml.<propietary>.<arrangement>.Value, ar) Then Me.Arrangement = ar

                    Dim bool As Boolean
                    If Boolean.TryParse(xml.<propietary>.<showTime>.Value, bool) Then Me.ShowTime = bool
                    If Boolean.TryParse(xml.<propietary>.<showDate>.Value, bool) Then Me.ShowDate = bool
                    If Boolean.TryParse(xml.<propietary>.<showSeconds>.Value, bool) Then Me.ShowSeconds = bool
                    If Boolean.TryParse(xml.<propietary>.<flashSecondsSeparator>.Value, bool) Then Me.FlashSecondsSeparator = bool

                    Me.CustomTimeFormat = xml.<propietary>.<customTimeFormat>.Value
                    Me.CustomDateFormat = xml.<propietary>.<customDateFormat>.Value

                    Me.BorderColor = Color.FromArgb(xml.<propietary>.<analogBorderColor>.Value)
                    Me.BackgroundColor = Color.FromArgb(xml.<propietary>.<analogBackgroundColor>.Value)
                    Me.HoursTicksColor = Color.FromArgb(xml.<propietary>.<analogHoursTicksColor>.Value)
                    Me.HoursNeedleColor = Color.FromArgb(xml.<propietary>.<analogHoursNeedleColor>.Value)
                    Me.MinutesNeedleColor = Color.FromArgb(xml.<propietary>.<analogMinutesNeedleColor>.Value)
                    Me.SecondsNeedleColor = Color.FromArgb(xml.<propietary>.<analogSecondsNeedleColor>.Value)
                End Sub

                Private Function GenNeedle(ByVal bk As KeyFrame, ByVal angle As Single, ByVal location As Point, ByVal needleLength As Integer, ByVal needleWidth As Integer) As Point()
                    Dim p(2) As Point
                    p(0) = New Point
                    p(1) = New Point
                    p(2) = New Point

                    p(0).X = bk.Bounds.X + location.X + needleWidth * Math.Cos(angle - 90 * toRadians)
                    p(0).Y = bk.Bounds.Y + location.Y + needleWidth * Math.Sin(angle - 90 * toRadians)

                    p(1).X = bk.Bounds.X + location.X + needleLength * Math.Cos(angle)
                    p(1).Y = bk.Bounds.Y + location.Y + needleLength * Math.Sin(angle)

                    p(2).X = bk.Bounds.X + location.X + needleWidth * Math.Cos(angle + 90 * toRadians)
                    p(2).Y = bk.Bounds.Y + location.Y + needleWidth * Math.Sin(angle + 90 * toRadians)

                    Return p
                End Function
            End Class
        End Class
    End Class
End Class
