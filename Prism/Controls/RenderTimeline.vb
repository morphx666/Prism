Imports Prism.Project.Track.Layer
Imports System.ComponentModel
Imports Prism.Project.Track

Public Class RenderTimeline
    ' 10 pixels = Resolution
    Private Enum ResolutionConstants
        Seconds
        Minutes
        Hours
    End Enum

    Private Enum OverObjectConstants
        None
        Split
        Element
        Layer
        TimeRuler
    End Enum

    Private Enum OverElementSectionConstants
        None
        LeftSide
        Body
        RightSide
        KeyFrame
        KeyFramesArea
        FadeIn
        FadeOut
        LoopFrom
        LoopTo
    End Enum

    Public Class TimeLineStyle
        Private mScaleFont As Font = New Font("Consolas", 10, FontStyle.Regular, GraphicsUnit.Pixel)
        Private mTimeFont As Font = New Font("Consolas", 12, FontStyle.Bold, GraphicsUnit.Pixel)
        Private mScaleFontSize As Size
        Private mTimeFontSize As Size

        <Category("Appearance")> Public Property CellHeight As Integer = 24
        <Category("Appearance")> Public Property Margin As Integer = 8

        <Category("Color"), TypeConverter(GetType(ColorConverter)), Editor(GetType(ColorEditor), GetType(Drawing.Design.UITypeEditor))> Public Property GridColor As Pen = Pens.Gray
        <Category("Color"), TypeConverter(GetType(ColorConverter)), Editor(GetType(ColorEditor), GetType(Drawing.Design.UITypeEditor))> Public Property TickLinesColor As Pen = Pens.DarkGray
        <Category("Color"), TypeConverter(GetType(ColorConverter)), Editor(GetType(ColorEditor), GetType(Drawing.Design.UITypeEditor))> Public Property TickLinesSmallColor As Pen = Pens.LightGray
        <Category("Color"), TypeConverter(GetType(ColorConverter)), Editor(GetType(ColorEditor), GetType(Drawing.Design.UITypeEditor))> Public Property TickTextColor As Brush = Brushes.DimGray
        <Category("Color"), TypeConverter(GetType(ColorConverter)), Editor(GetType(ColorEditor), GetType(Drawing.Design.UITypeEditor))> Public Property TimeColor As Brush = Brushes.Black
        <Category("Color"), TypeConverter(GetType(ColorConverter)), Editor(GetType(ColorEditor), GetType(Drawing.Design.UITypeEditor))> Public Property TimelineBackColor As Brush = New SolidBrush(Color.FromKnownColor(KnownColor.Control))
        <Category("Color"), TypeConverter(GetType(ColorConverter)), Editor(GetType(ColorEditor), GetType(Drawing.Design.UITypeEditor))> Public Property LayerSelectedBackColor As Brush = Brushes.Gainsboro
        <Category("Color"), TypeConverter(GetType(ColorConverter)), Editor(GetType(ColorEditor), GetType(Drawing.Design.UITypeEditor))> Public Property LayerNotVisible As Brush = Brushes.Gray
        <Category("Color"), TypeConverter(GetType(ColorConverter)), Editor(GetType(ColorEditor), GetType(Drawing.Design.UITypeEditor))> Public Property LayerTextColor As Brush = Brushes.Black
        <Category("Color"), TypeConverter(GetType(ColorConverter)), Editor(GetType(ColorEditor), GetType(Drawing.Design.UITypeEditor))> Public Property LayerSelectedTextColor As Brush = Brushes.Black
        <Category("Color"), TypeConverter(GetType(ColorConverter)), Editor(GetType(ColorEditor), GetType(Drawing.Design.UITypeEditor))> Public Property LayerDisabledSection As Color = Color.FromArgb(32, Color.Brown)
        <Category("Color"), TypeConverter(GetType(ColorConverter)), Editor(GetType(ColorEditor), GetType(Drawing.Design.UITypeEditor))> Public Property ElementBackColor As Brush = Brushes.Azure
        <Category("Color"), TypeConverter(GetType(ColorConverter)), Editor(GetType(ColorEditor), GetType(Drawing.Design.UITypeEditor))> Public Property ElementTextColor As Brush = Brushes.Gray
        <Category("Color"), TypeConverter(GetType(ColorConverter)), Editor(GetType(ColorEditor), GetType(Drawing.Design.UITypeEditor))> Public Property ElementSelectedBackColor As Brush = Brushes.Lavender
        <Category("Color"), TypeConverter(GetType(ColorConverter)), Editor(GetType(ColorEditor), GetType(Drawing.Design.UITypeEditor))> Public Property ElementSelectedTextColor As Brush = Brushes.Black
        <Category("Color"), TypeConverter(GetType(ColorConverter)), Editor(GetType(ColorEditor), GetType(Drawing.Design.UITypeEditor))> Public Property ElementBorderColor As Pen = Pens.PowderBlue
        <Category("Color"), TypeConverter(GetType(ColorConverter)), Editor(GetType(ColorEditor), GetType(Drawing.Design.UITypeEditor))> Public Property ElementKeyFramesAreaColor As Brush = Brushes.LightYellow
        <Category("Color"), TypeConverter(GetType(ColorConverter)), Editor(GetType(ColorEditor), GetType(Drawing.Design.UITypeEditor))> Public Property ElementKeyFramesColor As Pen = Pens.Red
        <Category("Color"), TypeConverter(GetType(ColorConverter)), Editor(GetType(ColorEditor), GetType(Drawing.Design.UITypeEditor))> Public Property ElementFadeColor As Brush = Brushes.LightGray
        <Category("Color"), TypeConverter(GetType(ColorConverter)), Editor(GetType(ColorEditor), GetType(Drawing.Design.UITypeEditor))> Public Property ElementLoopColor As Brush = New SolidBrush(Color.FromArgb(128, Color.DarkGray))
        <Category("Color"), TypeConverter(GetType(ColorConverter)), Editor(GetType(ColorEditor), GetType(Drawing.Design.UITypeEditor))> Public Property ElementFadeAndLoopBorderColor As Pen = Pens.Gray
        <Category("Color"), TypeConverter(GetType(ColorConverter)), Editor(GetType(ColorEditor), GetType(Drawing.Design.UITypeEditor))> Public Property ElementWaveFormColor As Pen = New Pen(Color.FromArgb(128, Color.Blue))
        <Category("Color"), TypeConverter(GetType(ColorConverter)), Editor(GetType(ColorEditor), GetType(Drawing.Design.UITypeEditor))> Public Property SelectionRectangleBackgroundColor As SolidBrush = New SolidBrush(Color.FromArgb(128, Color.AliceBlue))
        <Category("Color"), TypeConverter(GetType(ColorConverter)), Editor(GetType(ColorEditor), GetType(Drawing.Design.UITypeEditor))> Public Property SelectionRectangleBorderColor As Pen = New Pen(Brushes.AliceBlue, 2)

        Public Sub New()
            UpdateFontSizes()
        End Sub

        <Browsable(False)>
        Public ReadOnly Property ScaleFontSize As Size
            Get
                Return mScaleFontSize
            End Get
        End Property

        <Browsable(False)>
        Public ReadOnly Property TimeFontSize As Size
            Get
                Return mTimeFontSize
            End Get
        End Property

        <Category("Font")>
        Public Property ScaleFont As Font
            Get
                Return mScaleFont
            End Get
            Set(ByVal value As Font)
                mScaleFont = value
                UpdateFontSizes()
            End Set
        End Property

        <Category("Font")>
        Public Property TimeFont As Font
            Get
                Return mTimeFont
            End Get
            Set(ByVal value As Font)
                mTimeFont = value
                UpdateFontSizes()
            End Set
        End Property

        Private Sub UpdateFontSizes()
            mScaleFontSize = Windows.Forms.TextRenderer.MeasureText("00:00:00", mScaleFont)
            mTimeFontSize = Windows.Forms.TextRenderer.MeasureText("00:00:00:00", mTimeFont)
            If CellHeight < mTimeFontSize.Height + Margin * 2 + 12 Then
                CellHeight = mTimeFontSize.Height + Margin * 2 + 12
            End If
        End Sub
    End Class

    Private tlStyle As TimeLineStyle = New TimeLineStyle()

    Private mTrack As Project.Track
    Private mSplit As Integer = 120
    Private mResolution As ResolutionConstants = ResolutionConstants.Seconds
    Private mZoom As Single = 1

    Private mSelectedKeyFrame As Element.KeyFrame = Nothing
    Private mCursorTime As TimeSpan = TimeSpan.Zero
    Private mPlaybackCursorTime As TimeSpan = TimeSpan.Zero
    Private mSelectedElements As List(Of Element) = New List(Of Element)
    Private mOverElement As Element = Nothing

    Private stringFormat As Drawing.StringFormat = New Drawing.StringFormat(Drawing.StringFormat.GenericDefault)

    Private Const maxVolVal As Integer = Short.MaxValue

    Private overObject As OverObjectConstants = OverObjectConstants.None
    Private elementsBounds As Dictionary(Of Element, Rectangle) = New Dictionary(Of Element, Rectangle)
    Private layersBounds As Dictionary(Of Layer, Rectangle) = New Dictionary(Of Layer, Rectangle)
    Private overElementSection As OverElementSectionConstants = OverElementSectionConstants.None
    Private overLayer As Layer
    Private isDragging As Boolean = False
    Private isMouseDown As Boolean = False
    Private mouseOrigin As Point = New Point(0, 0)
    Private isShiftDown As Boolean = False
    Private isCtrlDown As Boolean = False
    Private clipboard As Element.KeyFrame
    Private selectionRect As Rectangle = Rectangle.Empty

    Private addKeyFrameCursor As Cursor
    Private deleteKeyFrameCursor As Cursor

    Public Event ObjectsSelected(ByVal elements As List(Of Element), ByVal keyFrame As Element.KeyFrame, ByVal layer As Layer)
    Public Event CursorPositionChanged(ByVal time As TimeSpan)
    Public Event KeyFramesChanged(sender As Object, e As PropertyValueChangedEventArgs)

    Public Property Track As Project.Track
        Get
            Return mTrack
        End Get
        Set(ByVal value As Project.Track)
            mTrack = value

            If mTrack IsNot Nothing Then
                AddHandler mTrack.LayersChanged, Sub() UpdateUI()
            End If

            UpdateUI()
        End Set
    End Property

    Public Sub Repaint()
        Me.Invalidate()
    End Sub

    Private Sub UpdateUI()
        Me.Enabled = mTrack IsNot Nothing
        VScrollBarMain.Value = 0
        HScrollBarMain.Value = 0
        UpdateLayersCursorTime(TimeSpanToX(mCursorTime))
        Repaint()
    End Sub

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
            Repaint()
        End Set
    End Property

    Public Property CursorTime As TimeSpan
        Get
            Return mCursorTime
        End Get
        Set(ByVal value As TimeSpan)
            If value.TotalMilliseconds >= 0 Then
                mCursorTime = If(mTrack Is Nothing,
                                    value,
                                    Project.Track.PadTime(value, mTrack.TimeInc))
            End If

            Repaint()
        End Set
    End Property

    Public Property PlaybackCursorTime As TimeSpan
        Get
            Return mPlaybackCursorTime
        End Get
        Set(ByVal value As TimeSpan)
            mPlaybackCursorTime = If(mTrack Is Nothing,
                                        value,
                                        Project.Track.PadTime(value, mTrack.TimeInc))

            Repaint()
        End Set
    End Property

    Private Sub RenderTimeline_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        isShiftDown = e.Shift
        isCtrlDown = e.Control
        Repaint()
    End Sub

    Private Sub RenderTimeline_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        isShiftDown = e.Shift
        isCtrlDown = e.Control
        Repaint()
    End Sub

    Private Sub RenderTimeline_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.SetStyle(ControlStyles.UserPaint, True)
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        Me.SetStyle(ControlStyles.Selectable, True)

        stringFormat.FormatFlags = StringFormatFlags.LineLimit Or StringFormatFlags.NoWrap
        stringFormat.LineAlignment = StringAlignment.Near
        stringFormat.Alignment = StringAlignment.Near

        AddHandler KeyFramesCollectionEditor.MyPropertyValueChanged, AddressOf HandleKeyFramesChanged
        AddHandler KeyFramesCollectionEditor.MyPropertiesCollectionChanged, AddressOf HandleKeyFramesChanged

        AddHandler ButtonHZoomIn.GotFocus, AddressOf ResetFocus
        AddHandler ButtonHZoomOut.GotFocus, AddressOf ResetFocus
        AddHandler ButtonVZoomIn.GotFocus, AddressOf ResetFocus
        AddHandler ButtonVZoomOut.GotFocus, AddressOf ResetFocus
        AddHandler HScrollBarMain.GotFocus, AddressOf ResetFocus
        AddHandler VScrollBarMain.GotFocus, AddressOf ResetFocus

        addKeyFrameCursor = CustomCursors.CreateCursor(CustomCursors.Type.HandPlus)
        deleteKeyFrameCursor = CustomCursors.CreateCursor(CustomCursors.Type.HandMinus)
    End Sub

    Private Sub ResetFocus()
        Me.Focus()
    End Sub

    Private Sub RenderTimeline_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        Select Case e.Button
            Case Windows.Forms.MouseButtons.Left, Windows.Forms.MouseButtons.Right
                isMouseDown = True
                isDragging = False
                mouseOrigin = e.Location
                mOverElement = Nothing
                mSelectedKeyFrame = Nothing
                overLayer = Nothing
                mSelectedElements.Clear()

                DetectMouseAction(e.Location)

                If overObject = OverObjectConstants.TimeRuler Then
                Else
                    If overObject = OverObjectConstants.Element Then
                        If isShiftDown AndAlso mSelectedElements.Count > 0 AndAlso overElementSection = OverElementSectionConstants.KeyFramesArea Then
                            Dim cursorTime As TimeSpan = XToTimeSpan(e.X - mSplit)

                            mSelectedElements.Clear()
                            mSelectedElements.Add(mOverElement)

                            If mTrack.State <> Project.Track.StateConstants.Playing Then mOverElement.Layer.CursorTime = cursorTime

                            Dim p As Element.KeyFrame = mOverElement.GetBlendedKeyFrame(cursorTime)
                            p.Time = cursorTime
                            mOverElement.KeyFrames.Add(p)
                            mSelectedKeyFrame = p
                        Else
                            If mSelectedKeyFrame IsNot Nothing Then mSelectedElements.Clear()
                            If Not isCtrlDown Then
                                If Not mSelectedElements.Contains(mOverElement) Then mSelectedElements.Clear()
                            End If
                            If Not mSelectedElements.Contains(mOverElement) Then mSelectedElements.Add(mOverElement)
                        End If

                        Repaint()
                    ElseIf overObject <> OverObjectConstants.TimeRuler Then
                        mSelectedElements.Clear()
                    End If
                End If
        End Select
    End Sub

    Private Sub RenderTimeline_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If isMouseDown Then
            UpdateLayersCursorTime(e.X)

            isDragging = e.Location <> mouseOrigin AndAlso
                        overObject <> OverObjectConstants.None AndAlso
                        e.Button = Windows.Forms.MouseButtons.Left
        End If

        If isDragging Then
            Select Case overObject
                Case OverObjectConstants.Split
                    mSplit += (e.X - mouseOrigin.X)
                    If mSplit < tlStyle.TimeFontSize.Width + tlStyle.Margin * 2 Then mSplit = tlStyle.TimeFontSize.Width + tlStyle.Margin * 2
                    If mSplit > 300 Then mSplit = 300
                Case OverObjectConstants.Element
                    For Each element As Element In mSelectedElements
                        element.ClearBlendedKeyFramesCache()
                        Select Case overElementSection
                            Case OverElementSectionConstants.LeftSide
                                Dim t As TimeSpan = XToTimeSpan(e.X - mSplit)
                                Dim d As TimeSpan = element.InitialKeyFrame.Time - t

                                element.UpdateKeyFramesTimes(element.InitialKeyFrame, element.InitialKeyFrame.Time - t)

                                element.InitialKeyFrame.Time = t
                                element.Duration += d
                            Case OverElementSectionConstants.RightSide
                                element.Duration += XToTimeSpan(e.X - mouseOrigin.X)
                            Case OverElementSectionConstants.Body
                                element.InitialKeyFrame.Time += XToTimeSpan(e.X - mouseOrigin.X)

                                If mSelectedElements.Count = 1 Then
                                    If e.Y > elementsBounds(element).Bottom + 4 Then
                                        For i As Integer = 0 To mTrack.Layers.Count - 1
                                            If mTrack.Layers(i).Equals(element.Layer) Then
                                                If i < mTrack.Layers.Count - 1 Then
                                                    element.Layer = mTrack.Layers(i + 1)
                                                    Exit For
                                                End If
                                            End If
                                        Next
                                    ElseIf e.Y < elementsBounds(element).Top - 4 Then
                                        For i As Integer = 0 To mTrack.Layers.Count - 1
                                            If mTrack.Layers(i).Equals(element.Layer) Then
                                                If i > 0 Then
                                                    element.Layer = mTrack.Layers(i - 1)
                                                    Exit For
                                                End If
                                            End If
                                        Next
                                    End If
                                End If
                            Case OverElementSectionConstants.KeyFrame
                                If Not element.InitialKeyFrame.Equals(mSelectedKeyFrame) Then
                                    mSelectedKeyFrame.Time += XToTimeSpan(e.X - mouseOrigin.X)
                                End If
                            Case OverElementSectionConstants.FadeIn
                                element.FadeIn.Duration += XToTimeSpan(e.X - mouseOrigin.X)
                            Case OverElementSectionConstants.FadeOut
                                element.FadeOut.Duration += XToTimeSpan(mouseOrigin.X - e.X)
                            Case OverElementSectionConstants.LoopFrom
                                element.LoopFrom += XToTimeSpan(e.X - mouseOrigin.X)
                            Case OverElementSectionConstants.LoopTo
                                element.LoopTo += XToTimeSpan(mouseOrigin.X - e.X)
                        End Select
                    Next
            End Select
            mouseOrigin = e.Location
            Repaint()
        Else
            If isMouseDown Then
                If overObject = OverObjectConstants.None Then
                    Dim p1 As Point = New Point(Math.Min(mouseOrigin.X, e.Location.X), Math.Min(mouseOrigin.Y, e.Location.Y))
                    Dim p2 As Point = New Point(Math.Max(mouseOrigin.X, e.Location.X), Math.Max(mouseOrigin.Y, e.Location.Y))

                    selectionRect = New Rectangle(p1, New Size(p2.X - p1.X, p2.Y - p1.Y))
                    mSelectedElements.Clear()
                    For Each eb In elementsBounds
                        If selectionRect.IntersectsWith(eb.Value) Then mSelectedElements.Add(eb.Key)
                    Next
                End If

                Repaint()
            Else
                DetectMouseAction(e.Location)
            End If
        End If
    End Sub

    Private Sub RenderTimeline_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        selectionRect = Rectangle.Empty

        RaiseEvent ObjectsSelected(mSelectedElements, mSelectedKeyFrame, overLayer)

        UpdateLayersCursorTime(e.X)

        If e.Button = Windows.Forms.MouseButtons.Right Then
            Select Case overElementSection
                Case OverElementSectionConstants.Body,
                        OverElementSectionConstants.FadeIn,
                        OverElementSectionConstants.FadeOut,
                        OverElementSectionConstants.LeftSide,
                        OverElementSectionConstants.RightSide
                    ContextMenuElement.Show(Me, e.Location)
                Case OverElementSectionConstants.KeyFramesArea
                    ContextMenuKeyFramesAdd.Enabled = True
                    ContextMenuKeyFramesDelete.Enabled = False
                    ContextMenuKeyFramesCopy.Enabled = False
                    ContextMenuKeyFramesPaste.Enabled = (clipboard IsNot Nothing)
                    ContextMenuKeyFrames.Show(Me, e.Location)
                Case OverElementSectionConstants.KeyFrame
                    ContextMenuKeyFramesAdd.Enabled = False
                    ContextMenuKeyFramesDelete.Enabled = Not mOverElement.InitialKeyFrame.Equals(mSelectedKeyFrame)
                    ContextMenuKeyFramesCopy.Enabled = True
                    ContextMenuKeyFramesPaste.Enabled = (clipboard IsNot Nothing)
                    ContextMenuKeyFrames.Show(Me, e.Location)
                Case OverElementSectionConstants.None
                    If Not ContextMenuTimelineAddElement.HasDropDownItems Then
                        Dim elementsTypes As List(Of Type) = Project.GetAvailableElementsTypes()
                        For Each elementType As Type In elementsTypes
                            Dim mi As ToolStripMenuItem = New ToolStripMenuItem(elementType.Name.Replace("Element", "")) With {.Tag = elementType}
                            ContextMenuTimelineAddElement.DropDownItems.Add(mi)
                            AddHandler mi.Click, AddressOf AddNewElementFromContextMenu
                        Next
                    End If

                    For Each mi As ToolStripMenuItem In ContextMenuTimelineAddElement.DropDownItems
                        mi.Enabled = (overLayer IsNot Nothing)
                    Next

                    ContextMenuTimelineAddElement.Enabled = overLayer IsNot Nothing
                    ContextMenuTimelineDeleteLayer.Enabled = overLayer IsNot Nothing
                    ContextMenuTimelineAddLayer.Enabled = mTrack IsNot Nothing

                    ContextMenuTimeline.Show(Me, e.Location)
            End Select
        End If

        isMouseDown = False
        isDragging = False
        Repaint()
    End Sub

    Private Sub RenderTimeline_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        Dim g As Graphics = e.Graphics
        Dim r As Rectangle = Me.DisplayRectangle
        r.Width -= 1
        r.Height -= 1

        elementsBounds.Clear()

        If mTrack Is Nothing Then
            HScrollBarMain.Maximum = 0
            VScrollBarMain.Maximum = 0
            g.Clear(Color.FromKnownColor(KnownColor.ControlDark))
        End If

        DrawGrid(g, r)
        DrawLayers(g, r)
        DrawTickMarks(g, r)

        ' Draw Element's Guidelines
        If isDragging AndAlso overObject = OverObjectConstants.Element Then
            Select Case overElementSection
                Case OverElementSectionConstants.LeftSide
                    If elementsBounds(mOverElement).X > mSplit Then g.DrawLine(Pens.Red, elementsBounds(mOverElement).X, 0, elementsBounds(mOverElement).X, r.Height)
                Case OverElementSectionConstants.Body
                    If elementsBounds(mOverElement).Right > mSplit Then
                        g.DrawLine(Pens.LightPink, elementsBounds(mOverElement).Right, 0, elementsBounds(mOverElement).Right, r.Height)
                        If mouseOrigin.X > mSplit Then
                            g.DrawLine(Pens.Red, mouseOrigin.X, 0, mouseOrigin.X, r.Height)
                            If elementsBounds(mOverElement).X > mSplit Then g.DrawLine(Pens.LightPink, elementsBounds(mOverElement).X, 0, elementsBounds(mOverElement).X, r.Height)
                        End If
                    End If
                Case OverElementSectionConstants.RightSide
                    If elementsBounds(mOverElement).Right > mSplit Then g.DrawLine(Pens.Red, elementsBounds(mOverElement).Right, 0, elementsBounds(mOverElement).Right, r.Height)
            End Select
        End If


        If Not isMouseDown OrElse mOverElement Is Nothing Then
            ' Draw Playback Cursor
            Using p As Pen = New Pen(Color.FromArgb(200, Color.Blue))
                Dim x As Integer = TimeSpanToX(mPlaybackCursorTime) - HScrollBarMain.Value
                If x > mSplit Then g.DrawLine(p, x, 0, x, r.Height)
            End Using

            ' Draw Cursor
            Using p As Pen = New Pen(Color.FromArgb(200, Color.LightGray))
                Dim x As Integer = TimeSpanToX(mCursorTime) - HScrollBarMain.Value
                If x > mSplit Then g.DrawLine(p, x, 0, x, r.Height)
            End Using
        End If

        DrawTime(g, r)

        ' Draw Selection Rectangle
        If overObject = OverObjectConstants.None Then
            g.FillRectangle(tlStyle.SelectionRectangleBackgroundColor, selectionRect)
            g.DrawRectangle(tlStyle.SelectionRectangleBorderColor, selectionRect)
        End If

        ' Erase small square
        Using b As SolidBrush = New SolidBrush(Me.BackColor)
            g.FillRectangle(b, ButtonHZoomOut.Right, ButtonVZoomOut.Bottom, VScrollBarMain.Width - 1, HScrollBarMain.Height - 1)
        End Using

        ' Draw border
        g.DrawRectangle(Pens.Gray, r)
    End Sub

    Private Sub DetectMouseAction(ByVal p As Point)
        'If Not Me.Focused Then Exit Sub

        overObject = OverObjectConstants.None
        overElementSection = OverElementSectionConstants.None
        Me.Cursor = Cursors.Default

        If p.Y < tlStyle.CellHeight Then
            overObject = OverObjectConstants.TimeRuler
            Me.Cursor = Cursors.NoMoveHoriz
        End If

        If isMouseDown Then
            If mTrack IsNot Nothing Then
                Dim y As Integer = tlStyle.CellHeight
                For i As Integer = 0 To mTrack.Layers.Count - 1
                    If p.Y + VScrollBarMain.Value >= y AndAlso p.Y + VScrollBarMain.Value <= y + tlStyle.CellHeight Then
                        overLayer = mTrack.Layers(i)
                        Exit For
                    Else
                        y += tlStyle.CellHeight
                    End If
                Next
            End If
        End If

        Dim mouseRect As Rectangle = New Rectangle(p.X, p.Y, 1, 1)
        For Each elementBounds In elementsBounds
            If elementBounds.Value.IntersectsWith(mouseRect) Then
                Dim testOverElement As Element = elementBounds.Key
                Dim testOverKeyFrame As Element.KeyFrame = Nothing
                overObject = OverObjectConstants.Element

                If p.Y >= elementBounds.Value.Bottom - 3 AndAlso p.Y <= elementBounds.Value.Bottom Then
                    overElementSection = OverElementSectionConstants.KeyFramesArea
                    mSelectedKeyFrame = Nothing

                    If isShiftDown Then
                        Me.Cursor = addKeyFrameCursor
                    Else
                        For Each prop As Element.KeyFrame In testOverElement.KeyFrames
                            If Math.Abs(p.X - TimeSpanToX(prop.Time) + HScrollBarMain.Value) <= 3 Then
                                If mTrack.State <> Project.Track.StateConstants.Playing Then testOverElement.Layer.CursorTime = prop.Time
                                testOverKeyFrame = prop
                                overElementSection = OverElementSectionConstants.KeyFrame
                                Me.Cursor = Cursors.Hand
                                Exit For
                            End If
                        Next
                    End If
                Else
                    If mouseRect.X >= elementBounds.Value.X AndAlso mouseRect.X <= elementBounds.Value.X + 3 Then
                        overElementSection = OverElementSectionConstants.LeftSide
                        Me.Cursor = Cursors.SizeWE
                    ElseIf mouseRect.X >= elementBounds.Value.Right - 3 AndAlso mouseRect.X <= elementBounds.Value.Right Then
                        overElementSection = OverElementSectionConstants.RightSide
                        Me.Cursor = Cursors.SizeWE
                    Else
                        overElementSection = OverElementSectionConstants.Body

                        If testOverElement.FadeIn.Type = IElement.FadeEffect.TypeConstants.Fade Then
                            If Math.Abs(mouseRect.X - TimeSpanToX(testOverElement.InitialKeyFrame.Time + testOverElement.FadeIn.Duration)) <= 1 Then
                                overElementSection = OverElementSectionConstants.FadeIn
                                Me.Cursor = Cursors.SizeWE
                            End If
                        End If
                        If testOverElement.FadeOut.Type = IElement.FadeEffect.TypeConstants.Fade Then
                            If Math.Abs(mouseRect.X - TimeSpanToX(testOverElement.EndTime - testOverElement.FadeOut.Duration)) <= 1 Then
                                overElementSection = OverElementSectionConstants.FadeOut
                                Me.Cursor = Cursors.SizeWE
                            End If
                        End If

                        If testOverElement.LoopEnabled Then
                            If Math.Abs(mouseRect.X - TimeSpanToX(testOverElement.InitialKeyFrame.Time + testOverElement.LoopFrom)) <= 1 Then
                                overElementSection = OverElementSectionConstants.LoopFrom
                                Me.Cursor = Cursors.SizeWE
                            End If

                            If Math.Abs(mouseRect.X - TimeSpanToX(testOverElement.EndTime - testOverElement.LoopTo)) <= 1 Then
                                overElementSection = OverElementSectionConstants.LoopTo
                                Me.Cursor = Cursors.SizeWE
                            End If
                        End If

                        If overElementSection = OverElementSectionConstants.Body Then Me.Cursor = Cursors.SizeAll
                    End If
                End If

                If isMouseDown Then
                    mOverElement = testOverElement
                    mSelectedKeyFrame = testOverKeyFrame
                End If

                Exit Sub
            End If
        Next

        If p.X < mSplit - 2 Then
            overObject = OverObjectConstants.Layer
            Me.Cursor = Cursors.Default
        ElseIf p.X >= mSplit - 2 AndAlso p.X <= mSplit + 2 Then
            overObject = OverObjectConstants.Split
            Me.Cursor = Cursors.VSplit
        End If
    End Sub

    Private Sub DrawGrid(ByVal g As Graphics, ByVal r As Rectangle)
        r.Y -= VScrollBarMain.Value

        Dim h As Integer = r.Height
        If mTrack IsNot Nothing Then h = Math.Max(mTrack.Layers.Count * tlStyle.CellHeight, r.Height)

        For y As Integer = r.Y To h Step tlStyle.CellHeight
            g.DrawLine(Pens.Gray, 0, y, r.Width, y)
        Next
        g.DrawLine(tlStyle.GridColor, mSplit, 0, mSplit, r.Height)
    End Sub

    Private Sub DrawTickMarks(ByVal g As Graphics, ByVal r As Rectangle)
        Dim p As Point = New Point(mSplit - HScrollBarMain.Value, tlStyle.CellHeight - tlStyle.Margin \ 2)
        Dim lastX As Integer = -tlStyle.ScaleFontSize.Width * 4
        Dim t As TimeSpan

        g.Clip = New Region(New Rectangle(mSplit + 1, 0, r.Width, r.Height))

        If mTrack Is Nothing Then
            Using b = New SolidBrush(Color.FromKnownColor(KnownColor.ControlDark))
                g.FillRectangle(b, New Rectangle(1, 1, r.Width - 2, tlStyle.CellHeight))
            End Using
        Else
            g.FillRectangle(tlStyle.TimelineBackColor, New Rectangle(1, 1, r.Width - 2, tlStyle.CellHeight))
        End If

        For x As Integer = p.X To r.Width - 2 Step 2
            t = XToTimeSpan(x - mSplit + HScrollBarMain.Value)
            If t.Milliseconds = 0 Then
                If x - lastX > tlStyle.ScaleFontSize.Width + tlStyle.Margin Then
                    g.DrawLine(tlStyle.GridColor, x, p.Y - tlStyle.Margin - tlStyle.Margin \ 2, x, tlStyle.CellHeight)
                    g.DrawString(t.ToString(), tlStyle.ScaleFont, tlStyle.TickTextColor, x, p.Y - tlStyle.ScaleFontSize.Height - tlStyle.Margin \ 2, stringFormat)
                    lastX = x
                Else
                    g.DrawLine(tlStyle.TickLinesColor, x, p.Y - tlStyle.Margin \ 2, x, tlStyle.CellHeight)
                End If
            Else
                g.DrawLine(tlStyle.TickLinesSmallColor, x, p.Y, x, tlStyle.CellHeight)
            End If
        Next

        g.DrawLine(tlStyle.GridColor, 0, tlStyle.CellHeight, r.Width, tlStyle.CellHeight)

        g.ResetClip()
    End Sub

    Private Sub DrawLayers(ByVal g As Graphics, ByVal r As Rectangle)
        If mTrack Is Nothing Then Exit Sub

        r.Y -= VScrollBarMain.Value
        r.Width -= VScrollBarMain.Width
        r.Height -= HScrollBarMain.Height

        For Each layer In mTrack.Layers
            Dim textRect As Rectangle = New Rectangle(r.X + tlStyle.Margin,
                                                      tlStyle.CellHeight + r.Y + tlStyle.Margin,
                                                      mSplit - (r.X + tlStyle.Margin) * 2,
                                                      tlStyle.CellHeight - tlStyle.Margin * 2)

            Dim layerRect As Rectangle = New Rectangle(r.X, tlStyle.CellHeight + r.Y + 1, r.Width, tlStyle.CellHeight - 1)

            If Not layer.Visible Then g.FillRectangle(tlStyle.LayerNotVisible, layerRect)
            If layer.Equals(overLayer) Then
                If layer.Visible Then g.FillRectangle(tlStyle.LayerSelectedBackColor, layerRect)
                g.DrawString(layer.Name, Me.Font, tlStyle.LayerTextColor, textRect, stringFormat)
            Else
                g.DrawString(layer.Name, Me.Font, tlStyle.LayerSelectedTextColor, textRect, stringFormat)
            End If

            DrawVolumeMeters(g, layer, textRect)

            Dim er As Rectangle = New Rectangle(0, tlStyle.CellHeight + r.Y + 2, 0, tlStyle.CellHeight - 4)
            g.Clip = New Region(New Rectangle(mSplit + 1, er.Y, r.Width, r.Height))

            Dim disableLayer As Boolean = False
            Dim disableLayerFromX As Integer = Integer.MaxValue

            For Each element In layer.Elements
                er.X = TimeSpanToX(element.InitialKeyFrame.Time) - HScrollBarMain.Value
                er.Width = TimeSpanToX(element.Duration) - mSplit

                Dim isSelected As Boolean = mSelectedElements.Count > 0 AndAlso mSelectedElements.Contains(element)

                If isSelected Then
                    g.FillRectangle(tlStyle.ElementSelectedBackColor, er)
                Else
                    g.FillRectangle(tlStyle.ElementBackColor, er)
                End If

                If element.Type = IElement.TypeConstants.Audio OrElse element.Type = IElement.TypeConstants.Video Then DrawWaveForm(g, element, er, r)
                DrawFadeMarkers(g, element, er)
                DrawLoopMarkers(g, element, er)
                DrawKeyFrames(g, element, er)

                g.DrawRectangle(tlStyle.ElementBorderColor, er)

                Dim tr As Rectangle = er
                tr.X += 4
                tr.Width -= 4
                tr.Y = er.Y + 2
                tr.Height -= 4

                Dim text As String = element.Description
                If TypeOf element Is ElementText OrElse TypeOf element Is ElementMarquee Then
                    text = CType(element, ElementText).Text
                End If
                If isSelected Then
                    g.DrawString(text, tlStyle.ScaleFont, tlStyle.ElementSelectedTextColor, tr, stringFormat)
                Else
                    g.DrawString(text, tlStyle.ScaleFont, tlStyle.ElementTextColor, tr, stringFormat)
                End If

                If element.TypeIsMedia Then
                    Dim mediaDuaration As Double = 0
                    Select Case element.Type
                        Case IElement.TypeConstants.Audio : mediaDuaration = CType(element, ElementAudio).MediaDuration.TotalMilliseconds
                        Case IElement.TypeConstants.Video : mediaDuaration = CType(element, ElementVideo).MediaDuration.TotalMilliseconds
                        Case IElement.TypeConstants.ImageSequence : mediaDuaration = CType(element, ElementImageSequence).MediaDuration.TotalMilliseconds
                    End Select
                    If mediaDuaration > 0 Then
                        For i As Integer = element.InitialKeyFrame.Time.TotalMilliseconds To element.EndTime.TotalMilliseconds Step mediaDuaration
                            Dim x = TimeSpanToX(i) - HScrollBarMain.Value
                            g.FillPolygon(tlStyle.ElementFadeColor, CreateTriangle(x, er.Y, x + 5, er.Y + 5))
                            g.FillPolygon(tlStyle.ElementFadeColor, CreateTriangle(x + 10, er.Y, x + 5, er.Y + 5))
                        Next
                    End If
                End If

                elementsBounds.Add(element, er)

                If element.LoopEnabled Then
                    disableLayer = True
                    disableLayerFromX = Math.Min(disableLayerFromX, TimeSpanToX(element.EndTime))
                End If
            Next

            If disableLayer Then
                Using b As Drawing2D.HatchBrush = New Drawing2D.HatchBrush(Drawing2D.HatchStyle.DiagonalCross, tlStyle.LayerDisabledSection, Color.Transparent)
                    g.FillRectangle(b, disableLayerFromX, layerRect.Y, layerRect.Width - disableLayerFromX, layerRect.Height - 1)
                End Using
            End If

            g.ResetClip()

            r.Y += tlStyle.CellHeight
        Next

        Dim totalHeight As Integer = (mTrack.Layers.Count + 1) * tlStyle.CellHeight
        VScrollBarMain.Maximum = If(r.Y < totalHeight,
                                    Math.Max(totalHeight - r.Height + HScrollBarMain.Height, 0),
                                    0)

        If elementsBounds.Count > 0 Then
            Dim totalWidth As Integer = (From eb In elementsBounds Select eb.Value.Right).Max() + tlStyle.Margin + HScrollBarMain.Value + VScrollBarMain.Width
            HScrollBarMain.Maximum = If(totalWidth > r.Width,
                                        Math.Max(totalWidth - r.Width, 0),
                                        0)
        Else
            HScrollBarMain.Maximum = 0
        End If
    End Sub

    Private Sub DrawWaveForm(g As Graphics, element As Element, er As Rectangle, cr As Rectangle)
        'TODO: This code needs to be improved, considerably
        'Exit Sub

        Dim mp As Integer = er.Y + er.Height / 2
        Dim p1 As Point = New Point(er.X, mp)
        Dim p2 As Point = p1
        Dim points As List(Of Point) = New List(Of Point)
        Dim duration As Double
        Dim maxWidth As Integer
        Dim peaks As List(Of WavePeaks.Peak) = Nothing

        Select Case element.Type
            Case IElement.TypeConstants.Audio
                With CType(element, ElementAudio)
                    duration = .MediaDuration.TotalMilliseconds
                    maxWidth = TimeSpanToX(duration) - mSplit
                    peaks = .Peaks
                End With
            Case IElement.TypeConstants.Video
                With CType(element, ElementVideo)
                    duration = .MediaDuration.TotalMilliseconds
                    maxWidth = TimeSpanToX(duration) - mSplit
                    peaks = .Peaks
                End With
        End Select

        points.Add(p1)
        Try
            For i As Integer = 0 To peaks.Count - 1
                p2.X = er.X + (peaks(i).Time.TotalMilliseconds / duration) * maxWidth
                If p2.X >= er.Right OrElse p2.X >= cr.Right Then Exit For

                If p2.X > mSplit Then
                    p2.Y = mp + ((peaks(i).Left / 2) / maxVolVal * er.Height)
                    points.Add(p2)
                End If
                p1 = p2
            Next
        Catch
        End Try

        If points.Count > 1 Then g.DrawLines(tlStyle.ElementWaveFormColor, points.ToArray())
    End Sub

    Private Sub DrawKeyFrames(ByVal g As Graphics, ByVal element As Element, ByVal r As Rectangle)
        Dim x As Integer
        g.FillRectangle(tlStyle.ElementKeyFramesAreaColor, r.X, r.Bottom - 6, r.Width, 6)
        For Each p In element.KeyFrames
            x = TimeSpanToX(p.Time) - HScrollBarMain.Value
            If x >= r.X AndAlso x <= r.Right Then
                g.DrawLine(tlStyle.ElementKeyFramesColor, x, r.Bottom - 6, x, r.Bottom - 1)
            End If
        Next
    End Sub

    Private Sub DrawLoopMarkers(ByVal g As Graphics, ByVal element As Element, ByVal r As Rectangle)
        If element.LoopEnabled Then
            Dim fr As Rectangle
            fr = New Rectangle(r.X + 1, r.Top + 1, TimeSpanToX(element.LoopFrom) - mSplit, r.Height - 2)
            g.FillRectangle(tlStyle.ElementLoopColor, fr)
            g.DrawLine(tlStyle.ElementFadeAndLoopBorderColor, fr.Right, fr.Top, fr.Right, fr.Bottom)

            fr = New Rectangle(TimeSpanToX(element.EndTime - element.LoopTo) - HScrollBarMain.Value, r.Top + 1, TimeSpanToX(element.LoopTo) - mSplit, r.Height - 2)
            g.FillRectangle(tlStyle.ElementLoopColor, fr)
            g.DrawLine(tlStyle.ElementFadeAndLoopBorderColor, fr.Left, fr.Top, fr.Left, fr.Bottom)
        End If
    End Sub

    Private Sub DrawFadeMarkers(ByVal g As Graphics, ByVal element As Element, ByVal r As Rectangle)
        Dim tps() As Point
        If element.FadeIn.Type = IElement.FadeEffect.TypeConstants.Fade Then
            tps = CreateTriangle(r.X + 1, r.Bottom - 6, r.X + TimeSpanToX(element.FadeIn.Duration) - mSplit, r.Top + 1)
            g.FillPolygon(tlStyle.ElementFadeColor, tps)
            g.DrawLines(tlStyle.ElementFadeAndLoopBorderColor, tps)
        End If

        If element.FadeOut.Type = IElement.FadeEffect.TypeConstants.Fade Then
            tps = CreateTriangle(r.Right - 1, r.Bottom - 6, r.Right - (TimeSpanToX(element.FadeOut.Duration) - mSplit), r.Top + 1)
            g.FillPolygon(tlStyle.ElementFadeColor, tps)
            g.DrawLines(tlStyle.ElementFadeAndLoopBorderColor, tps)
        End If
    End Sub

    Private Sub DrawVolumeMeters(ByVal g As Graphics, ByVal layer As Layer, ByVal r As Rectangle)
        Dim vr As Rectangle = New Rectangle(r.X + 2, r.Bottom - 8, r.Width - 4, 6)
        Dim l As IElement.Level = New IElement.Level
        Dim layerTime As TimeSpan = layer.CursorTime

        If layer.Track.State = Project.Track.StateConstants.Playing Then
            For Each levels As IElement.Level In (From ea In layer.Elements
                                                  Where (ea.Type = IElement.TypeConstants.Audio OrElse ea.Type = IElement.TypeConstants.Video) AndAlso
                                                        mPlaybackCursorTime >= ea.InitialKeyFrame.Time AndAlso
                                                        mPlaybackCursorTime <= ea.EndTime
                                                  Select ea.Levels(mPlaybackCursorTime - ea.InitialKeyFrame.Time))
                l.LeftChannel = levels.LeftChannel
                l.RightChannel = levels.RightChannel
            Next
        End If

        g.FillRectangle(Brushes.Blue, New Rectangle(vr.X, vr.Y, Math.Min(l.LeftChannel / maxVolVal * vr.Width, vr.Width), vr.Height))
        g.DrawRectangle(tlStyle.TickLinesColor, vr)

        vr.Y += 6
        g.FillRectangle(Brushes.Blue, New Rectangle(vr.X, vr.Y, Math.Min(l.RightChannel / maxVolVal * vr.Width, vr.Width), vr.Height))
        g.DrawRectangle(tlStyle.TickLinesColor, vr)
    End Sub

    Private Sub DrawTime(g As Graphics, r As Rectangle)
        Dim time As TimeSpan = If(mTrack IsNot Nothing AndAlso mTrack.State = Project.Track.StateConstants.Playing,
                                    mPlaybackCursorTime,
                                    mCursorTime)

        Dim frame As Integer = If(mTrack Is Nothing,
                                time.TotalMilliseconds * 30 / 1000 Mod 30,
                                time.TotalMilliseconds * mTrack.FrameRate / 1000 Mod mTrack.FrameRate)
        g.DrawString(String.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", time.Hours, time.Minutes, time.Seconds, frame),
                     tlStyle.TimeFont, tlStyle.TimeColor, (mSplit - tlStyle.TimeFontSize.Width) / 2, (tlStyle.CellHeight - tlStyle.TimeFontSize.Height) / 2)
    End Sub

    Private Function CreateTriangle(x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer) As Point()
        Return CreateTriangle(New Point(x1, y1), New Point(x2, y2))
    End Function

    Private Function CreateTriangle(p1 As Point, p2 As Point) As Point()
        Dim p(2) As Point

        p(0) = p1
        p(1) = p2
        p(2) = New Point(p2.X, p1.Y)

        Return p
    End Function

    Private Function XToTimeSpan(x As Integer) As TimeSpan
        Return New TimeSpan(0, 0, 0, 0, x * mZoom * 100)
    End Function

    Private Function TimeSpanToX(t As TimeSpan) As Integer
        Return t.TotalMilliseconds / (mZoom * 100) + mSplit
    End Function

    Private Function TimeSpanToX(t As Double) As Integer
        Return t / (mZoom * 100) + mSplit
    End Function

    Private Sub VScrollBarMain_Scroll(sender As Object, e As ScrollEventArgs) Handles VScrollBarMain.Scroll
        Repaint()
    End Sub

    Private Sub HScrollBarMain_Scroll(sender As Object, e As ScrollEventArgs) Handles HScrollBarMain.Scroll
        Repaint()
    End Sub

    Private Sub ButtonVZoomIn_Click(sender As Object, e As EventArgs) Handles ButtonVZoomIn.Click
        tlStyle.CellHeight += 10
        Repaint()
    End Sub

    Private Sub ButtonVZoomOut_Click(sender As Object, e As EventArgs) Handles ButtonVZoomOut.Click
        If tlStyle.CellHeight > tlStyle.TimeFontSize.Height + tlStyle.Margin * 2 Then
            tlStyle.CellHeight -= 10
            Repaint()
        End If
    End Sub

    Private Sub ButtonHZoomOut_Click(sender As Object, e As EventArgs) Handles ButtonHZoomOut.Click
        mZoom += 0.2
        Repaint()
    End Sub

    Private Sub ButtonHZoomIn_Click(sender As Object, e As EventArgs) Handles ButtonHZoomIn.Click
        If mZoom > 0.21 Then
            mZoom -= 0.2
            Repaint()
        End If
    End Sub

    Private Sub UpdateLayersCursorTime(x As Integer)
        If x - mSplit < 0 Then
            mCursorTime = TimeSpan.Zero
        Else
            CursorTime = XToTimeSpan(x - mSplit + HScrollBarMain.Value)
        End If

        If Not (mTrack Is Nothing OrElse mTrack.Layers Is Nothing) Then
            If mTrack.State <> Project.Track.StateConstants.Playing OrElse isCtrlDown Then
                mPlaybackCursorTime = mCursorTime
                mTrack.CursorTime = mCursorTime

                For Each layer In mTrack.Layers
                    layer.CursorTime = mCursorTime

                    If mTrack.State = Project.Track.StateConstants.Playing AndAlso isCtrlDown Then
                        For Each element As Element In (From e In layer.Elements Where e.TypeIsMedia AndAlso e.Type <> IElement.TypeConstants.Image Select e)
                            element.Position = mCursorTime - element.InitialKeyFrame.Time
                        Next
                    End If
                Next

                RaiseEvent CursorPositionChanged(mCursorTime)
            End If
        End If
    End Sub

    Private Sub HandleKeyFramesChanged(sender As Object, e As PropertyValueChangedEventArgs)
        Repaint()
        RaiseEvent KeyFramesChanged(sender, e)
    End Sub

    Private Sub HandleKeyFramesChanged(sender As Object, e As MouseEventArgs)
        Repaint()
        RaiseEvent KeyFramesChanged(sender, New PropertyValueChangedEventArgs(Nothing, Nothing))
    End Sub

    Private Sub ContextMenuElementDelete_Click(sender As Object, e As EventArgs) Handles ContextMenuElementDelete.Click
        mOverElement.Layer.Elements.Remove(mOverElement)
        mSelectedElements.Remove(mOverElement)
        RaiseEvent ObjectsSelected(mSelectedElements, Nothing, overLayer)

        Repaint()
    End Sub

    Private Sub ContextMenuKeyFramesAdd_Click(sender As Object, e As EventArgs) Handles ContextMenuKeyFramesAdd.Click
        mSelectedKeyFrame = mOverElement.GetBlendedKeyFrame()
        mSelectedKeyFrame.Time = mOverElement.Layer.CursorTime
        mOverElement.KeyFrames.Add(mSelectedKeyFrame)

        RaiseEvent ObjectsSelected(mSelectedElements, mSelectedKeyFrame, overLayer)
        Repaint()
    End Sub

    Private Sub ContextMenuKeyFramesDelete_Click(sender As Object, e As EventArgs) Handles ContextMenuKeyFramesDelete.Click
        mOverElement.KeyFrames.Remove(mSelectedKeyFrame)

        RaiseEvent ObjectsSelected(mSelectedElements, Nothing, overLayer)
        Repaint()
    End Sub

    Private Sub ContextMenuKeyFramesCopy_Click(sender As Object, e As EventArgs) Handles ContextMenuKeyFramesCopy.Click
        clipboard = mSelectedKeyFrame.Clone()
    End Sub

    Private Sub ContextMenuKeyFramesPaste_Click(sender As Object, e As EventArgs) Handles ContextMenuKeyFramesPaste.Click
        clipboard.Time = mOverElement.Layer.CursorTime

        If mSelectedKeyFrame IsNot Nothing Then
            mOverElement.KeyFrames.Remove(mSelectedKeyFrame)
        End If

        mSelectedKeyFrame = clipboard.Clone()
        mOverElement.KeyFrames.Add(mSelectedKeyFrame)

        RaiseEvent ObjectsSelected(mSelectedElements, mSelectedKeyFrame, overLayer)
        Repaint()
    End Sub

    Friend Sub AddNewElementFromContextMenu(sender As Object, e As EventArgs)
        Dim mi As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        Dim elementType As Type = CType(mi.Tag, Type)
        Dim element As Element = Nothing

        Select Case elementType
            Case GetType(Layer.ElementClock)
                element = New ElementClock(overLayer)
            Case GetType(Layer.ElementImage)
                element = New ElementImage(overLayer)
            Case GetType(Layer.ElementMarquee)
                element = New ElementMarquee(overLayer)
            Case GetType(Layer.ElementText)
                element = New ElementText(overLayer)
            Case GetType(Layer.ElementVideo)
                element = New ElementVideo(overLayer)
            Case GetType(Layer.ElementAudio)
                element = New ElementAudio(overLayer)
            Case GetType(Layer.ElementShape)
                element = New ElementShape(overLayer)
            Case GetType(Layer.ElementTimer)
                element = New ElementTimer(overLayer)
            Case GetType(Layer.ElementImageSequence)
                element = New ElementImageSequence(overLayer)
        End Select

        element.InitialKeyFrame.Time = overLayer.CursorTime
        overLayer.Elements.Add(element)

        mSelectedElements.Clear()
        mSelectedElements.Add(element)

        overObject = OverObjectConstants.Element
        mOverElement = element
        mSelectedKeyFrame = element.InitialKeyFrame

        element.InitialKeyFrame.Bounds = New Rectangle(0, 0, 200, 100)

        Select Case element.Type
            Case IElement.TypeConstants.Audio
                element.SourceFile = Layer.Element.SelectSourceFile(element.SourceFile, element.Type)
                If IO.File.Exists(element.SourceFile) Then element.Duration = CType(element, ElementAudio).MediaDuration
            Case IElement.TypeConstants.Video
                element.SourceFile = Layer.Element.SelectSourceFile(element.SourceFile, element.Type)
                If IO.File.Exists(element.SourceFile) Then
                    Dim elementVideo As ElementVideo = CType(element, ElementVideo)

                    AddHandler elementVideo.NewFrame, AddressOf AutoSizeElementVideo
                    elementVideo.Position = TimeSpan.FromMilliseconds(100)

                    elementVideo.Duration = elementVideo.MediaDuration
                    elementVideo.InitialKeyFrame.BackColor.Color1 = Color.Transparent
                End If
            Case IElement.TypeConstants.Image
                Dim elementImage As ElementImage = CType(element, ElementImage)
                element.SourceFile = Layer.Element.SelectSourceFile(element.SourceFile, element.Type)
                If IO.File.Exists(element.SourceFile) AndAlso elementImage.Image IsNot Nothing Then
                    element.InitialKeyFrame.Bounds = New Rectangle(Point.Empty, elementImage.Image.Size)
                    element.InitialKeyFrame.BackColor.Color1 = Color.Transparent
                End If
            Case IElement.TypeConstants.Shape
                element.InitialKeyFrame.BorderSize = 2
                element.InitialKeyFrame.BorderColor = Color.LightBlue
            Case IElement.TypeConstants.ImageSequence
                element.SourceFile = Layer.Element.SelectSourceFile(element.SourceFile, element.Type)
                If IO.File.Exists(element.SourceFile) Then
                    element.Duration = CType(element, ElementImageSequence).MediaDuration
                    element.InitialKeyFrame.Bounds = New Rectangle(Point.Empty, CType(element, ElementImageSequence).Image.Size)
                    element.InitialKeyFrame.BackColor.Color1 = Color.Transparent
                End If
        End Select

        SetElementLocationAndSize(element)

        RaiseEvent ObjectsSelected(mSelectedElements, element.InitialKeyFrame, overLayer)

        Repaint()
    End Sub

    Private Delegate Sub SafeRaiseEvent()
    Private Sub AutoSizeElementVideo(ByVal sender As ElementVideo, ByVal image As Bitmap)
        RemoveHandler sender.NewFrame, AddressOf AutoSizeElementVideo

        sender.ClearBlendedKeyFramesCache()
        sender.Position = TimeSpan.Zero
        If IO.File.Exists(sender.SourceFile) Then sender.InitialKeyFrame.Bounds = New Rectangle(Point.Empty, image.Size)
        SetElementLocationAndSize(sender)

        Me.Invoke(New SafeRaiseEvent(Sub()
                                         RaiseEvent ObjectsSelected(mSelectedElements, mSelectedKeyFrame, overLayer)
                                     End Sub
                ))
    End Sub

    Private Sub SetElementLocationAndSize(ByVal element As Element)
        element.InitialKeyFrame.Bounds = New Rectangle(
                                                        New Point(
                                                            (element.Layer.Track.Project.Resolution.Width - element.InitialKeyFrame.Bounds.Width) / 2,
                                                            (element.Layer.Track.Project.Resolution.Height - element.InitialKeyFrame.Bounds.Height) / 2),
                                                        element.InitialKeyFrame.Bounds.Size
                                                        )
    End Sub

    Private Sub ContextMenuTimelineProperties_Click(sender As Object, e As EventArgs) Handles ContextMenuTimelineProperties.Click
        Using dlg As FormEditTimeLineProperties = New FormEditTimeLineProperties()
            dlg.TimeLineControl = tlStyle
            AddHandler dlg.PropertyGridElement.PropertyValueChanged, Sub(sender1 As Object, e1 As PropertyValueChangedEventArgs)
                                                                         Repaint()
                                                                     End Sub
            dlg.ShowDialog(Me)
        End Using
    End Sub

    Private Sub MenuAddLayer_Click(sender As Object, e As EventArgs) Handles ContextMenuElementAddLayer.Click, ContextMenuTimelineAddLayer.Click
        If mTrack IsNot Nothing Then
            Dim index As Integer = (From l In mTrack.Layers Select l Where l.Name.StartsWith("New Layer ")).Count + 1
            Dim strIndex As String = IIf(index < 10, "0", "") + index.ToString()
            overLayer = New Project.Track.Layer(mTrack, "New Layer " + strIndex)
            mTrack.Layers.Add(overLayer)

            Repaint()

            RaiseEvent ObjectsSelected(Nothing, Nothing, overLayer)
        End If
    End Sub

    Private Sub ContextMenuElementDeleteLayer_Click(sender As Object, e As EventArgs) Handles ContextMenuElementDeleteLayer.Click, ContextMenuTimelineDeleteLayer.Click
        If mTrack IsNot Nothing AndAlso overLayer IsNot Nothing Then
            mTrack.Layers.Remove(overLayer)
            overLayer = Nothing

            Repaint()

            RaiseEvent ObjectsSelected(Nothing, Nothing, overLayer)
        End If
    End Sub
End Class