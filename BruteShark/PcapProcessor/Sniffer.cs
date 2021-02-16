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
        public event EventHandler SniffingStoped;

        private TcpSessionsBuilder _tcpSessionsBuilder;
        private UdpStreamBuilder _udpStreamBuilder;

        private Queue<PacketDotNet.Packet> _packets { get; set; }
        private object _packetsQueueLock { get; set; }
        private bool _souldStopPacketProcessingThread;
        private Thread _packetProcessingThread;

        public bool BuildTcpSessions { get; set; }
        public bool BuildUdpSessions { get; set; }
        public bool PromisciousMode { get; set; }
        public string Filter { get; set; }
        public string SelectedInterface { get; set; }
        public bool SouldStopCapture { get; set; }


        public List<string> AvailiableDevicesNames => CaptureDeviceList.Instance.Select(d => (PcapDevice)d).Select(d => d.Interface.FriendlyName).ToList();


        public Sniffer()
        {
            Filter = string.Empty;
            SouldStopCapture = false;
            BuildTcpSessions = false;
            BuildUdpSessions = false;
            _tcpSessionsBuilder = new TcpSessionsBuilder();
            _udpStreamBuilder = new UdpStreamBuilder();
            _packets = new Queue<PacketDotNet.Packet>();
            _packetsQueueLock = new object();
        }

        public void StartSniffing()
        {
            this.SouldStopCapture = false;
            _tcpSessionsBuilder.Clear();
            _tcpSessionsBuilder.IsLiveCapture = true;
            _udpStreamBuilder.Clear();

            
            var availiableDevices = CaptureDeviceList.Instance;

            if (!AvailiableDevicesNames.Contains(SelectedInterface))
            {
                throw new Exception($"No such device {SelectedInterface}");
            }

            ICaptureDevice selectedDevice = availiableDevices[AvailiableDevicesNames.IndexOf(SelectedInterface)];
            int readTimeoutMilliseconds = 1000;

            if (selectedDevice is NpcapDevice)
            {
                var nPcap = selectedDevice as NpcapDevice;
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
            else if (selectedDevice is LibPcapLiveDevice)
            {
                var livePcapDevice = selectedDevice as LibPcapLiveDevice;
                livePcapDevice.Open(PromisciousMode ? DeviceMode.Promiscuous : DeviceMode.Normal);
            }
            else
            {
                throw new InvalidOperationException("Unknown device type of " + SelectedInterface.GetType().ToString());
            }

            // Setup capture filter.
            if (Filter != string.Empty)
            {
                selectedDevice.Filter = this.Filter;
            }

            // Register our handler function to the 'packet arrival' event.
            selectedDevice.OnPacketArrival += InsertPacketToQueue;

            // Start the packet procesing thread.
            StartPacketProcessingThread();

            // Start the capturing process
            selectedDevice.StartCapture();

            // Wait for sniffing to be stoped by user.
            WaitForStopSniffingSignal();

            // Stop the capturing process.
            selectedDevice.StopCapture();

            // Waiting on the packet procesing thread to finish.
            StopPacketProcessingThread();

            // Close the pcap device
            selectedDevice.Close();

            /*
            _tcpSessionsBuilder.Sessions.AsParallel().ForAll(session => TcpSessionArrived?.Invoke(this, new TcpSessionArivedEventArgs()
            {
                TcpSession = session
            }));

            _udpStreamBuilder.Sessions.AsParallel().ForAll(session => UdpSessionArrived?.Invoke(this, new UdpSessionArrivedEventArgs()
            {
                UdpSession = session
            }));
            */

            SniffingStoped?.Invoke(this, new EventArgs());
        }

        private void StartPacketProcessingThread()
        {
            _packetProcessingThread = new Thread(ProcessPacketsQueue) { Name = "Packets Processing Thread", IsBackground = true };
            _packetProcessingThread.Start();
        }

        private void StopPacketProcessingThread()
        {
            _souldStopPacketProcessingThread = true;
            _packetProcessingThread.Join();
        }

        private void WaitForStopSniffingSignal()
        {
            while (true)
            {
                Thread.Sleep(1000);

                if (this.SouldStopCapture)
                {
                    break;
                }
            }
        }

        private void ProcessPacket(PacketDotNet.Packet packet)
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

            lock (_packetsQueueLock)
            {
                _packets.Enqueue(packet);
            }
        }

        private void ProcessPacketsQueue()
        {
            _souldStopPacketProcessingThread = false;

            while (true)
            {
                lock (_packetsQueueLock)
                {
                    while (_packets.Count > 0)
                    {
                        ProcessPacket(_packets.Dequeue());

                        if (_souldStopPacketProcessingThread)
                        {
                            break;
                        }
                    }
                }

                Thread.Sleep(1000);
            }
        }

    }
}


