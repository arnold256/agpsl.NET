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
using System.Collections.Generic;
using System.Threading.Tasks;
using agpsl.NET.NMEA;
using agpsl.NET.PMTK;
using RJCP.IO.Ports;

namespace agpsl.NET
{
    public class GPS
    {
        private readonly SerialPortStream _sp;
        private string _messageBuffer;

        public delegate void GPSEventEventHandler(GPS sender, MNEAHelper message);

        public event GPSEventEventHandler GPSEvent;

        public bool LogToConsole = false;

        private HashSet<PmtkHelper> _commands = new HashSet<PmtkHelper>();

        /// <summary>
        /// Create and open serial port connection
        /// </summary>
        /// <param name="port"></param>
        /// <param name="baud"></param>
        public GPS(string port, int baud)
        {
            _sp = new SerialPortStream(port, baud);
            _sp.DataReceived += DataReceived;
            _sp.ReceivedBytesThreshold = 1;
            _sp.Handshake = Handshake.None;
            _sp.Open();

        }
        
        /// <summary>
        /// Data received on the commport event.  Extract NMEA messages and
        /// pass them on to be processed.
        /// </summary>
        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType != SerialData.Chars)
                return;

            _messageBuffer += _sp.ReadExisting();

            // Allow multiple NMEA messages to be processed by a single event
            while(!string.IsNullOrEmpty(_messageBuffer))
            {
                if (!_messageBuffer.StartsWith("$"))
                {
                    // find start of NMEA message $
                    var startpos = _messageBuffer.IndexOf('$');
                    if (startpos < 0)
                    {
                        // no start found clear message
                        _messageBuffer = "";
                        return;
                    }

                    // get the contents of rhe message after the starting position
                    _messageBuffer = _messageBuffer.Substring(startpos + 1);
                }

                // find the end of NMEA message \r\n
                var endpos = _messageBuffer.IndexOf("\r\n", StringComparison.Ordinal);

                if (endpos < 0)
                    return; // no end found wait for more data

                var message = _messageBuffer.Substring(0, endpos + 2);

                // process the message
                ProcessMessage(message);

                // remove the message from the buffer and continue processing
                _messageBuffer = _messageBuffer.Substring(endpos + 2);
            }
        }

        /// <summary>
        /// process the NMEA message and invoke event
        /// </summary>
        /// <param name="message"></param>
        private void ProcessMessage(string message)
        {
            var nmeaMessage = MNEAHelper.ProcessMessage(message);

            if (nmeaMessage != null)
            {

                if(LogToConsole) Console.Write($"Processed - {message}");
                GPSEvent?.Invoke(this, nmeaMessage);
            }
            else
            {
                if (message.StartsWith("$PMTK"))
                {
                    // Remove the command when it is finished
                    _commands.RemoveWhere(c => c.ResponseType != PmtkHelper.PmtkResponseType.WaitingResponse);

                    // Add response to each of the waiting messages
                    foreach (var command in _commands)                    
                        command.AddResponse(message);                   
                }
                else if (LogToConsole)
                    Console.Write($"Not Processed - {message}");                
            }
        }

        /// <summary>
        /// Send a command to the GPS module
        /// </summary>
        public Task<PmtkHelper.PmtkResponseType> SendCommandAsync(PmtkHelper command)
        {
            _sp.Write(command.GetMessage());
            return command.WaitForResponse();
        }
    }
}
