Public Class RenderTrack
    Private mTrack As Project.Track = Nothing
    Private mSelected As Boolean

	Public Property Track As Project.Track
		Get
			Return mTrack
		End Get
		Set(ByVal value As Project.Track)
			mTrack = value

			UpdateUI()
		End Set
    End Property

    Public Property Selected As Boolean
        Get
            Return mSelected
        End Get
        Set(ByVal value As Boolean)
            mSelected = value

            Me.BackColor = IIf(mSelected, Color.Lavender, Color.AliceBlue)
        End Set
    End Property

    Private Sub RenderTrack_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.SetStyle(ControlStyles.Selectable, True)
        UpdateUI()
    End Sub

    Private Sub UpdateUI()
		If mTrack Is Nothing Then
			lblTrackName.Text = "Track Name"
			lblDescription.Text = "Description"
			lblShortcutKey.Text = Keys.None.ToString
		Else
			lblTrackName.Text = mTrack.Name
			lblDescription.Text = mTrack.Description
			lblShortcutKey.Text = mTrack.ShortcutKeyToString()

			lblShortcutKey.Left = Me.Width - lblShortcutKey.Width
		End If
	End Sub
End Class
