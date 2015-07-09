using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;


public static class Test
{
    static void ShowDevices()
    {
        var devices = CaptureDeviceList.Instance;

        Console.WriteLine("Devices:");
        foreach (var dev in devices)
        {
            string mac = null;
            if (dev is LibPcapLiveDevice)
            {
                var addrs = ((LibPcapLiveDevice)dev).Addresses;
                if (addrs.Count > 0)
                    mac = string.Format("({0})", addrs[0].Addr);
            }

            Console.WriteLine("\t{0}\t- {1} {2}", dev.Name, dev.Description, mac);
        }
    }


    static ICaptureDevice SelectDeviceByName(string name)
    {
        return CaptureDeviceList.Instance.SingleOrDefault(dev => dev.Name == name);
    }


    static void OnPacket(object sender, CaptureEventArgs ea)
    {
        var parsed = PacketDotNet.Packet.ParsePacket(ea.Packet.LinkLayerType, ea.Packet.Data);
        Console.WriteLine("<<<<<< {0}\n", parsed);
    }


    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            ShowDevices();
            Console.ReadLine();
            return;
        }

        var device = SelectDeviceByName(args[0]);
        if (device == null)
        {
            Console.WriteLine("No such device: {0}", args[0]);
            Console.WriteLine("Run without arguments to show devices list");
            return;
        }

        device.OnPacketArrival += OnPacket;
        device.Open(DeviceMode.Promiscuous, -1);
        if (args.Length > 1)
            device.Filter = args[1];

        try
        {
            device.StartCapture();
            while (true)
            {
                var prompt = Console.ReadLine();
                if (prompt == "q")
                    break;
                if (prompt.StartsWith("arp "))
                {
                    var arp = new ARPPacket(
                        ARPOperation.Request,
                        PhysicalAddress.Parse("00-00-00-00-00-00"),
                        IPAddress.Parse(prompt.Substring(4)),
                        device.MacAddress,
                        IPAddress.Parse("0.0.0.0")
                    );
                    var ether = new EthernetPacket(
                        device.MacAddress,
                        PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"),
                        EthernetPacketType.None
                    );
                    ether.PayloadPacket = arp;
                    Console.WriteLine(">>>>>> {0}", ether);
                    device.SendPacket(ether);
                }
            }
                device.StopCaptureTimeout = TimeSpan.FromMilliseconds(1000);
                device.StopCapture();
        }
        finally
        {
            device.Close();
        }
    }
}
