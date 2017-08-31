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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace agpsl.NET.PMTK
{
    /// <summary>
    /// Standard Input message.  All input messages follow this format
    /// </summary>
    public abstract class InputMessage : Message
    {               
        public delegate void ResponseReceivedEventHandler(InputMessage sender, PmtkResponseType e);
        public event ResponseReceivedEventHandler ResponseReceived;                
        public PmtkResponseType ResponseType { get; private set; } = PmtkResponseType.WaitingResponse;

        /// <summary>
        /// Timeout used when waiting for packet responses
        /// </summary>
        public TimeSpan Timeout { get; set;  } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Used to generate the message checksum
        /// </summary>
        public static string CheckSum(string message)
        {
            int checksum = 0;
            for (int i = 0; i < message.Length; i++)
            {
                checksum ^= Convert.ToByte(message[i]);
            }
            return checksum.ToString("X2");
        }

        /// <summary>
        /// Wait for packet response Asynchronous
        /// </summary>
        public async Task<PmtkResponseType> WaitForResponse()
        {
            var tcs = new TaskCompletionSource<PmtkResponseType>();
            var cts = new CancellationTokenSource(Timeout);

            // Create and register function to handle PacketHasCompletelyFinished event
            void Handler(InputMessage sender, PmtkResponseType e) => tcs.TrySetResult(e);

            try
            {
                lock (this)
                {
                    if (ResponseType != PmtkResponseType.WaitingResponse)
                        return ResponseType;

                    ResponseReceived += Handler;
                }


                using (cts.Token.Register(() =>
                {
                    ResponseType = PmtkResponseType.Timeout;
                    tcs.SetResult(PmtkResponseType.Timeout);

                }, useSynchronizationContext: false))
                {
                    return await tcs.Task.ConfigureAwait(continueOnCapturedContext: false);
                }
            }
            finally
            {
                ResponseReceived -= Handler;
            }
        }

        /// <summary>
        /// Gets the string to be sent to the GPS Module
        /// </summary>        
        public string GetMessage()
        {
            var message = string.IsNullOrEmpty(Payload) ? $"{Pmtk}{Command.ToString().PadLeft(3,'0')}":$"{Pmtk}{Command},{Payload}";
            return $"${message}*{CheckSum(message)}\r\n";
        }

        /// <summary>
        /// Returns bytes to be sent to the GPS Module
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            return Encoding.ASCII.GetBytes(GetMessage());
        }


        /// <summary>
        /// Used to add response messages to the InputMessage
        /// </summary>
        public bool AddResponse(string response)
        {
            lock (this)
            {
                var command = ResponseMessage.ParseMessage(response);

                if (command is Ack ack && ack.Cmd == Command)
                {
                    // valid response to packet was found
                    ResponseType = ack.Flag;
                    ResponseReceived?.Invoke(this, ResponseType);
                    return true;
                }
                else
                {
                    var result = OnResponseAdded(command, out PmtkResponseType responseType);
                    if (result)
                    {
                        ResponseType = responseType;
                        ResponseReceived?.Invoke(this, ResponseType);
                    }
                    return result;
                }                                
            }
        }

        /// <summary>
        /// Method for processing packet responses.  This method can be overridden for InputMessages which 
        /// donot use the standard Ack Response.
        /// </summary>
        protected virtual bool OnResponseAdded(ResponseMessage response, out PmtkResponseType responseType)
        {
            responseType = PmtkResponseType.WaitingResponse;
            return false;
        }
    }
}
