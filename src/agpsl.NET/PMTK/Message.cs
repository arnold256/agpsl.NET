using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace agpsl.NET.PMTK
{
    public abstract class Message
    {
        public enum PmtkResponseType
        {
            InvalidCommand = 0,
            UnsupportedCommand = 1,
            ValidCommandFailed = 2,
            ValidCommandSuccess = 3,
            Timeout = 4,
            WaitingResponse = 255,


        }

        public const string Pmtk = "PMTK";

        public abstract string Payload { get; }

        public abstract int Command { get; }

        public abstract string CommandName { get; }

    }
}
