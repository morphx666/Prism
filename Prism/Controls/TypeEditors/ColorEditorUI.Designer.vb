<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ColorEditorUI
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
        Me.ButtonOK = New System.Windows.Forms.Button()
        Me.ButtonCancel = New System.Windows.Forms.Button()
        Me.TabControlControl = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.ColorWheelControl = New Prism.ColorWheel()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.ListBoxWebColors = New System.Windows.Forms.ListBox()
        Me.TabControlControl.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.SuspendLayout()
        '
        'ButtonOK
        '
        Me.ButtonOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonOK.Location = New System.Drawing.Point(126, 309)
        Me.ButtonOK.Name = "ButtonOK"
        Me.ButtonOK.Size = New System.Drawing.Size(75, 23)
        Me.ButtonOK.TabIndex = 1
        Me.ButtonOK.Text = "OK"
        Me.ButtonOK.UseVisualStyleBackColor = True
        '
        'ButtonCancel
        '
        Me.ButtonCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonCancel.Location = New System.Drawing.Point(207, 309)
        Me.ButtonCancel.Name = "ButtonCancel"
        Me.ButtonCancel.Size = New System.Drawing.Size(75, 23)
        Me.ButtonCancel.TabIndex = 2
        Me.ButtonCancel.Text = "Cancel"
        Me.ButtonCancel.UseVisualStyleBackColor = True
        '
        'TabControlControl
        '
        Me.TabControlControl.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TabControlControl.Controls.Add(Me.TabPage1)
        Me.TabControlControl.Controls.Add(Me.TabPage2)
        Me.TabControlControl.Location = New System.Drawing.Point(8, 8)
        Me.TabControlControl.Margin = New System.Windows.Forms.Padding(8)
        Me.TabControlControl.Name = "TabControlControl"
        Me.TabControlControl.SelectedIndex = 0
        Me.TabControlControl.Size = New System.Drawing.Size(274, 300)
        Me.TabControlControl.TabIndex = 3
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.ColorWheelControl)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(266, 274)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Custom"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'ColorWheelControl
        '
        Me.ColorWheelControl.Color = System.Drawing.Color.FromArgb(CType(CType(173, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(47, Byte), Integer))
        Me.ColorWheelControl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ColorWheelControl.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ColorWheelControl.Location = New System.Drawing.Point(3, 3)
        Me.ColorWheelControl.Margin = New System.Windows.Forms.Padding(8)
        Me.ColorWheelControl.Name = "ColorWheelControl"
        Me.ColorWheelControl.Size = New System.Drawing.Size(260, 268)
        Me.ColorWheelControl.TabIndex = 0
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.ListBoxWebColors)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(266, 274)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Common"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'ListBoxWebColors
        '
        Me.ListBoxWebColors.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.ListBoxWebColors.FormattingEnabled = True
        Me.ListBoxWebColors.IntegralHeight = False
        Me.ListBoxWebColors.Location = New System.Drawing.Point(6, 6)
        Me.ListBoxWebColors.Name = "ListBoxWebColors"
        Me.ListBoxWebColors.Size = New System.Drawing.Size(254, 257)
        Me.ListBoxWebColors.TabIndex = 0
        '
        'ColorEditorUI
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TabControlControl)
        Me.Controls.Add(Me.ButtonCancel)
        Me.Controls.Add(Me.ButtonOK)
        Me.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "ColorEditorUI"
        Me.Size = New System.Drawing.Size(290, 340)
        Me.TabControlControl.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ColorWheelControl As ColorWheel
    Friend WithEvents ButtonOK As System.Windows.Forms.Button
    Friend WithEvents ButtonCancel As System.Windows.Forms.Button
    Friend WithEvents TabControlControl As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents ListBoxWebColors As System.Windows.Forms.ListBox

End Class
