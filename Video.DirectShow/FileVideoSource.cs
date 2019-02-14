// AForge Direct Show Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2011
// contacts@aforgenet.com
//

namespace AForge.Video.DirectShow {
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Threading;
    using System.Runtime.InteropServices;

    using AForge.Video;
    using AForge.Video.DirectShow.Internals;

    /// <summary>
    /// Video source for video files.
    /// </summary>
    /// 
    /// <remarks><para>The video source provides access to video files. DirectShow is used to access video
    /// files.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create video source
    /// FileVideoSource videoSource = new FileVideoSource( fileName );
    /// // set NewFrame event handler
    /// videoSource.NewFrame += new NewFrameEventHandler( video_NewFrame );
    /// // start the video source
    /// videoSource.Start( );
    /// // ...
    /// // signal to stop
    /// videoSource.SignalToStop( );
    /// // ...
    /// 
    /// // New frame event handler, which is invoked on each new available video frame
    /// private void video_NewFrame( object sender, NewFrameEventArgs eventArgs )
    /// {
    ///     // get new frame
    ///     Bitmap bitmap = eventArgs.Frame;
    ///     // process the frame
    /// }
    /// </code>
    /// </remarks>
    /// 
    public class FileVideoSource : IVideoSource {
        // video file name
        private string fileName;
        // received frames count
        private int framesReceived;
        // recieved byte count
        private long bytesReceived;
        // prevent freezing
        private bool preventFreezing = false;
        // reference clock for the graph - when disabled, graph processes frames ASAP
        private bool referenceClockEnabled = true;
        private bool isValid = false;

        private WaveFormatEx wavFormat;

        object graphObject = null;
        object grabberObjectVideo = null;
        object grabberObjectAudio = null;

        IGraphBuilder graph = null;
        IBaseFilter sourceBase = null;
        IBaseFilter grabberBaseVideo = null;
        ISampleGrabber sampleGrabberVideo = null;
        IBaseFilter grabberBaseAudio = null;
        ISampleGrabber sampleGrabberAudio = null;
        IMediaEventEx mediaEvent = null;
        IMediaControl mediaControl = null;
        IMediaSeeking mediaSeekControl = null;
        IBasicAudio basicAudio = null;

        GrabberVideo grabberVideo = null;
        GrabberAudio grabberAudio = null;

        private bool isPaused = false;
        private bool useAudioGrabber = true;

        private Thread thread = null;
        private ManualResetEvent stopEvent = null;

        public event NewAudioFrameEventHandler NewAudioFrame;
        public event AudioSourceErrorEventHandler AudioSourceError;

        /// <summary>
        /// New frame event.
        /// </summary>
        /// 
        /// <remarks><para>Notifies clients about new available frame from video source.</para>
        /// 
        /// <para><note>Since video source may have multiple clients, each client is responsible for
        /// making a copy (cloning) of the passed video frame, because the video source disposes its
        /// own original copy after notifying of clients.</note></para>
        /// </remarks>
        /// 
        public event NewFrameEventHandler NewFrame;

        /// <summary>
        /// Video source error event.
        /// </summary>
        /// 
        /// <remarks>This event is used to notify clients about any type of errors occurred in
        /// video source object, for example internal exceptions.</remarks>
        /// 
        public event VideoSourceErrorEventHandler VideoSourceError;

        /// <summary>
        /// Video playing finished event.
        /// </summary>
        /// 
        /// <remarks><para>This event is used to notify clients that the video playing has finished.</para>
        /// </remarks>
        /// 
        public event PlayingFinishedEventHandler PlayingFinished;

        /// <summary>
        /// Video source.
        /// </summary>
        /// 
        /// <remarks>Video source is represented by video file name.</remarks>
        /// 
        public virtual string Source {
            get { return fileName; }
            set {
                fileName = value;
                CreateFilters();
            }
        }

