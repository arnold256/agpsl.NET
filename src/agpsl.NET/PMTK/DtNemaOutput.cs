using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace agpsl.NET.PMTK
{
    /// <summary>
    /// This is a response to PMTK414, which return current NMEA sentence output frequency setting
    /// </summary>
    public class DtNemaOutput : ResponseMessage
    {
        public DtNemaOutput(string payload):base(payload)
        {}        

        public override int Command => 514;
        public override string CommandName => "PMTK_DT_NMEA_OUTPUT";
    }
}
