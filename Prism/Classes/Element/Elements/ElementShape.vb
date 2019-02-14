Imports System.ComponentModel

Partial Public Class Project
    Partial Public Class Track
        Partial Public Class Layer
            <Serializable()>
            Public Class ElementShape
                Inherits Element

                Public Enum ShapeTypeConstants
                    Rectangle
                    Circle
                    Polygon
                End Enum

                <Category("Appearance"),
                DisplayName("Shape Type")>
                Public Property ShapeType As ShapeTypeConstants

                <Category("Appearance")>
                Public Property Sides As Integer = 3

                Public Sub New(ByVal layer As Layer)
                    MyBase.New(layer)
                End Sub

                Public Overrides ReadOnly Property Name As String
                    Get
                        Return "Shape"
                    End Get
                End Property

                Public Overrides ReadOnly Property Type As IElement.TypeConstants
                    Get
                        Return IElement.TypeConstants.Shape
                    End Get
                End Property

                Public Overrides ReadOnly Property IsValid As Boolean
                    Get
                        Return True
                    End Get
                End Property

                Public Overrides Function Render(g As Graphics, r As Rectangle, m As Drawing2D.Matrix) As KeyFrame
                    Dim bk As KeyFrame = MyBase.RenderBackground(g, m)

                    Select Case ShapeType
                        Case ShapeTypeConstants.Rectangle
                            DrawRectangle(g, bk)
                        Case ShapeTypeConstants.Circle
                            DrawCircle(g, bk)
                        Case ShapeTypeConstants.Polygon
                            If Sides > 2 Then DrawNSidedPolygon(g, bk)
                    End Select

                    Return bk
                End Function

                Private Sub DrawRectangle(g As Graphics, bk As KeyFrame)
                    Using b As Brush = bk.BackColor.ToBrush()
                        If bk.BorderRadius <> 0 Then
                            g.FillRoundedRectangle(b, bk.Bounds, bk.BorderRadius)
                        Else
                            g.FillRectangle(b, bk.Bounds)
                        End If
                    End Using

                    If bk.BorderSize > 0 Then
                        Using p As Pen = New Pen(bk.BorderColor, bk.BorderSize)
                            Dim br As Rectangle = bk.Bounds
                            br.Inflate(bk.BorderSize / 2, bk.BorderSize / 2)
                            If bk.BorderRadius <> 0 Then
                                g.DrawRoundedRectangle(p, br, bk.BorderRadius)
                            Else
                                g.DrawRectangle(p, br)
                            End If
                        End Using
                    End If
                End Sub

                Private Sub DrawCircle(g As Graphics, bk As KeyFrame)
                    Using b As Brush = bk.BackColor.ToBrush()
                        If bk.BorderRadius <> 0 Then
                            g.FillEllipse(b, bk.Bounds)
                        Else
                            g.FillEllipse(b, bk.Bounds)
                        End If
                    End Using

                    If bk.BorderSize > 0 Then
                        Dim br As Rectangle = bk.Bounds
                        br.Inflate(bk.BorderSize / 2, bk.BorderSize / 2)
                        Using p As Pen = New Pen(bk.BorderColor, bk.BorderSize)
                            If bk.BorderRadius <> 0 Then
                                g.DrawEllipse(p, br)
                            Else
                                g.DrawEllipse(p, br)
                            End If
                        End Using
                    End If
                End Sub

                Private Sub DrawNSidedPolygon(g As Graphics, bk As KeyFrame)
                    Const toRad As Double = Math.PI / 180
                    Dim p1 As PointF = PointF.Empty
                    Dim p2 As PointF = PointF.Empty
                    Dim a As Double = -90
                    Dim offset As Point = New Point(bk.Bounds.X + bk.Bounds.Width / 2, bk.Bounds.Y + bk.Bounds.Height / 2)
                    Dim size As Size = New Size(bk.Bounds.Width / 2 + bk.BorderSize, bk.Bounds.Height / 2 + bk.BorderSize)
                    Dim points(Sides) As PointF

                    p1.X = offset.X + size.Width * Math.Cos(a * toRad)
                    p1.Y = offset.Y + size.Height * Math.Sin(a * toRad)

                    points(0) = p1

                    For n As Integer = 1 To Sides
                        a += 360 / Sides

                        p2.X = offset.X + size.Width * Math.Cos(a * toRad)
                        p2.Y = offset.Y + size.Height * Math.Sin(a * toRad)

                        points(n) = p2
                        p1 = p2
                    Next

                    Using b As Brush = bk.BackColor.ToBrush()
                        g.FillPolygon(b, points)
                    End Using

                    If bk.BorderSize > 0 Then
                        Using p As Pen = New Pen(bk.BorderColor, bk.BorderSize)
                            g.DrawPolygon(p, points)
                        End Using
                    End If
                End Sub

                Public Overrides Function ToXML() As XElement
                    Dim xml = MyBase.ToXML()
                    xml.Add(
                            <propietary>
                                <shapeType><%= Me.ShapeType %></shapeType>
                                <sides><%= Me.Sides %></sides>
                            </propietary>
                    )
                    Return xml
                End Function

                Public Overrides Sub SetFromXML(xml As XElement)
                    Dim t As ShapeTypeConstants = ShapeTypeConstants.Rectangle
                    If [Enum].TryParse(xml.<propietary>.<shapeType>.Value, t) Then Me.ShapeType = t

                    Integer.TryParse(xml.<propietary>.<sides>.Value, Me.Sides)
                End Sub
            End Class
        End Class
    End Class
End Class