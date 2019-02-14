Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Drawing.Design

Partial Public Class Project
    Partial Public Class Track
        Partial Public Class Layer
            <Serializable()>
            Public MustInherit Class Element
                Implements IElement, ICloneable

                Private mDescription As String = ""
                Private mLayer As Layer
                Private mDuration As TimeSpan = New TimeSpan(0, 0, 10)
                Private mFadeIn As IElement.FadeEffect = New IElement.FadeEffect(0, IElement.FadeEffect.TypeConstants.Instant, IElement.FadeEffect.ModeConstants.FadeIn)
                Private mFadeOut As IElement.FadeEffect = New IElement.FadeEffect(0, IElement.FadeEffect.TypeConstants.Instant, IElement.FadeEffect.ModeConstants.FadeOut)
                Private mKeyFrames As ObservableCollection(Of KeyFrame) = New ObservableCollection(Of KeyFrame)
                Private mPadding As Padding = New Padding(2)
                Private mSourceFile As String

                Private mLoopFrom As TimeSpan
                Private mLoopTo As TimeSpan

                Private blendedKeyFramesCache As Dictionary(Of TimeSpan, KeyFrame)
                Private elementVideo As ElementVideo
                Private elementAudio As ElementAudio
                Private elementImage As ElementImage
                Private elementImageSequence As ElementImageSequence

                <CategoryAttribute("Global Settings")>
                Public MustOverride ReadOnly Property Name() As String Implements IElement.Name
                <DisplayName("Filter Type")>
                Public MustOverride ReadOnly Property Type() As IElement.TypeConstants Implements IElement.Type

                Public MustOverride Sub SetFromXML(xml As XElement) Implements IElement.SetFromXML

                <NonSerialized()>
                Public Event KeyFramesChanged(sender As Element) Implements IElement.KeyFramesChanged

                Public Sub New(layer As Layer)
                    mLayer = layer

                    If Me.Type = IElement.TypeConstants.Audio Then elementAudio = CType(Me, ElementAudio)
                    If Me.Type = IElement.TypeConstants.Image Then elementImage = CType(Me, ElementImage)
                    If Me.Type = IElement.TypeConstants.Video Then
                        elementVideo = CType(Me, ElementVideo)
                        elementImage = CType(Me, ElementImage)
                    End If
                    If Me.Type = IElement.TypeConstants.ImageSequence Then
                        elementImageSequence = CType(Me, ElementImageSequence)
                        elementImage = CType(Me, ElementImage)
                    End If

                    CreateDefaultConditions()
                    Initialize()
                End Sub

                Public Sub Initialize()
                    mLayer.Track.Project.HasChanged = True

                    blendedKeyFramesCache = New Dictionary(Of TimeSpan, KeyFrame)

                    If Type = IElement.TypeConstants.Video Then Me.SourceFile = Me.SourceFile

                    AddHandler mKeyFrames.CollectionChanged, Sub(sender As Object, e As Specialized.NotifyCollectionChangedEventArgs)
                                                                 If e.Action = Specialized.NotifyCollectionChangedAction.Add Then
                                                                     For i As Integer = 0 To e.NewItems.Count - 1
                                                                         Dim kf As KeyFrame = CType(e.NewItems(i), KeyFrame)
                                                                         If kf.Element Is Nothing Then
                                                                             kf.Element = Me

                                                                             Dim cTime As TimeSpan = mLayer.CursorTime

                                                                             Do
                                                                                 If (From p1 In mKeyFrames Where p1.Time = cTime Select p1).Count > 0 Then
                                                                                     cTime += (New TimeSpan(0, 0, 1))
                                                                                 Else
                                                                                     Exit Do
                                                                                 End If
                                                                             Loop
                                                                             kf.Time = cTime

                                                                             With GetBlendedKeyFrame(cTime)
                                                                                 kf.BackColor = .BackColor.Clone()
                                                                                 kf.BorderColor = .BorderColor
                                                                                 kf.BorderSize = .BorderSize
                                                                                 kf.Bounds = .Bounds
                                                                                 kf.ContentAlignment = .ContentAlignment
                                                                                 kf.Font = .Font.Clone()
                                                                                 kf.ForeColor = .ForeColor.Clone()
                                                                                 kf.Padding = .Padding
                                                                                 kf.Rotation = .Rotation
                                                                                 kf.ChromaKeyColor = .ChromaKeyColor.Clone()
                                                                             End With

                                                                             AddHandler kf.KeyFrameChanged, Sub(changedKeyFrame As KeyFrame)
                                                                                                                changedKeyFrame.Element.ClearBlendedKeyFramesCache()
                                                                                                            End Sub
                                                                         End If
                                                                     Next
                                                                 End If

                                                                 ClearBlendedKeyFramesCache()
                                                                 RaiseEvent KeyFramesChanged(Me)
                                                             End Sub
                End Sub

                <CategoryAttribute("Global Settings")>
                Public Property Duration As TimeSpan Implements IElement.Duration
                    Get
                        Return mDuration
                    End Get
                    Set(value As TimeSpan)
                        mDuration = value
                    End Set
                End Property

                <CategoryAttribute("Fade Effects")>
                Public Property FadeIn As IElement.FadeEffect Implements IElement.FadeIn
                    Get
                        Return mFadeIn
                    End Get
                    Set(value As IElement.FadeEffect)
                        mFadeIn = value
                    End Set
                End Property

                <CategoryAttribute("Fade Effects")>
                Public Property FadeOut As IElement.FadeEffect Implements IElement.FadeOut
                    Get
                        Return mFadeOut
                    End Get
                    Set(value As IElement.FadeEffect)
                        mFadeOut = value
                    End Set
                End Property

                <Editor(GetType(FilteredFileNameEditor), GetType(UITypeEditor)), DisplayName("Source File")>
                Public Overridable Property SourceFile() As String Implements IElement.SourceFile
                    Get
                        Return mSourceFile
                    End Get
                    Set(value As String)
                        If mSourceFile = value Then Exit Property

                        Static isBusy As Boolean = False
                        If isBusy Then Exit Property
                        isBusy = True

                        mSourceFile = value

                        If Type = IElement.TypeConstants.Audio Then
                            elementAudio.SourceFile = value
                        ElseIf Type = IElement.TypeConstants.Video Then
                            elementVideo.SourceFile = value
                        ElseIf Type = IElement.TypeConstants.Image Then
                            elementImage.SourceFile = value
                        ElseIf Type = IElement.TypeConstants.ImageSequence Then
                            elementImageSequence.SourceFile = value
                        End If

                        isBusy = False
                    End Set
                End Property

                <CategoryAttribute("Appearance"),
                DisplayName("Initial KeyFrame")>
                Public Property InitialKeyFrame As KeyFrame Implements IElement.InitialKeyFrame
                    Get
                        If mKeyFrames.Count = 0 Then CreateDefaultConditions()
                        Return mKeyFrames(0)
                    End Get
                    Set(value As KeyFrame)
                        mKeyFrames(0) = value
                        If value Is Nothing Then CreateDefaultConditions()
                    End Set
                End Property

                <CategoryAttribute("Appearance"),
                Editor(GetType(KeyFramesCollectionEditor), GetType(UITypeEditor))>
                Public Property KeyFrames As ObservableCollection(Of KeyFrame) Implements IElement.KeyFrames
                    Get
                        Return mKeyFrames
                    End Get
                    Set(value As ObservableCollection(Of KeyFrame))
                        mKeyFrames = value
                        If value.Count = 0 Then CreateDefaultConditions()
                    End Set
                End Property

                <CategoryAttribute("Loop"),
                DisplayName("Enabled")>
                Public Property LoopEnabled As Boolean Implements IElement.LoopEnabled

                <Browsable(False)>
                Public MustOverride ReadOnly Property IsValid As Boolean Implements IElement.IsValid

                Public MustOverride Function Render(ByVal g As Graphics, ByVal r As Rectangle, ByVal m As Drawing2D.Matrix) As KeyFrame Implements IElement.Render

                Protected Overridable Function RenderBackground(ByVal g As Graphics, ByVal m As Drawing2D.Matrix) As KeyFrame Implements IElement.RenderBackground
                    Dim bk As KeyFrame = GetBlendedKeyFrame()

                    If bk.Rotation <> 0 Then m.RotateAt(bk.Rotation, New PointF(bk.Bounds.X + bk.Bounds.Width \ 2, bk.Bounds.Y + bk.Bounds.Height \ 2))
                    g.Transform = m

                    If Type <> IElement.TypeConstants.Shape Then
                        Using b As Brush = bk.BackColor.ToBrush()
                            If bk.BorderRadius <> 0 Then
                                g.FillRoundedRectangle(b, bk.Bounds, bk.BorderRadius)
                            Else
                                g.FillRectangle(b, bk.Bounds)
                            End If
                        End Using

                        If bk.BorderSize > 0 Then
                            Using p As Pen = New Pen(bk.BorderColor, bk.BorderSize)
                                Dim br As Rectangle = bk.Bounds
                                br.Inflate(bk.BorderSize / 2, bk.BorderSize / 2)
                                If bk.BorderRadius <> 0 Then
                                    g.DrawRoundedRectangle(p, br, bk.BorderRadius)
                                Else
                                    g.DrawRectangle(p, br)
                                End If
                            End Using
                        End If
                    End If

                    Return bk
                End Function

                Protected Overridable Function AlignContent(ByVal r As Rectangle, ByVal p As Padding, ByVal contentSize As Size, ByVal alignment As ContentAlignment) As Rectangle Implements IElement.AlignContent
                    Select Case alignment
                        Case ContentAlignment.TopLeft, ContentAlignment.MiddleLeft, ContentAlignment.BottomLeft
                            r.X += p.Left
                        Case ContentAlignment.TopCenter, ContentAlignment.MiddleCenter, ContentAlignment.BottomCenter
                            r.X += (r.Width - contentSize.Width) / 2 + p.Left
                        Case ContentAlignment.TopRight, ContentAlignment.BottomRight, ContentAlignment.MiddleRight
                            r.X += r.Width - contentSize.Width - p.Right
                    End Select
                    Select Case alignment
                        Case ContentAlignment.TopLeft, ContentAlignment.TopCenter, ContentAlignment.TopRight
                            r.Y += p.Top
                        Case ContentAlignment.MiddleLeft, ContentAlignment.MiddleCenter, ContentAlignment.MiddleRight
                            r.Y += (r.Height - contentSize.Height) / 2 + p.Top
                        Case ContentAlignment.BottomLeft, ContentAlignment.BottomCenter, ContentAlignment.BottomRight
                            r.Y += r.Height - contentSize.Height - p.Bottom
                    End Select
                    Return r
                End Function

                <CategoryAttribute("Loop"),
                DisplayName("From")>
                Public Property LoopFrom As TimeSpan Implements IElement.LoopFrom
                    Get
                        Return mLoopFrom
                    End Get
                    Set(value As TimeSpan)
                        mLoopFrom = value
                    End Set
                End Property

                <CategoryAttribute("Loop"),
                DisplayName("To")>
                Public Property LoopTo As TimeSpan Implements IElement.LoopTo
                    Get
                        Return mLoopTo
                    End Get
                    Set(value As TimeSpan)
                        mLoopTo = value
                    End Set
                End Property

                Public Overridable ReadOnly Property Levels(ByVal time As TimeSpan) As IElement.Level Implements IElement.Levels
                    Get
                        Return New IElement.Level()
                    End Get
                End Property

                Public Overridable Property Position(Optional ByVal updateFrame As Boolean = False) As TimeSpan Implements IElement.Position
                    Get
                        If Type = IElement.TypeConstants.Audio OrElse Type = IElement.TypeConstants.Video Then
                            Return elementVideo.Position
                        ElseIf Type = IElement.TypeConstants.ImageSequence Then
                            Return elementImageSequence.Position
                        Else
                            Return TimeSpan.Zero
                        End If
                    End Get
                    Set(value As TimeSpan)
                        If Type = IElement.TypeConstants.Audio Then
                            elementAudio.Position = value
                        ElseIf Type = IElement.TypeConstants.Video Then
                            elementVideo.Position = value
                        ElseIf Type = IElement.TypeConstants.ImageSequence Then
                            elementVideo.Position = value
                        End If
                    End Set
                End Property

                <Browsable(False)>
                Public Overridable ReadOnly Property MediaState As IElement.MediaPlaybackStateConstants Implements IElement.MediaState
                    Get
                        Return IElement.MediaPlaybackStateConstants.Idle
                    End Get
                End Property

                <Browsable(False)>
                Public ReadOnly Property TypeIsMedia As Boolean Implements IElement.TypeIsMedia
                    Get
                        Return Type = IElement.TypeConstants.Image OrElse
                               Type = IElement.TypeConstants.Video OrElse
                               Type = IElement.TypeConstants.Audio OrElse
                               Type = IElement.TypeConstants.ImageSequence
                    End Get
                End Property

                <CategoryAttribute("Global Settings")>
                Public Overridable Property Description As String Implements IElement.Description
                    Get
                        Return If(mDescription = "", Name, mDescription)
                    End Get
                    Set(value As String)
                        mDescription = value
                    End Set
                End Property

                Public Sub CreateDefaultConditions()
                    Dim p As New KeyFrame(Me) With {.Description = "Initial Conditions"}
                    AddHandler p.TimeChanged, AddressOf UpdateKeyFramesTimes
                    mKeyFrames.Add(p)
                End Sub

                <Browsable(False)>
                Public ReadOnly Property EndTime As TimeSpan Implements IElement.EndTime
                    Get
                        Return InitialKeyFrame.Time + mDuration
                    End Get
                End Property

                Public Overridable Sub MediaPause() Implements IElement.MediaPause
                    If Type = IElement.TypeConstants.Audio Then
                        elementAudio.MediaPause()
                    ElseIf Type = IElement.TypeConstants.Video Then
                        elementVideo.MediaPause()
                    End If
                End Sub

                Public Overridable Sub MediaPlay() Implements IElement.MediaPlay
                    If Type = IElement.TypeConstants.Audio Then
                        elementAudio.MediaPlay()
                    ElseIf Type = IElement.TypeConstants.Video Then
                        elementVideo.MediaPlay()
                    End If
                End Sub

                Public Overridable Sub MediaStop() Implements IElement.MediaStop
                    If Type = IElement.TypeConstants.Audio Then
                        elementAudio.MediaStop()
                    ElseIf Type = IElement.TypeConstants.Video Then
                        elementVideo.MediaStop()
                    End If
                End Sub

                Public Sub UpdateKeyFramesTimes(ByVal sender As KeyFrame, ByVal oldTime As TimeSpan, ByVal newTime As TimeSpan)
                    If sender.Equals(Me.InitialKeyFrame) Then
                        UpdateKeyFramesTimes(sender, newTime - oldTime)

                        If mLayer.Track.State = StateConstants.Playing Then
                            Me.Position = mLayer.CursorTime - sender.Time
                        End If
                    End If
                End Sub

                Public Sub UpdateKeyFramesTimes(ByVal sender As KeyFrame, ByVal offset As TimeSpan)
                    If sender.Equals(Me.InitialKeyFrame) AndAlso offset <> TimeSpan.Zero Then
                        For i = 1 To mKeyFrames.Count - 1
                            mKeyFrames(i).Time += offset
                        Next
                    End If
                End Sub

                Public Function GetClosestKeyFrame(ByVal time As TimeSpan) As KeyFrame Implements IElement.GetClosestKeyFrame
                    Dim layerTime As TimeSpan = mLayer.CursorTime
                    mLayer.CursorTime = time
                    Dim p As KeyFrame = GetClosestKeyFrame()
                    mLayer.CursorTime = layerTime
                    Return p
                End Function

                Public Function GetClosestKeyFrame() As KeyFrame Implements IElement.GetClosestKeyFrame
                    Return mKeyFrames.Aggregate(Function(test1, test2)
                                                    Dim t1 As Double = Math.Abs((test1.Time - mLayer.CursorTime).TotalMilliseconds)
                                                    Dim t2 As Double = Math.Abs((test2.Time - mLayer.CursorTime).TotalMilliseconds)
                                                    Return If(t1 < t2, test1, test2)
                                                End Function
                                                )
                End Function

                Public Sub ClearBlendedKeyFramesCache(time As TimeSpan) Implements IElement.ClearBlendedKeyFramesCache
                    If blendedKeyFramesCache.ContainsKey(time) Then
                        blendedKeyFramesCache.Remove(time)
                    End If
                End Sub

                Public Sub ClearBlendedKeyFramesCache() Implements IElement.ClearBlendedKeyFramesCache
                    blendedKeyFramesCache.Clear()

                    'Select Case Type
                    '    Case IElement.TypeConstants.Text
                    '        CType(Me, ElementText).ResetRenderer()
                    '    Case IElement.TypeConstants.Marquee
                    '        CType(Me, ElementMarquee).ResetRenderer()
                    'End Select
                End Sub

                Public Function GetBlendedKeyFrame(time As TimeSpan) As KeyFrame Implements IElement.GetBlendedKeyFrame
                    Dim layerTime As TimeSpan = mLayer.CursorTime
                    mLayer.CursorTime(True) = time
                    Dim p As KeyFrame = GetBlendedKeyFrame()
                    mLayer.CursorTime(True) = layerTime
                    Return p
                End Function

                Public Function GetBlendedKeyFrame() As KeyFrame Implements IElement.GetBlendedKeyFrame
                    Dim currentTime As TimeSpan = mLayer.CursorTime

                    If currentTime < mKeyFrames(0).Time Then Return mKeyFrames(0)
                    If LoopEnabled Then
                        Dim lf As TimeSpan = InitialKeyFrame.Time + LoopFrom
                        Dim lt As TimeSpan = EndTime - LoopTo

                        If currentTime > lt Then
                            currentTime = lf + TimeSpan.FromMilliseconds(currentTime.TotalMilliseconds Mod (lt - lf).TotalMilliseconds)
                        End If
                        If TypeIsMedia Then Position = currentTime - InitialKeyFrame.Time
                    ElseIf currentTime > EndTime Then
                        If TypeIsMedia Then Position = currentTime - InitialKeyFrame.Time
                        Return mKeyFrames.Last
                    End If

                    Dim alpha As Integer

                    If blendedKeyFramesCache.ContainsKey(currentTime) Then
                        If TypeIsMedia Then
                            If mLayer.Track.State <> StateConstants.Playing Then Me.Position = currentTime - Me.InitialKeyFrame.Time
                            Me.ResetImageAlpha()

                            If DoFade(IElement.FadeEffect.ModeConstants.FadeIn, currentTime) Then
                                alpha = GetFadeAlpha(IElement.FadeEffect.ModeConstants.FadeIn, currentTime)
                                Me.ApplyAlpha(alpha)
                            ElseIf DoFade(IElement.FadeEffect.ModeConstants.FadeOut, currentTime) Then
                                alpha = GetFadeAlpha(IElement.FadeEffect.ModeConstants.FadeOut, currentTime)
                                Me.ApplyAlpha(alpha)
                            End If
                        End If

                        Return blendedKeyFramesCache(currentTime)
                    End If

                    Dim p As KeyFrame = Nothing

                    For Each p1 In mKeyFrames
                        If p1.Time = currentTime Then
                            p = p1
                            Exit For
                        End If
                    Next

                    Dim leftKeyFrame As KeyFrame = Nothing
                    Dim rightKeyFrame As KeyFrame = Nothing
                    Dim testTimeLeft As TimeSpan
                    Dim testTimeRight As TimeSpan

                    If mKeyFrames.Count > 0 Then
                        If p Is Nothing Then
                            testTimeLeft = TimeSpan.MinValue
                            testTimeRight = TimeSpan.MaxValue
                            For Each p In mKeyFrames
                                If p.Time > testTimeLeft AndAlso p.Time < currentTime Then
                                    leftKeyFrame = p
                                    testTimeLeft = p.Time
                                End If
                                If p.Time < testTimeRight AndAlso p.Time > currentTime Then
                                    rightKeyFrame = p
                                    testTimeRight = p.Time
                                End If
                            Next
                        Else
                            leftKeyFrame = p
                        End If
                    End If

                    Dim blendedKeyFrame As KeyFrame

                    If leftKeyFrame Is Nothing Then
                        blendedKeyFrame = rightKeyFrame.Clone()
                    ElseIf rightKeyFrame Is Nothing Then
                        blendedKeyFrame = leftKeyFrame.Clone()
                    Else
                        blendedKeyFrame = New KeyFrame(Me)
                        Dim percentage As Single = (currentTime.TotalMilliseconds - leftKeyFrame.Time.TotalMilliseconds) / (rightKeyFrame.Time.TotalMilliseconds - leftKeyFrame.Time.TotalMilliseconds)
                        With blendedKeyFrame
                            .Time = currentTime
                            .Font = New Font(leftKeyFrame.Font.FontFamily, BlendValues(leftKeyFrame.Font.Size, rightKeyFrame.Font.Size, percentage), leftKeyFrame.Font.Style, leftKeyFrame.Font.Unit)
                            .Bounds = BlendRectangles(leftKeyFrame.Bounds, rightKeyFrame.Bounds, percentage)
                            .BorderColor = BlendColors(leftKeyFrame.BorderColor, rightKeyFrame.BorderColor, percentage)
                            .BorderSize = BlendValues(leftKeyFrame.BorderSize, rightKeyFrame.BorderSize, percentage)
                            .BorderRadius = BlendValues(leftKeyFrame.BorderRadius, rightKeyFrame.BorderRadius, percentage)
                            .ForeColor = BlendBrushes(leftKeyFrame.ForeColor, rightKeyFrame.ForeColor, percentage)
                            .BackColor = BlendBrushes(leftKeyFrame.BackColor, rightKeyFrame.BackColor, percentage)
                            .ContentAlignment = leftKeyFrame.ContentAlignment
                            .Padding = BlendPadding(leftKeyFrame.Padding, rightKeyFrame.Padding, percentage)
                            .Rotation = BlendValues(leftKeyFrame.Rotation, rightKeyFrame.Rotation, percentage)
                            .ChromaKeyColor = BlendChromaKeyColor(leftKeyFrame.ChromaKeyColor, rightKeyFrame.ChromaKeyColor, percentage)
                        End With
                    End If

                    If TypeIsMedia Then Me.ResetImageAlpha()

                    If DoFade(IElement.FadeEffect.ModeConstants.FadeIn, currentTime) Then
                        alpha = GetFadeAlpha(IElement.FadeEffect.ModeConstants.FadeIn, currentTime)
                        With blendedKeyFrame
                            .BorderColor = SetColorAlpha(.BorderColor, alpha)
                            .ForeColor.Color1 = SetColorAlpha(.ForeColor.Color1, alpha)
                            .ForeColor.Color2 = SetColorAlpha(.ForeColor.Color2, alpha)
                            .BackColor.Color1 = SetColorAlpha(.BackColor.Color1, alpha)
                            .BackColor.Color2 = SetColorAlpha(.BackColor.Color2, alpha)
                        End With
                        Me.ApplyAlpha(alpha)
                    ElseIf DoFade(IElement.FadeEffect.ModeConstants.FadeOut, currentTime) Then
                        alpha = GetFadeAlpha(IElement.FadeEffect.ModeConstants.FadeOut, currentTime)
                        With blendedKeyFrame
                            .BorderColor = SetColorAlpha(.BorderColor, alpha)
                            .ForeColor.Color1 = SetColorAlpha(.ForeColor.Color1, alpha)
                            .ForeColor.Color2 = SetColorAlpha(.ForeColor.Color2, alpha)
                            .BackColor.Color1 = SetColorAlpha(.BackColor.Color1, alpha)
                            .BackColor.Color2 = SetColorAlpha(.BackColor.Color2, alpha)
                        End With
                        Me.ApplyAlpha(alpha)
                    End If

                    blendedKeyFramesCache.Add(currentTime, blendedKeyFrame)
                    Return blendedKeyFrame
                End Function

                Public Sub PrecalculateBlendedKeyFrames(fromTime As TimeSpan, toTime As TimeSpan)
                    ClearBlendedKeyFramesCache()
                    If Not Me.TypeIsMedia Then
                        Dim time As TimeSpan = fromTime
                        Dim timeEnd As TimeSpan = toTime
                        Do
                            GetBlendedKeyFrame(time)
                            time += mLayer.Track.TimeInc
                        Loop While time < timeEnd
                    End If
                End Sub

                Public Sub ApplyAlpha(alpha As Integer) Implements IElement.ApplyAlpha
                    If Type = IElement.TypeConstants.Image OrElse Type = IElement.TypeConstants.ImageSequence OrElse Type = IElement.TypeConstants.Video Then
                        elementImage.ApplyAlpha(alpha)
                    End If
                End Sub

                Public Sub ResetImageAlpha() Implements IElement.ResetImageAlpha
                    If Type = IElement.TypeConstants.Image OrElse Type = IElement.TypeConstants.ImageSequence OrElse Type = IElement.TypeConstants.Video Then
                        elementImage.ResetImageAlpha()
                    End If
                End Sub

                Private Function GetFadeAlpha(type As IElement.FadeEffect.ModeConstants, currentTime As TimeSpan) As Integer
                    If mFadeIn.Duration.TotalMilliseconds = 0 Then Return 255
                    Select Case type
                        Case IElement.FadeEffect.ModeConstants.FadeIn
                            Return (currentTime.TotalMilliseconds - InitialKeyFrame.Time.TotalMilliseconds) / mFadeIn.Duration.TotalMilliseconds * 255
                        Case IElement.FadeEffect.ModeConstants.FadeOut
                            Return (EndTime.TotalMilliseconds - currentTime.TotalMilliseconds) / mFadeOut.Duration.TotalMilliseconds * 255
                        Case Else
                            Return 255
                    End Select
                End Function

                Private Function DoFade(type As IElement.FadeEffect.ModeConstants, currentTime As TimeSpan) As Boolean
                    Select Case type
                        Case IElement.FadeEffect.ModeConstants.FadeIn
                            Return mFadeIn.Type = IElement.FadeEffect.TypeConstants.Fade AndAlso currentTime <= InitialKeyFrame.Time + mFadeIn.Duration
                        Case IElement.FadeEffect.ModeConstants.FadeOut
                            Return mFadeOut.Type = IElement.FadeEffect.TypeConstants.Fade AndAlso currentTime >= EndTime - mFadeOut.Duration
                        Case Else
                            Return False
                    End Select
                End Function

                Private Function BlendRectangles(r1 As Rectangle, r2 As Rectangle, p As Single) As Rectangle
                    Return New Rectangle(
                                        BlendValues(r1.X, r2.X, p),
                                        BlendValues(r1.Y, r2.Y, p),
                                        BlendValues(r1.Width, r2.Width, p),
                                        BlendValues(r1.Height, r2.Height, p))
                End Function

                Private Function BlendPadding(r1 As Padding, r2 As Padding, p As Single) As Padding
                    Return New Padding(
                                        BlendValues(r1.Left, r2.Left, p),
                                        BlendValues(r1.Top, r2.Top, p),
                                        BlendValues(r1.Right, r2.Right, p),
                                        BlendValues(r1.Bottom, r2.Bottom, p))
                End Function

                Private Function BlendColors(c1 As Color, c2 As Color, p As Single) As Color
                    Return Color.FromArgb(
                                        BlendValues(c1.A, c2.A, p),
                                        BlendValues(c1.R, c2.R, p),
                                        BlendValues(c1.G, c2.G, p),
                                        BlendValues(c1.B, c2.B, p))
                End Function

                Private Function BlendBrushes(b1 As Brush2, b2 As Brush2, p As Single)
                    Dim b As New Brush2(BlendColors(b1.Color1, b2.Color1, p),
                                        BlendPoints(b1.Point1, b2.Point1, p),
                                        BlendColors(b1.Color2, b2.Color2, p),
                                        BlendPoints(b1.Point2, b2.Point2, p)) With {
                                            .Type = If(b1.Type = Brush2.TypeConstants.Gradient OrElse b2.Type = Brush2.TypeConstants.Gradient,
                                                        Brush2.TypeConstants.Gradient,
                                                        Brush2.TypeConstants.Solid)
                                        }
                    Return b
                End Function

                Private Function BlendPoints(p1 As Point, p2 As Point, p As Single)
                    Return New Point(BlendValues(p1.X, p2.X, p), BlendValues(p1.Y, p2.Y, p))
                End Function

                Private Function BlendValues(v1 As Integer, v2 As Integer, p As Single) As Integer
                    Return (v2 - v1) * p + v1
                End Function

                Private Function BlendValues(v1 As Single, v2 As Single, p As Single) As Single
                    Return (v2 - v1) * p + v1
                End Function

                Private Function BlendChromaKeyColor(c1 As KeyFrame.ChromaKey, c2 As KeyFrame.ChromaKey, p As Single) As KeyFrame.ChromaKey
                    Dim ck As New KeyFrame.ChromaKey With {.Enabled = c1.Enabled OrElse c2.Enabled}

                    ck.Red.Min = BlendValues(c1.Red.Min, c2.Red.Min, p)
                    ck.Red.Max = BlendValues(c1.Red.Max, c2.Red.Max, p)

                    ck.Green.Min = BlendValues(c1.Green.Min, c2.Green.Min, p)
                    ck.Green.Max = BlendValues(c1.Green.Max, c2.Green.Max, p)

                    ck.Blue.Min = BlendValues(c1.Blue.Min, c2.Blue.Min, p)
                    ck.Blue.Max = BlendValues(c1.Blue.Max, c2.Blue.Max, p)

                    Return ck
                End Function

                Private Shared Function SetColorAlpha(c1 As Color, a As Integer) As Color
                    Return Color.FromArgb(c1.A * a / 255, c1)
                End Function

                <Browsable(False)>
                Public Property Layer As Layer Implements IElement.Layer
                    Get
                        Return mLayer
                    End Get
                    Set(value As Layer)
                        mLayer.Elements.Remove(Me)
                        mLayer = value
                        mLayer.Elements.Add(Me)
                    End Set
                End Property

                Protected Sub ForceRaiseEvent()
                    RaiseEvent KeyFramesChanged(Me)
                End Sub

                Protected Overrides Sub Finalize()
                    Me.MediaStop()
                    MyBase.Finalize()
                End Sub

                Public Shared Function SelectSourceFile(fileName As String, type As IElement.TypeConstants) As String
                    Using dlg As OpenFileDialog = New OpenFileDialog()
                        dlg.FileName = fileName

                        Select Case type
                            Case Project.Track.Layer.IElement.TypeConstants.Text, Project.Track.Layer.IElement.TypeConstants.Marquee
                                dlg.Filter = "Text Files|*.txt"
                                dlg.FilterIndex = 1
                            Case Project.Track.Layer.IElement.TypeConstants.Image
                                dlg.Filter = "JPEG|*.jpg;*.jpeg;*.jpe;*.jfif;*.jp2"
                                dlg.Filter += "|Graphics Interchange Format|*.gif"
                                dlg.Filter += "|Portable Network Graphics|*.png"
                                dlg.Filter += "|Truevision Targa|*.tga"
                                dlg.Filter += "|Bitmap|*.bmp"
                                dlg.Filter += "|Interchange File Format|*.iff"
                                dlg.Filter += "|Tagged Image File Format|*.tif"
                                dlg.Filter += "|All Image Files|*.jpg;*.jpeg;*.jpe;*.jfif;*.jp2;*.gif;*.tga;*.png;*.bmp;*.iff;*.tif"
                                dlg.FilterIndex = 8
                            Case Project.Track.Layer.IElement.TypeConstants.Video
                                dlg.Filter = "AVI|*.avi"
                                dlg.Filter += "|MPG|*.mpg;*.mpeg"
                                dlg.Filter += "|QuickTime|*.mov"
                                dlg.Filter += "|Windows Media Video|*.wmv"
                                dlg.Filter += "|All Video Files|*.avi;*.mpg;*.mpeg;*.mov;*.wmv"
                                dlg.FilterIndex = 5
                            Case Project.Track.Layer.IElement.TypeConstants.Audio
                                dlg.Filter = "WAV|*.wav"
                                dlg.Filter += "|MP3|*.mp3"
                                dlg.Filter += "|All Audio Files|*.wav;*.mp3"
                                dlg.FilterIndex = 3
                            Case IElement.TypeConstants.ImageSequence
                                dlg.Filter = "JPEG|*.jpg;*.jpeg;*.jpe;*.jfif;*.jp2"
                                dlg.Filter += "|Graphics Interchange Format|*.gif"
                                dlg.Filter += "|Portable Network Graphics|*.png"
                                dlg.Filter += "|Truevision Targa|*.tga"
                                dlg.Filter += "|Bitmap|*.bmp"
                                dlg.Filter += "|Interchange File Format|*.iff"
                                dlg.Filter += "|Tagged Image File Format|*.tif"
                                dlg.Filter += "|All Image Files|*.jpg;*.jpeg;*.jpe;*.jfif;*.jp2;*.gif;*.tga;*.png;*.bmp;*.iff;*.tif"
                                dlg.FilterIndex = 8
                        End Select
                        dlg.Filter += "|All Files|*.*"

                        Return If(dlg.ShowDialog() = DialogResult.OK, dlg.FileName, "")
                    End Using
                End Function

                Public Overridable Function ToXML() As XElement Implements IElement.ToXML
                    Return <element type=<%= Me.Type %>>
                               <description><%= Me.Description %></description>
                               <duration><%= Me.Duration.TotalMilliseconds %></duration>
                               <sourceFile><%= Me.SourceFile %></sourceFile>

                               <%= Me.FadeIn.ToXML() %>
                               <%= Me.FadeOut.ToXML() %>

                               <loop>
                                   <enabled><%= Me.LoopEnabled %></enabled>
                                   <from><%= Me.LoopFrom.TotalMilliseconds %></from>
                                   <to><%= Me.LoopTo.TotalMilliseconds %></to>
                               </loop>

                               <keyFrames>
                                   <%= From keyFrame As KeyFrame In Me.KeyFrames Select (keyFrame.ToXML()) %>
                               </keyFrames>
                           </element>
                End Function

                Public Shared Function FromXML(layer As Layer, xml As XElement) As Element
                    Dim elementType As IElement.TypeConstants

                    Dim parseStandard = Sub(ByRef element As Element, xmle As XElement)
                                            element.Description = xmle.<description>.Value
                                            element.Duration = TimeSpan.FromMilliseconds(xmle.<duration>.Value)
                                            element.FadeIn = IElement.FadeEffect.FromXML(xmle.<fadeIn>(0))
                                            element.FadeOut = IElement.FadeEffect.FromXML(xmle.<fadeOut>(0))
                                            element.KeyFrames.Clear()
                                            For Each xkf In xml...<keyframe>
                                                Dim kf As KeyFrame = KeyFrame.FromXML(element, xkf)
                                                AddHandler kf.TimeChanged, AddressOf element.UpdateKeyFramesTimes
                                                element.KeyFrames.Add(kf)
                                            Next
                                            element.SourceFile = xmle.<sourceFile>.Value

                                            element.LoopEnabled = Project.Track.Layer.Element.ParseString(Of Boolean)(xmle.<loop>.<enabled>.Value)
                                            element.LoopFrom = TimeSpan.FromMilliseconds(xmle.<loop>.<from>.Value)
                                            element.LoopTo = TimeSpan.FromMilliseconds(xmle.<loop>.<to>.Value)
                                        End Sub

                    If [Enum].TryParse(xml.@type, elementType) Then
                        Select Case elementType
                            Case Project.Track.Layer.IElement.TypeConstants.Text
                                Dim element As ElementText = New ElementText(layer)
                                parseStandard(element, xml)
                                element.SetFromXML(xml)

                                Return element
                            Case IElement.TypeConstants.Marquee
                                Dim element As ElementMarquee = New ElementMarquee(layer)
                                parseStandard(element, xml)
                                element.SetFromXML(xml)

                                Return element
                            Case IElement.TypeConstants.Image
                                Dim element As ElementImage = New ElementImage(layer)
                                parseStandard(element, xml)
                                element.SetFromXML(xml)

                                Return element
                            Case IElement.TypeConstants.Video
                                Dim element As ElementVideo = New ElementVideo(layer)
                                parseStandard(element, xml)
                                element.SetFromXML(xml)

                                Return element
                            Case IElement.TypeConstants.Clock
                                Dim element As ElementClock = New ElementClock(layer)
                                parseStandard(element, xml)
                                element.SetFromXML(xml)

                                Return element
                            Case IElement.TypeConstants.Audio
                                Dim element As ElementAudio = New ElementAudio(layer)
                                parseStandard(element, xml)
                                element.SetFromXML(xml)

                                Return element
                            Case IElement.TypeConstants.Shape
                                Dim element As ElementShape = New ElementShape(layer)
                                parseStandard(element, xml)
                                element.SetFromXML(xml)

                                Return element
                            Case IElement.TypeConstants.Timer
                                Dim element As ElementTimer = New ElementTimer(layer)
                                parseStandard(element, xml)
                                element.SetFromXML(xml)

                                Return element
                            Case IElement.TypeConstants.ImageSequence
                                Dim element As ElementImageSequence = New ElementImageSequence(layer)
                                parseStandard(element, xml)
                                element.SetFromXML(xml)

                                Return element
                        End Select
                    End If

                    Return Nothing
                End Function

                Public Shared Function XMLToFont(xml As XElement) As Font
                    Dim fs As FontStyle = FontStyle.Regular
                    [Enum].TryParse(xml.<style>.Value, fs)

                    Return New Font(xml.<family>.Value, xml.<size>.Value, fs, GraphicsUnit.Point)
                End Function

                Public Shared Function ParseString(Of T)(value As String) As T
                    Dim type As Type = GetType(T)

                    Dim removeText = Function(text As String)
                                         Dim newText As String = ""
                                         For i As Integer = 0 To value.Length - 1
                                             If value(i) = "." OrElse value(i) = "," OrElse value(i) = "-" OrElse IsNumeric(value(i)) Then
                                                 newText += value(i)
                                             End If
                                         Next
                                         Return newText
                                     End Function

                    Select Case type
                        Case GetType(Point), GetType(Rectangle), GetType(Padding), GetType(Size)
                            value = removeText(value)
                        Case GetType(Boolean)
                            If value Is Nothing Then value = "False"
                        Case Else
                            Throw New Exception(String.Format("Unsupported Type: {0}", type.FullName))
                    End Select

                    Return TypeDescriptor.GetConverter(type).ConvertFromInvariantString(value)
                End Function

                Public Function Clone() As Object Implements ICloneable.Clone
                    Dim newInstance As Reflection.MethodInfo =
                                GetType(Element).GetMethod("MemberwiseClone",
                                                                 Reflection.BindingFlags.Instance Or
                                                                 Reflection.BindingFlags.NonPublic)
                    If newInstance IsNot Nothing Then
                        Dim e As Element = newInstance.Invoke(Me, Nothing)
                        Return e
                    Else
                        Return Nothing
                    End If

                    'Dim f As BinaryFormatter = New BinaryFormatter()
                    'Dim ms As IO.MemoryStream

                    'ms = New IO.MemoryStream()
                    'f.Serialize(ms, Me)
                    'ms.Seek(0, IO.SeekOrigin.Begin)

                    'Dim element As Element = f.Deserialize(ms)
                    'ms.Dispose()

                    'Return element
                End Function
            End Class
        End Class
    End Class
End Class

Public Class FilteredFileNameEditor
    Inherits UITypeEditor

    Public Overrides Function GetEditStyle(context As ITypeDescriptorContext) As UITypeEditorEditStyle
        Return UITypeEditorEditStyle.Modal
    End Function

    Public Overrides Function EditValue(context As ITypeDescriptorContext, ByVal provider As IServiceProvider, ByVal value As Object) As Object
        Dim fileName As String = Project.Track.Layer.Element.SelectSourceFile(CType(value, String), CType(context.Instance, Project.Track.Layer.Element).Type)

        Return If(fileName = "", MyBase.EditValue(context, provider, value), fileName)
    End Function
End Class

