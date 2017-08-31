using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace agpsl.NET.PMTK
{
    /// <summary>
    /// Query user defined datum.
    /// </summary>
    public class ApiQDatumAdvance : ApiQDatum
    {
        public override int Command => 431;
        public override string CommandName => "PMTK_API_Q_DATUM_ADVANCE";
    }
}
