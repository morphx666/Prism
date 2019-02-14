Imports System.ComponentModel
Imports Prism.Project.Track.Layer.Element
Imports System.Runtime.Serialization.Formatters.Binary

Partial Public Class Project
    Partial Public Class Track
        Partial Public Class Layer
            Partial Public Class Element
                <Serializable(),
                TypeConverter(GetType(KeyFramesConverter))>
                Public Class KeyFrame
                    Implements ICloneable

                    Private mChromaKeyColor As ChromaKey = New ChromaKey()
                    Private mForeColor As Brush2 = New Brush2(Color.Black)
                    Private mBackColor As Brush2 = New Brush2(Color.White)
                    Private mFont As Font = New Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Point)
                    Private mBounds As Rectangle = New Rectangle(0, 0, 80, 20)
                    Private mBorderColor As Color = Color.Black
                    Private mBorderRadius As Integer = 0
                    Private mBorderSize As Integer = 0
                    Private mRotation As Single = 0
                    Private mContentAlignment As ContentAlignment = ContentAlignment.MiddleCenter
                    Private mPadding As Padding = Padding.Empty

                    Private mTime As TimeSpan = New TimeSpan(0)
                    Private mElement As Element = Nothing

                    <Serializable(),
                    TypeConverter(GetType(ChromaKeyConverter))>
                    Public Class ChromaKey
                        Implements ICloneable

                        Public Property Red As IntRange = New IntRange(0, 255)
                        Public Property Blue As IntRange = New IntRange(0, 255)
                        Public Property Green As IntRange = New IntRange(0, 255)
                        Public Property Enabled As Boolean = False

                        <Serializable(), TypeConverter(GetType(IntRangeConverter))>
                        Public Class IntRange
                            Public Property Max As Integer
                            Public Property Min As Integer

                            Public Sub New(min As Integer, max As Integer)
                                Me.Max = max
                                Me.Min = min
                            End Sub

                            Public Function ToAForgeIntRange() As AForge.IntRange
                                Return New AForge.IntRange(Min, Max)
                            End Function

                            Public Function ToXML() As XElement
                                Return <intRange>
                                           <min><%= Me.Min %></min>
                                           <max><%= Me.Max %></max>
                                       </intRange>
                            End Function

                            Public Shared Function FromXML(xml As XElement) As IntRange
                                Return New IntRange(xml.<min>.Value, xml.<max>.Value)
                            End Function

                            Public Shared Operator =(ir1 As IntRange, ir2 As IntRange) As Boolean
                                Return ir1.Min = ir1.Min AndAlso ir1.Max = ir2.Max
                            End Operator

                            Public Shared Operator <>(ir1 As IntRange, ir2 As IntRange) As Boolean
                                Return Not ir1 = ir2
                            End Operator
                        End Class

                        Public Function Clone() As Object Implements ICloneable.Clone
                            Dim ck As New ChromaKey With {
                                .Red = Red,
                                .Green = Green,
                                .Blue = Blue,
                                .Enabled = Enabled
                            }
                            Return ck
                        End Function

                        Public Function ToXML() As XElement
                            Return <chromaKeyColor>
                                       <Red><%= Me.Red.ToXML() %></Red>
                                       <Blue><%= Me.Blue.ToXML() %></Blue>
                                       <Green><%= Me.Green.ToXML() %></Green>
                                       <enabled><%= Me.Enabled %></enabled>
                                   </chromaKeyColor>
                        End Function

                        Public Shared Function FromXML(xml As XElement) As ChromaKey
                            Dim ck As New ChromaKey With {
                                .Red = IntRange.FromXML(xml.<Red>.<intRange>(0)),
                                .Green = IntRange.FromXML(xml.<Green>.<intRange>(0)),
                                .Blue = IntRange.FromXML(xml.<Blue>.<intRange>(0))
                            }

                            Dim bool As Boolean
                            If Boolean.TryParse(xml.<enabled>.Value, bool) Then ck.Enabled = bool

                            Return ck
                        End Function

                        Public Shared Operator =(ck1 As ChromaKey, ck2 As ChromaKey) As Boolean
                            Return ck1.Enabled = ck2.Enabled AndAlso
                                    ck1.Red = ck2.Red AndAlso
                                    ck1.Green = ck2.Green AndAlso
                                    ck1.Blue = ck2.Blue
                        End Operator

                        Public Shared Operator <>(ck1 As ChromaKey, ck2 As ChromaKey) As Boolean
                            Return Not ck1 = ck2
                        End Operator
                    End Class

                    <NonSerialized()>
                    Public Event KeyFrameChanged(sender As KeyFrame)
                    <NonSerialized()>
                    Public Event TimeChanged(sender As KeyFrame, oldTime As TimeSpan, newTime As TimeSpan)

                    Public Sub New(element As Element)
                        mElement = element
                    End Sub

                    <Category("Appearance")>
                    Public Property ChromaKeyColor As ChromaKey
                        Get
                            Return mChromaKeyColor
                        End Get
                        Set(value As ChromaKey)
                            mChromaKeyColor = value
                            RaiseEvent KeyFrameChanged(Me)
                        End Set
                    End Property

                    <Category("Appearance")>
                    Public Property Description As String = ""

                    <Category("Appearance")>
                    Public Property ForeColor As Brush2
                        Get
                            Return mForeColor
                        End Get
                        Set(value As Brush2)
                            mForeColor = value
                            RaiseEvent KeyFrameChanged(Me)
                        End Set
                    End Property

                    <Category("Appearance")>
                    Public Property BackColor As Brush2
                        Get
                            Return mBackColor
                        End Get
                        Set(value As Brush2)
                            mBackColor = value
                            RaiseEvent KeyFrameChanged(Me)
                        End Set
                    End Property

                    <Category("Appearance")>
                    Public Property Font As Font
                        Get
                            Return mFont
                        End Get
                        Set(value As Font)
                            mFont = value
                            RaiseEvent KeyFrameChanged(Me)
                        End Set
                    End Property

                    <Category("Appearance")>
                    Public Property Bounds As Rectangle
                        Get
                            Return mBounds
                        End Get
                        Set(value As Rectangle)
                            If mBounds <> value Then
                                mBounds = value
                                RaiseEvent KeyFrameChanged(Me)
                            End If
                        End Set
                    End Property

                    <Category("Appearance")>
                    Public Property BorderColor As Color
                        Get
                            Return mBorderColor
                        End Get
                        Set(value As Color)
                            If mBorderColor <> value Then
                                mBorderColor = value
                                RaiseEvent KeyFrameChanged(Me)
                            End If
                        End Set
                    End Property

                    <Category("Appearance")>
                    Public Property BorderRadius As Integer
                        Get
                            Return mBorderRadius
                        End Get
                        Set(value As Integer)
                            If mBorderRadius <> value Then
                                mBorderRadius = value
                                RaiseEvent KeyFrameChanged(Me)
                            End If
                        End Set
                    End Property

                    <Category("Appearance")>
                    Public Property BorderSize As Integer
                        Get
                            Return mBorderSize
                        End Get
                        Set(value As Integer)
                            If mBorderSize <> value Then
                                mBorderSize = value
                                RaiseEvent KeyFrameChanged(Me)
                            End If
                        End Set
                    End Property

                    <Category("Appearance")>
                    Public Property Rotation As Single
                        Get
                            Return mRotation
                        End Get
                        Set(value As Single)
                            If mRotation <> value Then
                                mRotation = value
                                RaiseEvent KeyFrameChanged(Me)
                            End If
                        End Set
                    End Property

                    <Category("Appearance")>
                    Public Property ContentAlignment As ContentAlignment
                        Get
                            Return mContentAlignment
                        End Get
                        Set(value As ContentAlignment)
                            If mContentAlignment <> value Then
                                mContentAlignment = value
                                RaiseEvent KeyFrameChanged(Me)
                            End If
                        End Set
                    End Property

                    <Category("Appearance")>
                    Public Property Padding As Padding
                        Get
                            Return mPadding
                        End Get
                        Set(value As Padding)
                            If mPadding <> value Then
                                mPadding = value
                                RaiseEvent KeyFrameChanged(Me)
                            End If
                        End Set
                    End Property

                    <Category("Behavior")>
                    Public Property Time As TimeSpan
                        Get
                            Return mTime
                        End Get
                        Set(value As TimeSpan)
                            If mTime <> value Then
                                RaiseEvent TimeChanged(Me, mTime, value)
                                mTime = value
                            End If
                        End Set
                    End Property

                    <Browsable(False)>
                    Public Property Element As Element
                        Get
                            Return mElement
                        End Get
                        Set(value As Element)
                            mElement = value
                        End Set
                    End Property

                    Public Overrides Function ToString() As String
                        Return String.Format("{0} {1}", Time.ToString(), Me.Description)
                    End Function

                    Public Function Clone() As Object Implements ICloneable.Clone
                        Dim newInstance As Reflection.MethodInfo =
                                GetType(KeyFrame).GetMethod("MemberwiseClone",
                                                                 Reflection.BindingFlags.Instance Or
                                                                 Reflection.BindingFlags.NonPublic)
                        If newInstance IsNot Nothing Then
                            Dim kf As KeyFrame = newInstance.Invoke(Me, Nothing)
                            kf.BackColor = kf.BackColor.Clone()
                            kf.ForeColor = kf.ForeColor.Clone()
                            kf.Font = kf.Font.Clone()
                            kf.ChromaKeyColor = kf.ChromaKeyColor.Clone()

                            'kf.Element.ClearBlendedKeyFramesCache()

                            Return kf
                        Else
                            Return Nothing
                        End If
                    End Function

                    Public Function ToXML() As XElement
                        Return <keyframe>
                                   <description><%= Me.Description %></description>
                                   <time><%= Me.Time.TotalMilliseconds %></time>
                                   <%= Me.ChromaKeyColor.ToXML() %>

                                   <foreColor><%= Me.ForeColor.ToXML() %></foreColor>
                                   <backColor><%= Me.BackColor.ToXML() %></backColor>

                                   <font>
                                       <family><%= Me.Font.FontFamily.Name %></family>
                                       <size><%= Me.Font.SizeInPoints %></size>
                                       <style><%= Me.Font.Style %></style>
                                   </font>

                                   <bounds><%= Me.Bounds %></bounds>

                                   <borderColor><%= Me.BorderColor.ToArgb() %></borderColor>
                                   <borderSize><%= Me.BorderSize %></borderSize>
                                   <borderRadius><%= Me.BorderRadius %></borderRadius>

                                   <rotation><%= Me.Rotation %></rotation>
                                   <contentAlignment><%= Me.ContentAlignment %></contentAlignment>
                                   <padding><%= Me.Padding %></padding>
                               </keyframe>
                    End Function

                    Public Shared Function FromXML(element As Element, xml As XElement) As KeyFrame
                        Dim kf As New KeyFrame(element) With {
                            .Description = xml.<description>.Value,
                            .Time = TimeSpan.FromMilliseconds(xml.<time>.Value),
                            .ChromaKeyColor = ChromaKey.FromXML(xml.<chromaKeyColor>(0)),
                            .ForeColor = Brush2.FromXML(xml.<foreColor>.<brush2>(0)),
                            .BackColor = Brush2.FromXML(xml.<backColor>.<brush2>(0)),
                            .Font = Project.Track.Layer.Element.XMLToFont(xml.<font>(0)),
                            .Bounds = Project.Track.Layer.Element.ParseString(Of Rectangle)(xml.<bounds>.Value),
                            .BorderColor = Color.FromArgb(xml.<borderColor>.Value),
                            .BorderSize = xml.<borderSize>.Value,
                            .BorderRadius = xml.<borderRadius>.Value,
                            .Rotation = xml.<rotation>.Value
                        }

                        Dim ca As ContentAlignment = ContentAlignment.MiddleCenter
                        If [Enum].TryParse(xml.<contentAlignment>.Value, ca) Then kf.ContentAlignment = ca

                        kf.Padding = Project.Track.Layer.Element.ParseString(Of Padding)(xml.<padding>.Value)

                        Return kf
                    End Function

                    Public Shared Operator =(kf1 As KeyFrame, kf2 As KeyFrame) As Boolean
                        Return IsEqual(kf1, kf2, False)
                    End Operator

                    Public Shared Operator <>(kf1 As KeyFrame, kf2 As KeyFrame) As Boolean
                        Return Not kf1 = kf2
                    End Operator

                    Public Shared Function IsEqual(kf1 As KeyFrame, kf2 As KeyFrame, Optional ignoreTime As Boolean = False) As Boolean
                        Return kf1.BackColor = kf2.BackColor AndAlso
                                kf1.ForeColor = kf2.ForeColor AndAlso
                                kf1.ContentAlignment = kf2.ContentAlignment AndAlso
                                kf1.Font.Equals(kf2.Font) AndAlso
                                kf1.Padding = kf2.Padding AndAlso
                                kf1.BorderColor = kf2.BorderColor AndAlso
                                kf1.BorderRadius = kf2.BorderRadius AndAlso
                                kf1.BorderSize = kf2.BorderSize AndAlso
                                kf1.Bounds = kf2.Bounds AndAlso
                                kf1.Rotation = kf2.Rotation AndAlso
                                kf1.ChromaKeyColor = kf2.ChromaKeyColor AndAlso
                                IIf(ignoreTime, True, kf1.Time = kf2.Time)
                    End Function
                End Class
            End Class
        End Class
    End Class
