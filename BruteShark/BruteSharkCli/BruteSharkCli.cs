using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using PcapAnalyzer;
using PcapProcessor;
using CommandLine;

namespace BruteSharkCli
{

    internal class BruteSharkCli
    {
        private PcapProcessor.Processor _processor;
        private PcapAnalyzer.Analyzer _analyzer;
        private readonly string[] _args;

        public BruteSharkCli(string[] args)
        {
            _args = args;            
            _processor = new PcapProcessor.Processor();
            _analyzer = new PcapAnalyzer.Analyzer();

            // TODO: create command for this.
            _processor.BuildTcpSessions = true;
            _processor.BuildUdpSessions = true;

            // Contract the events.
            _processor.UdpPacketArived += (s, e) => _analyzer.Analyze(CastProcessorUdpPacketToAnalyzerUdpPacket(e.Packet));
            _processor.TcpPacketArived += (s, e) => _analyzer.Analyze(CastProcessorTcpPacketToAnalyzerTcpPacket(e.Packet));
            _processor.TcpSessionArrived += (s, e) => _analyzer.Analyze(CastProcessorTcpSessionToAnalyzerTcpSession(e.TcpSession));
            _processor.UdpSessionArrived += (s, e) => _analyzer.Analyze(CastProcessorUdpStreamToAnalyzerUdpStream(e.UdpSession));
        }

        private void RunShellMode()
        {
            var shell = new CliShell(_analyzer, _processor, seperator: "Brute-Shark > ");
            shell.Start();
        }

        private void RunSingleCommand()
        {
            try
            {
                var cli = new SingleCommandRunner(_analyzer, _processor, _args);
                cli.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal Error: {ex.Message}");
            }
        }

        public static PcapAnalyzer.UdpPacket CastProcessorUdpPacketToAnalyzerUdpPacket(PcapProcessor.UdpPacket udpPacket)
        {
            return new PcapAnalyzer.UdpPacket()
            {
                SourceIp = udpPacket.SourceIp,
                DestinationIp = udpPacket.DestinationIp,
                SourcePort = udpPacket.SourcePort,
                DestinationPort = udpPacket.DestinationPort,
                Data = udpPacket.Data
            };
        }

        private PcapAnalyzer.TcpPacket CastProcessorTcpPacketToAnalyzerTcpPacket(PcapProcessor.TcpPacket tcpPacket)
        {
            return new PcapAnalyzer.TcpPacket()
            {
                SourceIp = tcpPacket.SourceIp,
                DestinationIp = tcpPacket.DestinationIp,
                SourcePort = tcpPacket.SourcePort,
                DestinationPort = tcpPacket.DestinationPort,
                Data = tcpPacket.Data
            };
        }

        private PcapAnalyzer.TcpSession CastProcessorTcpSessionToAnalyzerTcpSession(PcapProcessor.TcpSession tcpSession)
        {
            return new PcapAnalyzer.TcpSession()
            {
                SourceIp = tcpSession.SourceIp,
                DestinationIp = tcpSession.DestinationIp,
                SourcePort = tcpSession.SourcePort,
                DestinationPort = tcpSession.DestinationPort,
                Data = tcpSession.Data,
                Packets = tcpSession.Packets.Select(p => CastProcessorTcpPacketToAnalyzerTcpPacket(p)).ToList()
            };
        }

        private PcapAnalyzer.UdpStream CastProcessorUdpStreamToAnalyzerUdpStream(PcapProcessor.UdpSession udpStream)
        {
            return new PcapAnalyzer.UdpStream()
            {
                SourceIp = udpStream.SourceIp,
                DestinationIp = udpStream.DestinationIp,
                SourcePort = udpStream.SourcePort,
                DestinationPort = udpStream.DestinationPort,
                Data = udpStream.Data,
                Packets = udpStream.Packets.Select(p => CastProcessorUdpPacketToAnalyzerUdpPacket(p)).ToList()
            };
        }

        internal void Start()
        {
            if (_args.Length == 0)
            {
                RunShellMode();
            }
            else
            {
                RunSingleCommand();
            }
        }

    }
}
