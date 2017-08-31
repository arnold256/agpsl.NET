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
    /// Enter standby mode for power saving. 
    /// </summary>
    public class CmdStandbyMode : InputMessage
    {
        /// <summary>
        /// Standby type：
        /// false = Stop mode, stop NMEA output, the receiver stays at ultra low power state
        /// true =  Sleep mode, stop NMEA output, the receiver stays at full on power state
        /// </summary>
        /// <param name="mode"></param>
        public CmdStandbyMode(bool mode)
        {
            Payload = mode ? "1" : "0";
        }

        public override int Command => 161;

        public override string CommandName => "PMTK_CMD_STANDBY_MODE";

        public override string Payload { get; }     
    }
}
