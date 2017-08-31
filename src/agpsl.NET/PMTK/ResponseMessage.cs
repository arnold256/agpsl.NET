using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace agpsl.NET.PMTK
{
    public abstract class ResponseMessage : Message
    {
        protected ResponseMessage(string payload)
        {
            Payload = payload;
        }

        public override string Payload { get; }

        public static ResponseMessage ParseMessage(string message)
        {
            var endpos = message.IndexOf('*');
            if (endpos < 0)
            {
                // no start found clear message
                throw new Exception("invalid NMEA message");
            }
            var command = message.Substring(5, 3);
            var payload = message.Substring(9, endpos - 9);

            switch (command)
            {
                case "001":
                    return new Ack(payload);
                case "530":
                    return new DtDatum(payload);
                case "514":
                    return new DtNemaOutput(payload);
            }

            return null;
        }
    }
}
