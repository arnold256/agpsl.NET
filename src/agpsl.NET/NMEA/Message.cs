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
using System.Globalization;

namespace agpsl.NET.NMEA
{    
    public abstract class Message
    {
        protected static readonly string[] DateTimeFormats = { "o", "ddMMyyHHmmss", "ddMMyy", "ddMMyyHHmmss.FFFFFF", "hhmmss.fff" };
        protected static readonly NumberFormatInfo NumberFormatEnUs = new CultureInfo("en-US", false).NumberFormat;

        /// <summary>
        /// Converts various GPS strings to a dateTime
        /// </summary>
        /// <param name="nmeaTime"></param>
        /// <returns></returns>
        protected DateTime ParseDateTime(string nmeaTime)
        {
            if (DateTime.TryParseExact(nmeaTime, DateTimeFormats, NumberFormatEnUs, DateTimeStyles.AssumeUniversal,
                out DateTime timestamp))
                return timestamp;

            return DateTime.MinValue;                        
        }

        /// <summary>
        /// Converts GPS position in d"dd.ddd' to decimal degrees ddd.ddddd
        /// </summary>
        protected static double GPSToDecimalDegrees(string dm, string dir)
        {
            try
            {
                if (dm == "" || dir == "")
                {
                    return 0.0;
                }
                //Get the fractional part of minutes
                //DM = '5512.45',  Dir='N'
                //DM = '12311.12', Dir='E'

                double fm = double.Parse(dm.Substring(dm.IndexOf(".", StringComparison.Ordinal)), NumberFormatEnUs);

                //Get the minutes.
                double min = double.Parse(dm.Substring(dm.IndexOf(".", StringComparison.Ordinal) - 2, 2), NumberFormatEnUs);

                //Degrees                
                double deg = double.Parse(dm.Substring(0, dm.IndexOf(".", StringComparison.Ordinal) - 2), NumberFormatEnUs);

                if (dir == "S" || dir == "W")
                    deg = -(deg + (min + fm) / 60);
                else
                    deg = deg + (min + fm) / 60;
                return deg;
            }
            catch
            {
                return 0.0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected static double ParseDouble(string str)
        {           
            double.TryParse(str,NumberStyles.Number, NumberFormatEnUs, out double dbl);
            return dbl;
        }

        protected static int ParseInt(string str, int defaultInt = -1)
        {
            if(int.TryParse(str, NumberStyles.Number, NumberFormatEnUs, out int numb))
                return numb;
            return defaultInt;
        }

        public static Message ProcessMessage(string message, GPGSV lastGsvMessage = null)
        {
            var command = message.Substring(1, 5);

            var endpos = message.IndexOf('*');
            if (endpos < 0)
            {
                // no start found clear message
               throw new Exception("invalid NMEA message");
            }

            message = message.Substring(1, endpos - 1);


            switch (command)
            {
                // Recommended minimum specific GPS/Transit data
                case "GPRMC":
                    return new GPRMC(message);

                // Global Positioning System Fix Data
                case "GPGGA":
                    return new GPGGA(message);

                // Satellites in view
                case "GPGSV":
                    if(lastGsvMessage == null)
                        lastGsvMessage = new GPGSV();

                    lastGsvMessage.AddSentence(message);
                    return lastGsvMessage;

                // GPS DOP and active satellites
                case "GPGSA":
                    return new GPGSA(message);

                // Geographic position, Latitude and Longitude
                case "GPGLL":
                    return new GPGLL(message);

                // Timestamp
                case "GPZDA":
                    return new GPZDA(message);
                                
            }
            return null;
        }

    }
}
