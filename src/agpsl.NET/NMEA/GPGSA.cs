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

using System.Collections.Generic;

namespace agpsl.NET.NMEA
{
    /// <summary>
    /// Recommended minimum specific GPS/Transit data
    /// </summary>
    public class GPGSA : Message
    {
        /// <summary>
        /// GSA Fix mode
        /// </summary>
        public enum GSAFixMode
        {
            /// <summary>
            /// No fix available
            /// </summary>
            FixNotAvailable = 0,
            /// <summary>
            /// Horisontal fix only
            /// </summary>
            _2D = 2,
            /// <summary>
            /// 3D fix
            /// </summary>
            _3D = 3
        }

        public GPGSA(string message)
        {
            var parts = message.Split(',');

            if (parts[1].Length > 0)
                Mode = parts[1][0];
            else
                Mode = ' ';

            if (parts[2].Length > 0)
            {
                switch (parts[2])
                {
                    case "2":
                        FixMode = GSAFixMode._2D;
                        break;
                    case "3":
                        FixMode = GSAFixMode._3D;
                        break;

                    default:
                        FixMode = GSAFixMode.FixNotAvailable; break;
                }
            }

            PrnInSolution = new List<string>();

            for (int i = 0; i <= 11; i++)
                if (parts[i + 3] != "")
                    PrnInSolution.Add(parts[i + 3]);

            PDOP = ParseDouble(parts[15]);
            HDOP = ParseDouble(parts[16]);
            VDOP = ParseDouble(parts[17]);           
        }

        /// <summary>
        /// Mode. M=Manuel, A=Auto (forced/not forced to operate in 2D or 3D mode)
        /// </summary>
        public char Mode { get; }      

        /// <summary>
        /// Fix not available / 2D / 3D
        /// </summary>
        public GSAFixMode FixMode { get; }

        /// <summary>
        /// PRN Numbers used in solution
        /// </summary>
        public List<string> PrnInSolution { get; }

        /// <summary>
        /// Point Dilution of Precision
        /// </summary>
        public double PDOP { get; }

        /// <summary>
        /// Horisontal Dilution of Precision
        /// </summary>
        public double HDOP { get; }

        /// <summary>
        /// Vertical Dilution of Precision
        /// </summary>
        public double VDOP { get; }

        public override string ToString()
        {
            return $"$GPGSA  Mode: {Mode} - FixMode: {FixMode} - PrnInSolution: {string.Join(",", PrnInSolution)} - PDOP: {PDOP} - HDOP: {HDOP} - VDOP: {VDOP}";
        }

    }
}
