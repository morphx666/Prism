Imports System.ComponentModel.Design

Public Class KeyFramesCollectionEditor
    Inherits CollectionEditor

    Public Delegate Sub MyPropertyValueChangedEventHandler(sender As Object, e As PropertyValueChangedEventArgs)
    Public Shared Event MyPropertyValueChanged As MyPropertyValueChangedEventHandler

    Public Delegate Sub MyPropertiesCollectionChangedEventHandler(sender As Object, e As MouseEventArgs)
    Public Shared Event MyPropertiesCollectionChanged As MyPropertiesCollectionChangedEventHandler

    Public Sub New(ByVal type As Type)
        MyBase.New(type)
    End Sub

    Protected Overrides Function CreateCollectionForm() As CollectionEditor.CollectionForm
        Dim collectionForm As CollectionForm = MyBase.CreateCollectionForm()

        Dim frmCollectionEditorForm As Form = CType(collectionForm, Form)
        frmCollectionEditorForm.Text = "KeyFrame Editor"
        Dim tlpLayout As TableLayoutPanel = CType(frmCollectionEditorForm.Controls(0), TableLayoutPanel)

        If tlpLayout IsNot Nothing Then
            For Each c In tlpLayout.Controls
                If TypeOf c Is PropertyGrid Then
                    Dim propertyGrid As PropertyGrid = CType(c, PropertyGrid)
                    AddHandler propertyGrid.PropertyValueChanged, AddressOf HandlePropertyValueChanged
                End If

                If TypeOf c Is TableLayoutPanel Then
                    Dim table As TableLayoutPanel = CType(c, TableLayoutPanel)

                    For Each c1 In table.Controls
                        If TypeOf c1 Is Button Then
                            If c1.Name = "addButton" OrElse c1.Name = "removeButton" Then
                                AddHandler CType(c1, Button).Click, AddressOf HandlePropertiesCollectionChanged
                            End If
                        End If
                    Next
                End If
            Next

            AddHandler frmCollectionEditorForm.FormClosed, Sub() RaiseEvent MyPropertiesCollectionChanged(Me, New MouseEventArgs(MouseButtons.None, 0, 0, 0, 0))
        End If

        Return collectionForm
    End Function

    Private Sub HandlePropertiesCollectionChanged(sender As Object, e As MouseEventArgs)
        RaiseEvent MyPropertiesCollectionChanged(Me, e)
    End Sub

    Private Sub HandlePropertyValueChanged(sender As Object, e As PropertyValueChangedEventArgs)
        RaiseEvent MyPropertyValueChanged(Me, e)
    End Sub
End Class