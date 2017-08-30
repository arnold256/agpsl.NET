// MIT License

// Copyright(c) 2017 Ben Arnold
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;

namespace agpsl.NET.NMEA
{
    /// <summary>
    /// Date & Time
    /// </summary>
    public class GPZDA : MNEAHelper
    {
      public GPZDA(string message)
        {
            var parts = message.Split(new [] {','}, StringSplitOptions.None);

            /*
             *   UTC, day, month, year, and local time zone.
             *
             *   0      1         2  3  4    5  6
             *   $--ZDA,hhmmss.ss,xx,xx,xxxx,xx,xx
             *   (1) hhmmss.ss = UTC 
             *   (2) xx = Day, 01 to 31 
             *   (3) xx = Month, 01 to 12 
             *   (4) xxxx = Year 
             *   (5) xx = Local zone description, 00 to +/- 13 hours 
             *   (6) xx = Local zone minutes description (same sign as hours)
             */

            // Convert to 'o' format 2009-06-15T13:45:30 (DateTimeKind.Local) --> 2009-06-15T13:45:30.0000000-07:00
            if (string.IsNullOrEmpty(parts[5]))
                parts[5] = "+00";

            if (string.IsNullOrEmpty(parts[6]))
                parts[6] = "00";

            var time = $"{parts[4]}-{parts[3]}-{parts[2]}T{parts[1].Substring(0,2)}:{parts[1].Substring(2,2)}:{parts[1].Substring(4, 2)}.{parts[1].Substring(7, 2)}00000{parts[5]}:{parts[6]}";

            //Calculate the time of the fix
            Timestamp = ParseDateTime(time);

        }

        /// <summary>
        /// Current Time
        /// </summary>
        public DateTime Timestamp { get; }


        public override string ToString()
        {
            return $"$GPZDA Timestamp: {Timestamp}";
        }

    }
}
