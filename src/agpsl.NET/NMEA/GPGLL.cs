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
    /// Recommended minimum specific GPS/Transit data
    /// </summary>
    public class GPGLL : Message
    {       
        public GPGLL(string message)
        {
            var parts = message.Split(',');

            //Calculate the time of the fix
            TimeOfPosition = ParseDateTime(parts[5]);

            //Calculate Longitude
            Longitude = GPSToDecimalDegrees(parts[3], parts[4]);

            //Calculate Latitude
            Latitude = GPSToDecimalDegrees(parts[1], parts[2]);

            DataValid = parts[6] == "A";
        }

        /// <summary>
        /// Latitude recieved position
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude recieved position
        /// </summary>
        public double Longitude { get; }

        /// <summary>
        /// Time of Position
        /// </summary>
        public DateTime TimeOfPosition { get; }

        /// <summary>
        /// Data valid (true for valid or false for data invalid).
        /// </summary>
        public bool DataValid { get; }

        public override string ToString()
        {
            return $"$GPGLL TimeOfPosition: {TimeOfPosition} - Latitude: {Latitude} - Longitude: {Longitude} - DataValid: {DataValid}";
        }

    }
}
