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
    /// Global Positioning System Fix Data
    /// </summary>
    public class GPGGA : Message
    {
        /// <summary>
        /// Fix Quality.
        /// </summary>
        public enum GGAFixQuality
        {
            /// <summary>
            /// Invalid fix
            /// </summary>
            Invalid = 0,
            /// <summary>
            /// GPS fix
            /// </summary>
            GPS = 1,
            /// <summary>
            /// DGPS fix
            /// </summary>
            DGPS = 2
        }

        public GPGGA(string message)
        {
            var parts = message.Split(',');

            //Calculate the time of the fix
            TimeOfFix = parts[1].Length >= 6 ? ParseDateTime(parts[1]) : new DateTime();


            //Calculate Longitude
            Longitude = GPSToDecimalDegrees(parts[4], parts[5]);

            //Calculate Latitude
            Latitude = GPSToDecimalDegrees(parts[2], parts[3]);

            switch (parts[6])
            {
                case "1":
                    FixQuality = GGAFixQuality.GPS;
                    break;

                case "2":
                    FixQuality = GGAFixQuality.DGPS;
                    break;

                default:
                    FixQuality = GGAFixQuality.Invalid;
                    break;
            }

            NumberOfSatellites = Convert.ToByte(parts[7]);
            Dilution = ParseDouble(parts[8]);
            Altitude = ParseDouble(parts[9]);
            AltitudeUnits = parts[10][0];
            HeightOfGeoid = ParseDouble(parts[11]);
            DGPSUpdate = ParseInt(parts[13]);
            DGPSStationID = parts[14];
        }

        /// <summary>
        /// Indicates the current status of the GPS receiver.
        /// </summary>
        public GGAFixQuality FixQuality { get; }

        /// <summary>
        /// Latitude recieved position
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude recieved position
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Date and Time of fix - GMT.
        /// </summary>
        public DateTime TimeOfFix { get; }

        /// <summary>
        /// number of satellites being tracked.
        /// </summary>
        public byte NumberOfSatellites { get; }

        /// <summary>
        /// Altitude above sea level.
        /// </summary>
        public double Altitude { get; }

        /// <summary>
        /// Altitude Units - M (meters).
        /// </summary>
        public char AltitudeUnits { get; }

        /// <summary>
        /// Horizontal dilution of position (HDOP).
        /// </summary>
        public double Dilution { get; }

        /// <summary>
        /// Height of geoid (mean sea level) above WGS84 ellipsoid.
        /// </summary>
        public double HeightOfGeoid { get; }

        /// <summary>
        /// Time in seconds since last DGPS update.
        /// </summary>
        public int DGPSUpdate { get; }

        /// <summary>
        /// DGPS station ID number.
        /// </summary>
        public string DGPSStationID { get; }


        public override string ToString()
        {
            return $"$GPRMC FixQuality: {FixQuality} - Timestamp: {TimeOfFix} - Latitude: {Latitude} - Longitude: {Longitude} - NumberOfSatellites: {NumberOfSatellites} - Altitude: {Altitude} - AltitudeUnits: {AltitudeUnits} - Dilution: {Dilution} - HeightOfGeoid: {HeightOfGeoid} - DGPSUpdate: {DGPSUpdate} - DGPSStationID: {DGPSStationID}";
        }

    }
}
