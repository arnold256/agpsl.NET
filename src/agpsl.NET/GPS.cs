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
using System.IO;
using System.Text;
using System.Threading.Tasks;
using agpsl.NET.NMEA;
using agpsl.NET.PMTK;
using System.IO.Ports;

namespace agpsl.NET
{
    public class GPS : IDisposable
    {
        private readonly SerialPort _sp;
        private string _messageBuffer;

        public delegate void GPSEventEventHandler(GPS sender, NMEA.Message message);

        public event GPSEventEventHandler GPSEvent;

        public bool LogToConsole = false;

        private HashSet<InputMessage> _commands = new HashSet<InputMessage>();

        private GPGSV _gsvMessage;

        private bool _disposed;

        /// <summary>
        /// Create and open serial port connection
        /// </summary>
        /// <param name="port"></param>
        /// <param name="baud"></param>
        public GPS(string port, int baud)
        {

            _sp = new SerialPort(port, baud)
            {
                Handshake = Handshake.None
            };
            _sp.Open();
            KickoffRead();
        }

        void KickoffRead()
        {
            byte[] buffer = new byte[1024];
            _sp.BaseStream.BeginRead(buffer, 0, buffer.Length, delegate (IAsyncResult ar)
            {
                try
                {
                    int actualLength = _sp.BaseStream.EndRead(ar);
                    byte[] received = new byte[actualLength];
                    Buffer.BlockCopy(buffer, 0, received, 0, actualLength);
                    DataReceived(Encoding.ASCII.GetString(received));
                }
                catch //(IOException exc)
                {
                    // Not sure what to do with this.
                }
                if(!_disposed)
                    KickoffRead();
            }, null);
        }

        /// <summary>
        /// Data received on the commport event.  Extract NMEA messages and
        /// pass them on to be processed.
        /// </summary>
        private void DataReceived(string receivedMessage)
        {          
            _messageBuffer += receivedMessage;

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
            var nmeaMessage = NMEA.Message.ProcessMessage(message, _gsvMessage);
            if (LogToConsole) Console.Write($"{message}");

            if (nmeaMessage != null)
            {
                if (nmeaMessage is GPGSV gsv)
                {
                    _gsvMessage = gsv;
                    if (!_gsvMessage.LastMsg)
                        return;
                }
                
                GPSEvent?.Invoke(this, nmeaMessage);
            }
            else
            {
                if (message.StartsWith("$PMTK"))
                {
                    // Remove the command when it is finished
                    _commands.RemoveWhere(c => c.ResponseType != InputMessage.PmtkResponseType.WaitingResponse);

                    // Add response to each of the waiting messages
                    foreach (var command in _commands)                    
                        command.AddResponse(message);                   
                }                    
            }
        }

        /// <summary>
        /// Send a command to the GPS module
        /// </summary>
        public Task<PMTK.Message.PmtkResponseType> SendCommandAsync(InputMessage command)
        {
            _commands.Add(command);
            _sp.Write(command.GetMessage());
            //Console.Write(command.GetMessage());
            return command.WaitForResponse();
        }

        /// <summary>
        /// Sends a test (ping) to the GPS module to ensure it is working correctly.
        /// </summary>
        /// <returns></returns>
        public PMTK.Message.PmtkResponseType SendTestMessage()
        {
            return SendCommandAsync(new Test()).Result;
        }

        public void Dispose()
        {
            _sp?.Dispose();
            _disposed = true;
        }
    }
}
