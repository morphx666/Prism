Imports System.Collections.ObjectModel
Imports System.ComponentModel

<Serializable()>
Public Class Project
    Implements ICloneable

    Private mIsIdle As Boolean = True
    Private lastFramePosition As TimeSpan = TimeSpan.Zero

    Public Property Name As String = "<Untitled Project>"
    Public Property Description As String = ""
    Public Property Tracks As ObservableCollection(Of Track) = New ObservableCollection(Of Track)
    Public Property Resolution As Size = New Size(1024, 768)
    Public Property FileName As String = ""
    Public Property HasChanged As Boolean = False

    Public Event NewFrame(ByVal sender As Track, ByVal cursorTime As TimeSpan)
    Public Event StateChanged(ByVal sender As Track, ByVal state As Track.StateConstants)
    Public Event TracksChanged(ByVal sender As Project)

    Public Sub New()
        AddHandler Tracks.CollectionChanged, AddressOf HandleCollectionChanged
    End Sub

    Public Sub New(name As String)
        Me.New()
        Me.Name = name
    End Sub

    Public Sub New(name As String, description As String)
        Me.New(name)
        Me.Description = description
    End Sub

    Public ReadOnly Property IsIdle As Boolean
        Get
            Return mIsIdle
        End Get
    End Property

    Protected Overrides Sub Finalize()
        StopTracks()
    End Sub

    Public Sub StopTracks()
        For Each track In Tracks
            track.Stop()
            For Each layer In track.Layers
                For Each element As Project.Track.Layer.Element In (From e In layer.Elements Where e.TypeIsMedia Select e)
                    element.MediaStop()
                Next
            Next
        Next
    End Sub

    Public Sub InitalizeElements()
        For Each track In Tracks
            For Each layer In track.Layers
                For Each element In layer.Elements
                    element.Initialize()
                Next
            Next
        Next
    End Sub

    Private Sub HandleCollectionChanged(sender As Object, e As Specialized.NotifyCollectionChangedEventArgs)
        HasChanged = True

        If e.Action = Specialized.NotifyCollectionChangedAction.Add Then
            For Each track In Tracks
                AddHandler track.NewFrame, Sub(srcTrack As Track, cursorTime As TimeSpan)
                                               If lastFramePosition <> cursorTime Then
                                                   lastFramePosition = cursorTime
                                                   RaiseEvent NewFrame(srcTrack, cursorTime)
                                               End If
                                           End Sub

                AddHandler track.StateChanged, Sub(srcTrack As Track, state As Track.StateConstants)
                                                   mIsIdle = ((From t As Track In Tracks Select t Where t.State <> Project.Track.StateConstants.Idle).Count() = 0)
                                                   lastFramePosition = TimeSpan.Zero
                                                   RaiseEvent StateChanged(srcTrack, state)
                                               End Sub
            Next
        End If

        RaiseEvent TracksChanged(Me)
    End Sub

    Public Shared Function GetAvailableElementsTypes() As List(Of Type)
        Dim elements As List(Of Type) = New List(Of Type)

        Dim elementType As Type = GetType(Project.Track.Layer.Element)
        Dim asm As Reflection.Assembly = Reflection.Assembly.GetAssembly(Reflection.Assembly.GetExecutingAssembly.GetType)

        For Each t As Type In GetType(Project.Track.Layer).GetNestedTypes()
            If t.BaseType IsNot Nothing AndAlso (t.BaseType Is elementType OrElse t.BaseType.BaseType Is elementType) Then
                If Not t.IsAbstract Then elements.Add(t)
            End If
        Next

        elements.Sort(New Comparison(Of Type)(Function(e1, e2) e1.ToString() < e2.ToString()))

        Return elements
    End Function

    Private Shared lockObject As New Object()
    'Private Shared bmpCache As New Dictionary(Of String, Bitmap)
    'Public Shared Function LoadBitmap(fileName As String) As Bitmap
    '    SyncLock lockObject
    '        If bmpCache.ContainsKey(fileName) Then
    '            Return bmpCache(fileName)
    '        ElseIf IO.File.Exists(fileName) Then
    '            Try
    '                Dim tmpBmp = Bitmap.FromFile(fileName)

    '                If tmpBmp.PixelFormat = Imaging.PixelFormat.Format24bppRgb Then
    '                    Dim tmpBtmp2 = New Bitmap(tmpBmp.Width, tmpBmp.Height, Imaging.PixelFormat.Format32bppArgb)
    '                    Using g = Graphics.FromImage(tmpBtmp2)
    '                        g.DrawImageUnscaled(tmpBmp, 0, 0)
    '                    End Using
    '                    tmpBmp.Dispose()
    '                    tmpBmp = tmpBtmp2
    '                End If

    '                If tmpBmp IsNot Nothing Then bmpCache.Add(fileName, tmpBmp)
    '                Return tmpBmp
    '            Catch
    '            End Try
    '        End If
    '    End SyncLock

    '    Return Nothing
    'End Function

    Public Shared Function LoadBitmap(fileName As String) As Bitmap
        SyncLock lockObject
            If IO.File.Exists(fileName) Then
                Try
                    Dim tmpBmp = Bitmap.FromFile(fileName)

                    If tmpBmp.PixelFormat = Imaging.PixelFormat.Format24bppRgb Then
                        Dim tmpBtmp2 = New Bitmap(tmpBmp.Width, tmpBmp.Height, Imaging.PixelFormat.Format32bppArgb)
                        Using g = Graphics.FromImage(tmpBtmp2)
                            g.DrawImageUnscaled(tmpBmp, 0, 0)
                        End Using
                        tmpBmp.Dispose()
                        tmpBmp = tmpBtmp2
                    End If

                    Return tmpBmp
                Catch
                End Try
            End If
        End SyncLock

        Return Nothing
    End Function

    Public Function ToXML() As XElement
        Return <project>
                   <name><%= Me.Name %></name>
                   <description><%= Me.Description %></description>
                   <fileName><%= Me.FileName %></fileName>
                   <resolution><%= Me.Resolution %></resolution>
                   <tracks>
                       <%= From track In Me.Tracks Select (track.ToXML()) %>
                   </tracks>
               </project>
    End Function

    Public Shared Function FromXML(xml As XElement) As Project
        Dim p As Project = New Project(xml.<name>.Value, xml.<description>.Value) With {
            .FileName = xml.<fileName>.Value,
            .Resolution = Project.Track.Layer.Element.ParseString(Of Size)(xml.<resolution>(0))
        }

        For Each xt In xml...<track>
            Dim track As Project.Track = Project.Track.FromXML(p, xt)

            For Each xl In xt...<layer>
                Dim layer As Project.Track.Layer = Project.Track.Layer.FromXML(track, xl)

                For Each xe In xl...<element>
                    layer.Elements.Add(Project.Track.Layer.Element.FromXML(layer, xe))
                Next

                track.Layers.Add(layer)
            Next

            p.Tracks.Add(track)
        Next

        Return p
    End Function

    Public Function Clone() As Object Implements ICloneable.Clone
        Dim newProject As Project = Project.FromXML(Me.ToXML())

        For trackIndex As Integer = 0 To Tracks.Count - 1
            newProject.Tracks(trackIndex).ID = Tracks(trackIndex).ID
        Next

        Return newProject
    End Function
End Class