End Class

Public Class IntRangeConverter
    Inherits ExpandableObjectConverter

    Private intRange As KeyFrame.ChromaKey.IntRange

    Public Overrides Function CanConvertTo(context As ITypeDescriptorContext, destinationType As Type) As Boolean
        Return If(destinationType Is GetType(KeyFrame.ChromaKey.IntRange), True, MyBase.CanConvertTo(context, destinationType))
    End Function

    Public Overrides Function ConvertTo(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object, destinationType As Type) As Object
        If destinationType Is GetType(String) AndAlso TypeOf value Is KeyFrame.ChromaKey.IntRange Then
            intRange = CType(value, KeyFrame.ChromaKey.IntRange)

            Return String.Format("{0}, {1}", intRange.Min, intRange.Max)
        End If

        Return MyBase.ConvertTo(context, culture, value, destinationType)
    End Function

    Public Overrides Function ConvertFrom(context As ITypeDescriptorContext, ByVal culture As Globalization.CultureInfo, value As Object) As Object
        If TypeOf value Is String Then
            Try
                Dim s As String = CType(value, String)
                Dim min As Integer = s.Split(",")(0)
                Dim max As Integer = s.Split(",")(1)
                Return New KeyFrame.ChromaKey.IntRange(min, max)
            Catch ex As Exception
                Throw New Exception(ex.Message)
            End Try
        End If

        Return MyBase.ConvertFrom(context, culture, value)
    End Function

    Public Overrides Function CanConvertFrom(ByVal context As ITypeDescriptorContext, sourceType As Type) As Boolean
        Return If(sourceType Is GetType(String), True, MyBase.CanConvertFrom(context, sourceType))
    End Function
