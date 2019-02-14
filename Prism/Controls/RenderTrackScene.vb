Imports Prism.Project.Track.Layer.Element
Imports Prism.Project.Track.Layer
Imports System.Threading
Imports System.ComponentModel

Public Class RenderTrackScene
    Private mProject As Project = Nothing
    Private mEditingTrack As Project.Track = Nothing
    Private mSelectedElements As List(Of Element) = New List(Of Element)
    Private mAutoCreateKeyFrames As Boolean = False
    Private mIsMainRenderer As Boolean

    Private mouseSize As New Size(1, 1)

    Private Structure GrabHandle
        Public Bounds As Rectangle
        Public Enabled As Boolean

        Public Sub New(ByVal bounds As Rectangle, ByVal enabled As Boolean)
            Me.Bounds = bounds
            Me.Enabled = enabled
        End Sub
    End Structure

    Private selectedBounds As Rectangle
    Private selectedRotation As Single
    Private grabHandles As Dictionary(Of GrabHandleConstants, GrabHandle) = New Dictionary(Of GrabHandleConstants, GrabHandle)
    Private mouseOrigin As Point = Point.Empty
    Private trueMouseOrigin As Point = Point.Empty
    Private isDragging As Boolean
    Private isCtrlDown As Boolean
    Private overGrabHandle As GrabHandleConstants = GrabHandleConstants.None
    Private selectionRect As Rectangle = Rectangle.Empty
    Private elementsBounds As Dictionary(Of Element, Rectangle) = New Dictionary(Of Element, Rectangle)

    Private Enum GrabHandleConstants
        None

        TopLeft
        TopCenter
        TopRight

        CenterLeft
        Body
        CenterRight

        BottomLeft
        BottomCenter
        BottomRight

        Rotation
    End Enum

    Public Event ElementsSelected(ByVal elements As List(Of Element))
    Public Event SceneChangedByUser()

    Public Property SafeAreasVisible As Boolean = False
    Public Property EnableRendering As Boolean = True

    Public Property IsMainRenderer As Boolean
        Get
            Return mIsMainRenderer
        End Get
        Set(ByVal value As Boolean)
            mIsMainRenderer = value

            Me.Size = New Size(720, 480)
        End Set
    End Property

    Public Property AutoCreateKeyFrames As Boolean
        Get
            Return mAutoCreateKeyFrames
        End Get
        Set(ByVal value As Boolean)
            mAutoCreateKeyFrames = value
        End Set
    End Property

    Public Property Project As Project
        Get
            Return mProject
        End Get
        Set(ByVal value As Project)
            mProject = value
            If mProject Is Nothing Then Exit Property

            AddHandler mProject.TracksChanged, Sub(p As Project)
                                                   For Each trk In p.Tracks
                                                       AddHandler trk.StateChanged, Sub(t As Project.Track, state As Project.Track.StateConstants)
                                                                                        Me.Invalidate()
                                                                                    End Sub
                                                   Next
                                               End Sub
        End Set
    End Property

    Public Property EditingTrack As Project.Track
        Get
            Return mEditingTrack
        End Get
        Set(ByVal value As Project.Track)
            If value Is Nothing Then
                mEditingTrack = Nothing
            Else
                mEditingTrack = (From trk In mProject.Tracks Select trk Where trk.ID = value.ID).Single()

                For Each layer As Project.Track.Layer In mEditingTrack.Layers
                    For Each element As Element In layer.Elements
                        If element.Type = IElement.TypeConstants.Video Then
                            AddHandler CType(element, ElementVideo).NewFrame, AddressOf Repaint
                        End If
                    Next
                Next
            End If
        End Set
    End Property

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property SelectedElements As List(Of Element)
        Get
            Return mSelectedElements
        End Get
        Set(ByVal value As List(Of Element))
            If value Is Nothing Then
                mSelectedElements.Clear()
            Else
                mSelectedElements = value
            End If
            Me.Invalidate()
        End Set
    End Property

    Private Sub RenderTrackScene_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        isCtrlDown = e.Control
    End Sub

    Private Sub RenderTrackScene_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        isCtrlDown = e.Control
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As System.Windows.Forms.Message, ByVal keyData As System.Windows.Forms.Keys) As Boolean
        Dim keyFrame As KeyFrame = Nothing
        Dim createNewKeyFrame As Boolean

        Dim isShiftPressed As Boolean = (keyData And Keys.Shift) = Keys.Shift
        Dim isCtrlPressed As Boolean = (keyData And Keys.Control) = Keys.Control
        Dim isAltPressed As Boolean = (keyData And Keys.Alt) = Keys.Alt

        If isShiftPressed Then keyData = keyData Xor Keys.Shift
        If isCtrlPressed Then keyData = keyData Xor Keys.Control
        If isAltPressed Then keyData = keyData Xor Keys.Alt

        If keyData = Keys.Up OrElse keyData = Keys.Right OrElse keyData = Keys.Down OrElse keyData = Keys.Left Then
            Dim incPos As Integer = IIf(isCtrlPressed, 10, 1)
            Dim incSize As Integer = IIf(isShiftPressed, incPos, 0) * IIf(isAltPressed, 2, 1)
            mSelectedElements.ForEach(Sub(element As Element) element.ClearBlendedKeyFramesCache())

            For Each element As Element In mSelectedElements
                If mAutoCreateKeyFrames Then
                    createNewKeyFrame = True
                    For Each keyFrame In element.KeyFrames
                        If keyFrame.Time = element.Layer.CursorTime Then
                            createNewKeyFrame = False
                            Exit For
                        End If
                    Next
                    If createNewKeyFrame Then
                        keyFrame = element.GetBlendedKeyFrame(element.Layer.CursorTime)
                        keyFrame.Time = element.Layer.CursorTime
                        element.KeyFrames.Add(keyFrame)
                    End If
                Else
                    keyFrame = element.GetClosestKeyFrame()
                End If

                Select Case keyData
                    Case Keys.Up
                        keyFrame.Bounds = New Rectangle(keyFrame.Bounds.X,
                                                         keyFrame.Bounds.Y - incPos,
                                                         keyFrame.Bounds.Width,
                                                         keyFrame.Bounds.Height + incSize)
                    Case Keys.Down
                        keyFrame.Bounds = New Rectangle(keyFrame.Bounds.X,
                                                         keyFrame.Bounds.Y + incPos,
                                                         keyFrame.Bounds.Width,
                                                         keyFrame.Bounds.Height - incSize)
                    Case Keys.Left
                        keyFrame.Bounds = New Rectangle(keyFrame.Bounds.X - incPos,
                                                         keyFrame.Bounds.Y,
                                                         keyFrame.Bounds.Width + incSize,
                                                         keyFrame.Bounds.Height)
                    Case Keys.Right
                        keyFrame.Bounds = New Rectangle(keyFrame.Bounds.X + incPos,
                                                         keyFrame.Bounds.Y,
                                                         keyFrame.Bounds.Width - incSize,
                                                         keyFrame.Bounds.Height)
                End Select
            Next

            Me.Invalidate()
            RaiseEvent SceneChangedByUser()
            Return True
        End If

        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub RenderTrackScene_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.SetStyle(ControlStyles.UserPaint, True)
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        Me.SetStyle(ControlStyles.Selectable, True)

        'VideoGrabber1.Visible = False

        'Using g As Graphics = Graphics.FromImage(bmp)
        'g.Clear(Me.BackColor)
        'End Using

        'With VideoGrabber1
        '    .FrameRate = 29.97

        '    .VideoRenderer = VidGrab.TVideoRenderer.vr_AutoSelect
        '    .VideoSource = VidGrab.TVideoSource.vs_VideoCaptureDevice
        '    .VideoRendererExternal = VidGrab.TVideoRendererExternal.vre_Decklink_Extreme

        '    .VideoInput = 3

        '    .UseNearestVideoSize(720, 480, True)
        '    '.SetVideoDoubleBuffered(True)

        '    .StartPreview()

        '    .Dock = DockStyle.Fill
        'End With
    End Sub

    Private ReadOnly Property GetActiveElements(ByVal reverse As Boolean, ByVal track As Project.Track) As List(Of Element)
        Get
            Dim elements As List(Of Element) = New List(Of Element)
            If track IsNot Nothing Then
                For Each layer As Prism.Project.Track.Layer In track.Layers
                    If layer.Visible Then
                        Dim cursorTime As TimeSpan = layer.CursorTime
                        elements.AddRange(From el In layer.Elements
                                          Where (el.LoopEnabled AndAlso cursorTime >= el.InitialKeyFrame.Time) OrElse (cursorTime >= el.InitialKeyFrame.Time AndAlso cursorTime <= el.EndTime)
                                          Select el)
                    End If
                Next

                If reverse Then elements.Reverse()
            End If
            Return elements
        End Get
    End Property

    Public Sub Repaint()
        Repaint(False)
    End Sub

    Public Sub Repaint(ByVal resetCache As Boolean)
        If resetCache AndAlso mSelectedElements IsNot Nothing AndAlso mProject IsNot Nothing Then
            mSelectedElements.ForEach(Sub(element As Element) element.ClearBlendedKeyFramesCache(element.Layer.CursorTime))
        End If

        Me.Invalidate()
    End Sub

    Private Sub RenderTrackScene_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        If mIsMainRenderer Then Exit Sub

        isDragging = (e.Button = Windows.Forms.MouseButtons.Left)
        trueMouseOrigin = e.Location
        mouseOrigin = TransformPoint(e.Location, False)
        selectedRotation = 0
        selectionRect = Rectangle.Empty

        If overGrabHandle = GrabHandleConstants.None Then
            Dim mouseRect As Rectangle
            For Each eb In elementsBounds
                mouseRect = New Rectangle(TransformPoint(e.Location, True, eb.Key), mouseSize)
                If eb.Value.IntersectsWith(mouseRect) Then
                    If Not isCtrlDown Then mSelectedElements.Clear()
                    mSelectedElements.Add(eb.Key)
                    RaiseEvent ElementsSelected(mSelectedElements)
                    Exit Sub
                End If
            Next

            mSelectedElements.Clear()
            RaiseEvent ElementsSelected(mSelectedElements)
        End If
    End Sub

    Private Sub RenderTrackScene_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If mIsMainRenderer Then Exit Sub

        Dim doInvalidate As Boolean = False
        Dim mousePos As Point = TransformPoint(e.Location, False)

        If isDragging AndAlso overGrabHandle = GrabHandleConstants.None Then
            Dim p1 As Point = New Point(Math.Min(trueMouseOrigin.X, e.Location.X), Math.Min(trueMouseOrigin.Y, e.Location.Y))
            Dim p2 As Point = New Point(Math.Max(trueMouseOrigin.X, e.Location.X), Math.Max(trueMouseOrigin.Y, e.Location.Y))

            selectionRect = New Rectangle(p1, New Size(p2.X - p1.X, p2.Y - p1.Y))

            doInvalidate = True
        End If

        If isDragging AndAlso mouseOrigin <> mousePos AndAlso overGrabHandle <> GrabHandleConstants.None Then
            mSelectedElements.ForEach(Sub(element As Element) element.ClearBlendedKeyFramesCache())

            Dim keyFrame As KeyFrame = Nothing

            Dim createNewKeyFrame As Boolean
            For Each element As Element In mSelectedElements
                If mAutoCreateKeyFrames Then
                    createNewKeyFrame = True
                    For Each keyFrame In element.KeyFrames
                        If keyFrame.Time = element.Layer.CursorTime Then
                            createNewKeyFrame = False
                            Exit For
                        End If
                    Next
                    If createNewKeyFrame Then
                        keyFrame = element.GetBlendedKeyFrame(element.Layer.CursorTime)
                        keyFrame.Time = element.Layer.CursorTime
                        element.KeyFrames.Add(keyFrame)
                    End If
                Else
                    keyFrame = element.GetClosestKeyFrame()
                End If

                Select Case overGrabHandle
                    Case GrabHandleConstants.TopLeft
                        keyFrame.Bounds = New Rectangle(keyFrame.Bounds.X + (mousePos.X - mouseOrigin.X),
                                                 keyFrame.Bounds.Y + (mousePos.Y - mouseOrigin.Y),
                                                 keyFrame.Bounds.Width - (mousePos.X - mouseOrigin.X),
                                                 keyFrame.Bounds.Height - (mousePos.Y - mouseOrigin.Y))
                    Case GrabHandleConstants.TopCenter
                        keyFrame.Bounds = New Rectangle(keyFrame.Bounds.X,
                                                 keyFrame.Bounds.Y + (mousePos.Y - mouseOrigin.Y),
                                                 keyFrame.Bounds.Width,
                                                 keyFrame.Bounds.Height - (mousePos.Y - mouseOrigin.Y))
                    Case GrabHandleConstants.TopRight
                        keyFrame.Bounds = New Rectangle(keyFrame.Bounds.X,
                                                 keyFrame.Bounds.Y + (mousePos.Y - mouseOrigin.Y),
                                                 keyFrame.Bounds.Width + (mousePos.X - mouseOrigin.X),
                                                 keyFrame.Bounds.Height - (mousePos.Y - mouseOrigin.Y))
                    Case GrabHandleConstants.CenterLeft
                        keyFrame.Bounds = New Rectangle(keyFrame.Bounds.X + (mousePos.X - mouseOrigin.X),
                                                 keyFrame.Bounds.Y,
                                                 keyFrame.Bounds.Width - (mousePos.X - mouseOrigin.X),
                                                 keyFrame.Bounds.Height)
                    Case GrabHandleConstants.CenterRight
                        keyFrame.Bounds = New Rectangle(keyFrame.Bounds.X,
                                                 keyFrame.Bounds.Y,
                                                 keyFrame.Bounds.Width + (mousePos.X - mouseOrigin.X),
                                                 keyFrame.Bounds.Height)
                    Case GrabHandleConstants.BottomLeft
                        keyFrame.Bounds = New Rectangle(keyFrame.Bounds.X + (mousePos.X - mouseOrigin.X),
                                                 keyFrame.Bounds.Y,
                                                 keyFrame.Bounds.Width - (mousePos.X - mouseOrigin.X),
                                                 keyFrame.Bounds.Height + (mousePos.Y - mouseOrigin.Y))
                    Case GrabHandleConstants.BottomCenter
                        keyFrame.Bounds = New Rectangle(keyFrame.Bounds.X,
                                                 keyFrame.Bounds.Y,
                                                 keyFrame.Bounds.Width,
                                                 keyFrame.Bounds.Height + (mousePos.Y - mouseOrigin.Y))
                    Case GrabHandleConstants.BottomRight
                        keyFrame.Bounds = New Rectangle(keyFrame.Bounds.X,
                                                 keyFrame.Bounds.Y,
                                                 keyFrame.Bounds.Width + (mousePos.X - mouseOrigin.X),
                                                 keyFrame.Bounds.Height + (mousePos.Y - mouseOrigin.Y))
                    Case GrabHandleConstants.Body
                        keyFrame.Bounds = New Rectangle(keyFrame.Bounds.X + (mousePos.X - mouseOrigin.X),
                                                 keyFrame.Bounds.Y + (mousePos.Y - mouseOrigin.Y),
                                                 keyFrame.Bounds.Width,
                                                 keyFrame.Bounds.Height)
                    Case GrabHandleConstants.Rotation
                        keyFrame.Rotation = 360 - (Atan2(mouseOrigin.Y - mousePos.Y, mouseOrigin.X - mousePos.X) Mod 360) - 90
                        'selectedRotation += Math.Atan2(mouseOrigin.Y - mousePos.Y, mouseOrigin.X - mousePos.X)
                End Select
            Next

            If overGrabHandle = GrabHandleConstants.Rotation Then mousePos = mouseOrigin

            doInvalidate = True
            mouseOrigin += (mousePos - mouseOrigin)

            RaiseEvent SceneChangedByUser()
        ElseIf Not isDragging Then
            Select Case mSelectedElements.Count
                Case 0
                Case 1
                    For Each element As Element In mSelectedElements
                        DetectMouseAction(TransformPoint(e.Location, True, element))
                        If overGrabHandle <> GrabHandleConstants.None Then Exit For
                    Next
                Case Else
                    DetectMouseAction(TransformPoint(e.Location, True))
            End Select
        End If

        If doInvalidate Then Me.Invalidate()
    End Sub

    Private Sub RenderTrackScene_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        If mIsMainRenderer Then Exit Sub

        If isDragging Then
            isDragging = False

            If selectionRect.Width <> 0 Then
                mSelectedElements.Clear()

                For Each eb In elementsBounds
                    Dim p1 As Point = TransformPoint(selectionRect.Location, True, eb.Key)
                    Dim p2 As Point = TransformPoint(New Point(selectionRect.Right, selectionRect.Bottom), True, eb.Key)
                    p2.X -= p1.X
                    p2.Y -= p1.Y
                    Dim testRect As Rectangle = New Rectangle(p1, p2)
                    If testRect.IntersectsWith(eb.Value) Then mSelectedElements.Add(eb.Key)
                Next

                selectionRect = Rectangle.Empty
                Me.Invalidate()

                RaiseEvent ElementsSelected(mSelectedElements)
            End If
        Else
            RaiseEvent ElementsSelected(mSelectedElements)
        End If
    End Sub

    Private Function TransformPoint(p As Point, doRotation As Boolean, Optional element As Element = Nothing) As Point
        Dim pts() As Point = New Point() {p}
        Dim bounds As Rectangle
        Dim angle As Single
        Dim useKeyFrame As Boolean = True

        If element Is Nothing AndAlso mSelectedElements.Count > 0 Then
            useKeyFrame = False
            element = mSelectedElements(0)
        End If

        If element IsNot Nothing Then
            If useKeyFrame Then
                Dim kf As KeyFrame = element.GetBlendedKeyFrame()
                bounds = kf.Bounds
                angle = kf.Rotation
            Else
                bounds = selectedBounds
                angle = selectedRotation
            End If

            Dim r As Rectangle = Me.DisplayRectangle
            r.Width -= 1
            r.Height -= 1

            Using m As Drawing2D.Matrix = New Drawing2D.Matrix()
                m.Scale(r.Width / mProject.Resolution.Width, r.Height / mProject.Resolution.Height)
                If doRotation AndAlso angle <> 0 Then m.RotateAt(angle, New PointF(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2))
                m.Invert()
                m.TransformPoints(pts)
            End Using
        End If

        Return pts(0)
    End Function

    Protected Overrides Sub OnPaintBackground(ByVal e As PaintEventArgs)
        'MyBase.OnPaintBackground(e)
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        DrawFrame(e.Graphics)
    End Sub

    Private Sub DrawFrame(g As Graphics)
        If mProject Is Nothing OrElse Not EnableRendering Then Exit Sub

        Dim bk As KeyFrame = Nothing
        Dim r As Rectangle = Me.DisplayRectangle
        r.Width -= 1
        r.Height -= 1

        If mIsMainRenderer Then
            g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
            g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
            g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
            g.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
            g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
        Else
            elementsBounds.Clear()
            grabHandles.Clear()
        End If

        Dim rotationAngle As Single = 0
        Dim hs As Single = Me.DisplayRectangle.Width / mProject.Resolution.Width
        Dim vs As Single = Me.DisplayRectangle.Height / mProject.Resolution.Height

        selectedBounds = Rectangle.Empty

        For Each trk In mProject.Tracks
            If mIsMainRenderer Then
                If Not mProject.IsIdle AndAlso trk.State = Prism.Project.Track.StateConstants.Idle Then Continue For
            End If

            Using m As Drawing2D.Matrix = g.Transform
                For Each element As Element In GetActiveElements(True, trk)
                    If Not mIsMainRenderer Then m.Scale(hs, vs)

                    bk = element.Render(g, r, m)

                    If Not mIsMainRenderer Then
                        elementsBounds.Add(element, bk.Bounds)

                        If element.Type <> IElement.TypeConstants.Audio AndAlso mSelectedElements.Contains(element) Then
                            If selectedBounds = Rectangle.Empty Then
                                selectedBounds = bk.Bounds
                                rotationAngle = bk.Rotation
                            Else
                                selectedBounds = Rectangle.Union(selectedBounds, bk.Bounds)
                                rotationAngle = selectedRotation
                            End If
                        End If
                    End If

                    m.Reset()
                    g.Transform = m
                Next
            End Using
        Next

        If Not mIsMainRenderer Then
            If mSelectedElements.Count > 0 Then
                Using m As Drawing2D.Matrix = New Drawing2D.Matrix()
                    m.Scale(hs, vs)
                    If rotationAngle <> 0 Then m.RotateAt(rotationAngle, New PointF(selectedBounds.X + selectedBounds.Width \ 2, selectedBounds.Y + selectedBounds.Height \ 2))
                    g.Transform = m

                    AddGrabHandle(g, selectedBounds, m, hs, vs)

                    m.Reset()
                    g.Transform = m
                End Using
            ElseIf mSelectedElements.Count = 0 Then
                g.FillRectangle(New SolidBrush(Color.FromArgb(128, Color.AliceBlue)), selectionRect)
                g.DrawRectangle(New Pen(Brushes.AliceBlue, 2), selectionRect)
            End If

            DrawSafeAreas(g, r)
        End If
    End Sub

    Private Sub AddGrabHandle(ByVal g As Graphics, ByVal r As Rectangle, ByVal m As Drawing2D.Matrix, ByVal hs As Single, ByVal vs As Single)
        Dim w As Integer = 8 * 1 / hs
        Dim h As Integer = 8 * 1 / vs
        DrawGrabHandle(g, New Rectangle(r.Left - w, r.Top - h, w, h), GrabHandleConstants.TopLeft)
        DrawGrabHandle(g, New Rectangle(r.Left + (r.Width - w) / 2, r.Top - 8, w, h), GrabHandleConstants.TopCenter)
        DrawGrabHandle(g, New Rectangle(r.Right, r.Top - h, w, h), GrabHandleConstants.TopRight)

        DrawGrabHandle(g, New Rectangle(r.Left - w, r.Top + (r.Height - h) / 2, w, h), GrabHandleConstants.CenterLeft)
        DrawGrabHandle(g, New Rectangle(r.Right, r.Top + (r.Height - h) / 2, w, h), GrabHandleConstants.CenterRight)

        DrawGrabHandle(g, New Rectangle(r.Left - w, r.Bottom, w, h), GrabHandleConstants.BottomLeft)
        DrawGrabHandle(g, New Rectangle(r.Left + (r.Width - w) / 2, r.Bottom, w, h), GrabHandleConstants.BottomCenter)
        DrawGrabHandle(g, New Rectangle(r.Right, r.Bottom, w, h), GrabHandleConstants.BottomRight)

        DrawGrabHandle(g, r, GrabHandleConstants.Rotation)

        DrawGrabHandle(g, r, GrabHandleConstants.Body)
    End Sub

    Private Sub DrawSafeAreas(ByVal g As Graphics, ByVal r As Rectangle)
        If SafeAreasVisible = False Then Exit Sub

        g.SmoothingMode = Drawing2D.SmoothingMode.Default

        Dim ar As Rectangle
        Dim dash() As Single = {4, 2}

        ' Safe Action Area (90%)
        ar = r
        ar.X += ar.Width * 0.05
        ar.Width -= ar.Width * 0.1
        ar.Y += ar.Height * 0.05
        ar.Height -= ar.Height * 0.1
        Using p As Pen = New Pen(Brushes.Green)
            p.DashPattern = dash
            g.DrawRectangle(p, ar)
        End Using

        ' Safe Title Area (80%)
        ar = r
        ar.X += ar.Width * 0.1
        ar.Width -= ar.Width * 0.2
        ar.Y += ar.Height * 0.1
        ar.Height -= ar.Height * 0.2
        Using p As Pen = New Pen(Brushes.Yellow)
            p.DashPattern = dash
            g.DrawRectangle(p, ar)
        End Using
    End Sub

    Private Sub DrawGrabHandle(ByVal g As Graphics, ByVal r As Rectangle, ByVal type As GrabHandleConstants)
        Dim enabled As Boolean = (mSelectedElements.Count = 1)

        Select Case type
            Case GrabHandleConstants.Rotation
                Dim s As Integer = Math.Max(Math.Min(Math.Min(r.Width, r.Height), 80), 40)

                Using p As Pen = New Pen(Color.FromArgb(128, Color.White), 4)
                    g.DrawEllipse(p, r.X + (r.Width - s) \ 2, r.Y + (r.Height - s) \ 2, s, s)
                End Using

                s += 4
                r = New Rectangle(r.X + (r.Width - s) \ 2, r.Y + (r.Height - s) \ 2, s, s)
                g.DrawEllipse(Pens.Black, r)

                s -= 8
                g.DrawEllipse(Pens.Black, r.X + (r.Width - s) \ 2, r.Y + (r.Height - s) \ 2, s, s)

                enabled = True
            Case GrabHandleConstants.Body
                enabled = True
                Using p As Pen = New Pen(Color.FromArgb(128, Color.WhiteSmoke))
                    p.DashPattern = {8, 4}
                    g.DrawRectangle(p, r)
                End Using
            Case Else
                If enabled Then
                    ControlPaint.DrawGrabHandle(g, r, True, True)
                Else
                    ControlPaint.DrawContainerGrabHandle(g, r)
                End If
        End Select

        grabHandles.Add(type, New GrabHandle(r, enabled))
    End Sub

    Private Sub DetectMouseAction(ByVal p As Point)
        Dim mouseRect As Rectangle = New Rectangle(p, mouseSize)
        overGrabHandle = GrabHandleConstants.None

        For Each gh In grabHandles
            If gh.Value.Enabled AndAlso gh.Value.Bounds.IntersectsWith(mouseRect) Then
                overGrabHandle = gh.Key
                Exit For
            End If
        Next

        Select Case overGrabHandle
            Case GrabHandleConstants.None
                Me.Cursor = Cursors.Arrow
            Case GrabHandleConstants.TopLeft, GrabHandleConstants.BottomRight
                Me.Cursor = Cursors.SizeNWSE
            Case GrabHandleConstants.TopCenter, GrabHandleConstants.BottomCenter
                Me.Cursor = Cursors.SizeNS
            Case GrabHandleConstants.TopRight, GrabHandleConstants.BottomLeft
                Me.Cursor = Cursors.SizeNESW
            Case GrabHandleConstants.CenterLeft, GrabHandleConstants.CenterRight
                Me.Cursor = Cursors.SizeWE
            Case GrabHandleConstants.Rotation
                Me.Cursor = Cursors.NoMove2D
            Case GrabHandleConstants.Body
                Me.Cursor = Cursors.SizeAll
        End Select
    End Sub

