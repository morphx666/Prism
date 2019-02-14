<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class RenderTimeline
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
        Me.components = New System.ComponentModel.Container()
        Me.VScrollBarMain = New System.Windows.Forms.VScrollBar()
        Me.HScrollBarMain = New System.Windows.Forms.HScrollBar()
        Me.ButtonVZoomIn = New System.Windows.Forms.Button()
        Me.ButtonVZoomOut = New System.Windows.Forms.Button()
        Me.ButtonHZoomOut = New System.Windows.Forms.Button()
        Me.ButtonHZoomIn = New System.Windows.Forms.Button()
        Me.ContextMenuElement = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ContextMenuElementDelete = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripSeparator()
        Me.ContextMenuElementAddLayer = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuElementDeleteLayer = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuKeyFrames = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ContextMenuKeyFramesAdd = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuKeyFramesDelete = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ContextMenuKeyFramesCopy = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuKeyFramesPaste = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuTimeline = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ContextMenuTimelineAddElement = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ContextMenuTimelineAddLayer = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuTimelineDeleteLayer = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator()
        Me.ContextMenuTimelineProperties = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuElement.SuspendLayout()
        Me.ContextMenuKeyFrames.SuspendLayout()
        Me.ContextMenuTimeline.SuspendLayout()
        Me.SuspendLayout()
        '
        'VScrollBarMain
        '
        Me.VScrollBarMain.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.VScrollBarMain.Location = New Point(407, 1)
        Me.VScrollBarMain.Name = "VScrollBarMain"
        Me.VScrollBarMain.Size = New Size(17, 226)
        Me.VScrollBarMain.TabIndex = 0
        '
        'HScrollBarMain
        '
        Me.HScrollBarMain.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.HScrollBarMain.Location = New Point(1, 259)
        Me.HScrollBarMain.Name = "HScrollBarMain"
        Me.HScrollBarMain.Size = New Size(374, 17)
        Me.HScrollBarMain.TabIndex = 1
        '
        'ButtonVZoomIn
        '
        Me.ButtonVZoomIn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonVZoomIn.FlatAppearance.BorderColor = Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.ButtonVZoomIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonVZoomIn.Location = New Point(407, 227)
        Me.ButtonVZoomIn.Margin = New System.Windows.Forms.Padding(0)
        Me.ButtonVZoomIn.Name = "ButtonVZoomIn"
        Me.ButtonVZoomIn.Size = New Size(17, 17)
        Me.ButtonVZoomIn.TabIndex = 2
        Me.ButtonVZoomIn.TabStop = False
        Me.ButtonVZoomIn.Text = "+"
        Me.ButtonVZoomIn.UseCompatibleTextRendering = True
        Me.ButtonVZoomIn.UseMnemonic = False
        Me.ButtonVZoomIn.UseVisualStyleBackColor = True
        '
        'ButtonVZoomOut
        '
        Me.ButtonVZoomOut.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonVZoomOut.FlatAppearance.BorderColor = Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.ButtonVZoomOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonVZoomOut.Location = New Point(407, 243)
        Me.ButtonVZoomOut.Margin = New System.Windows.Forms.Padding(0)
        Me.ButtonVZoomOut.Name = "ButtonVZoomOut"
        Me.ButtonVZoomOut.Size = New Size(17, 17)
        Me.ButtonVZoomOut.TabIndex = 3
        Me.ButtonVZoomOut.TabStop = False
        Me.ButtonVZoomOut.Text = "-"
        Me.ButtonVZoomOut.UseCompatibleTextRendering = True
        Me.ButtonVZoomOut.UseMnemonic = False
        Me.ButtonVZoomOut.UseVisualStyleBackColor = True
        '
        'ButtonHZoomOut
        '
        Me.ButtonHZoomOut.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonHZoomOut.FlatAppearance.BorderColor = Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.ButtonHZoomOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonHZoomOut.Location = New Point(391, 259)
        Me.ButtonHZoomOut.Margin = New System.Windows.Forms.Padding(0)
        Me.ButtonHZoomOut.Name = "ButtonHZoomOut"
        Me.ButtonHZoomOut.Size = New Size(17, 17)
        Me.ButtonHZoomOut.TabIndex = 5
        Me.ButtonHZoomOut.TabStop = False
        Me.ButtonHZoomOut.Text = "-"
        Me.ButtonHZoomOut.UseCompatibleTextRendering = True
        Me.ButtonHZoomOut.UseMnemonic = False
        Me.ButtonHZoomOut.UseVisualStyleBackColor = True
        '
        'ButtonHZoomIn
        '
        Me.ButtonHZoomIn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonHZoomIn.FlatAppearance.BorderColor = Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.ButtonHZoomIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonHZoomIn.Location = New Point(375, 259)
        Me.ButtonHZoomIn.Margin = New System.Windows.Forms.Padding(0)
        Me.ButtonHZoomIn.Name = "ButtonHZoomIn"
        Me.ButtonHZoomIn.Size = New Size(17, 17)
        Me.ButtonHZoomIn.TabIndex = 4
        Me.ButtonHZoomIn.TabStop = False
        Me.ButtonHZoomIn.Text = "+"
        Me.ButtonHZoomIn.UseCompatibleTextRendering = True
        Me.ButtonHZoomIn.UseMnemonic = False
        Me.ButtonHZoomIn.UseVisualStyleBackColor = True
        '
        'ContextMenuElement
        '
        Me.ContextMenuElement.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ContextMenuElementDelete, Me.ToolStripMenuItem4, Me.ContextMenuElementAddLayer, Me.ContextMenuElementDeleteLayer})
        Me.ContextMenuElement.Name = "cmsElement"
        Me.ContextMenuElement.Size = New Size(154, 76)
        '
        'ContextMenuElementDelete
        '
        Me.ContextMenuElementDelete.Name = "ContextMenuElementDelete"
        Me.ContextMenuElementDelete.Size = New Size(153, 22)
        Me.ContextMenuElementDelete.Text = "Delete Element"
        '
        'ToolStripMenuItem4
        '
        Me.ToolStripMenuItem4.Name = "ToolStripMenuItem4"
        Me.ToolStripMenuItem4.Size = New Size(150, 6)
        '
        'ContextMenuElementAddLayer
        '
        Me.ContextMenuElementAddLayer.Name = "ContextMenuElementAddLayer"
        Me.ContextMenuElementAddLayer.Size = New Size(153, 22)
        Me.ContextMenuElementAddLayer.Text = "Add Layer"
        '
        'ContextMenuElementDeleteLayer
        '
        Me.ContextMenuElementDeleteLayer.Name = "ContextMenuElementDeleteLayer"
        Me.ContextMenuElementDeleteLayer.Size = New Size(153, 22)
        Me.ContextMenuElementDeleteLayer.Text = "Delete Layer"
        '
        'ContextMenuKeyFrames
        '
        Me.ContextMenuKeyFrames.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ContextMenuKeyFramesAdd, Me.ContextMenuKeyFramesDelete, Me.ToolStripMenuItem1, Me.ContextMenuKeyFramesCopy, Me.ContextMenuKeyFramesPaste})
        Me.ContextMenuKeyFrames.Name = "cmsKeyFrames"
        Me.ContextMenuKeyFrames.Size = New Size(166, 98)
        '
        'ContextMenuKeyFramesAdd
        '
        Me.ContextMenuKeyFramesAdd.Name = "ContextMenuKeyFramesAdd"
        Me.ContextMenuKeyFramesAdd.Size = New Size(165, 22)
        Me.ContextMenuKeyFramesAdd.Text = "Add Key Frame"
        '
        'ContextMenuKeyFramesDelete
        '
        Me.ContextMenuKeyFramesDelete.Name = "ContextMenuKeyFramesDelete"
        Me.ContextMenuKeyFramesDelete.Size = New Size(165, 22)
        Me.ContextMenuKeyFramesDelete.Text = "Delete Key Frame"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New Size(162, 6)
        '
        'ContextMenuKeyFramesCopy
        '
        Me.ContextMenuKeyFramesCopy.Name = "ContextMenuKeyFramesCopy"
        Me.ContextMenuKeyFramesCopy.Size = New Size(165, 22)
        Me.ContextMenuKeyFramesCopy.Text = "Copy"
        '
        'ContextMenuKeyFramesPaste
        '
        Me.ContextMenuKeyFramesPaste.Name = "ContextMenuKeyFramesPaste"
        Me.ContextMenuKeyFramesPaste.Size = New Size(165, 22)
        Me.ContextMenuKeyFramesPaste.Text = "Paste"
        '
        'ContextMenuTimeline
        '
        Me.ContextMenuTimeline.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ContextMenuTimelineAddElement, Me.ToolStripMenuItem2, Me.ContextMenuTimelineAddLayer, Me.ContextMenuTimelineDeleteLayer, Me.ToolStripMenuItem3, Me.ContextMenuTimelineProperties})
        Me.ContextMenuTimeline.Name = "cmsTimeline"
        Me.ContextMenuTimeline.Size = New Size(143, 104)
        '
        'ContextMenuTimelineAddElement
        '
        Me.ContextMenuTimelineAddElement.Name = "ContextMenuTimelineAddElement"
        Me.ContextMenuTimelineAddElement.Size = New Size(142, 22)
        Me.ContextMenuTimelineAddElement.Text = "Add Element"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New Size(139, 6)
        '
        'ContextMenuTimelineAddLayer
        '
        Me.ContextMenuTimelineAddLayer.Name = "ContextMenuTimelineAddLayer"
        Me.ContextMenuTimelineAddLayer.Size = New Size(142, 22)
        Me.ContextMenuTimelineAddLayer.Text = "Add Layer"
        '
        'ContextMenuTimelineDeleteLayer
        '
        Me.ContextMenuTimelineDeleteLayer.Name = "ContextMenuTimelineDeleteLayer"
        Me.ContextMenuTimelineDeleteLayer.Size = New Size(142, 22)
        Me.ContextMenuTimelineDeleteLayer.Text = "Delete Layer"
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New Size(139, 6)
        '
        'ContextMenuTimelineProperties
        '
        Me.ContextMenuTimelineProperties.Name = "ContextMenuTimelineProperties"
        Me.ContextMenuTimelineProperties.Size = New Size(142, 22)
        Me.ContextMenuTimelineProperties.Text = "Properties..."
        '
        'RenderTimeline
        '
        Me.AutoScaleDimensions = New SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange
        Me.Controls.Add(Me.ButtonHZoomOut)
        Me.Controls.Add(Me.ButtonHZoomIn)
        Me.Controls.Add(Me.ButtonVZoomOut)
        Me.Controls.Add(Me.ButtonVZoomIn)
        Me.Controls.Add(Me.HScrollBarMain)
        Me.Controls.Add(Me.VScrollBarMain)
        Me.Font = New Font("Segoe UI", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
        Me.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Name = "RenderTimeline"
        Me.Size = New Size(425, 277)
        Me.ContextMenuElement.ResumeLayout(False)
        Me.ContextMenuKeyFrames.ResumeLayout(False)
        Me.ContextMenuTimeline.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents VScrollBarMain As System.Windows.Forms.VScrollBar
    Friend WithEvents HScrollBarMain As System.Windows.Forms.HScrollBar
    Friend WithEvents ButtonVZoomIn As System.Windows.Forms.Button
    Friend WithEvents ButtonVZoomOut As System.Windows.Forms.Button
    Friend WithEvents ButtonHZoomOut As System.Windows.Forms.Button
    Friend WithEvents ButtonHZoomIn As System.Windows.Forms.Button
    Friend WithEvents ContextMenuElement As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ContextMenuElementDelete As ToolStripMenuItem
    Friend WithEvents ContextMenuKeyFrames As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ContextMenuKeyFramesAdd As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ContextMenuKeyFramesDelete As ToolStripMenuItem
    Friend WithEvents ContextMenuTimeline As ContextMenuStrip
    Friend WithEvents ContextMenuTimelineAddElement As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ContextMenuKeyFramesCopy As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ContextMenuKeyFramesPaste As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ContextMenuTimelineProperties As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ContextMenuTimelineAddLayer As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ContextMenuTimelineDeleteLayer As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripMenuItem4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ContextMenuElementAddLayer As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ContextMenuElementDeleteLayer As System.Windows.Forms.ToolStripMenuItem
End Class
