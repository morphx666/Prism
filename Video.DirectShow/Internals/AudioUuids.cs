// AForge Direct Show Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright � AForge.NET, 2009-2012
// contacts@aforgenet.com
//

namespace AForge.Video.DirectShow.Internals
{
    using System;
    using System.Runtime.InteropServices;

	/// <summary>
    /// DirectShow media subtypes.
    /// </summary>
    /// 
    internal partial class MediaAudioSubType
    {
        public static readonly Guid MEDIASUBTYPE_PCM = new Guid("00000001-0000-0010-8000-00AA00389B71"); // PCM audio.
        public static readonly Guid MEDIASUBTYPE_PCMAudioObsolete = new Guid("e436eb8a-524f-11ce-9f53-0020af0ba770"); // Obsolete. Do not use.
        public static readonly Guid MEDIASUBTYPE_MPEG1Packet = new Guid("e436eb80-524f-11ce-9f53-0020af0ba770"); // MPEG1 Audio packet.
        public static readonly Guid MEDIASUBTYPE_MPEG1Payload = new Guid("e436eb81-524f-11ce-9f53-0020af0ba770"); // MPEG1 Audio Payload.
        public static readonly Guid MEDIASUBTYPE_MPEG2_AUDIO = new Guid("e06d802b-db46-11cf-b4d1-00805f6cbbea"); // MPEG-2 audio data
        public static readonly Guid MEDIASUBTYPE_DVD_LPCM_AUDIO = new Guid("e06d8032-db46-11cf-b4d1-00805f6cbbea"); // DVD audio data
        public static readonly Guid MEDIASUBTYPE_DRM_Audio = new Guid("00000009-0000-0010-8000-00aa00389b71"); // Corresponds to WAVE_FORMAT_DRM.
        public static readonly Guid MEDIASUBTYPE_IEEE_FLOAT = new Guid("00000003-0000-0010-8000-00aa00389b71"); // Corresponds to WAVE_FORMAT_IEEE_FLOAT
        public static readonly Guid MEDIASUBTYPE_DOLBY_AC3 = new Guid("e06d802c-db46-11cf-b4d1-00805f6cbbea"); // Dolby data
        public static readonly Guid MEDIASUBTYPE_DOLBY_AC3_SPDIF = new Guid("00000092-0000-0010-8000-00aa00389b71"); // Dolby AC3 over SPDIF.
        public static readonly Guid MEDIASUBTYPE_RAW_SPORT = new Guid("00000240-0000-0010-8000-00aa00389b71"); // Equivalent to MEDIASUBTYPE_DOLBY_AC3_SPDIF.
        public static readonly Guid MEDIASUBTYPE_SPDIF_TAG_241h = new Guid("00000241-0000-0010-8000-00aa00389b71"); // Equivalent to MEDIASUBTYPE_DOLBY_AC3_SPDIF.
        //http://msdn.microsoft.com/en-us/library/dd757532%28VS.85%29.aspx
        public static readonly Guid WMMEDIASUBTYPE_MP3 = new Guid("00000055-0000-0010-8000-00AA00389B71");
        // others?
        public static readonly Guid MEDIASUBTYPE_WAVE = new Guid("e436eb8b-524f-11ce-9f53-0020af0ba770");
        public static readonly Guid MEDIASUBTYPE_AU = new Guid("e436eb8c-524f-11ce-9f53-0020af0ba770");
        public static readonly Guid MEDIASUBTYPE_AIFF = new Guid("e436eb8d-524f-11ce-9f53-0020af0ba770");
    }
}
