using SharpPcap;
using SharpPcap.LibPcap;
using SharpPcap.Npcap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcapProcessor
{
    public class Sniffer
    {
        public bool PromisciousMode { get; set; }
        public List<string> AvailiableDevicesNames = CaptureDeviceList.Instance.Select(d => (PcapDevice)d).Select(d => d.Interface.FriendlyName).ToList();
        internal Queue<PacketDotNet.Packet> _packets { get; set; }
        internal object _packets_queue_lock { get; set; }
        internal string _networkInterface { get; set; }
        public event SnifferPacketArrivedEventHandler SnifferPacketArrived;
        public delegate void SnifferPacketArrivedEventHandler(object sender, SnifferPacketArrivedEventArgs e);
        public Sniffer(string networkInterface, bool promisciousMode = false)
        {
            _networkInterface = networkInterface;
            _packets = new Queue<PacketDotNet.Packet>();
            _packets_queue_lock = new object();
            PromisciousMode = promisciousMode;
        }

        public void StartSniffing()
        {
            var backgroundThread = new System.Threading.Thread(RaisePacketArrivedEvent);
            var availiableDevices = CaptureDeviceList.Instance;

            if (AvailiableDevicesNames.Contains(_networkInterface))
            {
                ICaptureDevice _device = availiableDevices[AvailiableDevicesNames.IndexOf(_networkInterface)];
                int readTimeoutMilliseconds = 1000;

                if (_device is NpcapDevice)
                {

                    var nPcap = _device as NpcapDevice;
                    if (PromisciousMode)
                    {
                        nPcap.Open(SharpPcap.Npcap.OpenFlags.Promiscuous, readTimeoutMilliseconds);
                    }
                    else
                    {
                        nPcap.Open();
                    }

                    nPcap.Mode = CaptureMode.Packets;
                }
                else if (_device is LibPcapLiveDevice)
                {
                    var livePcapDevice = _device as LibPcapLiveDevice;
                    livePcapDevice.Open(PromisciousMode ? DeviceMode.Promiscuous : DeviceMode.Normal);
                }
                else
                {
                    throw new InvalidOperationException("unknown device type of " + _networkInterface.GetType().ToString());
                }

                // Register our handler function to the 'packet arrival' event
                _device.OnPacketArrival += new PacketArrivalEventHandler(InsertPacketToQueue);

                // Start the capturing process
                backgroundThread.Start();
                _device.StartCapture();

                // Wait for 'ctrl-c' from the user.
                Console.TreatControlCAsInput = true;
                Console.ReadLine();

                // Stop the capturing process
                _device.StopCapture();

                //waiting on the packet procesing thread to finish

                backgroundThread.Join();
                // Close the pcap device
                _device.Close();
            }
            else
            {
                throw new Exception($"No such device {_networkInterface}");
            }
        }

        private void InsertPacketToQueue(object sender, CaptureEventArgs e)
        {
            var packet = PacketDotNet.Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            lock (_packets_queue_lock)
            {
                _packets.Enqueue(packet);
            }
        }
        private void RaisePacketArrivedEvent()
        {
            bool shouldSleep = true;

            lock (_packets_queue_lock)
            {
                if (_packets.Count != 0)
                {
                    shouldSleep = false;
                }
            }
            if (shouldSleep)
            {
                System.Threading.Thread.Sleep(1000);
            }

            while (_packets.Count > 0)
            {
                {
                    lock (_packets_queue_lock)
                    {
                        SnifferPacketArrived?.Invoke(this, new SnifferPacketArrivedEventArgs()
                        {
                            Packet = _packets.Dequeue()
                        });
                    }
                }
            }
        }
    }
}


