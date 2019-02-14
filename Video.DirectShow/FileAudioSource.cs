using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AForge.Video.DirectShow {
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Threading;
    using System.Runtime.InteropServices;

    using AForge.Video;
    using AForge.Video.DirectShow.Internals;

    public class FileAudioSource {
        // audio file name
        private string fileName;
        // received frames count
        private int framesReceived;
        // received byte count
        private int bytesReceived;
        // reference clock for the graph - when disabled, graph processes frames ASAP
        private bool referenceClockEnabled = true;
        private bool isValid = false;
        private bool useNullRenderer = false;

        private WaveFormatEx wavFormat;

        object graphObject = null;
        object grabberObjectAudio = null;
        object nullRendererObjectAudio = null;

        IGraphBuilder graph = null;
        IBaseFilter sourceBase = null;
        IBaseFilter grabberBaseAudio = null;
        ISampleGrabber sampleGrabberAudio = null;
        IMediaEventEx mediaEvent = null;
        IMediaControl mediaControl = null;
        IMediaSeeking mediaSeekControl = null;
        IBasicAudio basicAudio = null;

        GrabberAudio grabberAudio = null;

        private bool isPaused = false;

        private Thread thread = null;
        private ManualResetEvent stopEvent = null;

        public event NewAudioFrameEventHandler NewFrame;

        public event AudioSourceErrorEventHandler AudioSourceError;

        public event PlayingFinishedEventHandler PlayingFinished;

        public FileAudioSource() { }

        public FileAudioSource(string fileName) {
            Source = fileName;
        }

        public FileAudioSource(string fileName, bool useNullRenderer) {
            this.useNullRenderer = useNullRenderer;
            this.referenceClockEnabled = !useNullRenderer;
            Source = fileName;
        }

        public virtual string Source {
            get { return fileName; }
            set {
                fileName = value;
                CreateFilters(MediaAudioSubType.MEDIASUBTYPE_PCM);
                if(!isValid) CreateFilters(MediaAudioSubType.MEDIASUBTYPE_MPEG2_AUDIO);
            }
        }

        public long Position {
            get {
                if(mediaSeekControl == null) return 0;

                mediaSeekControl.GetCurrentPosition(out long position);
                return position;
            }
            set {
                if(mediaSeekControl == null) return;

                if(IsRunning) {
                    mediaSeekControl.SetPositions(value, SeekingFlags.AbsolutePositioning, 0, SeekingFlags.NoPositioning);
                } else {
                    try {
                        // Seek to the desired position
                        mediaSeekControl.SetPositions(value, SeekingFlags.AbsolutePositioning, 0, SeekingFlags.NoPositioning);
                    } catch(Exception exception) {
                        // provide information to clients
                        AudioSourceError?.Invoke(this, new AudioSourceErrorEventArgs(exception.Message));
                    }
                }
            }
        }

        public void SetPositionAsync(long position) {
            if(mediaSeekControl == null) return;
            mediaSeekControl.SetPositions(position, SeekingFlags.AbsolutePositioning, 0, SeekingFlags.NoPositioning);
        }

        public long Duration {
            get {
                long duration = 0;
                if(mediaSeekControl != null) mediaSeekControl.GetDuration(out duration);
                return duration;
            }
        }

        // -10,000 - 0
        public int Volume {
            get {
                int vol = 0;
                if(basicAudio != null) basicAudio.get_Volume(out vol);
                return vol;
            }
            set {
                if(basicAudio != null && value >= -10000 && value <= 0) basicAudio.put_Volume(value);
            }
        }

        // -10,000 - 10,000
        public int Balance {
            get {
                int bal = 0;
                if(basicAudio != null) basicAudio.get_Balance(out bal);
                return bal;
            }
            set {
                if(basicAudio != null && value >= -10000 && value <= 10000) basicAudio.put_Balance(value);
            }
        }

        public bool IsRunning {
            get {
                if(thread != null) {
                    // check thread status
                    if(thread.Join(0) == false)
                        return true;

                    // the thread is not running, free resources
                    Free();
                }
                return false;
            }
        }

        public bool IsValid {
            get { return isValid; }
        }

        /// <summary>
        /// Enables/disables reference clock on the graph.
        /// </summary>
        /// 
        /// <remarks><para>Disabling reference clocks causes DirectShow graph to run as fast as
        /// it can process data. When enabled, it will process frames according to presentation
        /// time of a video file.</para>
        /// 
        /// <para><note>The property should be set before calling <see cref="Start"/> method
        /// of the class to have effect.</note></para>
        /// 
        /// <para>Default value of this property is set to <b>true</b>.</para>
        /// </remarks>
        /// 
        public bool ReferenceClockEnabled {
            get { return referenceClockEnabled; }
            set { referenceClockEnabled = value; }
        }

        public bool UseNullRenderer {
            get { return useNullRenderer; }
        }

        /// <summary>
        /// Start video source.
        /// </summary>
        /// 
        /// <remarks>Starts video source and return execution to caller. Video source
        /// object creates background thread and notifies about new frames with the
        /// help of <see cref="NewFrame"/> event.</remarks>
        /// 
        public void Start() {
            if(isPaused) {
                Pause();
            } else {
                if(!IsRunning) {
                    // check source
                    if((fileName == null) || (fileName == string.Empty))
                        throw new ArgumentException("Audio source is not specified");

                    framesReceived = 0;
                    bytesReceived = 0;

                    // create events
                    stopEvent = new ManualResetEvent(false);

                    // create and start new thread
                    thread = new Thread(new ThreadStart(WorkerThread)) {
                        Name = fileName // mainly for debugging
                    };
                    thread.Start();
                }
            }
        }

        public void Pause() {
            if(mediaControl != null && IsRunning) {
                if(isPaused)
                    mediaControl.Run();
                else
                    mediaControl.Pause();
                isPaused = !isPaused;
            }
        }

        public void SignalToStop() {
            // stop thread
            if(thread != null) {
                // signal to stop
                stopEvent.Set();
            }
        }

        public void WaitForStop() {
            if(thread != null) {
                // wait for thread stop
                thread.Join();
                Free();
            }
        }

        public void Stop() {
            isPaused = false;

            if(this.IsRunning) {
                thread.Abort();
                WaitForStop();
            }
        }

        public WaveFormatEx AudioFormat {
            get { return wavFormat; }
        }

        private void Free() {
            thread = null;

            // release events
            stopEvent.Close();
            stopEvent = null;
        }

        private void CreateFilters(Guid audioSubType) {
            isValid = false;
            int r;

            // grabber
            grabberAudio = new GrabberAudio(this);

            // objects
            graphObject = null;
            grabberObjectAudio = null;

            try {
                // get type for filter graph
                Type type = Type.GetTypeFromCLSID(Clsid.FilterGraph);
                if(type == null)
                    throw new ApplicationException("Failed creating filter graph");

                // create filter graph
                graphObject = Activator.CreateInstance(type);
                graph = (IGraphBuilder)graphObject;

                // create source device's object
                r = graph.AddSourceFilter(fileName, "source", out sourceBase);
                if(sourceBase == null)
                    throw new ApplicationException("Failed creating source filter");

                // get type for sample grabber
                type = Type.GetTypeFromCLSID(Clsid.SampleGrabber);
                if(type == null)
                    throw new ApplicationException("Failed creating sample grabber");

                // create sample grabber
                grabberObjectAudio = Activator.CreateInstance(type);
                sampleGrabberAudio = (ISampleGrabber)grabberObjectAudio;
                grabberBaseAudio = (IBaseFilter)grabberObjectAudio;

                // add grabber filters to graph
                r = graph.AddFilter(grabberBaseAudio, "grabberAudio");

                // set media type
                AMMediaType mediaType = new AMMediaType {
                    MajorType = MediaType.Audio,
                    SubType = audioSubType,
                    FormatType = FormatType.WaveEx
                };
                r = sampleGrabberAudio.SetMediaType(mediaType);

                // render pin
                // TODO: Improve this! We can't always assume that the second pin will always be the audio pin -- we need to find it.
                IPin sbPin = Tools.GetOutPin(sourceBase, 1);
                if(sbPin == null) sbPin = Tools.GetOutPin(sourceBase, 0);
                r = graph.Render(sbPin);

                IPin outPin = Tools.GetOutPin(grabberBaseAudio, 0);
                AMMediaType mt = new AMMediaType();
                r = outPin.ConnectionMediaType(mt);
                if(!Tools.IsPinConnected(outPin))
                    throw new ApplicationException("Failed obtaining media information");

                // disable clock, if someone requested it
                if(!referenceClockEnabled) {
                    IMediaFilter mediaFilter = (IMediaFilter)graphObject;
                    r = mediaFilter.SetSyncSource(null);
                }

                wavFormat = new WaveFormatEx();
                Marshal.PtrToStructure(mt.FormatPtr, wavFormat);
                Marshal.ReleaseComObject(outPin);

                // configure sample grabber
                r = sampleGrabberAudio.SetBufferSamples(false);
                r = sampleGrabberAudio.SetOneShot(false);
                r = sampleGrabberAudio.SetCallback(grabberAudio, 1);

                if(useNullRenderer) {
                    // Get a list of all the filters connected to the sample grabber
                    List<Tools.FilterInfo2> filtersInfo2 = new List<Tools.FilterInfo2>();
                    Tools.FilterInfo2 testFilterInfo2 = Tools.GetNextFilter(grabberBaseAudio, PinDirection.Output, 0);
                    while(true) {
                        filtersInfo2.Add(testFilterInfo2);
                        testFilterInfo2 = Tools.GetNextFilter(testFilterInfo2.Filter, PinDirection.Output, 0);
                        if(testFilterInfo2.Filter == null) break;
                    }
                    // Remove the last filter, the audio renderer
                    r = graph.RemoveFilter(filtersInfo2[filtersInfo2.Count - 1].Filter);

                    // create null renderer
                    type = Type.GetTypeFromCLSID(Clsid.NullRenderer);
                    if(type == null)
                        throw new ApplicationException("Failed creating null renderer");

                    nullRendererObjectAudio = Activator.CreateInstance(type);
                    IBaseFilter nullRendererAudio = (IBaseFilter)nullRendererObjectAudio;

                    // add grabber filters to graph
                    r = graph.AddFilter(nullRendererAudio, "nullRenderer");

                    //outPin = Tools.GetOutPin(filtersInfo2[filtersInfo2.Count - 2].Filter, 0);
                    outPin = Tools.GetOutPin(grabberBaseAudio, 0);
                    IPin inPin = Tools.GetInPin(nullRendererAudio, 0);
                    if(graph.Connect(outPin, inPin) < 0)
                        throw new ApplicationException("Failed obtaining media audio information");
                    Marshal.ReleaseComObject(outPin);
                    Marshal.ReleaseComObject(inPin);
                }

                // configure video window
                IVideoWindow window = (IVideoWindow)graphObject;
                if(window != null) {
                    window.put_AutoShow(false);
                    window = null;
                }

                // get media control
                mediaControl = (IMediaControl)graphObject;

                // get media seek control
                mediaSeekControl = (IMediaSeeking)graphObject;
                mediaSeekControl.SetTimeFormat(TimeFormat.MediaTime);

                // get media events' interface
                mediaEvent = (IMediaEventEx)graphObject;

                // get media audio control
                basicAudio = (IBasicAudio)graphObject;

                isValid = true;
            } catch(Exception exception) {
                DestroyFilters();

                // provide information to clients
                AudioSourceError?.Invoke(this, new AudioSourceErrorEventArgs(exception.Message));
            }
        }

        private void DestroyFilters() {
            isValid = false;
            if(mediaControl != null) mediaControl.Stop();

            // release all objects
            graph = null;
            mediaControl = null;
            mediaEvent = null;
            mediaSeekControl = null;

            if(graphObject != null) {
                Marshal.ReleaseComObject(graphObject);
                graphObject = null;
            }
            if(sourceBase != null) {
                Marshal.ReleaseComObject(sourceBase);
                sourceBase = null;
            }
            if(grabberObjectAudio != null) {
                Marshal.ReleaseComObject(grabberObjectAudio);
                grabberObjectAudio = null;
            }
            if(nullRendererObjectAudio != null) {
                Marshal.ReleaseComObject(nullRendererObjectAudio);
                grabberObjectAudio = null;
            }
        }

        /// <summary>
        /// Worker thread.
        /// </summary>
        /// 
        private void WorkerThread() {
            ReasonToFinishPlaying reasonToStop = ReasonToFinishPlaying.StoppedByUser;

            try {
                // run
                mediaControl.Run();

                while(!stopEvent.WaitOne(0, true)) {
                    Thread.Sleep(100);

                    if(mediaEvent != null) {
                        if(mediaEvent.GetEvent(out DsEvCode code, out IntPtr p1, out IntPtr p2, 0) >= 0) {
                            mediaEvent.FreeEventParams(code, p1, p2);

                            if(code == DsEvCode.Complete) {
                                reasonToStop = ReasonToFinishPlaying.EndOfStreamReached;
                                break;
                            }
                        }
                    }
                }
                mediaControl.StopWhenReady();
            } catch(Exception exception) {
                // provide information to clients
                AudioSourceError?.Invoke(this, new AudioSourceErrorEventArgs(exception.Message));
            } finally {
                DestroyFilters();
            }

            PlayingFinished?.Invoke(this, reasonToStop);
        }

        protected void OnNewAudioFrame(short[] audio, int bufferLen, double sampleTime) {
            if(IsRunning) {
                framesReceived++;
                if((!stopEvent.WaitOne(0, true)) && (NewFrame != null))
                    NewFrame(this, new NewAudioFrameEventArgs(audio, bufferLen, sampleTime));
            } else {
                NewFrame?.Invoke(this, new NewAudioFrameEventArgs(audio, bufferLen, sampleTime));
            }
        }

        private class GrabberAudio : ISampleGrabberCB {
            private FileAudioSource parent;
            private readonly short[] savedArray = new short[96000];

            // Constructor
            public GrabberAudio(FileAudioSource parent) {
                this.parent = parent;
            }

            // Callback to receive samples
            public int SampleCB(double sampleTime, IntPtr sample) {
                return 0;
            }

            // Callback method that receives a pointer to the sample buffer
            public int BufferCB(double sampleTime, IntPtr buffer, int bufferLen) {
                if(parent.NewFrame != null) {
                    bufferLen /= 2;
                    if(bufferLen < this.savedArray.Length) {
                        Marshal.Copy(buffer, this.savedArray, 0, bufferLen);

                        // notify parent
                        parent.OnNewAudioFrame(this.savedArray, bufferLen, sampleTime);
                    }
                }

                return 0;
            }
        }
    }
}

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public class WaveFormatEx {
    public ushort wFormatTag;
    public ushort nChannels;
    public uint nSamplesPerSec;
    public uint nAvgBytesPerSec;
    public ushort nBlockAlign;
    public ushort wBitsPerSample;
    public ushort cbSize;
}
