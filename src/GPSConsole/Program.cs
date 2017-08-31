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
using agpsl.NET.PMTK;

namespace GPSConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var defaultstr = Environment.OSVersion.Platform == PlatformID.Unix ? "/dev/ttyAPP1" : "COM6";

            Console.WriteLine($"Please Enter comport ({defaultstr}):");

            var commport = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(commport))
                commport = defaultstr;

            defaultstr = "115200";
            Console.WriteLine($"Please Enter Baudrate ({defaultstr}):");
            var baud = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(baud))
                baud = defaultstr;

            var gps = new agpsl.NET.GPS(commport, int.Parse(baud)) {LogToConsole = false};
            
            var response = gps.SendTestMessage();
            Console.WriteLine($"Response to test message was {response}");


            // Set the type of output you are interested in.
            response = gps.SendCommandAsync(new ApiSetNmeaOutput(5, 0, 0, 0, 0, 5, 0, 0, 1, 0, 0)).Result;
            Console.WriteLine($"Response to ApiSetNmeaOutput was {response}");

            InputMessage request = new ApiQDatum();
            response = gps.SendCommandAsync(request).Result;

            if(request.ResponseType == Message.PmtkResponseType.ValidCommandSuccess)
                Console.WriteLine($"The default Datum was {((ApiQDatum)request).Datum}");
            else            
                Console.WriteLine($"Response to ApiQDatum was {response}");

            request = new ApiQDatumAdvance();
            response = gps.SendCommandAsync(request).Result;

            if (request.ResponseType == Message.PmtkResponseType.ValidCommandSuccess)
                Console.WriteLine($"The user defined Datum was {((ApiQDatum)request).Datum}");
            else
                Console.WriteLine($"Response to ApiQDatumAdvance was {response}");


            request = new ApiQNemaOutput();
            response = gps.SendCommandAsync(request).Result;

            if (request.ResponseType == Message.PmtkResponseType.ValidCommandSuccess)
                Console.WriteLine($"The NemaOutput was {((ApiQNemaOutput)request).ResponseString()}");
            else
                Console.WriteLine($"Response to ApiQNemaOutput was {response}");

            // Set the type of output you are interested in.
            response = gps.SendCommandAsync(new SetDatum(35)).Result;            
            Console.WriteLine($"Response to SetDatum was {response}");


            Console.WriteLine("Press any key to start NMEA output");
            Console.ReadLine();
            gps.GPSEvent += (sender, message) => { Console.WriteLine(message); };

            Console.WriteLine("Press any key to enter standby mode");
            Console.ReadLine();
            response = gps.SendCommandAsync(new CmdStandbyMode(false)).Result;
            Console.WriteLine($"Response to CmdStandbyMode was {response}");


            Console.WriteLine("Press any key to enter Run mode");
            Console.ReadLine();
            response = gps.SendCommandAsync(new CmdHotStart()).Result;
            Console.WriteLine($"Response to CmdHotStart was {response}");

            Console.ReadLine();
        }        
    }
}
