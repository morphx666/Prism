Imports System.ComponentModel

<Serializable(), TypeConverter(GetType(Brush2.Brush2Converter))>
Public Class Brush2
    Implements ICloneable

    Public Enum TypeConstants
        Solid
        Gradient
    End Enum

    Public Property Color1 As Color = Color.Black
    Public Property Color2 As Color = Color.Black
    Public Property Point1 As Point = New Point(0, 0)
    Public Property Point2 As Point = New Point(0, 0)
    Public Property Type As TypeConstants = TypeConstants.Solid

    Private mSmoothing As Single = 0.5

    Public Sub New()
    End Sub

    Public Sub New(ByVal color As Color)
        Me.Color1 = color
        Me.Color2 = color
        Me.Type = TypeConstants.Solid
    End Sub

    Public Sub New(color1 As Color, point1 As Point, color2 As Color, point2 As Point)
        Me.Color1 = color1
        Me.Point1 = point1
        Me.Color2 = color2
        Me.Point2 = point2
        Me.Type = TypeConstants.Solid
    End Sub

    <Category("Appearance")>
    Public Property Smoothing As Single
        Get
            Return mSmoothing
        End Get
        Set(ByVal value As Single)
            mSmoothing = Math.Min(Math.Max(value, 0), 1)
        End Set
    End Property

    Public Function ToSolidBrush() As SolidBrush
        Return New SolidBrush(Color1)
    End Function

    Public Function ToLinearGradientBrush() As Drawing2D.LinearGradientBrush
        Return If(Point1 = Point2 AndAlso Point1 = Point.Empty,
            New Drawing2D.LinearGradientBrush(Point1, New Point(1, 1), Color1, Color2),
            New Drawing2D.LinearGradientBrush(Point1, Point2, Color1, Color2))
    End Function

    Public Function ToColor() As Color
        Return Color1
    End Function

    Public Function ToBrush() As Brush
        If Me.Type = TypeConstants.Solid Then
            Return ToSolidBrush()
        Else
            Dim lgb = ToLinearGradientBrush()
            lgb.SetBlendTriangularShape(mSmoothing)
            lgb.SetSigmaBellShape(mSmoothing)
            Return lgb
        End If
    End Function

    Public Overrides Function ToString() As String
        Dim s = If(Me.Type = TypeConstants.Solid,
            String.Format("Color1={0}, Type={1}",
                                        Color1.ToString().Split(" "c)(1),
                                        Type.ToString()),
            String.Format("Color1={0}, Point1={1},  Color2={2}, Point2={3}, Type={4}",
                                        Color1.ToString().Split(" "c)(1),
                                        Point1.ToString(),
                                        Color2.ToString().Split(" "c)(1),
                                        Point2.ToString(),
                                        Type.ToString()))

        s = "[" + s.Replace("[", "").Replace("]", "") + "]"
        Return s
    End Function

    Public Function Clone() As Object Implements ICloneable.Clone
        Dim newInstance As Reflection.MethodInfo = GetType(Brush2).GetMethod("MemberwiseClone", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
        Return newInstance?.Invoke(Me, Nothing)
    End Function

    Public Function ToXML() As XElement
        Return <brush2>
                   <color1><%= Me.Color1.ToArgb() %></color1>
                   <point1><%= Me.Point1 %></point1>
                   <color2><%= Me.Color2.ToArgb() %></color2>
                   <point2><%= Me.Point2 %></point2>
                   <type><%= Me.Type %></type>
                   <smoothing><%= Me.Smoothing %></smoothing>
               </brush2>
    End Function

    Public Shared Function FromXML(ByVal xml As XElement) As Brush2
        Dim b As New Brush2 With {
            .Color1 = Color.FromArgb(xml.<color1>.Value),
            .Point1 = Project.Track.Layer.Element.ParseString(Of Point)(xml.<point1>.Value),
            .Color2 = Color.FromArgb(xml.<color2>.Value),
            .Point2 = Project.Track.Layer.Element.ParseString(Of Point)(xml.<point2>.Value),
            .Smoothing = xml.<smoothing>.Value
        }

        Dim type As TypeConstants = TypeConstants.Solid
        If [Enum].TryParse(xml.<type>.Value, type) Then b.Type = type

        Return b
    End Function

    Public Shared Operator =(b1 As Brush2, b2 As Brush2) As Boolean
        Dim r = b1.Color1 = b2.Color1 AndAlso
               b1.Color2 = b2.Color2 AndAlso
               b1.Point1 = b2.Point1 AndAlso
               b1.Point2 = b2.Point2 AndAlso
               b1.Type = b2.Type AndAlso
               b1.Smoothing = b2.Smoothing

        Return r
    End Operator

    Public Shared Operator <>(b1 As Brush2, b2 As Brush2) As Boolean
        Return Not b1 = b2
    End Operator

    Public Class Brush2Converter
        Inherits ExpandableObjectConverter

        Public Overrides Function CanConvertTo(context As ITypeDescriptorContext, destinationType As Type) As Boolean
            Return If(destinationType Is GetType(Brush2),
                        True,
                        MyBase.CanConvertTo(context, destinationType))
        End Function

        Public Overrides Function ConvertTo(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object, destinationType As Type) As Object
            If destinationType Is GetType(String) AndAlso TypeOf value Is Brush2 Then
                Dim so As Brush2 = CType(value, Brush2)
                Return so.ToString()
            End If

            Return MyBase.ConvertTo(context, culture, value, destinationType)
        End Function
    End Class
End Class
