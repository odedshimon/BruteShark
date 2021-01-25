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
            _processor.UdpPacketArived += (s, e) => _analyzer.Analyze(CommonUi.Casting.CastProcessorUdpPacketToAnalyzerUdpPacket(e.Packet));
            _processor.TcpPacketArived += (s, e) => _analyzer.Analyze(CommonUi.Casting.CastProcessorTcpPacketToAnalyzerTcpPacket(e.Packet));
            _processor.TcpSessionArrived += (s, e) => _analyzer.Analyze(CommonUi.Casting.CastProcessorTcpSessionToAnalyzerTcpSession(e.TcpSession));
            _processor.UdpSessionArrived += (s, e) => _analyzer.Analyze(CommonUi.Casting.CastProcessorUdpStreamToAnalyzerUdpStream(e.UdpSession));
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
