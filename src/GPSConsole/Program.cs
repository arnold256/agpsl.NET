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

namespace GPS
{
    class Program
    {
        public static PmtkHelper lastcommand;

        static void Main(string[] args)
        {
            Console.WriteLine("Please Enter comport (COM6):");

            var commport = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(commport))
                commport = "COM6";

            Console.WriteLine("Please Enter Baudrate (115200):");
            var baud = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(baud))
                baud = "115200";

            var gps = new agpsl.NET.GPS(commport, int.Parse(baud)) {LogToConsole = false};
            gps.GPSEvent += (sender, message) => { Console.WriteLine(message); };

            // Set the type of output you are interested in.
            gps.SendCommandAsync(new ApiSetNmeaOutput(5, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0));

            Console.WriteLine("Press any key to enter standby mode");
            Console.ReadLine();
            gps.SendCommandAsync(new CmdStandbyMode(false));
            

            Console.WriteLine("Press any key to enter Run mode");
            Console.ReadLine();
            gps.SendCommandAsync(new CmdHotStart());
            Console.ReadLine();
        }        
    }
}
