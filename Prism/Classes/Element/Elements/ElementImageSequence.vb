Imports Imaging
Imports System.ComponentModel

Partial Public Class Project
    Partial Public Class Track
        Partial Public Class Layer
            <Serializable()>
            Public Class ElementImageSequence
                Inherits ElementImage

                Private mImages As List(Of String) = New List(Of String)
                Private mPosition As TimeSpan
                Private mDuration As TimeSpan
                Private mFramesPerSecond As Single = 30

                Private imageIndex As Integer
                Private lastImageIndex As Integer = -1

                Public Event NewFrame(sender As ElementVideo, image As Bitmap)

                Public Sub New(ByVal layer As Layer)
                    MyBase.New(layer)
                End Sub

                Public Overrides ReadOnly Property Type As IElement.TypeConstants
                    Get
                        Return IElement.TypeConstants.ImageSequence
                    End Get
                End Property

                Public Overrides ReadOnly Property Name As String
                    Get
                        Return "ImageSequence"
                    End Get
                End Property

                Public Property FramesPerSecond As Single
                    Get
                        Return mFramesPerSecond
                    End Get
                    Set(ByVal value As Single)
                        mFramesPerSecond = value
                        SetDuration()
                    End Set
                End Property

                Public Overrides Property Position(Optional updateFrame As Boolean = False) As TimeSpan
                    Get
                        Return mPosition
                    End Get
                    Set(ByVal value As TimeSpan)
                        If mPosition = value Then Exit Property
                        mPosition = value

                        ' mDuration -> mImages.Count
                        ' mPosition -> imageIndex

                        If mImages.Count > 0 Then
                            imageIndex = mPosition.TotalSeconds * mImages.Count / mDuration.TotalSeconds
                            If imageIndex > 0 AndAlso imageIndex <> lastImageIndex Then
                                MyBase.SourceFile = mImages(imageIndex Mod mImages.Count)
                                lastImageIndex = imageIndex
                            End If
                        End If
                    End Set
                End Property

                Public Overrides Property SourceFile() As String
                    Get
                        Return MyBase.SourceFile
                    End Get
                    Set(ByVal value As String)
                        If MyBase.SourceFile = value Then Exit Property

                        Static isBusy As Boolean = False
                        If isBusy Then Exit Property
                        isBusy = True

                        MyBase.SourceFile = value

                        mImages.Clear()

                        If IO.File.Exists(value) Then
                            Dim firstFile As IO.FileInfo = New IO.FileInfo(value)
                            Dim sequence As String = GetSequenceToken(firstFile.Name.Replace(firstFile.Extension, ""))
                            Dim prefix As String = firstFile.Name.Substring(0, firstFile.Name.Replace(firstFile.Extension, "").Length - sequence.Length)
                            For Each file In firstFile.Directory.GetFiles("*" + firstFile.Extension)
                                If file.Name.Substring(0, file.Name.Replace(file.Extension, "").Length - sequence.Length) = prefix Then
                                    mImages.Add(file.FullName)
                                End If
                            Next

                            SetDuration()
                        End If

                        isBusy = False
                    End Set
                End Property

                Public ReadOnly Property MediaDuration As TimeSpan
                    Get
                        Return mDuration
                    End Get
                End Property

                Public ReadOnly Property Images As List(Of String)
                    Get
                        Return mImages
                    End Get
                End Property

                Private Sub SetDuration()
                    mDuration = TimeSpan.FromSeconds(mImages.Count / mFramesPerSecond)
                    lastImageIndex = -1
                End Sub

                Private Function GetSequenceToken(fileName As String) As String
                    Dim sequence As String = ""

                    For i As Integer = fileName.Length - 1 To 0 Step -1
                        If IsNumeric(fileName(i)) Then
                            sequence = fileName(i) + sequence
                        Else
                            Exit For
                        End If
                    Next

                    Return sequence
                End Function
            End Class
        End Class
    End Class
End Class

