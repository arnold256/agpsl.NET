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
    public class GPRMC : Message
    {
        /// <summary>
        /// Enum for the Receiver Status information.
        /// </summary>
        public enum StatusEnum
        {
            /// <summary>
            /// Fix warning
            /// </summary>
            Warning,
            /// <summary>
            /// Fix Ok
            /// </summary>
            Ok
        }

        public GPRMC(string message)
        {
            var parts = message.Split(',');

            //Calculate the time of the fix
            Timestamp = parts[9].Length >= 6 ? ParseDateTime(parts[9] + parts[1]) : new DateTime();

            // Calculate the status
            Status = parts[2] == "A" ? StatusEnum.Ok : StatusEnum.Warning;

            //Calculate Longitude
            Longitude = GPSToDecimalDegrees(parts[5], parts[6]);

            //Calculate Latitude
            Latitude = GPSToDecimalDegrees(parts[3], parts[4]);

            // Calculate Speed
            SpeedKnots = ParseDouble(parts[7]);

            // Calculate Course
            Course = ParseDouble(parts[8]);

            // Calculate MagneticVariation
            MagneticVariation = ParseDouble(parts[10]);
        }

        /// <summary>
        /// Indicates the current status of the GPS receiver.
        /// </summary>
        public StatusEnum Status { get; }

        /// <summary>
        /// Latitude recieved position
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude recieved position
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Date and Time of fix.
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Groundspeed in knots.
        /// </summary>
        public double SpeedKnots { get; set; }

        /// <summary>
        /// Groundspeed in km.
        /// </summary>
        public double Speed => SpeedKnots * 1.852;

        /// <summary>
        /// Course (true, not magnetic) in decimal degrees.
        /// </summary>
        public double Course { get; }

        /// <summary>
        /// MagneticVariation in decimal degrees.
        /// </summary>
        public double MagneticVariation { get; }

        public override string ToString()
        {
            return $"$GPGGA Status: {Status} - Timestamp: {Timestamp} - Latitude: {Latitude} - Longitude: {Longitude} - Course: {Course} - Speed: {Speed} - MagneticVariation: {MagneticVariation}";
        }

    }
}