End Class

Public Class ChromaKeyConverter
    Inherits ExpandableObjectConverter

    Public Overrides Function CanConvertTo(context As ITypeDescriptorContext, destinationType As Type) As Boolean
        Return If(destinationType Is GetType(KeyFrame.ChromaKey), True, MyBase.CanConvertTo(context, destinationType))
    End Function

    Public Overrides Function ConvertTo(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object, destinationType As Type) As Object
        If destinationType Is GetType(String) AndAlso TypeOf value Is KeyFrame.ChromaKey Then
            Dim chromaKey As KeyFrame.ChromaKey = CType(value, KeyFrame.ChromaKey)

            Return If(chromaKey.Enabled, "Enabled", "Disabled")
        End If

        Return MyBase.ConvertTo(context, culture, value, destinationType)
    End Function

    Public Overloads Overrides Function ConvertFrom(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object) As Object
        Return If(TypeOf value Is String, "ConvertFrom", MyBase.ConvertFrom(context, culture, value))
    End Function

    Public Overrides Function CanConvertFrom(context As ITypeDescriptorContext, sourceType As Type) As Boolean
        Return If(sourceType Is GetType(String), True, MyBase.CanConvertFrom(context, sourceType))
    End Function
End Class

Public Class KeyFramesConverter
    Inherits ExpandableObjectConverter

    Private selectedKeyFrame As KeyFrame

    Public Overrides Function CanConvertTo(context As ITypeDescriptorContext, destinationType As Type) As Boolean
        Return If(destinationType Is GetType(KeyFrame), True, MyBase.CanConvertTo(context, destinationType))
    End Function

    Public Overloads Overrides Function ConvertTo(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object, destinationType As Type) As Object
        If destinationType Is GetType(String) AndAlso TypeOf value Is KeyFrame Then
            selectedKeyFrame = CType(value, KeyFrame)

            Return selectedKeyFrame.ToString()
        End If

        Return MyBase.ConvertTo(context, culture, value, destinationType)
    End Function

    Public Overloads Overrides Function ConvertFrom(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object) As Object
        If TypeOf value Is String Then
            Dim description As String = CType(value, String)
            selectedKeyFrame.Description = description.Replace(selectedKeyFrame.Time.ToString() + " ", "")
            Return selectedKeyFrame
        End If

        Return MyBase.ConvertFrom(context, culture, value)
    End Function

    Public Overrides Function CanConvertFrom(context As ITypeDescriptorContext, sourceType As Type) As Boolean
        Return If(sourceType Is GetType(String), True, MyBase.CanConvertFrom(context, sourceType))
    End Function
End Class