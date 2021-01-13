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
    public enum FileType
    {
        Pcap,
        PcapNG
    }

    // TODO: use interface
    public class Processor
    {
        public delegate void FileProcessingStatusChangedEventHandler(object sender, FileProcessingStatusChangedEventArgs e);
        public event FileProcessingStatusChangedEventHandler FileProcessingStatusChanged;
        public delegate void ProcessingPrecentsChangedEventHandler(object sender, ProcessingPrecentsChangedEventArgs e);
        private ProcessingPrecentsPredicator _processingPrecentsPredicator;
        public delegate void UdpPacketArivedEventHandler(object sender, UdpPacketArivedEventArgs e);
        public event UdpPacketArivedEventHandler UdpPacketArived;
        public delegate void UdpSessionArrivedEventHandler(object sender, UdpSessionArrivedEventArgs e);
        public event UdpSessionArrivedEventHandler UdpSessionArrived;
        public delegate void TcpPacketArivedEventHandler(object sender, TcpPacketArivedEventArgs e);
        public event TcpPacketArivedEventHandler TcpPacketArived;
        public delegate void TcpSessionArivedEventHandler(object sender, TcpSessionArivedEventArgs e);
        public event TcpSessionArivedEventHandler TcpSessionArrived;
        public bool ProcessFilesParallel { get; set; }
        public bool BuildTcpSessions { get; set; }
        public bool BuildUdpSessions { get; set; }

        private TcpSessionsBuilder _tcpSessionsBuilder;
        private UdpStreamBuilder _udpStreamsBuilder;
        private string _filepath;
        


        public Processor(ProcessingPrecentsPredicator processingPrecentsPredicator, string filepath)
        {
            
            this._filepath = filepath;
            _processingPrecentsPredicator = processingPrecentsPredicator;
            this.ProcessFilesParallel = false;
            this.BuildTcpSessions = false;
            this.BuildUdpSessions = false;
            this._tcpSessionsBuilder = new TcpSessionsBuilder();
            this._udpStreamsBuilder = new UdpStreamBuilder();

            
        }

        private void invokeAndClear(object session)
        {
            if(session is TcpSession)
            {
                this._tcpSessionsBuilder.ClearSession((TcpSession)session);
                TcpSessionArrived?.Invoke(this, new TcpSessionArivedEventArgs()
                {
                    TcpSession = (TcpSession)session
                });
            }
            
            if (session is UdpSession)
            {
                this._udpStreamsBuilder.ClearSession((UdpSession)session);
                UdpSessionArrived?.Invoke(this, new UdpSessionArrivedEventArgs()
                {
                    UdpSession = (UdpSession)session
                });
                
            }
        }
        

        public void ProcessPcap()
        {
            try
            {
                RaiseFileProcessingStatusChangedEvent(FileProcessingStatus.Started, this._filepath);

                switch (GetFileType(_filepath))
                {
                    case FileType.Pcap:
                        ReadPcapFile(_filepath);
                        break;
                    case FileType.PcapNG:
                        ReadPcapNGFile(_filepath);
                        break;
                }

                this._udpStreamsBuilder.Sessions.AsParallel().ForAll(session => invokeAndClear(session));
                this._tcpSessionsBuilder.Sessions.AsParallel().ForAll(session => invokeAndClear(session));



                _processingPrecentsPredicator.NotifyAboutProcessedFile(new FileInfo(this._filepath));

                RaiseFileProcessingStatusChangedEvent(FileProcessingStatus.Finished, _filepath);
            }
            catch (Exception ex)
            {
                RaiseFileProcessingStatusChangedEvent(FileProcessingStatus.Faild, _filepath);
            }
        }

        private FileType GetFileType(string filePath)
        {
            if (IsPcapFile(filePath))
            {
                return FileType.Pcap;
            }
            else
            {
                return FileType.PcapNG;
            }
        }

        public bool IsPcapFile(string filename)
        {
            using (var reader = IReaderFactory.GetReader(filename))
            {
                return reader.GetType() != typeof(Haukcode.PcapngUtils.PcapNG.PcapNGReader);
            }
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

            var _packet_ether = PacketDotNet.Packet.ParsePacket(PacketDotNet.LinkLayers.Ethernet, packet.Data);
            var _packet_raw = PacketDotNet.Packet.ParsePacket(PacketDotNet.LinkLayers.Raw, packet.Data);

            if (_packet_ether.HasPayloadPacket)
            {
                if (typeof(PacketDotNet.IPPacket).IsInstanceOfType(_packet_ether.PayloadPacket))
                {
                    ProccessPcapNgPacket(_packet_ether);
                }
            }
            else if (_packet_raw.HasPayloadPacket)
            {
                if (typeof(PacketDotNet.IPPacket).IsInstanceOfType(_packet_raw.PayloadPacket))
                {
                    ProccessPcapNgPacket(_packet_raw);
                }

            }
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
            ProcessPacket(packet);
        }

        private void ProcessPcapPacket(object sender, CaptureEventArgs e)
        {
            var packet = PacketDotNet.Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            ProcessPacket(packet);
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
                        
                        _udpStreamsBuilder.HandlePacket(udpPacket);
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
                        _tcpSessionsBuilder.HandlePacket(tcpPacket);
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
