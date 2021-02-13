using PcapProcessor.Objects;
using SharpPcap;
using SharpPcap.LibPcap;
using SharpPcap.Npcap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PcapProcessor
{
    public class Sniffer
    {
        public delegate void UdpPacketArivedEventHandler(object sender, UdpPacketArivedEventArgs e);
        public event UdpPacketArivedEventHandler UdpPacketArived;
        public delegate void UdpSessionArrivedEventHandler(object sender, UdpSessionArrivedEventArgs e);
        public event UdpSessionArrivedEventHandler UdpSessionArrived;
        public delegate void TcpPacketArivedEventHandler(object sender, TcpPacketArivedEventArgs e);
        public event TcpPacketArivedEventHandler TcpPacketArived;
        public delegate void TcpSessionArivedEventHandler(object sender, TcpSessionArivedEventArgs e);
        public event TcpSessionArivedEventHandler TcpSessionArrived;
        public event EventHandler ProcessingFinished;

        private TcpSessionsBuilder _tcpSessionsBuilder;
        private UdpStreamBuilder _udpStreamBuilder;

        internal Queue<PacketDotNet.Packet> _packets { get; set; }
        internal object _packets_queue_lock { get; set; }

        public bool BuildTcpSessions { get; set; }
        public bool BuildUdpSessions { get; set; }
        public bool IsLiveCapture { get; set; }
        public bool PromisciousMode { get; set; }

        public string Filter { get; set; }
        public string _networkInterface { get; set; }
        public List<string> AvailiableDevicesNames => CaptureDeviceList.Instance.Select(d => (PcapDevice)d).Select(d => d.Interface.FriendlyName).ToList();


        public Sniffer()
        {
            Filter = string.Empty;
            BuildTcpSessions = false;
            BuildUdpSessions = false;
            _tcpSessionsBuilder = new TcpSessionsBuilder();
            _udpStreamBuilder = new UdpStreamBuilder();
            _packets = new Queue<PacketDotNet.Packet>();
            _packets_queue_lock = new object();
        }

        public void StartSniffing()
        {
            _tcpSessionsBuilder.Clear();
            _tcpSessionsBuilder.IsLiveCapture = true;
            _udpStreamBuilder.Clear();

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

                // Setup capture filter.
                if (Filter != string.Empty)
                {
                    _device.Filter = Filter;
                }

                // Register our handler function to the 'packet arrival' event.
                _device.OnPacketArrival += InsertPacketToQueue;

                // Start the capturing process
                backgroundThread.Start();
                _device.StartCapture();

                // TODO: Create a function for stoping the sniffer (Console is not valid object when running fron WinForms)
                // Wait for 'ctrl-c' from the user.
                // Console.TreatControlCAsInput = true;
                // Console.ReadLine();

                Thread.Sleep(1000 * 150);

                // Stop the capturing process
                _device.StopCapture();

                //waiting on the packet procesing thread to finish

                backgroundThread.Join();
                // Close the pcap device
                _device.Close();

                _tcpSessionsBuilder.Sessions.AsParallel().ForAll(session => TcpSessionArrived?.Invoke(this, new TcpSessionArivedEventArgs()
                {
                    TcpSession = session
                }));

                _udpStreamBuilder.Sessions.AsParallel().ForAll(session => UdpSessionArrived?.Invoke(this, new UdpSessionArrivedEventArgs()
                {
                    UdpSession = session
                }));

                ProcessingFinished?.Invoke(this, new EventArgs());
            }
            else
            {
                throw new Exception($"No such device {_networkInterface}");
            }
        }

        void ProcessPacket(PacketDotNet.Packet packet)
        {
            try
            {
                var tcpPacket = packet.Extract<PacketDotNet.TcpPacket>();
                var udpPacket = packet.Extract<PacketDotNet.UdpPacket>();

                if (udpPacket != null)
                {
                    var ipPacket = (PacketDotNet.IPPacket)udpPacket.ParentPacket;

                    UdpPacketArived?.Invoke(this, new UdpPacketArivedEventArgs
                    {
                        Packet = new UdpPacket
                        {
                            SourcePort = udpPacket.SourcePort,
                            DestinationPort = udpPacket.DestinationPort,
                            SourceIp = ipPacket.SourceAddress.ToString(),
                            DestinationIp = ipPacket.DestinationAddress.ToString(),
                            Data = udpPacket.PayloadData ?? new byte[] { }
                        }
                    });

                    if (this.BuildUdpSessions)
                    {
                        this._udpStreamBuilder.HandlePacket(udpPacket);
                    }
                    
                }
                else if (tcpPacket != null)
                {
                    var ipPacket = (PacketDotNet.IPPacket)tcpPacket.ParentPacket;

                    // Raise event Tcp packet arived event.
                    TcpPacketArived?.Invoke(this, new TcpPacketArivedEventArgs
                    {
                        Packet = new TcpPacket
                        {
                            SourcePort = tcpPacket.SourcePort,
                            DestinationPort = tcpPacket.DestinationPort,
                            SourceIp = ipPacket.SourceAddress.ToString(),
                            DestinationIp = ipPacket.DestinationAddress.ToString(),
                            Data = tcpPacket.PayloadData ?? new byte[] { }
                        }
                    });

                    if (this.BuildTcpSessions)
                    {
                        this._tcpSessionsBuilder.HandlePacket(tcpPacket);
                        _tcpSessionsBuilder.completedSessions.AsParallel().ForAll((session) =>
                        {
                            TcpSessionArrived?.Invoke(this, new TcpSessionArivedEventArgs()
                            {
                                TcpSession = session
                            });
                            _tcpSessionsBuilder.completedSessions.Remove(session);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: handle or throw this
                //Console.WriteLine(ex);
            }
        }

        public static bool CheckCaptureFilter(string filter)
        {
            return PcapDevice.CheckFilter(filter, out string outString);
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
                lock (_packets_queue_lock)
                {
                    ProcessPacket(_packets.Dequeue());
                }
            }
        }

    }
}


