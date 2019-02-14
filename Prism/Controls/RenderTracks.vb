Public Class RenderTracks
	Private mProject As Project = Nothing

    Public Event TrackSelected(sender As Object, e As EventArgs)

    Public Property Project As Project
        Get
            Return mProject
        End Get
        Set(ByVal value As Project)
            mProject = value

            If mProject IsNot Nothing Then
                AddHandler mProject.TracksChanged, Sub()
                                                       UpdateUI()
                                                   End Sub
            End If

            UpdateUI()
        End Set
    End Property

    Private Sub UpdateUI()
        While Me.Controls.Count > 0
            Me.Controls.Remove(Me.Controls(0))
        End While

        Dim p As Point = New Point(3, 3)

        If mProject IsNot Nothing Then
            For Each track In mProject.Tracks
                Dim rt As RenderTrack = New RenderTrack()
                rt.Tag = track
                Me.Controls.Add(rt)

                rt.Track = track
                rt.Location = p
                rt.Width = Me.Width - p.X * 2
                rt.Visible = True
                AddHandler rt.Click, AddressOf HandleTrackSelected

                p.Y += rt.Height + 3
            Next
        End If
    End Sub

    Private Sub HandleTrackSelected(sender As Object, e As EventArgs)
        SetSelectedTrack(CType(sender, RenderTrack))

        RaiseEvent TrackSelected(sender, e)
    End Sub

    Public Sub SetSelectedTrack(ByVal track As RenderTrack)
        For Each ctrl As Control In Me.Controls
            CType(ctrl, RenderTrack).Selected = False
        Next
        If track IsNot Nothing Then track.Selected = True
    End Sub

    Public Sub SetSelectedTrack(ByVal track As Project.Track)
        Dim selTrack As RenderTrack = Nothing

        For Each ctrl As Control In Me.Controls
            Dim curTrack As RenderTrack = CType(ctrl, RenderTrack)
            If curTrack.Tag.Equals(track) Then
                selTrack = curTrack
                Exit For
            End If
        Next

        SetSelectedTrack(selTrack)
    End Sub

    Private Sub RenderTracks_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Dim rt As RenderTrack
        For Each c In Me.Controls
            rt = CType(c, RenderTrack)
            rt.Width = Me.Width - rt.Left * 2
        Next
    End Sub
End Class
