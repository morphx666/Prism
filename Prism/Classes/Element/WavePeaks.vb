Imports System.Runtime.Serialization.Formatters.Binary

<Serializable()>
Public Class WavePeaks
    <Serializable()>
    Public Structure Peak
        Public Left As Short
        Public Right As Short
        Public Time As TimeSpan

        Public Sub New(ByVal time As TimeSpan, ByVal left As Integer, ByVal right As Integer)
            Me.Time = time
            Me.Left = left
            Me.Right = right
        End Sub
    End Structure

    Private mPeaks As List(Of Peak) = New List(Of Peak)
    Private mMedia As AForge.Video.DirectShow.FileAudioSource

    Public Event Progress(ByVal sender As WavePeaks, ByVal progress As Integer)
    Public Event DoneProcessing(ByVal sender As WavePeaks, ByVal peaks As List(Of Peak))

    Public Sub New()
    End Sub

    Public Sub New(fileName As String)
        Me.FileName = fileName
    End Sub

    Public ReadOnly Property Peaks As List(Of Peak)
        Get
            Return mPeaks
        End Get
    End Property

    Public Property FileName As String
    Public Property PeaksFileName As String

    Public Sub ProcessAudioFile()
        mPeaks.Clear()

        If IO.File.Exists(FileName) Then
            Dim fi As IO.FileInfo = New IO.FileInfo(FileName)
            PeaksFileName = FileName.Replace(fi.Extension, ".ppf")

            If IO.File.Exists(PeaksFileName) Then
                ReadPeaksFile()
                RaiseEvent DoneProcessing(Me, mPeaks)
            Else
                Try
                    mMedia = New AForge.Video.DirectShow.FileAudioSource(FileName, True)

                    If mMedia.IsValid Then
                        AddHandler mMedia.NewFrame, AddressOf NewAudioFrame
                        AddHandler mMedia.PlayingFinished, Sub(sender As Object, e As AForge.Video.ReasonToFinishPlaying)
                                                               WritePeaksFile()
                                                               mMedia.Stop()
                                                               RaiseEvent DoneProcessing(Me, mPeaks)
                                                           End Sub

                        mMedia.Start()
                    End If
                Catch
                End Try
            End If
        End If
    End Sub

    Private Sub ReadPeaksFile()
        Dim buffer() As Byte = DeCompress(IO.File.ReadAllBytes(PeaksFileName))
        Dim f As BinaryFormatter = New BinaryFormatter()
        Using ms As IO.MemoryStream = New IO.MemoryStream(buffer)
            mPeaks = CType(f.Deserialize(ms), List(Of Peak))
        End Using
    End Sub

    Private Sub WritePeaksFile()
        Dim f As BinaryFormatter = New BinaryFormatter()
        Using ms As IO.MemoryStream = New IO.MemoryStream()
            f.Serialize(ms, mPeaks)
            IO.File.WriteAllBytes(PeaksFileName, Compress(ms))
        End Using
    End Sub

    Private Function Compress(sourceStream As IO.MemoryStream) As Byte()
        Dim sourceBuffer() As Byte = sourceStream.GetBuffer()

        Dim ms As IO.MemoryStream = New IO.MemoryStream()
        Using gZip As IO.Compression.GZipStream = New IO.Compression.GZipStream(ms, IO.Compression.CompressionMode.Compress, True)
            gZip.Write(sourceBuffer, 0, sourceBuffer.Length)
        End Using

        ms.Position = 0
        Dim buffer(ms.Length - 1) As Byte
        ms.Read(buffer, 0, ms.Length)
        ms.Dispose()

        Return buffer
    End Function

    Private Function DeCompress(sourceBuffer() As Byte) As Byte()
        Dim ms As IO.MemoryStream = New IO.MemoryStream(sourceBuffer)
        Dim gZip As IO.Compression.GZipStream = New IO.Compression.GZipStream(ms, IO.Compression.CompressionMode.Decompress, True)

        Dim tmpBuffer(3) As Byte
        ms.Position = ms.Length - 4
        ms.Read(tmpBuffer, 0, 4)
        Dim size As Integer = BitConverter.ToInt32(tmpBuffer, 0)
        ms.Position = 0

        Dim decompressedBuffer(size - 1) As Byte
        gZip.Read(decompressedBuffer, 0, size)

        gZip.Dispose()
        ms.Dispose()

        Return decompressedBuffer
    End Function

    Private Sub NewAudioFrame(sender As Object, e As AForge.Video.NewAudioFrameEventArgs)
        ' 1 second        -> mMedia.AudioFormat.nAvgBytesPerSec bytes
        ' secondsInBuffer -> bytesInBuffer

        ' secondsInBuffer = bytesInBuffer / mMedia.AudioFormat.nAvgBytesPerSec

        Dim channels As Integer = mMedia.AudioFormat.nChannels
        Dim l As Short
        Dim r As Short
        Dim inc As Integer = channels * 1
        Dim bytesInBuffer As Integer = e.BufferLength - channels
        Dim secondsInBuffer As Double = bytesInBuffer / mMedia.AudioFormat.nAvgBytesPerSec
        Dim curTime As TimeSpan
        Dim lastTime As TimeSpan = New TimeSpan(0, 0, -1)
        Dim timeResolution As TimeSpan = TimeSpan.FromMilliseconds(5)

        For i As Integer = 0 To bytesInBuffer Step inc
            l = e.Frame(i)
            Select Case channels
                Case 1 : r = l
                Case Else : r = e.Frame(i + 1)
            End Select

            curTime = TimeSpan.FromSeconds(e.Time + i / bytesInBuffer * secondsInBuffer)
            If (curTime - lastTime) >= timeResolution Then
                lastTime = curTime
                mPeaks.Add(New Peak(curTime, l, r))
            End If
        Next
    End Sub
End Class