        /// <summary>
        /// Received frames count.
        /// </summary>
        /// 
        /// <remarks>Number of frames the video source provided from the moment of the last
        /// access to the property.
        /// </remarks>
        /// 
        public int FramesReceived {
            get {
                int frames = framesReceived;
                framesReceived = 0;
                return frames;
            }
        }

        public bool UseAudioGrabber {
            get {
                return useAudioGrabber;
            }
            set {
                useAudioGrabber = value;
                CreateFilters();
            }
        }

        /// <summary>
        /// Received bytes count.
        /// </summary>
        /// 
        /// <remarks>Number of bytes the video source provided from the moment of the last
        /// access to the property.
        /// </remarks>
        /// 
        public long BytesReceived {
            get {
                long bytes = bytesReceived;
                bytesReceived = 0;
                return bytes;
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
                mediaSeekControl.SetPositions(value, SeekingFlags.AbsolutePositioning, 0, SeekingFlags.NoPositioning);

                /*
                if (IsRunning)
                {
                    mediaSeekControl.SetPositions(value, SeekingFlags.AbsolutePositioning, 0, SeekingFlags.NoPositioning);
                }
                else
                {
                    try
                    {
                        int code;
                        IntPtr buffer = IntPtr.Zero;
                        int bufferLen = 0;
                        int hr;

                        // Force new samples to be buffered
                        hr = sampleGrabberVideo.SetBufferSamples(true);
                        Marshal.ThrowExceptionForHR(hr);

                        // Halt the grabber after the frame has been processed
                        hr = sampleGrabberVideo.SetOneShot(true);
                        Marshal.ThrowExceptionForHR(hr);

                        // Seek to the desired position
                        mediaSeekControl.SetPositions(value, SeekingFlags.AbsolutePositioning, 0, SeekingFlags.NoPositioning);

                        // Start the filer to that the frame is processed
                        mediaControl.Run();
                        mediaEvent.WaitForCompletion(200, out code);
                        if (code > 1) return;

                        // The new frame should have already been processed so we can pause the filter
                        mediaControl.Stop();

                        // Get the buffer size
                        hr = sampleGrabberVideo.GetCurrentBuffer(ref bufferLen, buffer);
                        Marshal.ThrowExceptionForHR(hr);

                        // Allocate the necessary memory to hold the bitmap
                        buffer = Marshal.AllocCoTaskMem(bufferLen);

                        // Get the pointer to the bitmap
                        sampleGrabberVideo.GetCurrentBuffer(ref bufferLen, buffer);
                        Marshal.ThrowExceptionForHR(hr);

                        // Force the rendering of the bitmap
                        grabberVideo.BufferCB(0, buffer, bufferLen);

                        sampleGrabberVideo.SetBufferSamples(false);
                        sampleGrabberVideo.SetOneShot(false);
                    }
                    catch (Exception exception)
                    {
                        // provide information to clients
                        if (VideoSourceError != null)
                        {
                            VideoSourceError(this, new VideoSourceErrorEventArgs(exception.Message));
                        }
                    }
                }
                */
            }
        }

        long quedPosition = 0;
        public void SetPositionAsync(long position) {
            if(mediaSeekControl == null) return;

            quedPosition = position;
            Thread sp = new Thread(SetPositionAsyncThread);
            sp.Start();
        }

