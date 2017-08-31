using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace agpsl.NET.PMTK
{
    /// <summary>
    /// Generic Ack response message
    /// </summary>
    public class Ack : ResponseMessage
    {
        public Ack(string payload):base(payload)
        {
            var parts = payload.Split(',');
            Cmd = int.Parse(parts[0]);
            Flag = (PmtkResponseType) int.Parse(parts[1]);
        }

        /// <summary>
        /// Command that the Ack responds to.
        /// </summary>
        public int Cmd { get; }

        /// <summary>
        /// Response type
        /// </summary>
        public PmtkResponseType Flag { get; }
        
        public override int Command => 001;
        public override string CommandName => "PMTK_ACK";
    }
}
