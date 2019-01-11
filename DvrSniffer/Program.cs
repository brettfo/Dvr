using System;
using System.IO;
using System.Net;

namespace DvrSniffer
{
    class Program
    {
        static void Main(string[] args)
        {
            var ip = "192.168.0.98";
            var port = 6036;
            var user = "admin";

            string password = null;
            try
            {
                password = File.ReadAllLines("password.txt")[0];
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to read password.  Ensure there's a 'password.txt' file in the solution and that its contents are set appropriately.");
                Console.WriteLine(ex);
                Environment.Exit(1);
            }

            var dvr = new Dvr();
            dvr.Connect(IPAddress.Parse(ip), port, user, password);

            Console.WriteLine("Connected to DVR:");
            Console.WriteLine($"    Device name:      {dvr.DeviceInformation.DeviceName}");
            Console.WriteLine($"    Hardware version: {dvr.DeviceInformation.HardwareVersion}");
            Console.WriteLine($"    Kernel version:   {dvr.DeviceInformation.KernelVersion}");
            Console.WriteLine($"    Firmware version: {dvr.DeviceInformation.FirmwareVersion}");
            Console.WriteLine($"    MCU Version:      {dvr.DeviceInformation.MCUVersion}");
            Console.WriteLine($"    Serial number:    {dvr.DeviceInformation.SerialNumber}");
            Console.WriteLine();
            Console.WriteLine("Cameras found:");
            foreach (var camera in dvr.CameraInfo.CameraNames)
            {
                Console.WriteLine("    " + camera);
            }

            var data = dvr.RequestVideo(0);
            dvr.Close();

            File.WriteAllBytes(@"C:\Users\Brett\Desktop\vid.raw", data);
        }
    }
}
