// AForge Direct Show Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

namespace AForge.Video.DirectShow.Internals
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(false)]
    internal class TimeFormat
    {
        // 00000000-0000-0000-0000-000000000000 TIME_FORMAT_NONE
        public static readonly Guid None = new Guid(0x0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        // 7b785570-8c82-11cf-bc0c-00aa00ac74f6 TIME_FORMAT_FRAME
        public static readonly Guid Frame = new Guid(0x7b785570, 0x8c82, 0x11cf, 0xbc, 0x0c, 0x00, 0xaa, 0x00, 0xac, 0x74, 0xf6);

        // 7b785571-8c82-11cf-bc0c-00aa00ac74f6 TIME_FORMAT_BYTE
        public static readonly Guid Byte = new Guid(0x7b785571, 0x8c82, 0x11cf, 0xbc, 0x0c, 0x00, 0xaa, 0x00, 0xac, 0x74, 0xf6);

        // 7b785572-8c82-11cf-bc0c-00aa00ac74f6 TIME_FORMAT_SAMPLE
        public static readonly Guid Sample = new Guid(0x7b785572, 0x8c82, 0x11cf, 0xbc, 0x0c, 0x00, 0xaa, 0x00, 0xac, 0x74, 0xf6);

        // 7b785573-8c82-11cf-bc0c-00aa00ac74f6 TIME_FORMAT_FIELD
        public static readonly Guid Field = new Guid(0x7b785573, 0x8c82, 0x11cf, 0xbc, 0x0c, 0x00, 0xaa, 0x00, 0xac, 0x74, 0xf6);

        // 7b785574-8c82-11cf-bc0c-00aa00ac74f6 TIME_FORMAT_MEDIA_TIME
        public static readonly Guid MediaTime = new Guid(0x7b785574, 0x8c82, 0x11cf, 0xbc, 0x0c, 0x00, 0xaa, 0x00, 0xac, 0x74, 0xf6);
    }
}
