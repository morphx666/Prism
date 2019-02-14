<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ColorWheel
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextBoxRed = New System.Windows.Forms.TextBox()
        Me.TextBoxGreen = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TextBoxBlue = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.SliderBlue = New Prism.Slider()
        Me.SliderGreen = New Prism.Slider()
        Me.SliderRed = New Prism.Slider()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(3, 273)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(27, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Red"
        '
        'TextBoxRed
        '
        Me.TextBoxRed.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBoxRed.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxRed.Location = New System.Drawing.Point(395, 268)
        Me.TextBoxRed.Name = "TextBoxRed"
        Me.TextBoxRed.Size = New System.Drawing.Size(27, 22)
        Me.TextBoxRed.TabIndex = 1
        Me.TextBoxRed.Text = "000"
        Me.TextBoxRed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TextBoxGreen
        '
        Me.TextBoxGreen.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBoxGreen.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxGreen.Location = New System.Drawing.Point(395, 289)
        Me.TextBoxGreen.Name = "TextBoxGreen"
        Me.TextBoxGreen.Size = New System.Drawing.Size(27, 22)
        Me.TextBoxGreen.TabIndex = 3
        Me.TextBoxGreen.Text = "000"
        Me.TextBoxGreen.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label2
        '
        Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(3, 294)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(38, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Green"
        '
        'TextBoxBlue
        '
        Me.TextBoxBlue.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBoxBlue.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxBlue.Location = New System.Drawing.Point(395, 310)
        Me.TextBoxBlue.Name = "TextBoxBlue"
        Me.TextBoxBlue.Size = New System.Drawing.Size(27, 22)
        Me.TextBoxBlue.TabIndex = 5
        Me.TextBoxBlue.Text = "000"
        Me.TextBoxBlue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label3
        '
        Me.Label3.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(3, 315)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(30, 13)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Blue"
        '
        'SliderBlue
        '
        Me.SliderBlue.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SliderBlue.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SliderBlue.GrooveSize = 6
        Me.SliderBlue.HighlightColor = System.Drawing.Color.LightGray
        Me.SliderBlue.HighlightColorEnd = System.Drawing.Color.Blue
        Me.SliderBlue.HighlightMode = Prism.Slider.HighlightModeConstants.Full
        Me.SliderBlue.HighlightStyle = Prism.Slider.HighlightStyleConstants.Gradient
        Me.SliderBlue.KnobVisible = True
        Me.SliderBlue.LargeChange = 10
        Me.SliderBlue.Location = New System.Drawing.Point(45, 311)
        Me.SliderBlue.Max = 100
        Me.SliderBlue.Min = 0
        Me.SliderBlue.Name = "SliderBlue"
        Me.SliderBlue.Orientation = Prism.Slider.OrientationConstants.Horitzontal
        Me.SliderBlue.Size = New System.Drawing.Size(344, 20)
        Me.SliderBlue.SmallChange = 1
        Me.SliderBlue.TabIndex = 8
        Me.SliderBlue.TextAlign = Prism.Slider.TextAlignConstants.TopLeft
        Me.SliderBlue.TextVisible = False
        Me.SliderBlue.TickFrequency = 10
        Me.SliderBlue.TickStyle = Prism.Slider.TickStyleConstants.NoTicks
        Me.SliderBlue.Value = 0
        '
        'SliderGreen
        '
        Me.SliderGreen.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SliderGreen.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SliderGreen.GrooveSize = 6
        Me.SliderGreen.HighlightColor = System.Drawing.Color.LightGray
        Me.SliderGreen.HighlightColorEnd = System.Drawing.Color.Green
        Me.SliderGreen.HighlightMode = Prism.Slider.HighlightModeConstants.Full
        Me.SliderGreen.HighlightStyle = Prism.Slider.HighlightStyleConstants.Gradient
        Me.SliderGreen.KnobVisible = True
        Me.SliderGreen.LargeChange = 10
        Me.SliderGreen.Location = New System.Drawing.Point(45, 290)
        Me.SliderGreen.Max = 100
        Me.SliderGreen.Min = 0
        Me.SliderGreen.Name = "SliderGreen"
        Me.SliderGreen.Orientation = Prism.Slider.OrientationConstants.Horitzontal
        Me.SliderGreen.Size = New System.Drawing.Size(344, 20)
        Me.SliderGreen.SmallChange = 1
        Me.SliderGreen.TabIndex = 7
        Me.SliderGreen.TextAlign = Prism.Slider.TextAlignConstants.TopLeft
        Me.SliderGreen.TextVisible = False
        Me.SliderGreen.TickFrequency = 10
        Me.SliderGreen.TickStyle = Prism.Slider.TickStyleConstants.NoTicks
        Me.SliderGreen.Value = 0
        '
        'SliderRed
        '
        Me.SliderRed.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SliderRed.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SliderRed.GrooveSize = 6
        Me.SliderRed.HighlightColor = System.Drawing.Color.LightGray
        Me.SliderRed.HighlightColorEnd = System.Drawing.Color.Red
        Me.SliderRed.HighlightMode = Prism.Slider.HighlightModeConstants.Full
        Me.SliderRed.HighlightStyle = Prism.Slider.HighlightStyleConstants.Gradient
        Me.SliderRed.KnobVisible = True
        Me.SliderRed.LargeChange = 10
        Me.SliderRed.Location = New System.Drawing.Point(45, 269)
        Me.SliderRed.Max = 100
        Me.SliderRed.Min = 0
        Me.SliderRed.Name = "SliderRed"
        Me.SliderRed.Orientation = Prism.Slider.OrientationConstants.Horitzontal
        Me.SliderRed.Size = New System.Drawing.Size(344, 20)
        Me.SliderRed.SmallChange = 1
        Me.SliderRed.TabIndex = 6
        Me.SliderRed.TextAlign = Prism.Slider.TextAlignConstants.TopLeft
        Me.SliderRed.TextVisible = False
        Me.SliderRed.TickFrequency = 10
        Me.SliderRed.TickStyle = Prism.Slider.TickStyleConstants.NoTicks
        Me.SliderRed.Value = 0
        '
        'ColorWheel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.SliderBlue)
        Me.Controls.Add(Me.SliderGreen)
        Me.Controls.Add(Me.SliderRed)
        Me.Controls.Add(Me.TextBoxBlue)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.TextBoxGreen)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.TextBoxRed)
        Me.Controls.Add(Me.Label1)
        Me.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(8)
        Me.Name = "ColorWheel"
        Me.Size = New System.Drawing.Size(425, 335)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TextBoxRed As System.Windows.Forms.TextBox
    Friend WithEvents TextBoxGreen As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents TextBoxBlue As TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents SliderRed As Slider
    Friend WithEvents SliderGreen As Slider
    Friend WithEvents SliderBlue As Slider

End Class
