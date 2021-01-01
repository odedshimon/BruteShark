using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PcapProcessor
{
    public class ProcessorEngine
    {
        public bool ProcessFilesParallel { get; set; }
        public bool BuildTcpSessions { get; set; }
        public bool BuildUdpSessions { get; set; }

        private ProcessingPrecentsPredicator _processingPrecentsPredicator;

        public event EventHandler ProcessingFinished;
        public delegate void UdpPacketArivedEventHandler(object sender, UdpPacketArivedEventArgs e);
        public event UdpPacketArivedEventHandler UdpPacketArived;
        public delegate void UdpSessionArrivedEventHandler(object sender, UdpSessionArrivedEventArgs e);
        public event UdpSessionArrivedEventHandler UdpSessionArrived;
        public delegate void TcpPacketArivedEventHandler(object sender, TcpPacketArivedEventArgs e);
        public event TcpPacketArivedEventHandler TcpPacketArived;
        public delegate void TcpSessionArivedEventHandler(object sender, TcpSessionArivedEventArgs e);
        public event TcpSessionArivedEventHandler TcpSessionArrived;
        public delegate void FileProcessingStatusChangedEventHandler(object sender, FileProcessingStatusChangedEventArgs e);
        public event FileProcessingStatusChangedEventHandler FileProcessingStatusChanged;

        private List<Processor> _processors;
        public delegate void ProcessingPrecentsChangedEventHandler(object sender, ProcessingPrecentsChangedEventArgs e);
        public event ProcessingPrecentsChangedEventHandler ProcessingPrecentsChanged;
        public ProcessorEngine(bool processFilesParallel)
        {
            this._processors = new List<Processor>();
            _processingPrecentsPredicator = new ProcessingPrecentsPredicator();
            _processingPrecentsPredicator.ProcessingPrecentsChanged += OnPredicatorProcessingPrecentsChanged;
            this.ProcessFilesParallel = processFilesParallel;

        }

        public void ProcessPcaps(IEnumerable<string> filesPaths)
        {
            _processingPrecentsPredicator.AddFiles(new HashSet<FileInfo>(filesPaths.Select(fp => new FileInfo(fp))));

            foreach (var filepath in filesPaths)
            {
                Processor processor = new Processor(_processingPrecentsPredicator, filepath);
                processor.BuildTcpSessions = BuildTcpSessions;
                processor.BuildUdpSessions= BuildUdpSessions;
                processor.UdpPacketArived += udpPacketArrived;
                processor.TcpPacketArived += tcpPacketArrived;
                processor.TcpSessionArrived+= tcpSessionArrived;
                processor.UdpSessionArrived += udpSessionArrived;
                processor.FileProcessingStatusChanged += fileProcessingStatusChanged;
                _processors.Add(processor);

            }
            
            if(!this.ProcessFilesParallel)
            {
                foreach (var processor in _processors)
                {

                    processor.ProcessPcap();
                }
            }
            else
            {
                this.processParallel();
            }

            ProcessingFinished?.Invoke(this, new EventArgs());

        }

        private void fileProcessingStatusChanged(object sender, FileProcessingStatusChangedEventArgs e)
        {
            FileProcessingStatusChanged?.Invoke(this, new FileProcessingStatusChangedEventArgs()
            {
                FilePath = e.FilePath,
                Status = e.Status
            });
        }

        private void udpSessionArrived(object sender, UdpSessionArrivedEventArgs e)
        {
            
            UdpSessionArrived?.Invoke(this, new UdpSessionArrivedEventArgs()
            {
                UdpSession = e.UdpSession 
            });
        }

        private void tcpSessionArrived(object sender, TcpSessionArivedEventArgs e)
        {
            TcpSessionArrived?.Invoke(this, new TcpSessionArivedEventArgs()
            {
                TcpSession = e.TcpSession
            });
        }

        private void tcpPacketArrived(object sender, TcpPacketArivedEventArgs e)
        {
            TcpPacketArived?.Invoke(this, new TcpPacketArivedEventArgs
            {
       
                Packet = e.Packet
            });
        }

        private void udpPacketArrived(object sender, UdpPacketArivedEventArgs e)
        {
            UdpPacketArived?.Invoke(this, new UdpPacketArivedEventArgs
            {
                Packet = e.Packet

            });
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

        private void processParallel()
        {
            _processors.AsParallel().ForAll(p => p.ProcessPcap());
        }
    }
}
