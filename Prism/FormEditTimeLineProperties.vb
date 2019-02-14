Public Class FormEditTimeLineProperties
    Private mTimeLineStyle As RenderTimeline.TimeLineStyle

    Public Property TimeLineControl As RenderTimeline.TimeLineStyle
        Get
            Return mTimeLineStyle
        End Get
        Set(ByVal value As RenderTimeline.TimeLineStyle)
            mTimeLineStyle = value
            PropertyGridElement.SelectedObject = mTimeLineStyle
        End Set
    End Property
End Class