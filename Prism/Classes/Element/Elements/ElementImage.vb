Imports Imaging
Imports System.ComponentModel
Imports System.Drawing.Imaging

Partial Public Class Project
    Partial Public Class Track
        Partial Public Class Layer
            <Serializable()>
            Public Class ElementImage
                Inherits Element

                Public Enum ZoomModeConstants
                    None
                    Zoom
                    ZoomWithAspectRatio
                End Enum

                <NonSerialized()>
                Private mImage As DirectBitmap
                <NonSerialized()>
                Private mImgAttributes As ImageAttributes
                <NonSerialized()>
                Private afColorFiltering As AForge.Imaging.Filters.ColorFiltering = New AForge.Imaging.Filters.ColorFiltering()

                Private imgAlphaIsReset As Boolean = False

                <Category("Appearance"), DisplayName("Zoom Mode")>
                Public Property ZoomMode As ZoomModeConstants = ZoomModeConstants.ZoomWithAspectRatio

                Public Sub New(ByVal layer As Layer)
                    MyBase.New(layer)
                End Sub

                Public Sub New(ByVal layer As Layer, ByVal sourceFile As String)
                    MyBase.New(layer)
                    Me.SourceFile = sourceFile
                End Sub

                Public Overrides ReadOnly Property Type As IElement.TypeConstants
                    Get
                        Return IElement.TypeConstants.Image
                    End Get
                End Property

                Public Overrides ReadOnly Property Name As String
                    Get
                        Return "Image"
                    End Get
                End Property

                Public Overrides ReadOnly Property IsValid As Boolean
                    Get
                        Return True
                    End Get
                End Property

                Protected ReadOnly Property ImageSizeWithZoom(kf As KeyFrame) As Size
                    Get
                        Dim imageSize As Size

                        Select Case ZoomMode
                            Case ZoomModeConstants.None
                                imageSize = mImage.Size
                            Case ZoomModeConstants.Zoom
                                imageSize = kf.Bounds.Size
                            Case ZoomModeConstants.ZoomWithAspectRatio
                                imageSize = mImage.Size
                                Dim ar As Single = imageSize.Width / imageSize.Height

                                If imageSize.Width > imageSize.Height Then
                                    imageSize.Width = kf.Bounds.Width
                                    imageSize.Height = imageSize.Width / ar
                                ElseIf imageSize.Height > imageSize.Width Then
                                    imageSize.Height = kf.Bounds.Height
                                    imageSize.Width = imageSize.Height * ar
                                End If

                                If imageSize.Width > kf.Bounds.Width Then
                                    imageSize.Width = kf.Bounds.Width
                                    imageSize.Height = imageSize.Width / ar
                                ElseIf imageSize.Height > kf.Bounds.Height Then
                                    imageSize.Height = kf.Bounds.Height
                                    imageSize.Width = imageSize.Height * ar
                                End If
                        End Select

                        Return imageSize
                    End Get
                End Property

                Public Overrides Function Render(g As Graphics, r As Rectangle, m As Drawing2D.Matrix) As KeyFrame
                    Dim bk As KeyFrame = MyBase.RenderBackground(g, m)

                    Try
                        If mImage IsNot Nothing Then
                            Dim imageSize As Size = ImageSizeWithZoom(bk)

                            Dim tr As Rectangle = AlignContent(bk.Bounds, bk.Padding, imageSize, bk.ContentAlignment)

                            If bk.ChromaKeyColor.Enabled Then
                                afColorFiltering.FillColor = New AForge.Imaging.RGB(Color.Transparent)
                                afColorFiltering.Red = bk.ChromaKeyColor.Red.ToAForgeIntRange()
                                afColorFiltering.Blue = bk.ChromaKeyColor.Blue.ToAForgeIntRange()
                                afColorFiltering.Green = bk.ChromaKeyColor.Green.ToAForgeIntRange()

                                Using bmp As Bitmap = afColorFiltering.Apply(mImage)
#If USE_BITBLT Then
                                Select Case ZoomMode
                                    Case ZoomModeConstants.None
                                        g.DrawImageFast(mImage, tr.Location)
                                    Case ZoomModeConstants.Zoom
                                        g.DrawImageFast(bmp, tr, 0, 0, mImage.Size.Width, mImage.Size.Height)
                                    Case ZoomModeConstants.ZoomWithAspectRatio
                                        g.DrawImageFast(bmp, New Rectangle(tr.X, tr.Y, tr.Width, tr.Height), 0, 0, mImage.Size.Width, mImage.Size.Height)
                                End Select
