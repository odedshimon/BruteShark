using PcapProcessor.Objects;
using SharpPcap;
using SharpPcap.LibPcap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PcapProcessor
{
    // TODO: use interface
    public class Processor
    {
        public delegate void UdpPacketArivedEventHandler(object sender, UdpPacketArivedEventArgs e);
        public event UdpPacketArivedEventHandler UdpPacketArived;
        public delegate void TcpPacketArivedEventHandler(object sender, TcpPacketArivedEventArgs e);
        public event TcpPacketArivedEventHandler TcpPacketArived;
        public delegate void TcpSessionArivedEventHandler(object sender, TcpSessionArivedEventArgs e);
        public event TcpSessionArivedEventHandler TcpSessionArrived;
        public delegate void UdpSessionArrivedEventHandler(object sender, UdpSessionArrivedEventArgs e);
        public event UdpSessionArrivedEventHandler UdpSessionArrived;
        public delegate void FileProcessingStartedEventHandler(object sender, FileProcessingStartedEventArgs e);
        public event FileProcessingStartedEventHandler FileProcessingStarted;
        public delegate void FileProcessingEndedEventHandler(object sender, FileProcessingEndedEventArgs e);
        public event FileProcessingEndedEventHandler FileProcessingEnded;
        public delegate void ProcessingPrecentsChangedEventHandler(object sender, ProcessingPrecentsChangedEventArgs e);
        public event ProcessingPrecentsChangedEventHandler ProcessingPrecentsChanged;
        public event EventHandler ProcessingFinished;

        public bool BuildTcpSessions { get; set; }
        public bool BuildUdpSessions { get; set; }
        private TcpSessionsBuilder _tcpSessionsBuilder;
        private UdpStreamBuilder _udpStreamBuilder;
        private ProcessingPrecentsPredicator _processingPrecentsPredicator;


        public Processor()
        {
            this.BuildTcpSessions = false;
            this.BuildUdpSessions = false;
            _tcpSessionsBuilder = new TcpSessionsBuilder();
            _udpStreamBuilder = new UdpStreamBuilder();
            _processingPrecentsPredicator = new ProcessingPrecentsPredicator();
            _processingPrecentsPredicator.ProcessingPrecentsChanged += OnPredicatorProcessingPrecentsChanged;
        }

        private void OnPredicatorProcessingPrecentsChanged(object sender, ProcessingPrecentsChangedEventArgs e)
        {
            // TODO: think of make this check in a dedicated extention method for events (e.g SafeInvoke())
            if (ProcessingPrecentsChanged is null)
                return;

            ProcessingPrecentsChanged.Invoke(this, new ProcessingPrecentsChangedEventArgs()
            {
                Precents = e.Precents
            });
        }
        
        public void ProcessPcaps(IEnumerable<string> filesPaths)
        {
            _processingPrecentsPredicator.AddFiles(new HashSet<FileInfo>(filesPaths.Select(fp => new FileInfo(fp))));

            foreach (var filePath in filesPaths)
            {
                this.ProcessPcap(filePath);
            }

            ProcessingFinished?.Invoke(this, new EventArgs());
        }

        public void ProcessPcap(string filePath)
        {
            try
            {
                FileProcessingStarted?.Invoke(this, new FileProcessingStartedEventArgs() { FilePath = filePath });
                _tcpSessionsBuilder.Clear();
                _udpStreamBuilder.Clear();

                // Get an offline device, handle packets registering for the Packet 
                // Arrival event and start capturing from that file.
                // NOTE: the capture function is blocking.
                ICaptureDevice device = new CaptureFileReaderDevice(filePath);
                device.OnPacketArrival += new PacketArrivalEventHandler(ProcessPacket);
                device.Open();
                device.Capture();

                // Raise event for each Tcp session that was built.
                // TODO: think about detecting complete sesions on the fly and raising 
                // events accordingly.
                foreach (var session in this._tcpSessionsBuilder.Sessions)
                {
                    TcpSessionArrived?.Invoke(this, new TcpSessionArivedEventArgs()
                    {
                        TcpSession = session
                    });
                }
                foreach (var session in this._udpStreamBuilder.Sessions)
                {
                    UdpSessionArrived?.Invoke(this, new UdpSessionArrivedEventArgs()
                    {
                        UdpSession = session
                    });
                }

                _processingPrecentsPredicator.NotifyAboutProcessedFile(new FileInfo(filePath));
                FileProcessingEnded?.Invoke(this, new FileProcessingEndedEventArgs() { FilePath = filePath });
            }
            catch (Exception ex)
            {
                //throw new PcapFileProcessingException(filePath);
            }
        }

        private void ProcessPacket(object sender, CaptureEventArgs e)
        {
            try
            {
                var packet = PacketDotNet.Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
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
                    _processingPrecentsPredicator.NotifyAboutProcessedData(packet.Bytes.Length);
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
                    }

                    _processingPrecentsPredicator.NotifyAboutProcessedData(packet.Bytes.Length);
                }
            }
            catch (Exception ex)
            {
                // TODO: handle or throw this
                //Console.WriteLine(ex);
            }

        }

    }
}
