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
    public class SetNemaBaudrate : InputMessage
    {
        /// <summary>
        /// Set NMEA port baudrate.Using PMTK251 command to setup baud rate setting, the setting will be back to
        /// defatult value in the two conditions:
        /// 1. Full cold start command is issued
        /// 2. Enter standby mode
        /// </summary>
        /// <param name="baudrate"></param>
        public SetNemaBaudrate(string baudrate)
        {
            Payload = baudrate;
        }

        public override int Command => 251;

        public override string CommandName => "PMTK_SET_NMEA_BAUDRATE ";

        public override string Payload { get;}
    }
}
