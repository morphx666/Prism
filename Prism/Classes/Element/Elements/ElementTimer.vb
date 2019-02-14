Imports System.ComponentModel

Partial Public Class Project
    Partial Public Class Track
        Partial Public Class Layer
            <Serializable()>
            Public Class ElementTimer
                Inherits Element

                Private textRenderer As TextRenderer = New TextRenderer()

                <Serializable(),
                TypeConverter(GetType(TimerTime.TimerTimeConverter))>
                Public Class TimerTime
                    Implements ICloneable

                    Private Const DaysInMonth As Double = 30.4368499
                    Private Const DaysInYear As Double = 365.242199

                    Private mYear As Integer
                    Private mMonth As Integer
                    Private mDay As Integer
                    Private mHour As Integer
                    Private mMinute As Integer
                    Private mSecond As Integer
                    Private mMillisecond As Integer

                    <DisplayName("Sync With System Time")>
                    Public Property SyncWithSystemTime As Boolean = False

                    Public Property Year As Integer
                        Get
                            Return If(SyncWithSystemTime, Now.Year, mYear)
                        End Get
                        Set(value As Integer)
                            If Not SyncWithSystemTime Then mYear = value
                        End Set
                    End Property

                    Public Property Month As Integer
                        Get
                            Return If(SyncWithSystemTime, Now.Month, mMonth)
                        End Get
                        Set(value As Integer)
                            If Not SyncWithSystemTime Then mMonth = value
                        End Set
                    End Property

                    Public Property Day As Integer
                        Get
                            Return If(SyncWithSystemTime, Now.Day, mDay)
                        End Get
                        Set(value As Integer)
                            If Not SyncWithSystemTime Then mDay = value
                        End Set
                    End Property

                    Public Property Hour As Integer
                        Get
                            Return If(SyncWithSystemTime, Now.Hour, mHour)
                        End Get
                        Set(value As Integer)
                            If Not SyncWithSystemTime Then mHour = value
                        End Set
                    End Property

                    Public Property Minute As Integer
                        Get
                            Return If(SyncWithSystemTime, Now.Minute, mMinute)
                        End Get
                        Set(value As Integer)
                            If Not SyncWithSystemTime Then mMinute = value
                        End Set
                    End Property

                    Public Property Second As Integer
                        Get
                            Return If(SyncWithSystemTime, Now.Second, mSecond)
                        End Get
                        Set(value As Integer)
                            If Not SyncWithSystemTime Then mSecond = value
                        End Set
                    End Property

                    Public Property Millisecond As Integer
                        Get
                            Return If(SyncWithSystemTime, Now.Millisecond, mMillisecond)
                        End Get
                        Set(value As Integer)
                            If Not SyncWithSystemTime Then mMillisecond = value
                        End Set
                    End Property

                    Public Sub New()
                    End Sub

                    Public Sub New(year As Integer, month As Integer, day As Integer, hour As Integer, minute As Integer, second As Integer, millisecond As Integer)
                        mYear = year
                        mMonth = month
                        mDay = day
                        mHour = hour
                        mMinute = minute
                        mSecond = second
                        mMillisecond = millisecond
                    End Sub

                    Public Shared Operator +(tt1 As TimerTime, tt2 As TimerTime) As TimerTime
                        Dim days As Integer

                        days = tt1.Day + tt1.Month * 30.4368499 + tt1.Year * DaysInYear
                        Dim ts1 As TimeSpan = New TimeSpan(days, tt1.Hour, tt1.Minute, tt1.Second, tt1.Millisecond)

                        days = tt2.Day + tt2.Month * DaysInMonth + tt2.Year * DaysInYear
                        Dim ts2 As TimeSpan = New TimeSpan(days, tt2.Hour, tt2.Minute, tt2.Second, tt2.Millisecond)

                        Return TimerTime.FromMillisconds((ts1 + ts2).TotalMilliseconds)
                    End Operator

                    Public Shared Operator -(tt1 As TimerTime, tt2 As TimerTime) As TimerTime
                        Dim days As Integer

                        days = tt1.Day + tt1.Month * DaysInMonth + tt1.Year * DaysInYear
                        Dim ts1 As TimeSpan = New TimeSpan(days, tt1.Hour, tt1.Minute, tt1.Second, tt1.Millisecond)

                        days = tt2.Day + tt2.Month * DaysInMonth + tt2.Year * 365.242199
                        Dim ts2 As TimeSpan = New TimeSpan(days, tt2.Hour, tt2.Minute, tt2.Second, tt2.Millisecond)

                        Return TimerTime.FromMillisconds((ts1 - ts2).TotalMilliseconds)
                    End Operator

                    Public Shared Operator =(tt1 As TimerTime, tt2 As TimerTime) As Boolean
                        Return tt1.Year = tt2.Year AndAlso
                                tt1.Month = tt2.Month AndAlso
                                tt1.Day = tt2.Day AndAlso
                                tt1.Hour = tt2.Hour AndAlso
                                tt1.Minute = tt2.Minute AndAlso
                                tt1.Second = tt2.Second AndAlso
                                tt1.Millisecond = tt2.Millisecond
                    End Operator

                    Public Shared Operator <>(tt1 As TimerTime, tt2 As TimerTime) As Boolean
                        Return Not tt1 = tt2
                    End Operator

                    Public Shared Operator >(tt1 As TimerTime, tt2 As TimerTime) As Boolean
                        If tt1.Year > tt2.Year Then Return True
                        If tt1.Year < tt2.Year Then Return False

                        If tt1.Month > tt2.Month Then Return True
                        If tt1.Month < tt2.Month Then Return False

                        If tt1.Day > tt2.Day Then Return True
                        If tt1.Day < tt2.Day Then Return False

                        If tt1.Hour > tt2.Hour Then Return True
                        If tt1.Hour < tt2.Hour Then Return False

                        If tt1.Minute > tt2.Minute Then Return True
                        If tt1.Minute < tt2.Minute Then Return False

                        If tt1.Second > tt2.Second Then Return True
                        If tt1.Second < tt2.Second Then Return False

                        If tt1.Millisecond > tt2.Millisecond Then Return True
                        If tt1.Millisecond < tt2.Millisecond Then Return False

                        Return False
                    End Operator

                    Public Shared Operator <(tt1 As TimerTime, tt2 As TimerTime) As Boolean
                        Return If(tt1 = tt2, False, Not tt1 > tt2)
                    End Operator

                    Public Shared Operator >=(tt1 As TimerTime, tt2 As TimerTime) As Boolean
                        Return tt1 = tt2 OrElse tt1 > tt2
                    End Operator

                    Public Shared Operator <=(ByVal tt1 As TimerTime, ByVal tt2 As TimerTime) As Boolean
                        Return If(tt1 = tt2, False, Not tt1 >= tt2)
                    End Operator

                    Public Overrides Function ToString() As String
                        Return "[" +
                                    TimerTime.PadValue(mYear, 4) + ":" +
                                    TimerTime.PadValue(mMonth, 2) + ":" +
                                    TimerTime.PadValue(mDay, 2) + " " +
                                    TimerTime.PadValue(mHour, 2) + ":" +
                                    TimerTime.PadValue(mMinute, 2) + ":" +
                                    TimerTime.PadValue(mSecond, 2) + ":" +
                                    TimerTime.PadValue(mMillisecond, 2) +
                            "]"
                    End Function

                    Public Shared Function PadValue(value As Integer, size As Integer) As String
                        Dim strValue As String = value.ToString()
                        If strValue.Length > size Then Return strValue

                        Return StrDup(size - strValue.Length, "0") + strValue
                    End Function

                    Public Function ToXML() As XElement
                        Return <timerTime>
                                   <year><%= mYear %></year>
                                   <month><%= mMonth %></month>
                                   <day><%= mDay %></day>
                                   <hour><%= mHour %></hour>
                                   <minute><%= mMinute %></minute>
                                   <second><%= mSecond %></second>
                                   <millisecond><%= mMillisecond %></millisecond>
                               </timerTime>
                    End Function

                    Public Shared Function FromXML(xml As XElement) As TimerTime
                        Dim tt As TimerTime = New TimerTime()

                        Integer.TryParse(xml.<year>.Value, tt.Year)
                        Integer.TryParse(xml.<month>.Value, tt.Month)
                        Integer.TryParse(xml.<day>.Value, tt.Day)
                        Integer.TryParse(xml.<hour>.Value, tt.Hour)
                        Integer.TryParse(xml.<minute>.Value, tt.Minute)
                        Integer.TryParse(xml.<second>.Value, tt.Second)
                        Integer.TryParse(xml.<millisecond>.Value, tt.Millisecond)

                        Return tt
                    End Function

                    Public Shared Function FromMillisconds(value As Double) As TimerTime
                        Dim ts As TimeSpan = TimeSpan.FromMilliseconds(value)
                        Return New TimerTime(0, 0, ts.Days, ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds)

                        'Dim years As Integer = ts.Days / DaysInYear
                        'Dim months As Integer = (years * DaysInYear) - ts.Days / DaysInMonth
                        'Dim days As Integer = (years * DaysInYear) - (months * DaysInMonth) - ts.Days

                        'Return New TimerTime(years, months, ts.Days, ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds)
                    End Function

                    Public Function Clone() As Object Implements ICloneable.Clone
                        Dim newInstance As Reflection.MethodInfo = GetType(TimerTime).GetMethod("MemberwiseClone", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
                        Return newInstance?.Invoke(Me, Nothing)
                    End Function

                    Public Class TimerTimeConverter
                        Inherits ExpandableObjectConverter

                        Public Overrides Function CanConvertTo(context As ITypeDescriptorContext, destinationType As Type) As Boolean
                            Return If(destinationType Is GetType(TimerTime), True, MyBase.CanConvertTo(context, destinationType))
                        End Function

                        Public Overrides Function ConvertTo(ByVal context As ITypeDescriptorContext, ByVal culture As Globalization.CultureInfo, ByVal value As Object, ByVal destinationType As System.Type) As Object
                            If destinationType Is GetType(System.String) AndAlso TypeOf value Is TimerTime Then
                                Dim tt As TimerTime = CType(value, TimerTime)
                                Return tt.ToString()
                            End If

                            Return MyBase.ConvertTo(context, culture, value, destinationType)
                        End Function
                    End Class
                End Class

                Public Sub New(ByVal layer As Layer)
                    MyBase.New(layer)
                End Sub

                Public Overrides ReadOnly Property Type As IElement.TypeConstants
                    Get
                        Return IElement.TypeConstants.Timer
                    End Get
                End Property

                Public Overrides ReadOnly Property Name As String
                    Get
                        Return "Timer"
                    End Get
                End Property

                Public Overrides ReadOnly Property IsValid As Boolean
                    Get
                        Return True
                    End Get
                End Property

                <Category("Timer"),
                DisplayName("From")>
                Public Property FromTime As TimerTime = New TimerTime()

                <Category("Timer"),
                DisplayName("To")>
                Public Property ToTime As TimerTime = New TimerTime()

                <Category("Appearance"),
                Description("YYYY = Years, MMM = Months, DD = Days, HH = Hours, MM = Minutes, SS = Seconds, MS = Milliseconds")>
                Public Property Format As String = "HH:MM:SS"

                Public Overrides Function Render(g As Graphics, r As Rectangle, m As Drawing2D.Matrix) As KeyFrame
                    Dim bk As KeyFrame = MyBase.RenderBackground(g, m)

                    Dim timerText As String = Format

                    Dim currentTime As TimerTime = New TimerTime()
                    Dim eleapsedTime As TimerTime = TimerTime.FromMillisconds((MyBase.Layer.Track.CursorTime - Me.InitialKeyFrame.Time).TotalMilliseconds)

                    If FromTime + eleapsedTime < ToTime Then
                        currentTime = eleapsedTime
                    ElseIf FromTime - eleapsedTime > ToTime Then
                        currentTime = (FromTime - eleapsedTime)
                    Else
                        currentTime = ToTime
                    End If

                    timerText = timerText.Replace("YYYY", TimerTime.PadValue(currentTime.Year, 4))
                    timerText = timerText.Replace("MMM", TimerTime.PadValue(currentTime.Month, 2))
                    timerText = timerText.Replace("DD", TimerTime.PadValue(currentTime.Day, 2))
                    timerText = timerText.Replace("HH", TimerTime.PadValue(currentTime.Hour, 2))
                    timerText = timerText.Replace("MM", TimerTime.PadValue(currentTime.Minute, 2))
                    timerText = timerText.Replace("SS", TimerTime.PadValue(currentTime.Second, 2))
                    timerText = timerText.Replace("MS", TimerTime.PadValue(currentTime.Millisecond, 3))

                    Dim bmp As Bitmap = textRenderer.Render(timerText, bk)
                    If bmp IsNot Nothing Then
                        Dim txtRect As Rectangle = AlignContent(bk.Bounds, bk.Padding, textRenderer.TextSize, bk.ContentAlignment)
                        g.DrawImageUnscaled(bmp, txtRect.Location)
                    End If

                    Return bk
                End Function

                Public Overrides Function ToXML() As XElement
                    Dim xml = MyBase.ToXML()
                    xml.Add(
                            <propietary>
                                <format><%= Me.Format %></format>

                                <fromTime><%= Me.FromTime.ToXML() %></fromTime>
                                <toTime><%= Me.ToTime.ToXML() %></toTime>
                            </propietary>
                    )
                    Return xml
                End Function

                Public Overrides Sub SetFromXML(xml As XElement)
                    Me.Format = xml.<propietary>.<format>.Value

                    Me.FromTime = TimerTime.FromXML(xml.<propietary>.<fromTime>.<timerTime>(0))
                    Me.ToTime = TimerTime.FromXML(xml.<propietary>.<toTime>.<timerTime>(0))
                End Sub
            End Class
        End Class
    End Class
End Class
