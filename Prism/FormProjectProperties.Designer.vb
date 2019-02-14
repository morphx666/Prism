<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormProjectProperties
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormProjectProperties))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextBoxName = New System.Windows.Forms.TextBox()
        Me.TextBoxDescription = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TextBoxWidth = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TextBoxHeight = New System.Windows.Forms.TextBox()
        Me.ButtonOK = New System.Windows.Forms.Button()
        Me.ButtonCancel = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.ButtonAuto = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New Size(36, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Name"
        '
        'TextBoxName
        '
        Me.TextBoxName.Location = New Point(102, 6)
        Me.TextBoxName.Name = "TextBoxName"
        Me.TextBoxName.Size = New Size(103, 22)
        Me.TextBoxName.TabIndex = 1
        '
        'TextBoxDescription
        '
        Me.TextBoxDescription.Location = New Point(102, 32)
        Me.TextBoxDescription.Name = "TextBoxDescription"
        Me.TextBoxDescription.Size = New Size(218, 22)
        Me.TextBoxDescription.TabIndex = 3
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New Point(12, 35)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New Size(66, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Description"
        '
        'TextBoxWidth
        '
        Me.TextBoxWidth.Location = New Point(102, 58)
        Me.TextBoxWidth.Name = "TextBoxWidth"
        Me.TextBoxWidth.Size = New Size(40, 22)
        Me.TextBoxWidth.TabIndex = 5
        Me.TextBoxWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New Point(12, 61)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New Size(63, 13)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Resolution"
        '
        'TextBoxHeight
        '
        Me.TextBoxHeight.Location = New Point(152, 58)
        Me.TextBoxHeight.Name = "TextBoxHeight"
        Me.TextBoxHeight.Size = New Size(40, 22)
        Me.TextBoxHeight.TabIndex = 6
        Me.TextBoxHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'ButtonOK
        '
        Me.ButtonOK.Location = New Point(164, 165)
        Me.ButtonOK.Name = "ButtonOK"
        Me.ButtonOK.Size = New Size(75, 23)
        Me.ButtonOK.TabIndex = 7
        Me.ButtonOK.Text = "OK"
        Me.ButtonOK.UseVisualStyleBackColor = True
        '
        'ButtonCancel
        '
        Me.ButtonCancel.Location = New Point(245, 165)
        Me.ButtonCancel.Name = "ButtonCancel"
        Me.ButtonCancel.Size = New Size(75, 23)
        Me.ButtonCancel.TabIndex = 8
        Me.ButtonCancel.Text = "Cancel"
        Me.ButtonCancel.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New Font("Segoe UI", 11.25!, FontStyle.Bold, GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New Point(140, 62)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New Size(14, 20)
        Me.Label4.TabIndex = 9
        Me.Label4.Text = "˟"
        '
        'ButtonAuto
        '
        Me.ButtonAuto.Location = New Point(200, 58)
        Me.ButtonAuto.Name = "ButtonAuto"
        Me.ButtonAuto.Size = New Size(58, 23)
        Me.ButtonAuto.TabIndex = 10
        Me.ButtonAuto.Text = "Auto"
        Me.ButtonAuto.UseVisualStyleBackColor = True
        '
        'FormProjectProperties
        '
        Me.AutoScaleDimensions = New SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New Size(332, 200)
        Me.Controls.Add(Me.ButtonAuto)
        Me.Controls.Add(Me.ButtonCancel)
        Me.Controls.Add(Me.ButtonOK)
        Me.Controls.Add(Me.TextBoxHeight)
        Me.Controls.Add(Me.TextBoxWidth)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.TextBoxDescription)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.TextBoxName)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label4)
        Me.Font = New Font("Segoe UI", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FormProjectProperties"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Project Properties"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TextBoxName As TextBox
    Friend WithEvents TextBoxDescription As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents TextBoxWidth As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TextBoxHeight As System.Windows.Forms.TextBox
    Friend WithEvents ButtonOK As System.Windows.Forms.Button
    Friend WithEvents ButtonCancel As System.Windows.Forms.Button
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents ButtonAuto As System.Windows.Forms.Button
End Class
