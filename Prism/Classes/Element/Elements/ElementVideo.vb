Imports System.Collections.Concurrent

Partial Public Class Project
    Partial Public Class Track
        Partial Public Class Layer
            <Serializable()>
            Public Class ElementVideo
                Inherits ElementImage

                <NonSerialized()>
                Private mMedia As AForge.Video.DirectShow.FileVideoSource
                Private mMediaState As IElement.MediaPlaybackStateConstants = IElement.MediaPlaybackStateConstants.Idle
                <NonSerialized()>
                Private mWavePeaks As WavePeaks = New WavePeaks()

                Private lastPosition As Long = -1

                <NonSerialized()>
                Public Event NewFrame(sender As ElementVideo, ByVal image As Bitmap)
                Public Event BuildingPeaks(sender As Element, ByVal progress As Integer)

                Private levelLeft As ConcurrentDictionary(Of Double, Integer) = New ConcurrentDictionary(Of Double, Integer)
                Private levelRight As ConcurrentDictionary(Of Double, Integer) = New ConcurrentDictionary(Of Double, Integer)
                Private levelAccLeft As Integer = 0
                Private levelAccRight As Integer = 0
                Private dataReceived As Integer = 0
                Private Const maxData As Integer = 8192 / 2
                Private samples As IOrderedEnumerable(Of KeyValuePair(Of Double, Integer))

                Public Sub New(layer As Layer)
                    MyBase.New(layer)
                End Sub

                Public Sub New(layer As Layer, sourceFile As String)
                    MyBase.New(layer)
                    Me.SourceFile = sourceFile
                End Sub

                Public Overrides ReadOnly Property Type As IElement.TypeConstants
                    Get
                        Return IElement.TypeConstants.Video
                    End Get
                End Property

                Public Overrides ReadOnly Property Name As String
                    Get
                        Return "Video"
                    End Get
                End Property

                Public Overrides ReadOnly Property IsValid As Boolean
                    Get
                        Return If(mMedia Is Nothing, False, mMedia.IsValid)
                    End Get
                End Property

                Public ReadOnly Property Peaks As List(Of WavePeaks.Peak)
                    Get
                        Return mWavePeaks.Peaks
                    End Get
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

                        mMedia = Nothing
                        MyBase.Image = Nothing

                        If IO.File.Exists(value) Then
                            mMedia = New AForge.Video.DirectShow.FileVideoSource(value)
                            AddHandler mMedia.NewAudioFrame, AddressOf NewAudioFrame
                            AddHandler mMedia.NewFrame, AddressOf RenderFrame
                            AddHandler mMedia.PlayingFinished, Sub(sender As Object, e As AForge.Video.ReasonToFinishPlaying)
                                                                   mMediaState = IElement.MediaPlaybackStateConstants.Idle
                                                               End Sub

                            AddHandler mMedia.VideoSourceError, Sub(sender As Object, e As AForge.Video.VideoSourceErrorEventArgs)
                                                                    Debug.WriteLine(e.Description)
                                                                End Sub

                            If MyBase.Layer.Track.State = Project.Track.StateConstants.Playing Then
                                Me.Position = MyBase.Layer.Track.CursorTime - InitialKeyFrame.Time
                                Me.MediaPlay()
                            End If

                            AddHandler mWavePeaks.DoneProcessing, Sub(sender As WavePeaks, peaks As List(Of WavePeaks.Peak))
                                                                      RaiseEvent BuildingPeaks(Me, 100)
                                                                  End Sub
                            AddHandler mWavePeaks.Progress, Sub(sender As WavePeaks, progress As Integer)
                                                                RaiseEvent BuildingPeaks(Me, progress)
                                                            End Sub
                            mWavePeaks.FileName = value
                            mWavePeaks.ProcessAudioFile()
                        End If

                        isBusy = False
                    End Set
                End Property

                Private Sub NewAudioFrame(sender As Object, e As AForge.Video.NewAudioFrameEventArgs)
                    Dim channels As Integer = mMedia.AudioFormat.nChannels
                    Const offset As Double = 0 ' 5.5

                    Dim l As Integer = 0
                    Dim r As Integer = 0
                    Dim n As Integer = channels * 128

                    For i As Integer = 0 To e.BufferLength - channels Step n
                        l += (e.Frame(i) / 80) ^ 2
                        If channels = 1 Then
                            r = l
                        Else
                            r += (e.Frame(i + 1) / 80) ^ 2
                        End If
                    Next

                    levelAccLeft += l
                    levelAccRight += r

                    dataReceived += e.BufferLength
                    If dataReceived >= maxData Then
                        If levelLeft.TryAdd(e.Time - offset, levelAccLeft / (dataReceived / n)) = False Then Exit Sub
                        If levelRight.TryAdd(e.Time - offset, levelAccRight / (dataReceived / n)) = False Then Exit Sub

                        levelAccLeft = 0
                        levelAccRight = 0
                        dataReceived = 0
                    End If
                End Sub

                Public ReadOnly Property MediaDuration As TimeSpan
                    Get
                        Return If(mMedia Is Nothing,
                                    TimeSpan.Zero,
                                    New TimeSpan(0, 0, 0, 0, mMedia.Duration / 10000))
                    End Get
                End Property

                Public Overrides Property Position(Optional updateFrame As Boolean = False) As TimeSpan
                    Get
                        Return If(mMedia Is Nothing,
                                    TimeSpan.Zero,
                                    TimeSpan.FromMilliseconds(mMedia.Position / 10000))
                    End Get
                    Set(value As TimeSpan)
                        Try
                            If value.TotalMilliseconds >= 0 AndAlso mMedia IsNot Nothing Then
                                Dim newPosition As Long = (value.TotalMilliseconds) * 10000
                                If mMedia.Position <> newPosition Then
                                    'If updateFrame Then
                                    If lastPosition <> newPosition Then
                                        lastPosition = newPosition
                                        'If mMediaState <> IElement.MediaPlaybackStateConstants.Playing Then mMedia.Position = newPosition
                                        mMedia.SetPositionAsync(newPosition)
                                    End If
                                    'Else
                                    'mMedia.SetPositionAsync(newPosition)
                                    'End If
                                End If
                            End If
                        Catch
                        End Try
                    End Set
                End Property

                Public Overrides ReadOnly Property MediaState As IElement.MediaPlaybackStateConstants
                    Get
                        Return mMediaState
                    End Get
                End Property

                Public Property Volume As Integer
                    Get
                        Return If(mMedia IsNot Nothing,
                                    -mMedia.Volume / 100,
                                    0)
                    End Get
                    Set(value As Integer)
                        If mMedia IsNot Nothing Then
                            value = Math.Max(Math.Min(value, 100), 0)
                            mMedia.Volume = -value * 100
                        End If
                    End Set
                End Property

                Public Property Balance As Integer
                    Get
                        Return If(mMedia IsNot Nothing,
                                    mMedia.Balance / 100,
                                    0)
                    End Get
                    Set(value As Integer)
                        If mMedia IsNot Nothing Then
                            value = Math.Max(Math.Min(value, 100), -100)
                            mMedia.Balance = value * 100
                        End If
                    End Set
                End Property

                Public Overrides ReadOnly Property Levels(time As TimeSpan) As IElement.Level
                    Get
                        Dim l As IElement.Level = New IElement.Level

                        If MyBase.Layer.Track.State = Project.Track.StateConstants.Playing Then
                            Dim t As Double = time.TotalMilliseconds / 1000

                            samples = (From v In levelLeft Where Math.Abs(v.Key - t) <= 0.1 Order By v.Key)
                            If samples.Count > 0 Then l.LeftChannel = samples.Average(Function(v) v.Value)

                            samples = (From v In levelRight Where Math.Abs(v.Key - t) <= 0.1 Order By v.Key)
                            If samples.Count > 0 Then l.RightChannel = samples.Average(Function(v) v.Value)

                            RemoveLevels(t - 0.2)
                        End If

                        Return l
                    End Get
                End Property

                Private Sub RemoveLevels(time As Double)
                    Dim v As Integer
                    Dim keys = From k In levelLeft.Keys Where k < time Select k
                    For Each key In keys
                        levelLeft.TryRemove(key, v)
                        levelRight.TryRemove(key, v)
                    Next
                End Sub

                Public Overrides Sub MediaPlay()
                    If mMedia IsNot Nothing AndAlso mMediaState <> IElement.MediaPlaybackStateConstants.Playing Then
                        levelLeft.Clear()
                        levelLeft.Clear()

                        mMedia.Start()
                        mMediaState = IElement.MediaPlaybackStateConstants.Playing
                    End If
                End Sub

                Public Overrides Sub MediaStop()
                    If mMedia IsNot Nothing Then
                        mMedia.Stop()
                        mMediaState = IElement.MediaPlaybackStateConstants.Idle
                    End If
                End Sub

                Public Overrides Sub MediaPause()
                    If mMedia IsNot Nothing AndAlso mMediaState = IElement.MediaPlaybackStateConstants.Playing Then
                        mMedia.Pause()
                        mMediaState = IElement.MediaPlaybackStateConstants.Paused
                    End If
                End Sub

                Private Sub RenderFrame(sender As Object, e As AForge.Video.NewFrameEventArgs)
                    MyBase.Image = CType(e.Frame.Clone(), Bitmap)

                    If MyBase.Layer.Track.State <> StateConstants.Playing Then RaiseEvent NewFrame(Me, MyBase.Image)
                End Sub

                Public Overrides Function ToXML() As XElement
                    Dim xml = MyBase.ToXML()
                    xml.<propietary>(0).Add(<volume><%= Me.Volume %></volume>)
                    xml.<propietary>(0).Add(<balance><%= Me.Balance %></balance>)
                    Return xml
                End Function

                Public Overrides Sub SetFromXML(xml As XElement)
                    MyBase.SetFromXML(xml)

                    Me.Volume = xml.<propietary>.<volume>.Value
                    Me.Balance = xml.<propietary>.<balance>.Value
                End Sub
            End Class
        End Class
    End Class
End Class
