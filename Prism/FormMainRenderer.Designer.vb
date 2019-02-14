<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormMainRenderer
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormMainRenderer))
        Me.RenderTrackSceneMain = New Prism.RenderTrackScene()
        Me.SuspendLayout()
        '
        'RenderTrackSceneMain
        '
        Me.RenderTrackSceneMain.AutoCreateKeyFrames = False
        Me.RenderTrackSceneMain.BackColor = Color.Black
        Me.RenderTrackSceneMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RenderTrackSceneMain.EditingTrack = Nothing
        Me.RenderTrackSceneMain.EnableRendering = True
        Me.RenderTrackSceneMain.IsMainRenderer = False
        Me.RenderTrackSceneMain.Location = New Point(0, 0)
        Me.RenderTrackSceneMain.Name = "RenderTrackSceneMain"
        Me.RenderTrackSceneMain.Project = Nothing
        Me.RenderTrackSceneMain.SafeAreasVisible = False
        Me.RenderTrackSceneMain.Size = New Size(284, 262)
        Me.RenderTrackSceneMain.TabIndex = 0
        '
        'FormMainRenderer
        '
        Me.AutoScaleDimensions = New SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New Size(284, 262)
        Me.Controls.Add(Me.RenderTrackSceneMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Me.Name = "FormMainRenderer"
        Me.Text = "Prism Main Renderer"
        Me.TopMost = True
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents RenderTrackSceneMain As Prism.RenderTrackScene
End Class
