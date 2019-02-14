<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class RenderTrack
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.lblTrackName = New System.Windows.Forms.Label()
        Me.lblShortcutKey = New System.Windows.Forms.Label()
        Me.lblDescription = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'lblTrackName
        '
        Me.lblTrackName.AutoSize = True
        Me.lblTrackName.Font = New Font("Segoe UI", 9.75!, FontStyle.Bold, GraphicsUnit.Point, CType(0, Byte))
        Me.lblTrackName.ForeColor = Color.MidnightBlue
        Me.lblTrackName.Location = New Point(3, 3)
        Me.lblTrackName.Margin = New System.Windows.Forms.Padding(3)
        Me.lblTrackName.Name = "lblTrackName"
        Me.lblTrackName.Size = New Size(81, 17)
        Me.lblTrackName.TabIndex = 0
        Me.lblTrackName.Text = "Track Name"
        Me.lblTrackName.UseMnemonic = False
        '
        'lblShortcutKey
        '
        Me.lblShortcutKey.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblShortcutKey.AutoSize = True
        Me.lblShortcutKey.Location = New Point(376, 3)
        Me.lblShortcutKey.Margin = New System.Windows.Forms.Padding(3)
        Me.lblShortcutKey.Name = "lblShortcutKey"
        Me.lblShortcutKey.Size = New Size(51, 13)
        Me.lblShortcutKey.TabIndex = 1
        Me.lblShortcutKey.Text = "Shift+F1"
        Me.lblShortcutKey.UseMnemonic = False
        '
        'lblDescription
        '
        Me.lblDescription.AutoSize = True
        Me.lblDescription.ForeColor = Color.Gray
        Me.lblDescription.Location = New Point(3, 22)
        Me.lblDescription.Name = "lblDescription"
        Me.lblDescription.Size = New Size(66, 13)
        Me.lblDescription.TabIndex = 2
        Me.lblDescription.Text = "Description"
        Me.lblDescription.UseMnemonic = False
        '
        'RenderTrack
        '
        Me.AutoScaleDimensions = New SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = Color.AliceBlue
        Me.Controls.Add(Me.lblDescription)
        Me.Controls.Add(Me.lblShortcutKey)
        Me.Controls.Add(Me.lblTrackName)
        Me.Font = New Font("Segoe UI", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "RenderTrack"
        Me.Size = New Size(425, 40)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblTrackName As Label
    Friend WithEvents lblShortcutKey As System.Windows.Forms.Label
    Friend WithEvents lblDescription As System.Windows.Forms.Label

End Class
