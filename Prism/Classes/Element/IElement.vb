Imports System.ComponentModel
Imports Prism.Project.Track.Layer
Imports System.Collections.ObjectModel

Partial Public Class Project
    Partial Public Class Track
        Partial Public Class Layer
            Public Interface IElement
                Enum TypeConstants
                    Image       ' * Image File
                    Text        ' * Some static text
                    Marquee     ' * Scrolling text
                    Clock       ' * Clock with or without date
                    Video       ' * A Video File
                    Audio       ' * An Audio File
                    Shape       ' * A Square, rectangle, triangle, circle, etc...
                    Timer       ' * A timer that counts upwards or downwards (YYYY:MM:DD HH:MM:SS:ms)
                    ImageSequence
                    WaitForKey  ' Pauses the current layer until a key if pressed
                    Action      ' Executes a special Action
                End Enum

                <Serializable(),
                 TypeConverter(GetType(FadeEffectConverter))>
                Class FadeEffect
                    Public Enum TypeConstants
                        Instant
                        Fade
                    End Enum

                    Public Enum ModeConstants
                        FadeIn
                        FadeOut
                    End Enum

                    Private mMode As ModeConstants

                    Public Property Duration As TimeSpan = New TimeSpan(0)
                    Public Property Type As TypeConstants
                    Public ReadOnly Property Mode As ModeConstants
                        Get
                            Return mMode
                        End Get
                    End Property

                    Public Sub New(duration As TimeSpan, type As TypeConstants, mode As ModeConstants)
                        Me.Duration = duration
                        Me.Type = type
                        mMode = mode
                    End Sub

                    Public Sub New(duration As Single, type As TypeConstants, mode As ModeConstants)
                        Me.New(New TimeSpan(0, 0, 0, 0, duration * 1000), type, mode)
                    End Sub

                    Public Function ToXML() As XElement
                        Select Case mMode
                            Case ModeConstants.FadeIn
                                Return <fadeIn>
                                           <type><%= Me.Type %></type>
                                           <mode><%= Me.Mode %></mode>
                                           <duration><%= Me.Duration.TotalMilliseconds %></duration>
                                       </fadeIn>
                            Case Else
                                Return <fadeOut>
                                           <type><%= Me.Type %></type>
                                           <mode><%= Me.Mode %></mode>
                                           <duration><%= Me.Duration.TotalMilliseconds %></duration>
                                       </fadeOut>
                        End Select
                    End Function

                    Public Shared Function FromXML(xml As XElement) As FadeEffect
                        Dim type As TypeConstants = TypeConstants.Instant
                        [Enum].TryParse(xml.<type>.Value, type)

                        Dim mode As ModeConstants = ModeConstants.FadeIn
                        [Enum].TryParse(xml.<mode>.Value, mode)

                        Return New FadeEffect(TimeSpan.FromMilliseconds(xml.<duration>.Value), type, mode)
                    End Function
                End Class

                Enum MediaPlaybackStateConstants
                    Idle
                    Playing
                    Paused
                End Enum

                Structure Level
                    Public LeftChannel As Integer
                    Public RightChannel As Integer
                End Structure

                ReadOnly Property Name As String            ' Name of the Element
                ReadOnly Property Type As TypeConstants     ' Type of Element
                Property Description As String
                Property Duration As TimeSpan               ' Length of time for which the element will be visible (if applies)
                ReadOnly Property EndTime As TimeSpan
                Property SourceFile() As String               ' Source file of the element (if applies)
                Property FadeIn As FadeEffect
                Property FadeOut As FadeEffect
                Property Layer As Layer
                Property KeyFrames As ObservableCollection(Of Element.KeyFrame)
                Property InitialKeyFrame As Element.KeyFrame
                Property LoopFrom As TimeSpan
                Property LoopTo As TimeSpan
                Property LoopEnabled As Boolean
                Property Position(Optional ByVal updateFrame As Boolean = False) As TimeSpan
                ReadOnly Property MediaState As MediaPlaybackStateConstants
                ReadOnly Property IsValid As Boolean
                ReadOnly Property Levels(ByVal time As TimeSpan) As Level
                ReadOnly Property TypeIsMedia As Boolean
                Function GetBlendedKeyFrame() As Element.KeyFrame
                Function GetBlendedKeyFrame(ByVal time As TimeSpan) As Element.KeyFrame
                Sub ClearBlendedKeyFramesCache()
                Sub ClearBlendedKeyFramesCache(ByVal time As TimeSpan)
                Sub ApplyAlpha(ByVal alpha As Integer)
                Sub ResetImageAlpha()
                Sub MediaPlay()
                Sub MediaPause()
                Sub MediaStop()
                Function Render(ByVal g As Graphics, ByVal r As Rectangle, ByVal m As Drawing2D.Matrix) As Element.KeyFrame
                Function RenderBackground(ByVal g As Graphics, ByVal m As Drawing2D.Matrix) As Element.KeyFrame
                Function AlignContent(ByVal r As Rectangle, ByVal p As Padding, ByVal contentSize As Size, ByVal alignment As ContentAlignment) As Rectangle
                Function GetClosestKeyFrame() As Element.KeyFrame
                Function GetClosestKeyFrame(ByVal time As TimeSpan) As Element.KeyFrame
                Function ToXML() As XElement
                Sub SetFromXML(ByVal xml As XElement)
                Event KeyFramesChanged(ByVal sender As Element)
            End Interface
        End Class
    End Class
End Class

Public Class FadeEffectConverter
    Inherits ExpandableObjectConverter

    Public Overrides Function CanConvertTo(context As ITypeDescriptorContext, destinationType As Type) As Boolean
        Return If(destinationType Is GetType(IElement.FadeEffect), True, MyBase.CanConvertTo(context, destinationType))
    End Function

    Public Overrides Function ConvertTo(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object, destinationType As Type) As Object
        If destinationType Is GetType(String) AndAlso TypeOf value Is IElement.FadeEffect Then
            Dim so As IElement.FadeEffect = CType(value, IElement.FadeEffect)

            Return "Duration: " + so.Duration.ToString() +
              ", Type: " + so.Type.ToString() +
              ", Mode: " + so.Mode.ToString()
        End If

        Return MyBase.ConvertTo(context, culture, value, destinationType)
    End Function
End Class