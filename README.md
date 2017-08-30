# agpsl.NET
Another GPS Library for .NET

I could not find a GPS library for .NET which supported PMTK commands to control the GPS module.

I created this library and heavily based it on SharpGPS (https://github.com/fivepmtechnology/SharpGPS/tree/master/src/SharpGps)

The library supports basic NMEA processing and the ability to send a (very) limited set of PMTK commands.

I am using the SerialPortStream library (https://github.com/jcurl/SerialPortStream) for accessing the comm port.  When running with mono under linux the DataReceived event does not fire.  Using this library allows agpsl.NET to function on linux.   

Please note that only minimal testing has been done as I only require a small set of the functionality.

So far the only GPS module this has been tested with is the SIM39EA.
