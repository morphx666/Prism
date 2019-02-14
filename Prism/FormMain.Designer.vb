<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                tmrRefreshPropertyGrid.Dispose()
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormMain))
        Me.SplitContainerMain01 = New System.Windows.Forms.SplitContainer()
        Me.RenderTracksPanel = New Prism.RenderTracks()
        Me.scMain02 = New System.Windows.Forms.SplitContainer()
        Me.CheckBoxEnablePreview = New System.Windows.Forms.CheckBox()
        Me.ButtonApplyChanges = New System.Windows.Forms.Button()
        Me.CheckBoxLiveMode = New System.Windows.Forms.CheckBox()
        Me.CheckBoxAutoCreateKeyFrames = New System.Windows.Forms.CheckBox()
        Me.ButtonStop = New System.Windows.Forms.Button()
        Me.ButtonPause = New System.Windows.Forms.Button()
        Me.ButtonPlay = New System.Windows.Forms.Button()
        Me.RenderTrackScenePreview = New Prism.RenderTrackScene()
        Me.scMain03 = New System.Windows.Forms.SplitContainer()
        Me.RenderTimeLinePanel = New Prism.RenderTimeline()
        Me.PropertyGridElement = New System.Windows.Forms.PropertyGrid()
        Me.MenuStripMain = New System.Windows.Forms.MenuStrip()
        Me.ToolStripMenuItemFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemFileNew = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripMenuItemFileOpen = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripMenuItemFileSave = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemFileSaveAs = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripMenuItemFileExit = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemProject = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemProjectTracks = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemProjectTracksAdd = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemProjectTracksDelete = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemProjectLayers = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemProjectLayersAdd = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemProjectLayersDelete = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemProjectElements = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemProjectElementsAdd = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemProjectElementsDelete = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripMenuItemProjectProperties = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemView = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemToggleMainRendererTransparency = New System.Windows.Forms.ToolStripMenuItem()
        CType(Me.SplitContainerMain01, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainerMain01.Panel1.SuspendLayout()
        Me.SplitContainerMain01.Panel2.SuspendLayout()
        Me.SplitContainerMain01.SuspendLayout()
        CType(Me.scMain02, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.scMain02.Panel1.SuspendLayout()
        Me.scMain02.Panel2.SuspendLayout()
        Me.scMain02.SuspendLayout()
        CType(Me.scMain03, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.scMain03.Panel1.SuspendLayout()
        Me.scMain03.Panel2.SuspendLayout()
        Me.scMain03.SuspendLayout()
        Me.MenuStripMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainerMain01
        '
        Me.SplitContainerMain01.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SplitContainerMain01.Location = New Point(12, 27)
        Me.SplitContainerMain01.Name = "SplitContainerMain01"
        '
        'SplitContainerMain01.Panel1
        '
        Me.SplitContainerMain01.Panel1.Controls.Add(Me.RenderTracksPanel)
        '
        'SplitContainerMain01.Panel2
        '
        Me.SplitContainerMain01.Panel2.Controls.Add(Me.scMain02)
        Me.SplitContainerMain01.Size = New Size(1158, 805)
        Me.SplitContainerMain01.SplitterDistance = 198
        Me.SplitContainerMain01.TabIndex = 3
        '
        'RenderTracksPanel
        '
        Me.RenderTracksPanel.AutoScroll = True
        Me.RenderTracksPanel.BackColor = Color.SteelBlue
        Me.RenderTracksPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RenderTracksPanel.Font = New Font("Segoe UI", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
        Me.RenderTracksPanel.Location = New Point(0, 0)
        Me.RenderTracksPanel.Name = "RenderTracksPanel"
        Me.RenderTracksPanel.Project = Nothing
        Me.RenderTracksPanel.Size = New Size(198, 805)
        Me.RenderTracksPanel.TabIndex = 0
        '
        'scMain02
        '
        Me.scMain02.Dock = System.Windows.Forms.DockStyle.Fill
        Me.scMain02.Location = New Point(0, 0)
        Me.scMain02.Name = "scMain02"
        Me.scMain02.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'scMain02.Panel1
        '
        Me.scMain02.Panel1.AutoScroll = True
        Me.scMain02.Panel1.Controls.Add(Me.CheckBoxEnablePreview)
        Me.scMain02.Panel1.Controls.Add(Me.ButtonApplyChanges)
        Me.scMain02.Panel1.Controls.Add(Me.CheckBoxLiveMode)
        Me.scMain02.Panel1.Controls.Add(Me.CheckBoxAutoCreateKeyFrames)
        Me.scMain02.Panel1.Controls.Add(Me.ButtonStop)
        Me.scMain02.Panel1.Controls.Add(Me.ButtonPause)
        Me.scMain02.Panel1.Controls.Add(Me.ButtonPlay)
        Me.scMain02.Panel1.Controls.Add(Me.RenderTrackScenePreview)
        '
        'scMain02.Panel2
        '
        Me.scMain02.Panel2.Controls.Add(Me.scMain03)
        Me.scMain02.Size = New Size(956, 805)
        Me.scMain02.SplitterDistance = 491
        Me.scMain02.TabIndex = 0
        '
        'CheckBoxEnablePreview
        '
        Me.CheckBoxEnablePreview.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CheckBoxEnablePreview.Appearance = System.Windows.Forms.Appearance.Button
        Me.CheckBoxEnablePreview.Checked = True
        Me.CheckBoxEnablePreview.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckBoxEnablePreview.Location = New Point(863, 217)
        Me.CheckBoxEnablePreview.Name = "CheckBoxEnablePreview"
        Me.CheckBoxEnablePreview.Size = New Size(75, 23)
        Me.CheckBoxEnablePreview.TabIndex = 5
        Me.CheckBoxEnablePreview.Text = "Preview"
        Me.CheckBoxEnablePreview.TextAlign = ContentAlignment.MiddleCenter
        Me.CheckBoxEnablePreview.UseVisualStyleBackColor = True
        '
        'ButtonApplyChanges
        '
        Me.ButtonApplyChanges.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonApplyChanges.Location = New Point(863, 147)
        Me.ButtonApplyChanges.Name = "ButtonApplyChanges"
        Me.ButtonApplyChanges.Size = New Size(75, 23)
        Me.ButtonApplyChanges.TabIndex = 4
        Me.ButtonApplyChanges.Text = "Apply"
        Me.ButtonApplyChanges.UseVisualStyleBackColor = True
        '
        'CheckBoxLiveMode
        '
        Me.CheckBoxLiveMode.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CheckBoxLiveMode.Appearance = System.Windows.Forms.Appearance.Button
        Me.CheckBoxLiveMode.Location = New Point(863, 118)
        Me.CheckBoxLiveMode.Name = "CheckBoxLiveMode"
        Me.CheckBoxLiveMode.Size = New Size(75, 23)
        Me.CheckBoxLiveMode.TabIndex = 3
        Me.CheckBoxLiveMode.Text = "Live Mode"
        Me.CheckBoxLiveMode.TextAlign = ContentAlignment.MiddleCenter
        Me.CheckBoxLiveMode.UseVisualStyleBackColor = True
        '
        'CheckBoxAutoCreateKeyFrames
        '
        Me.CheckBoxAutoCreateKeyFrames.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CheckBoxAutoCreateKeyFrames.Appearance = System.Windows.Forms.Appearance.Button
        Me.CheckBoxAutoCreateKeyFrames.Location = New Point(863, 459)
        Me.CheckBoxAutoCreateKeyFrames.Name = "CheckBoxAutoCreateKeyFrames"
        Me.CheckBoxAutoCreateKeyFrames.Size = New Size(75, 23)
        Me.CheckBoxAutoCreateKeyFrames.TabIndex = 2
        Me.CheckBoxAutoCreateKeyFrames.Text = "Auto Keys"
        Me.CheckBoxAutoCreateKeyFrames.TextAlign = ContentAlignment.MiddleCenter
        Me.CheckBoxAutoCreateKeyFrames.UseVisualStyleBackColor = True
        '
        'ButtonStop
        '
        Me.ButtonStop.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonStop.Location = New Point(863, 61)
        Me.ButtonStop.Name = "ButtonStop"
        Me.ButtonStop.Size = New Size(75, 23)
        Me.ButtonStop.TabIndex = 1
        Me.ButtonStop.Text = "Stop"
        Me.ButtonStop.UseVisualStyleBackColor = True
        '
        'ButtonPause
        '
        Me.ButtonPause.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonPause.Location = New Point(863, 32)
        Me.ButtonPause.Name = "ButtonPause"
        Me.ButtonPause.Size = New Size(75, 23)
        Me.ButtonPause.TabIndex = 1
        Me.ButtonPause.Text = "Pause"
        Me.ButtonPause.UseVisualStyleBackColor = True
        '
        'ButtonPlay
        '
        Me.ButtonPlay.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonPlay.Location = New Point(863, 3)
        Me.ButtonPlay.Name = "ButtonPlay"
        Me.ButtonPlay.Size = New Size(75, 23)
        Me.ButtonPlay.TabIndex = 1
        Me.ButtonPlay.Text = "Play"
        Me.ButtonPlay.UseVisualStyleBackColor = True
        '
        'RenderTrackScenePreview
        '
        Me.RenderTrackScenePreview.AutoCreateKeyFrames = False
        Me.RenderTrackScenePreview.BackColor = Color.Black
        Me.RenderTrackScenePreview.EditingTrack = Nothing
        Me.RenderTrackScenePreview.EnableRendering = True
        Me.RenderTrackScenePreview.IsMainRenderer = False
        Me.RenderTrackScenePreview.Location = New Point(3, 3)
        Me.RenderTrackScenePreview.Name = "RenderTrackScenePreview"
        Me.RenderTrackScenePreview.Project = Nothing
        Me.RenderTrackScenePreview.SafeAreasVisible = False
        Me.RenderTrackScenePreview.Size = New Size(577, 479)
        Me.RenderTrackScenePreview.TabIndex = 0
        '
        'scMain03
        '
        Me.scMain03.Dock = System.Windows.Forms.DockStyle.Fill
        Me.scMain03.Location = New Point(0, 0)
        Me.scMain03.Name = "scMain03"
        '
        'scMain03.Panel1
        '
        Me.scMain03.Panel1.Controls.Add(Me.RenderTimeLinePanel)
        '
        'scMain03.Panel2
        '
        Me.scMain03.Panel2.Controls.Add(Me.PropertyGridElement)
        Me.scMain03.Size = New Size(956, 310)
        Me.scMain03.SplitterDistance = 643
        Me.scMain03.TabIndex = 1
        '
        'RenderTimeLinePanel
        '
        Me.RenderTimeLinePanel.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange
        Me.RenderTimeLinePanel.CursorTime = System.TimeSpan.Parse("00:00:00")
        Me.RenderTimeLinePanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RenderTimeLinePanel.Font = New Font("Segoe UI", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
        Me.RenderTimeLinePanel.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.RenderTimeLinePanel.Location = New Point(0, 0)
        Me.RenderTimeLinePanel.Name = "RenderTimeLinePanel"
        Me.RenderTimeLinePanel.PlaybackCursorTime = System.TimeSpan.Parse("00:00:00")
        Me.RenderTimeLinePanel.Size = New Size(643, 310)
        Me.RenderTimeLinePanel.TabIndex = 1
        Me.RenderTimeLinePanel.Track = Nothing
        '
        'PropertyGridElement
        '
        Me.PropertyGridElement.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PropertyGridElement.Location = New Point(0, 0)
        Me.PropertyGridElement.Name = "PropertyGridElement"
        Me.PropertyGridElement.Size = New Size(309, 310)
        Me.PropertyGridElement.TabIndex = 2
        '
        'MenuStripMain
        '
        Me.MenuStripMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItemFile, Me.ToolStripMenuItemProject, Me.ToolStripMenuItemView})
        Me.MenuStripMain.Location = New Point(0, 0)
        Me.MenuStripMain.Name = "MenuStripMain"
        Me.MenuStripMain.Size = New Size(1182, 24)
        Me.MenuStripMain.TabIndex = 4
        Me.MenuStripMain.Text = "Main Menu"
        '
        'ToolStripMenuItemFile
        '
        Me.ToolStripMenuItemFile.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItemFileNew, Me.ToolStripMenuItem1, Me.ToolStripMenuItemFileOpen, Me.ToolStripMenuItem2, Me.ToolStripMenuItemFileSave, Me.ToolStripMenuItemFileSaveAs, Me.ToolStripMenuItem3, Me.ToolStripMenuItemFileExit})
        Me.ToolStripMenuItemFile.Name = "ToolStripMenuItemFile"
        Me.ToolStripMenuItemFile.Size = New Size(37, 20)
        Me.ToolStripMenuItemFile.Text = "File"
        '
        'ToolStripMenuItemFileNew
        '
        Me.ToolStripMenuItemFileNew.Name = "ToolStripMenuItemFileNew"
        Me.ToolStripMenuItemFileNew.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.N), System.Windows.Forms.Keys)
        Me.ToolStripMenuItemFileNew.Size = New Size(180, 22)
        Me.ToolStripMenuItemFileNew.Text = "New"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New Size(177, 6)
        '
        'ToolStripMenuItemFileOpen
        '
        Me.ToolStripMenuItemFileOpen.Name = "ToolStripMenuItemFileOpen"
        Me.ToolStripMenuItemFileOpen.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.O), System.Windows.Forms.Keys)
        Me.ToolStripMenuItemFileOpen.Size = New Size(180, 22)
        Me.ToolStripMenuItemFileOpen.Text = "Open..."
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New Size(177, 6)
        '
        'ToolStripMenuItemFileSave
        '
        Me.ToolStripMenuItemFileSave.Name = "ToolStripMenuItemFileSave"
        Me.ToolStripMenuItemFileSave.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.ToolStripMenuItemFileSave.Size = New Size(180, 22)
        Me.ToolStripMenuItemFileSave.Text = "Save"
        '
        'ToolStripMenuItemFileSaveAs
        '
        Me.ToolStripMenuItemFileSaveAs.Name = "ToolStripMenuItemFileSaveAs"
        Me.ToolStripMenuItemFileSaveAs.Size = New Size(180, 22)
        Me.ToolStripMenuItemFileSaveAs.Text = "Save As..."
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New Size(177, 6)
        '
        'ToolStripMenuItemFileExit
        '
        Me.ToolStripMenuItemFileExit.Name = "ToolStripMenuItemFileExit"
        Me.ToolStripMenuItemFileExit.Size = New Size(180, 22)
        Me.ToolStripMenuItemFileExit.Text = "Exit"
        '
        'ToolStripMenuItemProject
        '
        Me.ToolStripMenuItemProject.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItemProjectTracks, Me.ToolStripMenuItemProjectLayers, Me.ToolStripMenuItemProjectElements, Me.ToolStripMenuItem4, Me.ToolStripMenuItemProjectProperties})
        Me.ToolStripMenuItemProject.Name = "ToolStripMenuItemProject"
        Me.ToolStripMenuItemProject.Size = New Size(56, 20)
        Me.ToolStripMenuItemProject.Text = "Project"
        '
        'ToolStripMenuItemProjectTracks
        '
        Me.ToolStripMenuItemProjectTracks.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItemProjectTracksAdd, Me.ToolStripMenuItemProjectTracksDelete})
        Me.ToolStripMenuItemProjectTracks.Name = "ToolStripMenuItemProjectTracks"
        Me.ToolStripMenuItemProjectTracks.Size = New Size(180, 22)
        Me.ToolStripMenuItemProjectTracks.Text = "Tracks"
        '
        'ToolStripMenuItemProjectTracksAdd
        '
        Me.ToolStripMenuItemProjectTracksAdd.Name = "ToolStripMenuItemProjectTracksAdd"
        Me.ToolStripMenuItemProjectTracksAdd.Size = New Size(180, 22)
        Me.ToolStripMenuItemProjectTracksAdd.Text = "Add"
        '
        'ToolStripMenuItemProjectTracksDelete
        '
        Me.ToolStripMenuItemProjectTracksDelete.Name = "ToolStripMenuItemProjectTracksDelete"
        Me.ToolStripMenuItemProjectTracksDelete.Size = New Size(180, 22)
        Me.ToolStripMenuItemProjectTracksDelete.Text = "Delete"
        '
        'ToolStripMenuItemProjectLayers
        '
        Me.ToolStripMenuItemProjectLayers.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItemProjectLayersAdd, Me.ToolStripMenuItemProjectLayersDelete})
        Me.ToolStripMenuItemProjectLayers.Name = "ToolStripMenuItemProjectLayers"
        Me.ToolStripMenuItemProjectLayers.Size = New Size(180, 22)
        Me.ToolStripMenuItemProjectLayers.Text = "Layers"
        '
        'ToolStripMenuItemProjectLayersAdd
        '
        Me.ToolStripMenuItemProjectLayersAdd.Name = "ToolStripMenuItemProjectLayersAdd"
        Me.ToolStripMenuItemProjectLayersAdd.Size = New Size(180, 22)
        Me.ToolStripMenuItemProjectLayersAdd.Text = "Add"
        '
        'ToolStripMenuItemProjectLayersDelete
        '
        Me.ToolStripMenuItemProjectLayersDelete.Name = "ToolStripMenuItemProjectLayersDelete"
        Me.ToolStripMenuItemProjectLayersDelete.Size = New Size(180, 22)
        Me.ToolStripMenuItemProjectLayersDelete.Text = "Delete"
        '
        'ToolStripMenuItemProjectElements
        '
        Me.ToolStripMenuItemProjectElements.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItemProjectElementsAdd, Me.ToolStripMenuItemProjectElementsDelete})
        Me.ToolStripMenuItemProjectElements.Name = "ToolStripMenuItemProjectElements"
        Me.ToolStripMenuItemProjectElements.Size = New Size(180, 22)
        Me.ToolStripMenuItemProjectElements.Text = "Elements"
        '
        'ToolStripMenuItemProjectElementsAdd
        '
        Me.ToolStripMenuItemProjectElementsAdd.Name = "ToolStripMenuItemProjectElementsAdd"
        Me.ToolStripMenuItemProjectElementsAdd.Size = New Size(180, 22)
        Me.ToolStripMenuItemProjectElementsAdd.Text = "Add"
        '
        'ToolStripMenuItemProjectElementsDelete
        '
        Me.ToolStripMenuItemProjectElementsDelete.Name = "ToolStripMenuItemProjectElementsDelete"
        Me.ToolStripMenuItemProjectElementsDelete.Size = New Size(180, 22)
        Me.ToolStripMenuItemProjectElementsDelete.Text = "Delete"
        '
        'ToolStripMenuItem4
        '
        Me.ToolStripMenuItem4.Name = "ToolStripMenuItem4"
        Me.ToolStripMenuItem4.Size = New Size(177, 6)
        '
        'ToolStripMenuItemProjectProperties
        '
        Me.ToolStripMenuItemProjectProperties.Name = "ToolStripMenuItemProjectProperties"
        Me.ToolStripMenuItemProjectProperties.Size = New Size(180, 22)
        Me.ToolStripMenuItemProjectProperties.Text = "Properties..."
        '
        'ToolStripMenuItemView
        '
        Me.ToolStripMenuItemView.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItemToggleMainRendererTransparency})
        Me.ToolStripMenuItemView.Name = "ToolStripMenuItemView"
        Me.ToolStripMenuItemView.Size = New Size(44, 20)
        Me.ToolStripMenuItemView.Text = "View"
        '
        'ToolStripMenuItemToggleMainRendererTransparency
        '
        Me.ToolStripMenuItemToggleMainRendererTransparency.Name = "ToolStripMenuItemToggleMainRendererTransparency"
        Me.ToolStripMenuItemToggleMainRendererTransparency.Size = New Size(263, 22)
        Me.ToolStripMenuItemToggleMainRendererTransparency.Text = "Toggle Main Renderer Transparency"
        '
        'FormMain
        '
        Me.AutoScaleDimensions = New SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New Size(1182, 844)
        Me.Controls.Add(Me.SplitContainerMain01)
        Me.Controls.Add(Me.MenuStripMain)
        Me.Font = New Font("Segoe UI", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Me.Name = "FormMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Prism"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.SplitContainerMain01.Panel1.ResumeLayout(False)
        Me.SplitContainerMain01.Panel2.ResumeLayout(False)
        CType(Me.SplitContainerMain01, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerMain01.ResumeLayout(False)
        Me.scMain02.Panel1.ResumeLayout(False)
        Me.scMain02.Panel2.ResumeLayout(False)
        CType(Me.scMain02, System.ComponentModel.ISupportInitialize).EndInit()
        Me.scMain02.ResumeLayout(False)
        Me.scMain03.Panel1.ResumeLayout(False)
        Me.scMain03.Panel2.ResumeLayout(False)
        CType(Me.scMain03, System.ComponentModel.ISupportInitialize).EndInit()
        Me.scMain03.ResumeLayout(False)
        Me.MenuStripMain.ResumeLayout(False)
        Me.MenuStripMain.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents RenderTracksPanel As Prism.RenderTracks
    Friend WithEvents RenderTimeLinePanel As Prism.RenderTimeline
    Friend WithEvents PropertyGridElement As System.Windows.Forms.PropertyGrid
    Friend WithEvents SplitContainerMain01 As System.Windows.Forms.SplitContainer
    Friend WithEvents scMain03 As System.Windows.Forms.SplitContainer
    Friend WithEvents scMain02 As System.Windows.Forms.SplitContainer
    Friend WithEvents RenderTrackScenePreview As Prism.RenderTrackScene
    Friend WithEvents ButtonStop As System.Windows.Forms.Button
    Friend WithEvents ButtonPause As Button
    Friend WithEvents ButtonPlay As System.Windows.Forms.Button
    Friend WithEvents CheckBoxAutoCreateKeyFrames As System.Windows.Forms.CheckBox
    Friend WithEvents MenuStripMain As System.Windows.Forms.MenuStrip
    Friend WithEvents ToolStripMenuItemFile As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItemFileNew As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItemFileSave As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItemFileSaveAs As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripMenuItem3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripMenuItemFileExit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripMenuItemFileOpen As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItemProject As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItemProjectTracks As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItemProjectTracksAdd As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItemProjectTracksDelete As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItemProjectLayers As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItemProjectLayersAdd As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItemProjectLayersDelete As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItemProjectElements As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItemProjectElementsAdd As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItemProjectElementsDelete As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripMenuItemProjectProperties As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CheckBoxLiveMode As CheckBox
    Friend WithEvents ButtonApplyChanges As System.Windows.Forms.Button
    Friend WithEvents ToolStripMenuItemView As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItemToggleMainRendererTransparency As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CheckBoxEnablePreview As System.Windows.Forms.CheckBox

End Class
