Imports System.Collections.ObjectModel
Imports System.Threading
Imports Prism.Project.Track.Layer

Partial Public Class Project
    <Serializable()>
    Public Class Track
        Implements ICloneable, IDisposable

        Public Enum StateConstants
            Idle
            Playing
            Paused
        End Enum

        Private mID As Guid
        Private mState As StateConstants = StateConstants.Idle
        Private mFrameRate As Integer
        Private mCursorTime As TimeSpan
        Private mPlaybackRate As Integer
        Private mTimeInc As TimeSpan
        Private mProject As Project

        Private playbackThread As Thread
        Private playbackEvent As AutoResetEvent = New AutoResetEvent(False)
        Private stopThread As Boolean = False

        Public Event NewFrame(sender As Track, cursorTime As TimeSpan)
        Public Event StateChanged(sender As Track, state As StateConstants)
        Public Event LayersChanged(sender As Track)

        Public Property ID As Guid = Guid.NewGuid()
        Public Property Name As String = ""
        Public Property Description As String = ""
        Public Property Layers As ObservableCollection(Of Layer) = New ObservableCollection(Of Layer)
        Public Property ShortcutKey As Keys = Keys.None

        Public Sub New(project As Project, name As String)
            mProject = project
            Me.Name = name
            FrameRate = 30

            AddHandler Layers.CollectionChanged, AddressOf HandleCollectionChanged
        End Sub

        Public Sub New(project As Project, name As String, description As String)
            Me.New(project, name)
            Me.Description = description
        End Sub

        Private Sub HandleCollectionChanged(sender As Object, e As Specialized.NotifyCollectionChangedEventArgs)
            mProject.HasChanged = True

            RaiseEvent LayersChanged(Me)
        End Sub

        Public Function Duration() As TimeSpan
            Dim d As TimeSpan = New TimeSpan(0)
            For Each layer In Layers
                If layer.Duration > d Then d = layer.Duration
            Next
            Return d
        End Function

        Public Function ShortcutKeyToString() As String
            Dim modifier As String = ""
            Dim key As String = ""
            Dim keyModifiers As Keys = ShortcutKey And Keys.Modifiers
            Dim keyCode As Keys = ShortcutKey And Keys.KeyCode

            If keyModifiers <> Keys.None Then
                modifier = keyModifiers.ToString
                If modifier.Contains(", ") Then
                    modifier = modifier.Replace(", ", " + ") + " + "
                Else
                    modifier += " + "
                End If
            End If

            If keyCode >= 31 Then key = keyCode.ToString

            Return modifier + key
        End Function

        Public ReadOnly Property State As StateConstants
            Get
                Return mState
            End Get
        End Property

        Public Property FrameRate As Integer
            Get
                Return mFrameRate
            End Get
            Set(value As Integer)
                mFrameRate = value
                mPlaybackRate = 1 / mFrameRate * 1000
                mTimeInc = TimeSpan.FromMilliseconds(mPlaybackRate)
            End Set
        End Property

        Public ReadOnly Property Project As Project
            Get
                Return mProject
            End Get
        End Property

        Public ReadOnly Property PlaybackRate As Integer
            Get
                Return mPlaybackRate
            End Get
        End Property

        Public ReadOnly Property TimeInc As TimeSpan
            Get
                Return mTimeInc
            End Get
        End Property

        Public Property CursorTime As TimeSpan
            Get
                Return mCursorTime
            End Get
            Set(value As TimeSpan)
                mCursorTime = PadTime(value, mTimeInc)
            End Set
        End Property

        Public Sub Play()
            If mState = StateConstants.Playing OrElse Layers.Count = 0 Then Exit Sub

            CursorTime = Layers.First.CursorTime

            For Each layer In Layers
                layer.CursorTime = mCursorTime
                For Each element In layer.Elements
                    element.PrecalculateBlendedKeyFrames(element.InitialKeyFrame.Time, element.EndTime)
                Next
            Next

            If playbackThread Is Nothing Then
                playbackThread = New Thread(AddressOf PlaybackLoop)
                playbackThread.Start()
            End If

            mState = StateConstants.Playing

            RaiseEvent StateChanged(Me, mState)
        End Sub

        Public Sub Pause()
            If mState = StateConstants.Playing Then
                mState = StateConstants.Paused

                PauseAudioAndVideoElements(False)

                RaiseEvent StateChanged(Me, mState)
            End If
        End Sub

        Public Sub [Stop]()
            If mState <> StateConstants.Idle Then
                stopThread = True
                playbackEvent.Set()
                mState = StateConstants.Idle

                PauseAudioAndVideoElements(True)

                RaiseEvent StateChanged(Me, mState)
            End If
        End Sub

        Private Sub PauseAudioAndVideoElements(resetPosition As Boolean)
            For Each layer In Layers
                If resetPosition Then layer.CursorTime = TimeSpan.Zero

                For Each element In (From e In layer.Elements Where e.TypeIsMedia Select e)
                    element.MediaPause()
                Next
            Next
        End Sub

        Private Sub PlaybackLoop()
            Dim variableTimeInc As TimeSpan = mTimeInc
            Do
                playbackEvent.WaitOne(mPlaybackRate)

                If stopThread Then Exit Do
                If mState = StateConstants.Paused Then Continue Do

                Dim startTime = TimeSpan.FromTicks(Now.Ticks)

                mCursorTime += variableTimeInc
                For Each layer In Layers
                    layer.CursorTime += variableTimeInc
                Next

                Dim diffTime = TimeSpan.FromTicks(Now.Ticks) - startTime
                variableTimeInc = If(diffTime > mTimeInc, diffTime, mTimeInc)

                RaiseEvent NewFrame(Me, mCursorTime)
            Loop Until stopThread

            stopThread = False
            playbackThread = Nothing
        End Sub

        Public Shared Function PadTime(time As TimeSpan, timeInc As TimeSpan) As TimeSpan
            Return time - TimeSpan.FromMilliseconds(time.TotalMilliseconds Mod timeInc.TotalMilliseconds)
        End Function

        Public Function ToXML() As XElement
            Return <track>
                       <name><%= Me.Name %></name>
                       <description><%= Me.Description %></description>
                       <shortcutKey><%= Me.ShortcutKey %></shortcutKey>
                       <layers>
                           <%= From layer In Me.Layers Select (layer.ToXML()) %>
                       </layers>
                   </track>
        End Function

        Public Shared Function FromXML(project As Project, xml As XElement) As Track
            Dim key As Keys
            Dim track As Track = New Track(project, xml.<name>.Value, xml.<description>.Value)
            If [Enum].TryParse(xml.<shortcutKey>.Value, key) Then track.ShortcutKey = key
            Return track
        End Function

        Public Function Clone() As Object Implements System.ICloneable.Clone
            Dim newInstance As Reflection.MethodInfo =
                                GetType(Track).GetMethod("MemberwiseClone",
                                                                 Reflection.BindingFlags.Instance Or
                                                                 Reflection.BindingFlags.NonPublic)

            Return newInstance?.Invoke(Me, Nothing)
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If playbackEvent IsNot Nothing Then playbackEvent.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Class
