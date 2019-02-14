<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormEditTimeLineProperties
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormEditTimeLineProperties))
        Me.PropertyGridElement = New System.Windows.Forms.PropertyGrid()
        Me.SuspendLayout()
        '
        'PropertyGridElement
        '
        Me.PropertyGridElement.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PropertyGridElement.Location = New Point(0, 0)
        Me.PropertyGridElement.Name = "PropertyGridElement"
        Me.PropertyGridElement.Size = New Size(517, 370)
        Me.PropertyGridElement.TabIndex = 3
        '
        'FormEditTimeLineProperties
        '
        Me.AutoScaleDimensions = New SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New Size(517, 370)
        Me.Controls.Add(Me.PropertyGridElement)
        Me.Font = New Font("Segoe UI", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FormEditTimeLineProperties"
        Me.Text = "TimeLine Properties"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PropertyGridElement As System.Windows.Forms.PropertyGrid
End Class
