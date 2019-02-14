Public Class FormProjectProperties
    Private Sub ButtonOK_Click(sender As Object, e As EventArgs) Handles ButtonOK.Click
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub ButtonCancel_Click(sender As Object, e As EventArgs) Handles ButtonCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub FormProjectProperties_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ButtonAuto.Enabled = mainRendererIsAvailable
    End Sub

    Private Sub ButtonAuto_Click(sender As Object, e As EventArgs) Handles ButtonAuto.Click
        TextBoxWidth.Text = Screen.AllScreens(1).Bounds.Width.ToString()
        TextBoxHeight.Text = Screen.AllScreens(1).Bounds.Height.ToString()
    End Sub
End Class