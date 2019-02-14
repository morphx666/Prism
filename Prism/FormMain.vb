Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Xml

Public Class FormMain
    Private project As Project
    Private selectedTrack As Project.Track
    Private selectedLayer As Project.Track.Layer
    Private selectedElements As New List(Of Project.Track.Layer.Element)
    Private selectedKeyframe As Project.Track.Layer.Element.KeyFrame

    Private mainRendererProject As Project

    Private Delegate Sub SaafeInvoke()
    Private tmrRefreshPropertyGrid As Threading.Timer = New Threading.Timer(Sub() PropertyGridElement.Invoke(New SaafeInvoke(Sub() PropertyGridElement.Refresh())))

    Private Sub FormMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        CheckBoxLiveMode.Checked = False
        NewProject()
    End Sub

    Private Sub FormMain_Load(sender As Object, e1 As EventArgs) Handles MyBase.Load
        RenderTrackScenePreview.SafeAreasVisible = True
        RenderTrackScenePreview.IsMainRenderer = False

        NewProject()
    End Sub

    Private Sub RenderTracksPanel_TrackSelected(sender As Object, e As EventArgs) Handles RenderTracksPanel.TrackSelected
        Dim track As Project.Track = CType(sender, RenderTrack).Track
        SwitchSelectedTrack(track)
    End Sub

    Private Sub RenderTimeLinePanel_CursorPositionChanged(time As TimeSpan) Handles RenderTimeLinePanel.CursorPositionChanged
        RepaintRenderers(False)
        If selectedElements IsNot Nothing Then UpdatePropertyGrid()

        If mainRendererIsAvailable AndAlso CheckBoxLiveMode.Checked Then
            If mainRendererProject.IsIdle Then
                For Each track In mainRendererProject.Tracks
                    For Each layer In track.Layers
                        layer.CursorTime = time
                    Next
                Next
            End If
        End If
    End Sub

    Private Sub RenderTimeLinePanel_ObjectSelected(elements As List(Of Project.Track.Layer.Element), keyFrame As Project.Track.Layer.Element.KeyFrame, ByVal layer As Project.Track.Layer) Handles RenderTimeLinePanel.ObjectsSelected
        selectedKeyframe = keyFrame
        selectedLayer = layer
        selectedElements = elements

        If keyFrame IsNot Nothing Then
            If Not keyFrame.Equals(PropertyGridElement.SelectedObject) Then PropertyGridElement.SelectedObject = keyFrame
        ElseIf elements IsNot Nothing AndAlso elements.Count > 0 Then
            Dim doUpdate As Boolean = PropertyGridElement.SelectedObjects.Count <> elements.Count
            If Not doUpdate Then
                For Each e1 In elements
                    Dim wasFound As Boolean = False
                    For Each e2 In PropertyGridElement.SelectedObjects
                        If Not (TypeOf e2 Is Project.Track.Layer.Element) Then
                            Exit For
                        ElseIf CType(e2, Project.Track.Layer.Element).Equals(e1) Then
                            wasFound = True
                            Exit For
                        End If
                    Next
                    If Not wasFound Then
                        doUpdate = True
                        Exit For
                    End If
                Next
            End If

            If doUpdate Then PropertyGridElement.SelectedObjects = elements.ToArray()
        ElseIf layer IsNot Nothing Then
            If Not layer.Equals(PropertyGridElement.SelectedObject) Then PropertyGridElement.SelectedObject = layer
        Else
            PropertyGridElement.SelectedObject = Nothing
        End If

        RenderTrackScenePreview.SelectedElements = elements
    End Sub

    Private Sub RenderTimeLinePanel_PropertiesChanged(sender As Object, e As PropertyValueChangedEventArgs) Handles RenderTimeLinePanel.KeyFramesChanged
        RenderTimeLinePanel.Repaint()

        RenderTrackScenePreview.Repaint(True)
        If mainRendererIsAvailable Then mainRenderer.RenderTrackSceneMain.Repaint(True)
    End Sub

    Private Sub PropertyGridElement_PropertyValueChanged(ByVal s As Object, ByVal e As PropertyValueChangedEventArgs) Handles PropertyGridElement.PropertyValueChanged
        RenderTimeLinePanel.Repaint()

        RenderTrackScenePreview.Repaint(True)
        If mainRendererIsAvailable Then mainRenderer.RenderTrackSceneMain.Repaint(True)
    End Sub

    Private Sub RenderTrackScenePreview_ElementSelected(ByVal elements As List(Of Project.Track.Layer.Element)) Handles RenderTrackScenePreview.ElementsSelected
        PropertyGridElement.SelectedObjects = elements.ToArray()
        RenderTrackScenePreview.SelectedElements = elements
        RenderTimeLinePanel.SelectedElements = elements
    End Sub

    Private Sub ButtonPlay_Click(sender As Object, e As EventArgs) Handles ButtonPlay.Click
        If selectedTrack IsNot Nothing Then
            selectedTrack.Play()

            If mainRendererIsAvailable Then
                For Each track In mainRenderer.RenderTrackSceneMain.Project.Tracks
                    If track.ID = selectedTrack.ID Then
                        track.Play()
                        Exit For
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub ButtonPause_Click(sender As Object, e As EventArgs) Handles ButtonPause.Click
        If selectedTrack IsNot Nothing Then
            selectedTrack.Pause()
            If mainRendererIsAvailable Then
                For Each track In mainRenderer.RenderTrackSceneMain.Project.Tracks
                    If track.ID = selectedTrack.ID Then
                        track.Pause()
                        Exit For
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub ButtonStop_Click(sender As Object, e As EventArgs) Handles ButtonStop.Click
        If selectedTrack IsNot Nothing Then
            selectedTrack.Stop()
            If mainRendererIsAvailable Then
                For Each track In mainRenderer.RenderTrackSceneMain.Project.Tracks
                    If track.ID = selectedTrack.ID Then
                        track.Stop()
                        Exit For
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub RenderTrackScenePreview_SceneChangedByUser() Handles RenderTrackScenePreview.SceneChangedByUser
        If mainRendererIsAvailable Then mainRenderer.RenderTrackSceneMain.Repaint()
        UpdatePropertyGrid()
    End Sub

    Private Sub UpdatePropertyGrid()
        tmrRefreshPropertyGrid.Change(100, Threading.Timeout.Infinite)
    End Sub

    Private Sub CheckBoxAutoCreateKeyFrames_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxAutoCreateKeyFrames.CheckedChanged
        RenderTrackScenePreview.AutoCreateKeyFrames = CheckBoxAutoCreateKeyFrames.Checked
    End Sub

    Private Sub ToolStripMenuItemFileOpen_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemFileOpen.Click
        OpenProject()
    End Sub

    Private Sub ToolStripMenuItemFileSave_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemFileSave.Click
        SaveProject()
    End Sub

    Private Sub ToolStripMenuItemFileNew_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemFileNew.Click
        NewProject()
    End Sub

    Private Sub ToolStripMenuItemFileSaveAs_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemFileSaveAs.Click
        SaveAsProject()
    End Sub

    Private Sub ToolStripMenuItemFileExit_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemFileExit.Click
        Me.Close()
    End Sub

    Private Function NewProject() As Boolean
        ' TODO: Must check for changed project and then decide to save it or to cancel the process

        If project IsNot Nothing Then project.StopTracks()
        If mainRendererProject IsNot Nothing Then mainRendererProject.StopTracks()

        project = New Project()

        Dim track As Project.Track = New Project.Track(project, "New Track 01")
        track.Layers.Add(New Project.Track.Layer(track, "New Layer 01"))
        project.Tracks.Add(track)

        PrepareProject()
        SwitchSelectedTrack(track)

        Return True
    End Function

    Private Sub OpenProject()
        If NewProject() Then
            Using dlg = New OpenFileDialog()
                dlg.DefaultExt = "pprj"
                dlg.DereferenceLinks = True
                dlg.Filter = "Prism Projects|*.pprj"
                dlg.Title = "Open Prism Project"
                If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
                    If Not IO.File.Exists(dlg.FileName) Then
                        MsgBox("The selected file could not be found", MsgBoxStyle.Information)
                        Exit Sub
                    End If

                    Dim xml = XDocument.Load(dlg.FileName)
                    project = Project.FromXML(xml...<project>.First())
                    project.FileName = dlg.FileName

                    PrepareProject()
                End If
            End Using
        End If
    End Sub

    Private Sub SaveProject()
        Dim file As IO.FileInfo = Nothing

        Try
            file = New IO.FileInfo(project.FileName)
        Catch
            SaveAsProject()
            Exit Sub
        End Try

        If Not IO.Directory.Exists(file.DirectoryName) Then
            SaveAsProject()
        Else
            If IO.File.Exists(project.FileName) Then IO.File.Delete(project.FileName)

            Dim xml = <prism>
                          <projects>
                              <%= project.ToXML() %>
                          </projects>
                      </prism>

            xml.Save(project.FileName)
            PrepareProject(False)
        End If
    End Sub

    Private Sub SaveAsProject()
        Using dlg = New SaveFileDialog()
            dlg.DefaultExt = "pprj"
            dlg.DereferenceLinks = True
            dlg.Filter = "Prism Projects|*.pprj"
            dlg.Title = "Save Prism Project As"
            If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
                If IO.File.Exists(dlg.FileName) Then IO.File.Delete(dlg.FileName)
                project.FileName = dlg.FileName
                SaveProject()
            End If
        End Using
    End Sub

    Private Sub SetupRenderers()
        If Screen.AllScreens.Length > 1 Then
            If mainRenderer Is Nothing Then
                mainRenderer = New FormMainRenderer()
                mainRenderer.RenderTrackSceneMain.IsMainRenderer = True
                mainRenderer.StartPosition = FormStartPosition.Manual

                mainRenderer.Show()

                mainRendererIsAvailable = True
            End If

            mainRenderer.Bounds = New Rectangle(Screen.AllScreens(1).Bounds.Location, project.Resolution)
            If mainRendererProject IsNot Nothing AndAlso Not mainRendererProject.Equals(project) Then mainRendererProject.StopTracks()

            If project IsNot Nothing Then
                If CheckBoxLiveMode.Checked Then
                    mainRendererProject = project.Clone()
                    mainRendererProject.Name = "MAIN OUTPUT"
                    mainRenderer.RenderTrackSceneMain.Project = mainRendererProject


                    If project.Tracks.Count > 0 Then
                        For trackIndex = 0 To project.Tracks.Count - 1

                            For layerIndex As Integer = 0 To project.Tracks(trackIndex).Layers.Count - 1
                                mainRendererProject.Tracks(trackIndex).Layers(layerIndex).CursorTime = project.Tracks(trackIndex).Layers(layerIndex).CursorTime
                            Next

                            Select Case project.Tracks(trackIndex).State
                                Case Prism.Project.Track.StateConstants.Paused
                                    mainRendererProject.Tracks(trackIndex).Pause()
                                Case Prism.Project.Track.StateConstants.Playing
                                    mainRendererProject.Tracks(trackIndex).Play()
                            End Select
                        Next
                    End If
                Else
                    mainRendererProject = project
                End If
            Else
                mainRenderer.Bounds = Screen.AllScreens(1).Bounds
            End If
        End If

        If mainRendererIsAvailable AndAlso Not mainRendererProject.Equals(mainRenderer.RenderTrackSceneMain.Project) Then mainRenderer.RenderTrackSceneMain.Project = mainRendererProject
        If Not project.Equals(RenderTrackScenePreview.Project) Then RenderTrackScenePreview.Project = project

        CheckBoxLiveMode.Enabled = mainRendererIsAvailable
        ButtonApplyChanges.Enabled = CheckBoxLiveMode.Checked AndAlso mainRendererIsAvailable

        SetPreviewRendererAspectRatio()

        CheckBoxAutoCreateKeyFrames.Top = RenderTrackScenePreview.Bottom - CheckBoxAutoCreateKeyFrames.Height
        RepaintRenderers(True)
    End Sub

    Private Sub SetPreviewRendererAspectRatio()
        RenderTrackScenePreview.Height = scMain02.Panel1.Height - RenderTrackScenePreview.Top * 2
        RenderTrackScenePreview.Width = If(project Is Nothing,
            RenderTrackScenePreview.Height,
            RenderTrackScenePreview.Height * (project.Resolution.Width / project.Resolution.Height))
    End Sub

    Private Sub PrepareProject(Optional ByVal resetSelectedTrack As Boolean = True)
        project.HasChanged = False
        RenderTracksPanel.Project = project

        AddHandler project.NewFrame, Sub(srcTrack As Project.Track, cursorTime As TimeSpan)
                                         RepaintRenderers(False)
                                         RenderTimeLinePanel.PlaybackCursorTime = cursorTime
                                     End Sub

        AddHandler project.StateChanged, Sub(srcTrack As Project.Track, state As Project.Track.StateConstants)
                                             If state = Project.Track.StateConstants.Idle Then
                                                 RenderTimeLinePanel.PlaybackCursorTime = TimeSpan.Zero
                                                 ' Are these really necessary?
                                                 ' Shouldn't the rtTimeline.PlaybackCursorTime trigger an event to force a repaint?
                                                 'rtsPreview.Repaint(False)
                                                 'If mainRendererIsAvailable Then mainRenderer.rtsMain.Repaint(False)
                                             End If
                                         End Sub

        SetupRenderers()
        If resetSelectedTrack Then SwitchSelectedTrack(Nothing)
        Me.Text = project.Name + " - Prism"
    End Sub

    Private Sub RepaintRenderers(resetCache As Boolean)
        If mainRendererIsAvailable Then mainRenderer.RenderTrackSceneMain.Repaint(resetCache)
        RenderTrackScenePreview.Repaint(resetCache)
    End Sub

    Private Sub SwitchSelectedTrack(ByVal track As Project.Track)
        If selectedTrack IsNot Nothing AndAlso selectedTrack.Equals(track) Then Exit Sub

        selectedTrack = track
        selectedLayer = Nothing
        selectedElements = Nothing

        If selectedTrack IsNot Nothing AndAlso selectedTrack.State <> Prism.Project.Track.StateConstants.Playing Then selectedTrack.CursorTime = TimeSpan.Zero

        RenderTimeLinePanel.Track = selectedTrack
        RenderTrackScenePreview.EditingTrack = selectedTrack
        RenderTrackScenePreview.SelectedElements = selectedElements
        RenderTracksPanel.SetSelectedTrack(selectedTrack)

        If mainRendererIsAvailable Then
            mainRenderer.RenderTrackSceneMain.EditingTrack = If(CheckBoxLiveMode.Checked AndAlso track IsNot Nothing,
                (From trk In mainRenderer.RenderTrackSceneMain.Project.Tracks Select trk Where trk.ID = selectedTrack.ID).Single(),
                selectedTrack)
        End If
    End Sub

    Private Sub ToolStripMenuItemProject_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemProject.Click
        ToolStripMenuItemProjectTracksDelete.Enabled = selectedTrack IsNot Nothing
        ToolStripMenuItemProjectLayersAdd.Enabled = selectedTrack IsNot Nothing
        ToolStripMenuItemProjectLayersDelete.Enabled = selectedLayer IsNot Nothing
        ToolStripMenuItemProjectElementsAdd.Enabled = selectedLayer IsNot Nothing
        ToolStripMenuItemProjectElementsDelete.Enabled = selectedElements IsNot Nothing AndAlso selectedElements.Count <> 0

        If Not ToolStripMenuItemProjectElementsAdd.HasDropDownItems Then
            Dim elementsTypes As List(Of Type) = Project.GetAvailableElementsTypes()
            For Each elementType As Type In elementsTypes
                Dim mi As New ToolStripMenuItem(elementType.Name.Replace("Element", "")) With {.Tag = elementType}
                ToolStripMenuItemProjectElementsAdd.DropDownItems.Add(mi)
                AddHandler mi.Click, AddressOf RenderTimeLinePanel.AddNewElementFromContextMenu
            Next
        End If
    End Sub

    Private Sub ToolStripMenuItemProjectTracksAdd_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemProjectTracksAdd.Click
        Dim index As Integer = (From t In project.Tracks Select t Where t.Name.StartsWith("New Track ")).Count + 1
        Dim strIndex As String = IIf(index < 10, "0", "") + index.ToString()
        Dim newTrack = New Project.Track(project, "New Track " + strIndex)
        project.Tracks.Add(newTrack)
        SwitchSelectedTrack(newTrack)
    End Sub

    Private Sub ToolStripMenuItemProjectTracksDelete_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemProjectTracksDelete.Click
        Dim index As Integer = project.Tracks.IndexOf(selectedTrack)
        project.Tracks.Remove(selectedTrack)

        If project.Tracks.Count > 0 Then
            If index >= project.Tracks.Count Then
                SwitchSelectedTrack(project.Tracks.Last)
            Else
                SwitchSelectedTrack(project.Tracks(index))
            End If
        Else
            SwitchSelectedTrack(Nothing)
        End If
    End Sub

    Private Sub ToolStripMenuItemProjectLayersAdd_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemProjectLayersAdd.Click
        Dim index As Integer = (From l In selectedTrack.Layers Select l Where l.Name.StartsWith("New Layer ")).Count + 1
        Dim strIndex As String = IIf(index < 10, "0", "") + index.ToString()
        Dim newLayer = New Project.Track.Layer(selectedTrack, "New Layer " + strIndex)
        selectedTrack.Layers.Add(newLayer)
    End Sub

    Private Sub ToolStripMenuItemProjectLayersDelete_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemProjectLayersDelete.Click
        selectedTrack.Layers.Remove(selectedLayer)
    End Sub

    Private Sub ToolStripMenuItemProjectProperties_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemProjectProperties.Click
        Using dlg = New FormProjectProperties()
            dlg.TextBoxName.Text = project.Name
            dlg.TextBoxDescription.Text = project.Description
            dlg.TextBoxWidth.Text = project.Resolution.Width.ToString()
            dlg.TextBoxHeight.Text = project.Resolution.Height.ToString()
            If dlg.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
                project.Name = dlg.TextBoxName.Text
                project.Description = dlg.TextBoxDescription.Text

                Dim w As Integer = project.Resolution.Width
                Dim h As Integer = project.Resolution.Height
                Integer.TryParse(dlg.TextBoxWidth.Text, w)
                Integer.TryParse(dlg.TextBoxHeight.Text, h)
                project.Resolution = New Size(w, h)

                project.HasChanged = True

                PrepareProject()
            End If
        End Using
    End Sub

    Private Sub CheckBoxLiveMode_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxLiveMode.CheckedChanged
        SetupRenderers()
    End Sub

    Private Sub ButtonApplyChanges_Click(sender As Object, e As EventArgs) Handles ButtonApplyChanges.Click
        SetupRenderers()
    End Sub

    Private Sub SplitContainerMain02_Resize(sender As Object, e As EventArgs) Handles scMain02.Resize
        SetPreviewRendererAspectRatio()
    End Sub

    Private Sub ToggleMainRendererTransparencyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemToggleMainRendererTransparency.Click
        If mainRendererIsAvailable Then
            mainRenderer.TransparencyKey = If(mainRenderer.TransparencyKey = Color.Empty,
                                                Color.Black,
                                                Color.Empty)
        End If
    End Sub

    Private Sub CheckBoxEnableRendering_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxEnablePreview.CheckedChanged
        RenderTrackScenePreview.EnableRendering = CheckBoxEnablePreview.Checked
        RenderTrackScenePreview.Repaint()
    End Sub
End Class