        private void SetPositionAsyncThread() {
            if(mediaSeekControl == null) return;

            mediaSeekControl.SetPositions(quedPosition, SeekingFlags.AbsolutePositioning, 0, SeekingFlags.NoPositioning);
            if(!IsRunning) {
                mediaControl.Run();
                mediaControl.Pause();
            }
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

        /// <summary>
        /// State of the video source.
        /// </summary>
        /// 
        /// <remarks>Current state of video source object - running or not.</remarks>
        /// 
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
        /// Prevent video freezing after screen saver and workstation lock or not.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>The value specifies if the class should prevent video freezing during and
        /// after screen saver or workstation lock. To prevent freezing the <i>DirectShow</i> graph
        /// should not contain <i>Renderer</i> filter, which is added by <i>Render()</i> method
        /// of graph. However, in some cases it may be required to call <i>Render()</i> method of graph, since
        /// it may add some more filters, which may be required for playing video. So, the property is
        /// a trade off - it is possible to prevent video freezing skipping adding renderer filter or
        /// it is possible to keep renderer filter, but video may freeze during screen saver.</para>
        /// 
        /// <para><note>The property may become obsolete in the future when approach to disable freezing
        /// and adding all required filters is found.</note></para>
        /// 
        /// <para><note>The property should be set before calling <see cref="Start"/> method
        /// of the class to have effect.</note></para>
        /// 
        /// <para>Default value of this property is set to <b>false</b>.</para>
        /// 
        /// </remarks>
        /// 
        public bool PreventFreezing {
            get { return preventFreezing; }
            set { preventFreezing = value; }
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

        /// <summary>
        /// Initializes a new instance of the <see cref="FileVideoSource"/> class.
        /// </summary>
        /// 
        public FileVideoSource() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileVideoSource"/> class.
        /// </summary>
        /// 
        /// <param name="fileName">Video file name.</param>
        /// 
        public FileVideoSource(string fileName) {
            Source = fileName;
        }

        public FileVideoSource(string fileName, bool ignoreAudioGrabber) {
            useAudioGrabber = !ignoreAudioGrabber;
            Source = fileName;
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
            if(!IsRunning) {
                // check source
                if((fileName == null) || (fileName == string.Empty))
                    throw new ArgumentException("Video source is not specified");

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

        /// <summary>
        /// Pauses/Resumes video source.
        /// </summary>
        /// 
        public void Pause() {
            if(mediaControl != null && IsRunning) {
                if(isPaused)
                    mediaControl.Run();
                else
                    mediaControl.Pause();
                isPaused = !isPaused;
            }
        }

        /// <summary>
        /// Signal video source to stop its work.
        /// </summary>
        /// 
        /// <remarks>Signals video source to stop its background thread, stop to
        /// provide new frames and free resources.</remarks>
        /// 
        public void SignalToStop() {
            // stop thread
            if(thread != null) {
                // signal to stop
                stopEvent.Set();
            }
        }

        /// <summary>
        /// Wait for video source has stopped.
        /// </summary>
        /// 
        /// <remarks>Waits for source stopping after it was signalled to stop using
        /// <see cref="SignalToStop"/> method.</remarks>
        /// 
        public void WaitForStop() {
            if(thread != null) {
                // wait for thread stop
                thread.Join();

                Free();
            }
        }

        /// <summary>
        /// Stop video source.
        /// </summary>
        /// 
        /// <remarks><para>Stops video source aborting its thread.</para>
        /// 
        /// <para><note>Since the method aborts background thread, its usage is highly not preferred
        /// and should be done only if there are no other options. The correct way of stopping camera
        /// is <see cref="SignalToStop">signaling it stop</see> and then
        /// <see cref="WaitForStop">waiting</see> for background thread's completion.</note></para>
        /// </remarks>
        /// 
        public void Stop() {
            if(this.IsRunning) {
                thread.Abort();
                WaitForStop();
            }
        }

        public WaveFormatEx AudioFormat {
            get { return wavFormat; }
        }

        /// <summary>
        /// Free resource.
        /// </summary>
        /// 
        private void Free() {
            thread = null;

            // release events
            stopEvent.Close();
            stopEvent = null;
        }

        /// <summary>
        /// Notifies client about new frame.
        /// </summary>
        /// 
        /// <param name="image">New frame's image.</param>
        /// 
        protected void OnNewVideoFrame(Bitmap image) {
            if(IsRunning) {
                framesReceived++;
                if((!stopEvent.WaitOne(0, true)) && (NewFrame != null))
                    NewFrame(this, new NewFrameEventArgs(image));
            } else {
                if(NewFrame != null)
                    NewFrame(this, new NewFrameEventArgs(image));
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
                VideoSourceError?.Invoke(this, new VideoSourceErrorEventArgs(exception.Message));
            } finally {
                DestroyFilters();
            }

            PlayingFinished?.Invoke(this, reasonToStop);
        }

        private void CreateFilters() {
            isValid = true;

            // grabber
            grabberVideo = new GrabberVideo(this);
            grabberAudio = new GrabberAudio(this);

            // objects
            graphObject = null;
            grabberObjectVideo = null;
            grabberObjectAudio = null;

            int sourceBaseVideoPinIndex = 0;

            try {
                // get type for filter graph
                Type type = Type.GetTypeFromCLSID(Clsid.FilterGraph);
                if(type == null)
                    throw new ApplicationException("Failed creating filter graph");

                // create filter graph
                graphObject = Activator.CreateInstance(type);
                graph = (IGraphBuilder)graphObject;

                // create source device's object
                if(fileName.ToLower().EndsWith(".wmv")) {
                    type = Type.GetTypeFromCLSID(Clsid.WMASFReader);
                    if(type == null)
                        throw new ApplicationException("Failed creating ASF Reader filter");
                    sourceBase = (IBaseFilter)Activator.CreateInstance(type);
                    IFileSourceFilter sourceFile = (IFileSourceFilter)sourceBase;
                    sourceFile.Load(fileName, null);
                    graph.AddFilter(sourceBase, "source");
                    sourceBaseVideoPinIndex = 1;
                } else {
                    graph.AddSourceFilter(fileName, "source", out sourceBase);
                    if(sourceBase == null) {
                        try {
                            type = Type.GetTypeFromCLSID(Clsid.AsyncReader);
                            if(type == null)
                                throw new ApplicationException("Failed creating Async Reader filter");
                            sourceBase = (IBaseFilter)Activator.CreateInstance(type);
                            IFileSourceFilter sourceFile = (IFileSourceFilter)sourceBase;
                            sourceFile.Load(fileName, null);
                            graph.AddFilter(sourceBase, "source");
                        } catch {
                            throw new ApplicationException("Failed creating source filter");
                        }
                    }
                    sourceBaseVideoPinIndex = 0;
                }

                // get type for sample grabber
                type = Type.GetTypeFromCLSID(Clsid.SampleGrabber);
                if(type == null)
                    throw new ApplicationException("Failed creating sample grabber");

                // create sample grabber
                grabberObjectVideo = Activator.CreateInstance(type);
                sampleGrabberVideo = (ISampleGrabber)grabberObjectVideo;
                grabberBaseVideo = (IBaseFilter)grabberObjectVideo;

                // add grabber filters to graph
                graph.AddFilter(grabberBaseVideo, "grabberVideo");

                // set media type
                AMMediaType mediaType = new AMMediaType {
                    MajorType = MediaType.Video,
                    SubType = MediaSubType.ARGB32 /* MediaSubType.RGB24 */
                };
                ;
                sampleGrabberVideo.SetMediaType(mediaType);

                // connect pins
                IPin outPin = Tools.GetOutPin(sourceBase, sourceBaseVideoPinIndex);
                IPin inPin = Tools.GetInPin(grabberBaseVideo, 0);
                if(graph.Connect(outPin, inPin) < 0)
                    throw new ApplicationException("Failed connecting sourceBase to grabberBaseVideo");
                Marshal.ReleaseComObject(outPin);
                Marshal.ReleaseComObject(inPin);

                // get media type
                if(sampleGrabberVideo.GetConnectedMediaType(mediaType) == 0) {
                    VideoInfoHeader vih = (VideoInfoHeader)Marshal.PtrToStructure(mediaType.FormatPtr, typeof(VideoInfoHeader));

                    grabberVideo.Width = vih.BmiHeader.Width;
                    grabberVideo.Height = vih.BmiHeader.Height;
                    mediaType.Dispose();
                }

                if(useAudioGrabber) {

                    // *****************************************************************
                    // ******** Add the audio grabber to monitor audio peaks ***********
                    bool audioGrabberIsConnected = false;
                    Tools.FilterInfo2 filterInfo2 = Tools.GetNextFilter(sourceBase, PinDirection.Output, 0);
                    foreach(Tools.PinInfo2 pinInfo2 in filterInfo2.Pins) {
                        if(pinInfo2.PinInfo.Direction == PinDirection.Output) {
                            if(!Tools.IsPinConnected(pinInfo2.Pin)) {
                                try {
                                    graph.Render(pinInfo2.Pin);

                                    AMMediaType mt = new AMMediaType();
                                    pinInfo2.Pin.ConnectionMediaType(mt);
                                    if(mt.MajorType == MediaType.Audio) {
                                        // Obtain a reference to the filter connected to the audio output of the video splitter (usually, this is the audio decoder)
                                        Tools.FilterInfo2 decoderFilterInfo2 = Tools.GetNextFilter(pinInfo2.PinInfo.Filter, PinDirection.Output, 0);

                                        // Remove all the filters connected to the audio decoder filter
                                        System.Collections.Generic.List<Tools.FilterInfo2> filtersInfo2 = new System.Collections.Generic.List<Tools.FilterInfo2>();
                                        Tools.FilterInfo2 testFilterInfo2 = Tools.GetNextFilter(decoderFilterInfo2.Filter, PinDirection.Output, 0);
                                        while(true) {
                                            filtersInfo2.Add(testFilterInfo2);
                                            testFilterInfo2 = Tools.GetNextFilter(testFilterInfo2.Filter, PinDirection.Output, 0);
                                            if(testFilterInfo2.Filter == null) break;
                                        }
                                        foreach(Tools.FilterInfo2 fi2 in filtersInfo2) {
                                            graph.RemoveFilter(fi2.Filter);
                                            fi2.Release();
                                        }

                                        // get type for sample grabber
                                        type = Type.GetTypeFromCLSID(Clsid.SampleGrabber);
                                        if(type == null)
                                            throw new ApplicationException("Failed creating audio sample grabber");

                                        // create sample grabber
                                        grabberObjectAudio = Activator.CreateInstance(type);
                                        sampleGrabberAudio = (ISampleGrabber)grabberObjectAudio;
                                        grabberBaseAudio = (IBaseFilter)grabberObjectAudio;

                                        // add grabber filters to graph
                                        graph.AddFilter(grabberBaseAudio, "grabberAudio");

                                        // set media type
                                        AMMediaType mediaTypeAudio = new AMMediaType {
                                            MajorType = MediaType.Audio,
                                            SubType = MediaSubType.PCM,
                                            FormatType = FormatType.WaveEx
                                        };
                                        sampleGrabberAudio.SetMediaType(mediaTypeAudio);

                                        outPin = Tools.GetOutPin(decoderFilterInfo2.Filter, 0);
                                        inPin = Tools.GetInPin(grabberBaseAudio, 0);
                                        if(graph.Connect(outPin, inPin) < 0)
                                            throw new ApplicationException("Failed connecting filter to grabberBaseAudio");
                                        Marshal.ReleaseComObject(outPin);
                                        Marshal.ReleaseComObject(inPin);

                                        // Finally, connect the grabber to the audio renderer
                                        outPin = Tools.GetOutPin(grabberBaseAudio, 0);
                                        graph.Render(outPin);

                                        mt = new AMMediaType();
                                        outPin.ConnectionMediaType(mt);
                                        if(!Tools.IsPinConnected(outPin))
                                            throw new ApplicationException("Failed obtaining media audio information");
                                        wavFormat = new WaveFormatEx();
                                        Marshal.PtrToStructure(mt.FormatPtr, wavFormat);
                                        Marshal.ReleaseComObject(outPin);

                                        // configure sample grabber
                                        sampleGrabberAudio.SetBufferSamples(false);
                                        sampleGrabberAudio.SetOneShot(false);
                                        sampleGrabberAudio.SetCallback(grabberAudio, 1);

                                        audioGrabberIsConnected = true;
                                        break;
                                    }
                                } catch {
                                }
                            }
                        }
                    }
                    filterInfo2.Release();
                    if(!audioGrabberIsConnected) {
                        foreach(Tools.PinInfo2 pinInfo2 in Tools.GetPins(sourceBase)) {
                            if(!Tools.IsPinConnected(pinInfo2.Pin)) {
                                foreach(AMMediaType mt in Tools.GetMediaTypes(pinInfo2.Pin)) {
                                    if(mt.MajorType == MediaType.Audio) {
                                        // create sample grabber
                                        grabberObjectAudio = Activator.CreateInstance(type);
                                        sampleGrabberAudio = (ISampleGrabber)grabberObjectAudio;
                                        grabberBaseAudio = (IBaseFilter)grabberObjectAudio;

                                        // add grabber filters to graph
                                        graph.AddFilter(grabberBaseAudio, "grabberAudio");

                                        // set media type
                                        AMMediaType mediaTypeAudio = new AMMediaType {
                                            MajorType = MediaType.Audio,
                                            SubType = MediaSubType.PCM,
                                            FormatType = FormatType.WaveEx
                                        };
                                        sampleGrabberAudio.SetMediaType(mediaTypeAudio);

                                        inPin = Tools.GetInPin(grabberBaseAudio, 0);
                                        if(graph.Connect(pinInfo2.Pin, inPin) < 0)
                                            throw new ApplicationException("Failed connecting sourceBase to grabberBaseVideo");
                                        Marshal.ReleaseComObject(inPin);

                                        // Finally, connect the grabber to the audio renderer
                                        outPin = Tools.GetOutPin(grabberBaseAudio, 0);
                                        graph.Render(outPin);

                                        AMMediaType amt = new AMMediaType();
                                        outPin.ConnectionMediaType(amt);
                                        if(!Tools.IsPinConnected(outPin))
                                            throw new ApplicationException("Failed obtaining media audio information");
                                        wavFormat = new WaveFormatEx();
                                        Marshal.PtrToStructure(amt.FormatPtr, wavFormat);
                                        Marshal.ReleaseComObject(outPin);

                                        // configure sample grabber
                                        sampleGrabberAudio.SetBufferSamples(false);
                                        sampleGrabberAudio.SetOneShot(false);
                                        sampleGrabberAudio.SetCallback(grabberAudio, 1);

                                        audioGrabberIsConnected = true;

                                        break;
                                    }
                                }
                            }
                        }
                    }
                    // *****************************************************************
                }

                // let's do the rendering, if we don't need to prevent freezing
                if(!preventFreezing) {
                    // render pin
                    graph.Render(Tools.GetOutPin(grabberBaseVideo, 0));

                    // configure video window
                    IVideoWindow window = (IVideoWindow)graphObject;
                    window.put_AutoShow(false);
                    window = null;
                }

                // configure sample grabber
                sampleGrabberVideo.SetBufferSamples(false);
                sampleGrabberVideo.SetOneShot(false);
                sampleGrabberVideo.SetCallback(grabberVideo, 1);

                // disable clock, if someone requested it
                if(!referenceClockEnabled) {
                    IMediaFilter mediaFilter = (IMediaFilter)graphObject;
                    mediaFilter.SetSyncSource(null);
                }

                // get media control
                mediaControl = (IMediaControl)graphObject;

                // get media seek control
                mediaSeekControl = (IMediaSeeking)graphObject;

                // get media events' interface
                mediaEvent = (IMediaEventEx)graphObject;

                // get media audio control
                basicAudio = (IBasicAudio)graphObject;
            } catch(Exception exception) {
                DestroyFilters();

                // provide information to clients
                VideoSourceError?.Invoke(this, new VideoSourceErrorEventArgs(exception.Message));
            }
        }

        private void DestroyFilters() {
            isValid = false;

            if(mediaControl != null) mediaControl.Stop();

            // release all objects
            graph = null;
            grabberBaseVideo = null;
            sampleGrabberVideo = null;
            grabberBaseAudio = null;
            sampleGrabberAudio = null;
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
            if(grabberObjectVideo != null) {
                Marshal.ReleaseComObject(grabberObjectVideo);
                grabberObjectVideo = null;
            }
            if(grabberObjectAudio != null) {
                Marshal.ReleaseComObject(grabberObjectAudio);
                grabberObjectAudio = null;
            }
        }

        /// <summary>
        /// Notifies client about new frame.
        /// </summary>
        /// 
        /// <param name="image">New frame's image.</param>
        /// 
        protected void OnNewFrame(Bitmap image) {
            framesReceived++;
            bytesReceived += image.Width * image.Height * (Bitmap.GetPixelFormatSize(image.PixelFormat) >> 3);

            if((!stopEvent.WaitOne(0, false)) && (NewFrame != null))
                NewFrame(this, new NewFrameEventArgs(image));
        }

        //
        // Video grabber
        //
        private class GrabberVideo : ISampleGrabberCB {
            private FileVideoSource parent;
            private int width, height;

            // Width property
            public int Width {
                get { return width; }
                set { width = value; }
            }
            // Height property
            public int Height {
                get { return height; }
                set { height = value; }
            }

            // Constructor
            public GrabberVideo(FileVideoSource parent) {
                this.parent = parent;
            }

            // Callback to receive samples
            public int SampleCB(double sampleTime, IntPtr sample) {
                return 0;
            }

            // Callback method that receives a pointer to the sample buffer
            public int BufferCB(double sampleTime, IntPtr buffer, int bufferLen) {
                if(parent.NewFrame != null) {
                    // create new image
                    System.Drawing.Bitmap image = new Bitmap(width, height, PixelFormat.Format32bppArgb /* PixelFormat.Format24bppRgb */ );

                    // lock bitmap data
                    BitmapData imageData = image.LockBits(
                        new Rectangle(0, 0, width, height),
                        ImageLockMode.ReadWrite,
                        PixelFormat.Format32bppArgb /* PixelFormat.Format24bppRgb */ );

                    // copy image data
                    int srcStride = imageData.Stride;
                    int dstStride = imageData.Stride;

                    int dst = imageData.Scan0.ToInt32() + dstStride * (height - 1);
                    int src = buffer.ToInt32();

                    for(int y = 0; y < height; y++) {
                        Win32.MemCopy(dst, src, srcStride);
                        dst -= dstStride;
                        src += srcStride;
                    }

                    // unlock bitmap data
                    image.UnlockBits(imageData);

                    // notify parent
                    parent.OnNewVideoFrame(image);

                    // release the image
                    image.Dispose();
                }

                return 0;
            }
        }

        protected void OnNewAudioFrame(short[] audio, int bufferLen, double sampleTime) {
            if(IsRunning) {
                framesReceived++;
                if((!stopEvent.WaitOne(0, true)) && (NewFrame != null))
                    NewAudioFrame(this, new NewAudioFrameEventArgs(audio, bufferLen, sampleTime));
            } else {
                if(NewFrame != null)
                    NewAudioFrame(this, new NewAudioFrameEventArgs(audio, bufferLen, sampleTime));
            }
        }

        private class GrabberAudio : ISampleGrabberCB {
            private FileVideoSource parent;
            private short[] savedArray = new short[96000];

            // Constructor
            public GrabberAudio(FileVideoSource parent) {
                this.parent = parent;
            }

            // Callback to receive samples
            public int SampleCB(double sampleTime, IntPtr sample) {
                return 0;
            }

            // Callback method that receives a pointer to the sample buffer
            public int BufferCB(double sampleTime, IntPtr buffer, int bufferLen) {
                if(parent.NewAudioFrame != null) {
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
