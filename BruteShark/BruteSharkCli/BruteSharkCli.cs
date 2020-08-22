using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PcapAnalyzer;

namespace BruteSharkCli
{
    internal class BruteSharkCli
    {
        private ulong _tcpPacketsCount;
        private ulong _udpPacketsCount;
        private int _tcpSessionsCount;
        private int _udpStreamsCount;
        private PcapProcessor.Processor _processor;
        private PcapAnalyzer.Analyzer _analyzer;
        private List<string> _files;
        private HashSet<PcapAnalyzer.NetworkPassword> _passwords;
        private HashSet<PcapAnalyzer.NetworkHash> _hashes;
        private object _printingLock;
        private CliShell _shell;
        

        public BruteSharkCli()
        {
            _tcpPacketsCount = 0;
            _tcpSessionsCount = 0;
            _printingLock = new object();
            _passwords = new HashSet<PcapAnalyzer.NetworkPassword>();
            _hashes = new HashSet<NetworkHash>();
            _files = new List<string>();

            _processor = new PcapProcessor.Processor();
            _analyzer = new PcapAnalyzer.Analyzer();
            _shell = new CliShell(seperator:"Brute-Shark > ");

            // TODO: create command for this.
            _processor.BuildTcpSessions = true;
            LoadAllModules();

            // Contract the events.
            _processor.UdpPacketArived += (s, e) => _analyzer.Analyze(CastProcessorUdpPacketToAnalyzerUdpPacket(e.Packet));
            _processor.TcpPacketArived += (s, e) => _analyzer.Analyze(CastProcessorTcpPacketToAnalyzerTcpPacket(e.Packet));
            _processor.TcpPacketArived += (s, e) => this.UpdateTcpPacketsCount();
            _processor.UdpPacketArived += (s, e) => this.UpdateUdpPacketsCount();
            _processor.TcpSessionArived += (s, e) => this.UpdateTcpSessionsCount();
            _processor.UdpStreamArrived += (s, e) => this.UpdateUdpStreamsCount();
            _processor.TcpSessionArived += (s, e) => _analyzer.Analyze(CastProcessorTcpSessionToAnalyzerTcpSession(e.TcpSession));
            _analyzer.ParsedItemDetected += OnParsedItemDetected;

            // Add commands to the Cli Shell.
            _shell.AddCommand(new CliShellCommand("add-file", p => _files.Add(p), "Add file to analyze. Usage: add-file <FILE-PATH>"));
            _shell.AddCommand(new CliShellCommand("start", p => StartAnalyzing(), "Start analyzing"));
            _shell.AddCommand(new CliShellCommand("show-passwords", p => PrintPasswords(), "Print passwords."));
            _shell.AddCommand(new CliShellCommand("show-hashes", p => PrintHashes(), "Print Hashes"));
            _shell.AddCommand(new CliShellCommand("export-hashes", p => ExportHashes(p), "Export all Hashes to Hascat format input files. Usage: export-hashes <OUTPUT-DIRECTORY>"));

        }

        private void LoadAllModules()
        {
            foreach (string m in _analyzer.AvailableModulesNames)
            {
                _analyzer.AddModule(m);
            }
        }

        private void OnParsedItemDetected(object sender, ParsedItemDetectedEventArgs e)
        {
            if (e.ParsedItem is PcapAnalyzer.NetworkPassword)
            {
                _passwords.Add(e.ParsedItem as PcapAnalyzer.NetworkPassword);
            }
            if (e.ParsedItem is PcapAnalyzer.NetworkHash)
            {
                _hashes.Add(e.ParsedItem as PcapAnalyzer.NetworkHash);
            }

            UpdateAnalyzingStatus();
        }

        private void UpdateTcpSessionsCount()
        {
            if (++_tcpSessionsCount % 10 == 0)
            {
                UpdateAnalyzingStatus();
            }
        }

        private void UpdateUdpStreamsCount()
        {
            if (++_udpStreamsCount% 10 == 0)
            {
                UpdateAnalyzingStatus();
            }
        }
        private void UpdateTcpPacketsCount()
        {
            if (++_tcpPacketsCount % 10 == 0)
            {
                UpdateAnalyzingStatus();
            }
        }
        private void UpdateUdpPacketsCount()
        {
            if (++_udpPacketsCount % 10 == 0)
            {
                UpdateAnalyzingStatus();
            }
        }
        private void UpdateAnalyzingStatus()
        {
            lock (_printingLock)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"\r[+] TCP Packets Analyzed: {++_tcpPacketsCount}");
                Console.WriteLine($"\r[+] UDP Packets Analyzed: {++_udpPacketsCount}");
                Console.WriteLine($"\r[+] TCP Sessions Analyzed: {++_tcpSessionsCount}");
                Console.WriteLine($"\r[+] UDP Streams Analyzed: {++_udpStreamsCount}");
                Console.WriteLine($"\r[+] Passwords Found: {_passwords.Count}");
                Console.WriteLine($"\r[+] Hashes Found: {_hashes.Count}");
                Console.SetCursorPosition(0, Console.CursorTop - 4);
                Console.ForegroundColor = ConsoleColor.White;
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

        internal void Start()
        {
            Utilities.PrintBruteSharkAsciiArt();
            _shell.RunCommand("help");
            _shell.Start();
        }

        private void PrintPasswords()
        {
            _passwords.ToDataTable().Print();
        }

        private void PrintHashes()
        {
            this._hashes.ToDataTable(itemLengthLimit:15).Print();
        }

        private void StartAnalyzing()
        {
            _processor.ProcessPcaps(this._files);
            Console.SetCursorPosition(0, Console.CursorTop + 4);
        }

        public string MakeUnique(string path)
        {
            string dir = Path.GetDirectoryName(path);
            string fileName = Path.GetFileNameWithoutExtension(path);
            string fileExt = Path.GetExtension(path);

            for (int i = 1; ; ++i)
            {
                if (!File.Exists(path))
                    return new FileInfo(path).FullName;

                path = Path.Combine(dir, fileName + " " + i + fileExt);
            }
        }

        private void ExportHashes(string filePath)
        {
            // Run on each Hash Type we found.
            foreach (string hashType in _hashes.Select(h => h.HashType).Distinct())
            {
                // Convert all hashes from that type to Hashcat format.
                var hashesToExport = _hashes.Where(h => (h as PcapAnalyzer.NetworkHash).HashType == hashType)
                                            .Select(h => BruteForce.Utilities.ConvertToHashcatFormat(
                                                         Casting.CastAnalyzerHashToBruteForceHash(h)));

                var outputFilePath = MakeUnique(Path.Combine(filePath, $"Brute Shark - {hashType} Hashcat Export.txt"));

                using (var streamWriter = new StreamWriter(outputFilePath, true))
                {
                    foreach (var hash in hashesToExport)
                    {
                        streamWriter.WriteLine(hash);
                    }
                }

                Console.WriteLine("Hashes file created: " + outputFilePath);
            }
        }
    
    }
}