#Region "Black Magic"
    '    Private Sub VideoGrabber1_OnFrameBitmap(sender As Object, e As VidGrab.TOnFrameBitmapEventArgs) Handles VideoGrabber1.OnFrameBitmap
    '        Dim FrameInfo As VidGrab.TFrameInfo = Runtime.InteropServices.Marshal.PtrToStructure(CType(e.frameInfo, IntPtr), GetType(VidGrab.TFrameInfo))
    '        Dim FrameBitmap As VidGrab.TFrameBitmapInfo = Runtime.InteropServices.Marshal.PtrToStructure(CType(e.bitmapInfo, IntPtr), GetType(VidGrab.TFrameBitmapInfo))
    '    End Sub

    '    Private Sub DrawFrame(sender As Object, e As VidGrab.TOnFrameOverlayUsingDCEventArgs) Handles VideoGrabber1.OnFrameOverlayUsingDC
    '        Dim g As Graphics = Graphics.FromHdcInternal(New IntPtr(e.dc))
    '        Dim r As Rectangle = Me.DisplayRectangle
    '        r.Width -= 1
    '        r.Height -= 1

    '        If mEditingTrack IsNot Nothing Then
    '            Dim bk As KeyFrame = Nothing

    '            'g.Clear(Me.BackColor)

    '            elementsBounds.Clear()
    '            grabHandles.Clear()

    '            If mIsMainRenderer Then
    '                g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
    '                g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
    '                g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
    '            End If

    '            selectedBounds = Rectangle.Empty
    '            Dim rotationAngle As Single = 0
    '            Dim hs As Single = Me.DisplayRectangle.Width / mEditingTrack.Project.Resolution.Width
    '            Dim vs As Single = Me.DisplayRectangle.Height / mEditingTrack.Project.Resolution.Height

    '            Using m As Drawing2D.Matrix = g.Transform
    '                For Each element As Element In ActiveElements(True, mEditingTrack)
    '                    If Not mIsMainRenderer Then m.Scale(hs, vs)
    '                    bk = element.Render(g, r, m)
    '                    If Not mIsMainRenderer Then

    '                        elementsBounds.Add(element, bk.Bounds)

    '                        If element.Type <> IElement.TypeConstants.Audio AndAlso mSelectedElements.Contains(element) Then
    '                            If selectedBounds = Rectangle.Empty Then
    '                                selectedBounds = bk.Bounds
    '                                rotationAngle = bk.Rotation
    '                            Else
    '                                selectedBounds = Rectangle.Union(selectedBounds, bk.Bounds)
    '                                rotationAngle = selectedRotation
    '                            End If
    '                        End If
    '                    End If

    '                    m.Reset()
    '                    g.Transform = m
    '                Next
    '            End Using

    '            If Not mIsMainRenderer AndAlso mSelectedElements.Count > 0 Then
    '                Using m As Drawing2D.Matrix = New Drawing2D.Matrix()
    '                    m.Scale(hs, vs)
    '                    If rotationAngle <> 0 Then m.RotateAt(rotationAngle, New PointF(selectedBounds.X + selectedBounds.Width \ 2, selectedBounds.Y + selectedBounds.Height \ 2))
    '                    g.Transform = m

    '                    AddGrabHandle(g, selectedBounds, m, hs, vs)

    '                    m.Reset()
    '                    g.Transform = m
    '                End Using
    '            End If

    '            If mSelectedElements.Count = 0 Then
    '                g.FillRectangle(New SolidBrush(Color.FromArgb(128, Color.AliceBlue)), selectionRect)
    '                g.DrawRectangle(New Pen(Brushes.AliceBlue, 2), selectionRect)
    '            End If
    '        End If

    '        DrawSafeAreas(g, r)

    '        g.Dispose()
    '    End Sub
#End Region

    Private Const toRad As Double = Math.PI / 180
    Private Const toDeg As Double = 180 / Math.PI
    Private Shared Function Atan2(ByVal dx As Double, ByVal dy As Double) As Double
        Dim a As Double

        If dy = 0 Then
            If dx > 0 Then
                a = 0
            Else
                a = 180
            End If
        Else
            a = Math.Atan(dy / dx) * toDeg
            Select Case a
                Case Is > 0
                    If dx < 0 AndAlso dy < 0 Then a += 180
                Case 0
                    If dx < 0 Then a = 180
                Case Is < 0
                    If dy > 0 Then
                        If dx > 0 Then
                            a = Math.Abs(a)
                        Else
                            a += 180
                        End If
                    Else
                        a += 360
                    End If
            End Select
        End If

        Return a
    End Function
End Class
