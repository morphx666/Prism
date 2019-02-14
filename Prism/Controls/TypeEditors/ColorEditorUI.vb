Imports System.Windows.Forms.Design
Imports System.ComponentModel
Imports System.Drawing.Design

Public Class ColorEditorUI
    Private service As IWindowsFormsEditorService
    Public editValue As Object

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.SetStyle(ControlStyles.Selectable, True)
    End Sub

    Public Sub New(editorService As IWindowsFormsEditorService, value As Object)
        Me.New()

        service = editorService
        editValue = value
    End Sub

    Public Sub New(value As Object)
        Me.New()
        editValue = value
    End Sub

    Private Sub ButtonOK_Click(sender As Object, e As EventArgs) Handles ButtonOK.Click
        SaveChanges()
    End Sub

    Private Sub ColorEditorUI_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ListBoxWebColors.ItemHeight += 2
        For Each c In [Enum].GetNames(GetType(KnownColor))
            ListBoxWebColors.Items.Add(c)
        Next

        If TypeOf editValue Is Pen Then
            ColorWheelControl.Color = CType(editValue, Pen).Color
        ElseIf TypeOf editValue Is SolidBrush Then
            ColorWheelControl.Color = CType(editValue, SolidBrush).Color
        ElseIf TypeOf editValue Is Color Then
            ColorWheelControl.Color = CType(editValue, Color)
        End If
    End Sub

    Public Property ShowButtons As Boolean
        Get
            Return ButtonOK.Visible
        End Get
        Set(value As Boolean)
            If value = ButtonOK.Visible Then Exit Property

            Dim d As Integer = ButtonOK.Height + ButtonOK.Margin.Horizontal

            If value Then
                ButtonOK.Visible = True
                TabControlControl.Height -= d
            Else
                ButtonOK.Visible = False
                TabControlControl.Height += d
            End If
        End Set
    End Property

    Private Sub ButtonCancel_Click(sender As System.Object, e As EventArgs) Handles ButtonCancel.Click
        service?.CloseDropDown()
    End Sub

    Private Sub SaveChanges()
        If TypeOf editValue Is Pen Then
            editValue = New Pen(ColorWheelControl.Color)
        ElseIf TypeOf editValue Is SolidBrush Then
            editValue = New SolidBrush(ColorWheelControl.Color)
        ElseIf TypeOf editValue Is Color Then
            editValue = ColorWheelControl.Color
        End If

        service?.CloseDropDown()
    End Sub

    Private Sub ListBoxWebColors_DrawItem(sender As Object, e As DrawItemEventArgs) Handles ListBoxWebColors.DrawItem
        Dim g As Graphics = e.Graphics
        Dim r As Rectangle = e.Bounds
        Dim s As Integer = Math.Min(r.Width, r.Height) - 2
        Dim item = ListBoxWebColors.Items(e.Index)
        Dim c As Color = Color.FromName(item)
        Dim textColor As Color = ListBoxWebColors.ForeColor

        e.DrawBackground()

        If (e.State And DrawItemState.Selected) = DrawItemState.Selected Then
            textColor = Color.FromKnownColor(KnownColor.HighlightText)
        End If

        Using b As SolidBrush = New SolidBrush(c)
            g.FillRectangle(b, r.X + 1, r.Y + 1, s * 2, s)
        End Using

        r.Width -= s * 2 - 2
        r.X += s * 2 + 2
        Using b As SolidBrush = New SolidBrush(textColor)
            g.DrawString(item, ListBoxWebColors.Font, b, r)
        End Using

        e.DrawFocusRectangle()
    End Sub

    Private Sub ListBoxWebColors_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBoxWebColors.SelectedIndexChanged
        ColorWheelControl.Color = Color.FromName(ListBoxWebColors.SelectedItem)
    End Sub

    Private Sub ListBoxWebColors_DoubleClick(sender As Object, e As EventArgs) Handles ListBoxWebColors.DoubleClick
        SaveChanges()
    End Sub