#Else
                                    Select Case ZoomMode
                                        Case ZoomModeConstants.None
                                            g.DrawImageUnscaled(bmp, New Rectangle(tr.Location, imageSize))
                                        Case ZoomModeConstants.Zoom
                                            g.DrawImage(bmp, tr, 0, 0, mImage.Size.Width, mImage.Size.Height, GraphicsUnit.Pixel, mImgAttributes)
                                        Case ZoomModeConstants.ZoomWithAspectRatio
                                            g.DrawImage(bmp, New Rectangle(tr.Location, imageSize), 0, 0, mImage.Size.Width, mImage.Size.Height, GraphicsUnit.Pixel, mImgAttributes)
                                    End Select
#End If
                                End Using
                            Else
#If USE_BITBLT Then
                            Select Case ZoomMode
                                Case ZoomModeConstants.None
                                    g.DrawImageFast(mImage, New Rectangle(tr.Location, imageSize))
                                Case ZoomModeConstants.Zoom
                                    g.DrawImageFast(mImage, tr, 0, 0, mImage.Size.Width, mImage.Size.Height)
                                Case ZoomModeConstants.ZoomWithAspectRatio
                                    g.DrawImageFast(mImage, New Rectangle(tr.Location, imageSize), New Rectangle(Point.Empty, mImage.Size))
                            End Select
#Else
                                Select Case ZoomMode
                                    Case ZoomModeConstants.None
                                        g.DrawImageUnscaled(mImage, New Rectangle(tr.Location, imageSize))
                                    Case ZoomModeConstants.Zoom
                                        g.DrawImage(mImage.Bitmap, tr, 0, 0, mImage.Size.Width, mImage.Size.Height, GraphicsUnit.Pixel, mImgAttributes)
                                    Case ZoomModeConstants.ZoomWithAspectRatio
                                        g.DrawImage(mImage.Bitmap, New Rectangle(tr.Location, imageSize), 0, 0, mImage.Size.Width, mImage.Size.Height, GraphicsUnit.Pixel, mImgAttributes)
                                End Select
#End If
                            End If
                        End If
                    Catch
                    End Try

                    Return bk
                End Function

                <Browsable(False)>
                Public Property Image As Bitmap
                    Get
                        Return mImage
                    End Get
                    Set(ByVal value As Bitmap)
                        mImage = value
                    End Set
                End Property

                Public Overrides Property SourceFile() As String
                    Get
                        Return MyBase.SourceFile
                    End Get
                    Set(ByVal value As String)
                        If MyBase.SourceFile = value Then Exit Property

                        MyBase.SourceFile = value

                        If (Me.Type = IElement.TypeConstants.Image OrElse Me.Type = IElement.TypeConstants.ImageSequence) AndAlso IO.File.Exists(value) Then
                            Dim tmpBmp As Bitmap = Prism.Project.LoadBitmap(value)
                            If tmpBmp IsNot Nothing Then
                                'If mImage IsNot Nothing Then mImage.Dispose()
                                mImage = tmpBmp
                            End If
                        End If
                    End Set
                End Property

                <Browsable(False)>
                Public ReadOnly Property ImageAttributes As ImageAttributes
                    Get
                        Return mImgAttributes
                    End Get
                End Property

                Public Overloads Sub ApplyAlpha(alpha As Integer)
                    Dim ptsArray = {
                        New Single() {1, 0, 0, 0, 0},
                        New Single() {0, 1, 0, 0, 0},
                        New Single() {0, 0, 1, 0, 0},
                        New Single() {0, 0, 0, alpha / 255, 0},
                        New Single() {0, 0, 0, 0, 1}
                    }
                    Dim clrMatrix As ColorMatrix = New ColorMatrix(ptsArray)
                    If mImgAttributes IsNot Nothing Then mImgAttributes.Dispose()
                    mImgAttributes = New ImageAttributes()
                    mImgAttributes.SetColorMatrix(clrMatrix, ColorMatrixFlag.Default)

                    imgAlphaIsReset = (alpha = 255)
                End Sub

                Public Overloads Sub ResetImageAlpha()
                    If Not imgAlphaIsReset Then ApplyAlpha(255)
                End Sub

                Public Overrides Function ToXML() As XElement
                    Dim xml = MyBase.ToXML()
                    xml.Add(
                            <propietary>
                                <zoomMode><%= Me.ZoomMode %></zoomMode>
                            </propietary>
                    )
                    Return xml
                End Function

                Public Overrides Sub SetFromXML(xml As XElement)
                    Dim zm As ZoomModeConstants = ZoomModeConstants.None
                    If [Enum].TryParse(xml.<propietary>.<zoomMode>.Value, zm) Then Me.ZoomMode = zm
                End Sub
            End Class
        End Class
    End Class
End Class
