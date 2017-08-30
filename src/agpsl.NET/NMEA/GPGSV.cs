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
using System.Collections.Generic;

namespace agpsl.NET.NMEA
{
    /// <summary>
    /// Satellites in view
    /// </summary>
    public class GPGSV : MNEAHelper
    {
        private bool _firstMessageParsed = false;

        public GPGSV()
        {}


        /// <summary>
        /// Adds a GPGSV sentence, and parses it. 
        /// </summary>
        /// <param name="NMEAsentence">NMEA string</param>
        /// <returns>Returns true if this is the last message in GSV nmea sentences</returns>
        public bool AddSentence(string message)
        {
            var parts = message.Split(',');

            SatellitesInView = ParseInt(parts[3]);
            var msgCount = ParseInt(parts[1]);
            var msgNumber = ParseInt(parts[2]);

            if (msgCount < msgNumber || msgNumber < 1)
                return false;

            if (msgNumber == 1)
            {
                Satellites.Clear(); //First message. Let's clear the satellite list
                _firstMessageParsed = true;
            }
            else if (!_firstMessageParsed) //If we haven't received the first GSV message, return
                return false;

            var lastMsg = (msgCount == msgNumber); //Is this the last GSV message in the GSV messages?
            int satsInMsg;

            if (!lastMsg)
                satsInMsg = 4; //If this isn't the last message, the message will hold info for 4 satellites
            else
                satsInMsg = SatellitesInView - 4 * (msgNumber - 1); //calculate number of satellites in last message

            for (var i = 0; i < satsInMsg; i++)
            {
                var sat = new Satellite
                {
                    Prn = parts[i * 4 + 4],
                    Elevation = Convert.ToByte(parts[i * 4 + 5]),
                    Azimuth = Convert.ToInt16(parts[i * 4 + 6]),
                    Snr = Convert.ToByte(parts[i * 4 + 7])
                };
                Satellites.Add(sat);
            }

            return lastMsg;

        }

        /// <summary>
        /// Number of satellites visible
        /// </summary>
        public int SatellitesInView { get; private set; }
      
        /// <summary>
        /// Visible satellites
        /// </summary>
        public List<Satellite> Satellites { get; }  = new List<Satellite>(); 

        /// <summary>
        /// Space Vehicle (SV/Satellite) info structure
        /// </summary>
        public class Satellite
        {
            /// <summary>
            /// Pseudo-Random Number ID
            /// </summary>
            public string Prn;
            /// <summary>
            /// Elevation above horizon in degrees (0-90)
            /// </summary>
            public byte Elevation;
            /// <summary>
            /// Azimuth	in degrees (0-359)
            /// </summary>
            public short Azimuth;
            /// <summary>
            /// Signal-to-noise ratio in dBHZ (0-99)
            /// </summary>
            public byte Snr;

            public override string ToString()
            {
                return $"Prn: {Prn} - Elevation: {Elevation} - Azimuth: {Azimuth} - Snr: {Snr}";
            }
        }

        public override string ToString()
        {
            return $"$GPGSV  SatellitesInView: {SatellitesInView} - Satellites: {string.Join(",", Satellites)}";
        }

    }
}
