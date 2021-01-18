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
        private Dictionary<string, string> CliModulesNamesToAnalyzerNames = new Dictionary<string, string> {
            { "FileExtracting" , "File Extracting"},
            { "NetworkMap", "Network Map" },
            { "Credentials" ,"Credentials Extractor (Passwords, Hashes)"}
        };
        private PcapProcessor.Processor _processor;
        private PcapAnalyzer.Analyzer _analyzer;
        private string[] _args;

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

        private void parseArgs(CliFlags cliFlags)
        {
            if (ArgsDoesNotExist(cliFlags))
            {
                interactiveMode();
            }
            else
            {
                // run in single command 
                try
                {
                    // load modules
                    if (cliFlags.Modules != null)
                    {
                        LoadModules(parseCliModuleNames(cliFlags.Modules));
                    }

                    SingleCommandCli cli = new SingleCommandCli(_analyzer, _processor, cliFlags);
                    cli.Run();
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
        private List<string> parseCliModuleNames(IEnumerable<string> modules)
        {
            List<string> analyzerModulesToLoad = new List<string>();

            foreach(var cliModuleName in modules)
            {
                string analyzerModuleName = CliModulesNamesToAnalyzerNames.GetValueOrDefault(cliModuleName, defaultValue: null);
                
                if (analyzerModuleName != null)
                {
                    analyzerModulesToLoad.Add(analyzerModuleName);
                }
            }

            return analyzerModulesToLoad;
        }
        private bool ArgsDoesNotExist(CliFlags cliFlags)
        {
            if (cliFlags.InputDir == null && cliFlags.InputFiles.Count() == 0 && cliFlags.Modules.Count() == 0 && cliFlags.OutputDir == null && cliFlags.ParallelProcessing == false)
            {
                return true;
            }
            return false;

        }
        

        private void interactiveMode()
        {
            LoadModules(_analyzer.AvailableModulesNames);
            CliShell shell = new CliShell(_analyzer, _processor, seperator: "Brute-Shark > ");
            shell.Start();
        }
        private void LoadModules(List<string> modules)
        {
            foreach (string m in modules)
            {
                _analyzer.AddModule(m);
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
            Parser.Default.ParseArguments<CliFlags>(_args).WithParsed(parseArgs);
        }
    }
}
