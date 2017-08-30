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
    public abstract class PmtkHelper
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

        public abstract string Command { get; }

        public abstract string CommandName { get; }

        public PmtkResponseType ResponseType { get; private set; } = PmtkResponseType.WaitingResponse;

        public delegate void ResponseReceivedEventHandler(PmtkHelper sender, PmtkResponseType e);
        public event ResponseReceivedEventHandler ResponseReceived;

        public async Task<PmtkResponseType> WaitForResponse()
        {
            var timeout = TimeSpan.FromSeconds(5);
            var tcs = new TaskCompletionSource<PmtkResponseType>();
            var cts = new CancellationTokenSource(timeout);
            
            // Create and register function to handle PacketHasCompletelyFinished event
            void Handler(PmtkHelper sender, PmtkResponseType e) => tcs.TrySetResult(e);

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

        public abstract string Payload { get; }

        public static string CheckSum(string message)
        {
            int checksum = 0;
            for (int i = 0; i < message.Length; i++)
            {
                checksum ^= Convert.ToByte(message[i]);
            }
            return checksum.ToString("X2");
        }

        public string GetMessage()
        {
            var message = string.IsNullOrEmpty(Payload) ? $"{Pmtk}{Command}":$"{Pmtk}{Command},{Payload}";
            return $"${message}*{CheckSum(message)}\r\n";
        }

        public byte[] GetBytes()
        {
            return Encoding.ASCII.GetBytes(GetMessage());
        }

        public bool AddResponse(string response)
        {
            lock (this)
            {
                ResponseReceived?.Invoke(this, PmtkResponseType.ValidCommandSuccess);
                return true;
            }
        }
    }
}
