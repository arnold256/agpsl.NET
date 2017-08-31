using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace agpsl.NET.PMTK
{
    /// <summary>
    /// Query default datum.
    /// </summary>
    public class ApiQDatum : InputMessage
    {
        public override string Payload => "";
        public override int Command => 430;
        public override string CommandName => "PMTK_API_Q_DATUM";

        private DtDatum _response;

        protected override bool OnResponseAdded(ResponseMessage response, out PmtkResponseType responseType)
        {
            if (response is DtDatum command)
            {
                _response = command;
                // valid response to packet was found
                responseType = PmtkResponseType.ValidCommandSuccess;
                return true;
            }
            responseType = PmtkResponseType.WaitingResponse;
            return false;
        }

        public int Datum
        {
            get
            {
                if(_response == null)
                    throw new Exception("Response has not been received.");

                return _response.Datum;
            }
        }
    }
}
