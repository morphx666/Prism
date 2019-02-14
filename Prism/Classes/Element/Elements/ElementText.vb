Imports System.ComponentModel

Partial Public Class Project
	Partial Public Class Track
		Partial Public Class Layer
            <Serializable()>
            Public Class ElementText
                Inherits Element

                Private mText As String = ""
                Private textRenderer As TextRenderer = New TextRenderer()

                Public Event TextChanged(sender As ElementText)

                <CategoryAttribute("Appearance"),
                Editor(GetType(Design.MultilineStringEditor), GetType(Drawing.Design.UITypeEditor))>
                Public Property Text As String
                    Get
                        Return mText
                    End Get
                    Set(ByVal value As String)
                        If mText <> value Then
                            mText = value
                            ResetRenderer()
                            RaiseEvent TextChanged(Me)
                        End If
                    End Set
                End Property

                <CategoryAttribute("Appearance")>
                Public Property AllowWrapping As Boolean

                Public Sub New(layer As Layer)
                    MyBase.New(layer)
                End Sub

                Public Sub New(layer As Layer, text As String)
                    MyBase.New(layer)
                    Me.Text = text
                End Sub

                Public Overrides ReadOnly Property Type As IElement.TypeConstants
                    Get
                        Return IElement.TypeConstants.Text
                    End Get
                End Property

                Public Overrides ReadOnly Property Name As String
                    Get
                        Return "Text"
                    End Get
                End Property

                Public Overrides ReadOnly Property IsValid As Boolean
                    Get
                        Return True
                    End Get
                End Property

                Public Overridable Sub ResetRenderer()
                    textRenderer.ClearCache()
                End Sub

                Public Overrides Function Render(g As Graphics, r As Rectangle, m As Drawing2D.Matrix) As KeyFrame
                    Dim bk As KeyFrame = MyBase.RenderBackground(g, m)

                    Dim bmp As Bitmap = textRenderer.Render(mText, bk)
                    If bmp IsNot Nothing Then
                        Dim txtRect As Rectangle = AlignContent(bk.Bounds, bk.Padding, textRenderer.TextSize, bk.ContentAlignment)
                        g.DrawImageUnscaled(bmp, txtRect.Location)
                    End If

                    Return bk
                End Function

                Public Overrides Property SourceFile() As String
                    Get
                        Return MyBase.SourceFile
                    End Get
                    Set(ByVal value As String)
                        If MyBase.SourceFile = value Then Exit Property
                        MyBase.SourceFile = value

                        If IO.File.Exists(value) Then Me.Text = IO.File.ReadAllText(value)
                    End Set
                End Property

                Public Overrides Function ToXML() As XElement
                    Dim xml = MyBase.ToXML()
                    xml.Add(
                            <propietary>
                                <text><%= Me.Text %></text>
                                <allowWrapping><%= Me.AllowWrapping %></allowWrapping>
                            </propietary>
                    )
                    Return xml
                End Function

                Public Overrides Sub SetFromXML(xml As XElement)
                    Dim bool As Boolean
                    If Boolean.TryParse(xml.<propietary>.<allowWrapping>.Value, bool) Then Me.AllowWrapping = bool
                    Me.Text = xml.<propietary>.<text>.Value
                End Sub
            End Class
        End Class
    End Class
End Class
