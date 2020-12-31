using PcapProcessor.Objects;
using SharpPcap;
using SharpPcap.LibPcap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Haukcode.PcapngUtils;
using Haukcode.PcapngUtils.Common;   

namespace PcapProcessor
{
    // TODO: use interface
    public class Processor
    {
        public delegate void FileProcessingStatusChangedEventHandler(object sender, FileProcessingStatusChangedEventArgs e);
        public event FileProcessingStatusChangedEventHandler FileProcessingStatusChanged;
        public delegate void UdpPacketArivedEventHandler(object sender, UdpPacketArivedEventArgs e);
        public event UdpPacketArivedEventHandler UdpPacketArived;
        public delegate void UdpSessionArrivedEventHandler(object sender, UdpSessionArrivedEventArgs e);
        public event UdpSessionArrivedEventHandler UdpSessionArrived;
        public delegate void TcpPacketArivedEventHandler(object sender, TcpPacketArivedEventArgs e);
        public event TcpPacketArivedEventHandler TcpPacketArived;
        public delegate void TcpSessionArivedEventHandler(object sender, TcpSessionArivedEventArgs e);
        public event TcpSessionArivedEventHandler TcpSessionArrived;
        public delegate void ProcessingPrecentsChangedEventHandler(object sender, ProcessingPrecentsChangedEventArgs e);
        public event ProcessingPrecentsChangedEventHandler ProcessingPrecentsChanged;
        public event EventHandler ProcessingFinished;

        public bool ProcessFilesParallel { get; set; }
        public bool BuildTcpSessions { get; set; }
        public bool BuildUdpSessions { get; set; }

        private Dictionary<string, TcpSessionsBuilder> _filesToTCPBuilders;
        private Dictionary<string, UdpStreamBuilder> _filesToUDPBuilders;
        private ProcessingPrecentsPredicator _processingPrecentsPredicator;


        public Processor()
        {   

            this.ProcessFilesParallel = false;
            this.BuildTcpSessions = false;
            this.BuildUdpSessions = false;
            this._filesToTCPBuilders = new Dictionary<string, TcpSessionsBuilder>();
            this._filesToUDPBuilders = new Dictionary<string, UdpStreamBuilder>();

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
                string filename = filePath.Substring(filePath.LastIndexOf(@"\") + 1 );
                this._filesToUDPBuilders.Add(filename, new UdpStreamBuilder());
                this._filesToTCPBuilders.Add(filename, new TcpSessionsBuilder());
            }

            if (this.ProcessFilesParallel)
            {
                this.processParallel(filesPaths);
            }
            else
            {
                foreach (var filePath in filesPaths)
                {
                    this.ProcessPcap(filePath);
                }
            }

            ProcessingFinished?.Invoke(this, new EventArgs());

        }

        private void processParallel(IEnumerable<string> filesPaths)
        {
            filesPaths.AsParallel().ForAll(f => this.ProcessPcap(f));          
        }

        private void invokeAndClear(string filename, object session)
        {
            if(session is TcpSession)
            {
                this._filesToTCPBuilders[filename].ClearSession((TcpSession)session);
                TcpSessionArrived?.Invoke(this, new TcpSessionArivedEventArgs()
                {
                    TcpSession = (TcpSession)session
                });
            }
            
            if (session is UdpSession)
            {
                this._filesToUDPBuilders[filename].ClearSession((UdpSession)session);
                UdpSessionArrived?.Invoke(this, new UdpSessionArrivedEventArgs()
                {
                    UdpSession = (UdpSession)session
                });
                
            }
        }
        

        public void ProcessPcap(string filePath)
        {
            try
            {
                RaiseFileProcessingStatusChangedEvent(FileProcessingStatus.Started, filePath);

                // check if the file is a PcapNg format file or Pcap format

                if (IsPcapFile(filePath))
                {
                    ReadPcapFile(filePath);
                }
                else
                {
                    // TODO: Enable this after testing PCAPNG 
                    //ReadPcapNGFile(filePath);
                }
                string filename = filePath.Substring(filePath.LastIndexOf(@"\") + 1);
                
                this._filesToUDPBuilders[filename].Sessions.AsParallel().ForAll(session => invokeAndClear(filename ,session));
                this._filesToTCPBuilders[filename].Sessions.AsParallel().ForAll(session => invokeAndClear(filename, session));
                
                
                _filesToUDPBuilders.Remove(filename);
                _filesToTCPBuilders.Remove(filename);


                _processingPrecentsPredicator.NotifyAboutProcessedFile(new FileInfo(filePath));
                RaiseFileProcessingStatusChangedEvent(FileProcessingStatus.Finished, filePath);
            }
            catch (Exception ex)
            {
                RaiseFileProcessingStatusChangedEvent(FileProcessingStatus.Faild, filePath);
            }
        }

        public bool IsPcapFile(string filename)
        {
            
            using (var reader = IReaderFactory.GetReader(filename))
            if (reader.GetType() == typeof(Haukcode.PcapngUtils.PcapNG.PcapNGReader))
            {   
                return false;
            }
                return true;
        }

        private void ReadPcapNGFile(string filepath)
        {
            using (var reader = IReaderFactory.GetReader(filepath))
            {
                reader.OnReadPacketEvent += ConvertPacket;
                reader.ReadPackets(new CancellationToken());
            }
        }

        private void ReadPcapFile(string filepath)
        {
            // Get an offline device, handle packets registering for the Packet 
            // Arrival event and start capturing from that file.
            // NOTE: the capture function is blocking.
            ICaptureDevice device = new CaptureFileReaderDevice(filepath);
            device.OnPacketArrival += new PacketArrivalEventHandler(ProcessPcapPacket);
            device.Open();
            device.Capture();
        }
        private void ConvertPacket(object sender, IPacket packet)
        {
            
            var _packet = PacketDotNet.Packet.ParsePacket(PacketDotNet.LinkLayers.Ethernet, packet.Data);
            ProccessPcapNgPacket(_packet);
        }

        private void RaiseFileProcessingStatusChangedEvent(FileProcessingStatus status, string filePath)
        {
            FileProcessingStatusChanged?.Invoke(this, new FileProcessingStatusChangedEventArgs()
            {
                FilePath = filePath,
                Status = status
            });
        }

        private void ProccessPcapNgPacket(PacketDotNet.Packet packet)
        {
            ProcessPacket(packet, "");
        }

        private void ProcessPcapPacket(object sender, CaptureEventArgs e)
        {
            CaptureFileReaderDevice device = (CaptureFileReaderDevice)sender;
            string filename = device.FileName;
            var packet = PacketDotNet.Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            ProcessPacket(packet, filename);
        }
        void ProcessPacket(PacketDotNet.Packet packet, string filename)
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
                        UdpStreamBuilder udpBuilder = _filesToUDPBuilders[filename];
                        udpBuilder.HandlePacket(udpPacket);
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
                        TcpSessionsBuilder tcpBuilder = _filesToTCPBuilders[filename];
                        tcpBuilder.HandlePacket(tcpPacket);
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
