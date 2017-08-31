using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace agpsl.NET.PMTK
{
    public class DtDatum : ResponseMessage
    {
        public DtDatum(string payload):base(payload)
        {}        

        public override int Command => 530;
        public override string CommandName => "PMTK_DT_DATUM";
        public int Datum => int.Parse(Payload);
    }
}
