Imports System.Collections.ObjectModel
Imports System.ComponentModel

Partial Public Class Project
    Partial Public Class Track
        <Serializable()>
        Public Class Layer
            Implements ICloneable

            Public Property Name As String = "<Untitled Layer>"
            Public Property Visible As Boolean = True
            <Browsable(False)>
            Public Property Elements As ObservableCollection(Of Element) = New ObservableCollection(Of Element)
            Private Property CursorTime As TimeSpan = New TimeSpan(0)

            Private mTrack As Track

            Public Event CursorPositionChanged(sender As Layer, time As TimeSpan)
            Public Event ElementsChanged(sender As Layer)

            Public Sub New(track As Track)
                mTrack = track
                AddHandler Elements.CollectionChanged, AddressOf HandleCollectionChanged
            End Sub

            Public Sub New(track As Track, name As String)
                Me.New(track)
                Me.Name = name
            End Sub

            <Browsable(False)>
            Public ReadOnly Property Track As Track
                Get
                    Return mTrack
                End Get
            End Property

            Public Function Duration() As TimeSpan
                Dim d As TimeSpan = New TimeSpan(0)
                For Each element In Elements
                    d += element.Duration
                Next

                Return d
            End Function

            <Browsable(False)>
            Public Property CursorTime(Optional silentUpdate As Boolean = False) As TimeSpan
                Get
                    Return Me.CursorTime
                End Get
                Set(value As TimeSpan)
                    If CursorTime <> value Then
                        CursorTime = Project.Track.PadTime(value, mTrack.TimeInc)

                        If mTrack.State = StateConstants.Playing Then
                            Dim activeElements = GetActiveElements()
                            Dim audioAndVideoElements = (From e As Element In Elements
                                                         Where e.TypeIsMedia AndAlso
                                                            (CursorTime < e.InitialKeyFrame.Time OrElse CursorTime > e.EndTime) Select e).ToList()
                            audioAndVideoElements.ForEach(New Action(Of Element)(Sub(e As Element)
                                                                                     e.MediaPause()
                                                                                 End Sub
                            ))

                            For Each activeElement As Element In activeElements
                                If activeElement.TypeIsMedia Then
                                    If activeElement.MediaState <> IElement.MediaPlaybackStateConstants.Playing AndAlso CursorTime <= activeElement.EndTime Then
                                        activeElement.Position = CursorTime - activeElement.InitialKeyFrame.Time
                                        activeElement.MediaPlay()
                                    ElseIf activeElement.MediaState = IElement.MediaPlaybackStateConstants.Playing AndAlso CursorTime >= activeElement.EndTime - mTrack.TimeInc Then
                                        activeElement.MediaPause()
                                    End If
                                End If
                            Next
                        End If

                        If Not silentUpdate Then RaiseEvent CursorPositionChanged(Me, CursorTime)
                    End If
                End Set
            End Property

            Private Function GetActiveElements() As List(Of Element)
                Dim elements As List(Of Element) = New List(Of Element)
                If mTrack IsNot Nothing Then
                    For Each layer As Prism.Project.Track.Layer In Track.Layers
                        If layer.Visible Then
                            Dim cursorTime As TimeSpan = layer.CursorTime
                            elements.AddRange(From el In layer.Elements
                                                Where (el.LoopEnabled AndAlso cursorTime >= el.InitialKeyFrame.Time) OrElse (cursorTime >= el.InitialKeyFrame.Time AndAlso cursorTime <= el.EndTime)
                                                Select el)
                        End If
                    Next
                End If
                Return elements
            End Function

            Private Sub HandleCollectionChanged(sender As Object, e As Specialized.NotifyCollectionChangedEventArgs)
                mTrack.Project.HasChanged = True

                If e.Action = Specialized.NotifyCollectionChangedAction.Remove Then
                    For Each item In e.OldItems
                        CType(item, Element).MediaStop()
                    Next
                End If

                RaiseEvent ElementsChanged(Me)
            End Sub

            Public Function ToXML() As XElement
                Return <layer>
                           <name><%= Me.Name %></name>
                           <visible><%= Me.Visible %></visible>
                           <elements>
                               <%= From element In Me.Elements Select (element.ToXML()) %>
                           </elements>
                       </layer>
            End Function

            Public Shared Function FromXML(track As Track, xml As XElement) As Layer
                Dim bool As Boolean
                Dim layer As Layer = New Layer(track, xml.<name>.Value)
                If Boolean.TryParse(xml.<visible>.Value, bool) Then layer.Visible = bool
                Return layer
            End Function

            Public Function Clone() As Object Implements ICloneable.Clone
                Dim newInstance As Reflection.MethodInfo = GetType(Layer).GetMethod("MemberwiseClone", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
                Return newInstance?.Invoke(Me, Nothing)
            End Function
        End Class
    End Class
End Class