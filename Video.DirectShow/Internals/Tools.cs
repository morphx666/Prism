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
    using System.Collections.Generic;

    /// <summary>
    /// Some miscellaneous functions.
    /// </summary>
    /// 
    internal static partial class Tools
    {
        /// <summary>
        /// Get filter's pin.
        /// </summary>
        /// 
        /// <param name="filter">Filter to get pin of.</param>
        /// <param name="dir">Pin's direction.</param>
        /// <param name="num">Pin's number.</param>
        /// 
        /// <returns>Returns filter's pin.</returns>
        /// 
        public static IPin GetPin( IBaseFilter filter, PinDirection dir, int num )
        {
            IPin[] pin = new IPin[1];
            IEnumPins pinsEnum = null;

            // enum filter pins
            if ( filter.EnumPins( out pinsEnum ) == 0 )
            {
                PinDirection pinDir;
                int n;

                try
                {
                    // get next pin
                    while ( pinsEnum.Next( 1, pin, out n ) == 0 )
                    {
                        // query pin`s direction
                        pin[0].QueryDirection( out pinDir );

                        if ( pinDir == dir )
                        {
                            if ( num == 0 )
                                return pin[0];
                            num--;
                        }

                        Marshal.ReleaseComObject( pin[0] );
                        pin[0] = null;
                    }
                }
                finally
                {
                    Marshal.ReleaseComObject( pinsEnum );
                }
            }
            return null;
        }

        /// <summary>
        /// Get filter's input pin.
        /// </summary>
        /// 
        /// <param name="filter">Filter to get pin of.</param>
        /// <param name="num">Pin's number.</param>
        /// 
        /// <returns>Returns filter's pin.</returns>
        /// 
        public static IPin GetInPin( IBaseFilter filter, int num )
        {
            return GetPin( filter, PinDirection.Input, num );
        }

        /// <summary>
        /// Get filter's output pin.
        /// </summary>
        /// 
        /// <param name="filter">Filter to get pin of.</param>
        /// <param name="num">Pin's number.</param>
        /// 
        /// <returns>Returns filter's pin.</returns>
        /// 
        public static IPin GetOutPin( IBaseFilter filter, int num )
        {
            return GetPin( filter, PinDirection.Output, num );
        }

        public struct PinInfo2
        {
            public IPin Pin;
            public PinInfo PinInfo;

            public void Release()
            {
                if (Pin != null) Marshal.ReleaseComObject(Pin);
            }
        }

        public static List<PinInfo2> GetPins(IBaseFilter filter)
        {
            List<PinInfo2> pins = new List<PinInfo2>();
            IPin[] pina = new IPin[1];
            IEnumPins pinsEnum = null;

            // enum filter pins
            if (filter.EnumPins(out pinsEnum) == 0)
            {
                PinDirection pinDir;
                int n;

                try
                {
                    // get next pin
                    while (pinsEnum.Next(1, pina, out n) == 0)
                    {
                        // query pin's direction
                        pina[0].QueryDirection(out pinDir);

                        PinInfo pinInfo;
                        pina[0].QueryPinInfo(out pinInfo);

                        PinInfo2 pinInfo2;
                        pinInfo2.Pin = pina[0];
                        pinInfo2.PinInfo = pinInfo;
                        pins.Add(pinInfo2);
                    }
                }
                finally
                {
                    Marshal.ReleaseComObject(pinsEnum);
                }
            }
            return pins;
        }

        public static List<AMMediaType> GetMediaTypes(IPin pin)
        {
            List<AMMediaType> mediaTypes = new List<AMMediaType>();
            IEnumMediaTypes mediaTypesEnum = null;
            AMMediaType[] mta = new AMMediaType[1];

            if (pin.EnumMediaTypes(out mediaTypesEnum) == 0)
            {
                int n;

                try
                {
                    while (mediaTypesEnum.Next(1, mta, out n) == 0)
                    {
                        if (n == 1) mediaTypes.Add(mta[0]);
                    }
                }
                finally
                {
                    Marshal.ReleaseComObject(mediaTypesEnum);
                }
            }

            return mediaTypes;
        }

        public static bool IsPinConnected(IPin pin)
        {
            IPin pinNext;
            int result = pin.ConnectedTo(out pinNext);
            if (pinNext != null) Marshal.ReleaseComObject(pinNext);
            return (result == 0);
        }

        public struct FilterInfo2
        {
            public IBaseFilter Filter;
            public FilterInfo FilterInfo;
            public List<PinInfo2> Pins;

            public void Release()
            {
                if (Filter != null) Marshal.ReleaseComObject(Filter);
                if (Pins != null)
                    foreach (PinInfo2 pi2 in Pins)
                        pi2.Release();
            }
        }

        public static FilterInfo2 GetNextFilter(IBaseFilter filter, PinDirection direction, int offset)
        {
            int count = 0;
            FilterInfo2 nextFilter = new FilterInfo2();
            List<PinInfo2> pins = GetPins(filter);
            foreach (PinInfo2 pinInfo2 in pins)
            {
                if (pinInfo2.PinInfo.Direction == direction)
                {
                    IPin pinNext;
                    if (pinInfo2.Pin.ConnectedTo(out pinNext) == 0)
                    {
                        if (offset == count)
                        {
                            PinInfo pinInfo;
                            pinNext.QueryPinInfo(out pinInfo);

                            FilterInfo filterInfo;
                            pinInfo.Filter.QueryFilterInfo(out filterInfo);

                            nextFilter.Filter = pinInfo.Filter;
                            nextFilter.FilterInfo = filterInfo;
                            nextFilter.Pins = GetPins(pinInfo.Filter);

                            Marshal.ReleaseComObject(pinNext);

                            break;
                        }
                        else
                            count++;
                    }
                }
            }
            return nextFilter;
        }
    }
}
