using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace agpsl.NET.PMTK
{
    /// <summary>
    /// Query current NMEA sentence output frequencies
    /// </summary>
    public class ApiQNemaOutput : InputMessage
    {
        public override string Payload => "";
        public override int Command => 414;
        public override string CommandName => "PMTK_API_Q_NMEA_OUTPUT";

        private DtNemaOutput _response;

        protected override bool OnResponseAdded(ResponseMessage response, out PmtkResponseType responseType)
        {
            if (response is DtNemaOutput command)
            {
                _response = command;
                // valid response to packet was found
                responseType = PmtkResponseType.ValidCommandSuccess;
                return true;
            }
            responseType = PmtkResponseType.WaitingResponse;
            return false;
        }

       
        /// <summary>
        /// GPGLL interval - Geographic Position - Latitude longitude
        /// </summary>
        public int GLL => GetNmeaFrequency(0);

        /// <summary>
        /// GPRMC interval - Recomended Minimum Specific GNSS Sentence
        /// </summary>
        public int RMC => GetNmeaFrequency(1);

        /// <summary>
        /// GPVTG interval - Course Over Ground and Ground Speed
        /// </summary>
        public int VTG => GetNmeaFrequency(2);

        /// <summary>
        /// GPGGA interval - GPS Fix Data
        /// </summary>
        public int GGA => GetNmeaFrequency(3);

        /// <summary>
        /// GPGSA interval - GNSS DOPS and Active Satellites
        /// </summary>
        public int GSA => GetNmeaFrequency(4);

        /// <summary>
        /// GPGSV interval - GNSS Satellites in View
        /// </summary>
        public int GSV => GetNmeaFrequency(5);

        /// <summary>
        /// GPGRS interval – GNSS Range Residuals
        /// </summary>
        public int GRS => GetNmeaFrequency(6);

        /// <summary>
        /// GPGST interval – GNSS Pseudorange Errors Statistics
        /// </summary>
        public int GST => GetNmeaFrequency(7);

        /// <summary>
        /// GPZDA interval – Time & Date
        /// </summary>
        public int ZDA => GetNmeaFrequency(17);

        /// <summary>
        /// PMTKCHN interval – GNSS channel status
        /// </summary>
        public int MCHN => GetNmeaFrequency(18);

        /// <summary>
        /// GPDTM interval – Datum reference
        /// </summary>
        public int DMT => GetNmeaFrequency(19);

        private int GetNmeaFrequency(int index)
        {
            if(_response == null)
                throw new Exception("Response has not been received.");

            var parts = _response.Payload.Split(',');
            return int.Parse(parts[index]);
        }

        public string ResponseString()
        {
            return $"\nGLL: {GLL}\nRMC: {RMC}\nVTG: {VTG}\nGGA: {GGA}\nGSA: {GSA}\nGSV: {GSV}\nGRS: {GRS}\nGST: {GST}\nZDA: {ZDA}\nMCHN: {MCHN}\n";
        }

    }
}