End Class

Public Class ColorEditor
    Inherits UITypeEditor

    Public Overrides Function GetEditStyle(context As ITypeDescriptorContext) As Drawing.Design.UITypeEditorEditStyle
        Return UITypeEditorEditStyle.DropDown
    End Function

    Public Overrides Function EditValue(context As ITypeDescriptorContext, provider As IServiceProvider, value As Object) As Object
        Dim service As IWindowsFormsEditorService = CType(provider.GetService(GetType(IWindowsFormsEditorService)), IWindowsFormsEditorService)
        Dim v As Object = Nothing
        Using editor As ColorEditorUI = New ColorEditorUI(service, value)
            service.DropDownControl(editor)
            v = editor.editValue
        End Using
        Return v
    End Function

    Public Overrides Function GetPaintValueSupported(context As System.ComponentModel.ITypeDescriptorContext) As Boolean
        Return True
    End Function

    Public Overrides Sub PaintValue(e As PaintValueEventArgs)
        Dim g As Graphics = e.Graphics
        Dim r As Rectangle = e.Bounds
        Dim s As Integer = Math.Max(r.Width, r.Height) - 2
        Dim c As Color

        If TypeOf e.Value Is Pen Then
            c = CType(e.Value, Pen).Color
        ElseIf TypeOf e.Value Is Brush Then
            c = CType(e.Value, SolidBrush).Color
        ElseIf TypeOf e.Value Is Color Then
            c = CType(e.Value, Color)
        End If

        Using b As SolidBrush = New SolidBrush(c)
            If b.Color.A < 255 Then ColorWheel.DrawTransparency(g, e.Bounds, False)
            g.FillRectangle(b, r.X + 1, r.Y + 1, s, s)
        End Using

        MyBase.PaintValue(e)
    End Sub
End Class

Public Class ColorConverter
    Inherits TypeConverter

    Dim valueType As Type

    Public Overrides Function GetProperties(context As ITypeDescriptorContext, value As Object, attributes() As Attribute) As PropertyDescriptorCollection
        Return TypeDescriptor.GetProperties(value, attributes)
    End Function

    Public Overrides Function GetPropertiesSupported(context As ITypeDescriptorContext) As Boolean
        Return MyBase.GetPropertiesSupported(context)
    End Function

    Public Overrides Function ConvertTo(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object, destinationType As Type) As Object
        If destinationType Is GetType(String) Then
            Dim c As Color
            If TypeOf value Is Pen Then
                c = CType(value, Pen).Color
                valueType = GetType(Pen)
            ElseIf TypeOf value Is Brush Then
                c = CType(value, SolidBrush).Color
                valueType = GetType(SolidBrush)
            ElseIf TypeOf value Is Color Then
                c = CType(value, Color)
                valueType = GetType(Color)
            End If

            Return If(c.IsNamedColor,
                        String.Format("{0}", c.Name),
                        String.Format("{0}, {1}, {2}, {3}", c.A, c.R, c.G, c.B))
        End If
        Return MyBase.ConvertTo(value, destinationType)
    End Function

    Public Overrides Function CanConvertFrom(context As ITypeDescriptorContext, sourceType As Type) As Boolean
        Return If(sourceType Is GetType(String), True, MyBase.CanConvertFrom(context, sourceType))
    End Function

    Public Overrides Function ConvertFrom(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object) As Object
        If TypeOf value Is String Then
            Dim v() As String = CType(value, String).Split(",")

            Dim c = If(v.Length = 1,
                        Color.FromName(v(0)),
                        Color.FromArgb(v(0), v(1), v(2), v(3)))

            If valueType Is GetType(Pen) Then
                Return New Pen(c)
            ElseIf valueType Is GetType(SolidBrush) Then
                Return New SolidBrush(c)
            ElseIf valueType Is GetType(Color) Then
                Return c
            End If
        End If
        Return MyBase.ConvertFrom(context, culture, value)
    End Function
End Class

