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

namespace agpsl.NET.PMTK
{
    /// <summary>
    /// Set NMEA sentence output frequencies.
    /// There are totally 19 data fields that present output frequencies for the 19 supported NMEA sentences
    /// individually
    /// </summary>
    public class ApiSetNmeaOutput : PmtkHelper
    {
        /// <summary>
        /// Supported Frequency Setting
        /// 0 - Disabled or not supported sentence
        /// 1 - Output once every one position fix
        /// 2 - Output once every two position fixes
        /// 3 - Output once every three position fixes
        /// 4 - Output once every four position fixes
        /// 5 - Output once every five position fixes 
        /// </summary>
        /// <param name="GLL">GPGLL interval - Geographic Position - Latitude longitude </param>
        /// <param name="RMC">GPRMC interval - Recomended Minimum Specific GNSS Sentence</param>
        /// <param name="VTG">GPVTG interval - Course Over Ground and Ground Speed</param>
        /// <param name="GGA">GPGGA interval - GPS Fix Data </param>
        /// <param name="GSA">GPGSA interval - GNSS DOPS and Active Satellites </param>
        /// <param name="GSV">GPGSV interval - GNSS Satellites in View </param>
        /// <param name="GRS">GPGRS interval – GNSS Range Residuals</param>
        /// <param name="GST">GPGST interval – GNSS Pseudorange Errors Statistics</param>
        /// <param name="ZDA">GPZDA interval – Time & Date </param>
        /// <param name="MCHN">PMTKCHN interval – GNSS channel status</param>
        /// <param name="DMT">GPDTM interval – Datum reference</param>
        public ApiSetNmeaOutput(int GLL, int RMC, int VTG, int GGA, int GSA, int GSV, int GRS, int GST, int ZDA, int MCHN, int DMT )
        {
            //              0     1     2     3     4     5     6     7 8 9 1011121314151617    18     19
            Payload = $"{GLL},{RMC},{VTG},{GGA},{GSA},{GSV},{GRS},{GST},0,0,0,0,0,0,0,0,0,{ZDA},{MCHN},{DMT}";
        }

        public override string Command => "314";

        public override string CommandName => "PMTK_API_SET_NMEA_OUTPUT ";

        public override string Payload { get; }
    }
}
