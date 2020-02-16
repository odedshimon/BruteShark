using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PcapAnalyzer;

namespace BruteSharkCli
{
    internal class Cli
    {
        private ulong _tcpPacketsCount;
        private int _tcpSessionsCount;
        private PcapProcessor.Processor _processor;
        private PcapAnalyzer.Analyzer _analyzer;
        private List<string> _files;
        private HashSet<PcapAnalyzer.NetworkPassword> _passwords;
        private HashSet<PcapAnalyzer.NetworkHash> _hashes;
        private object _printingLock;
        

        public Cli()
        {
            _tcpPacketsCount = 0;
            _tcpSessionsCount = 0;
            _printingLock = new object();
            _passwords = new HashSet<PcapAnalyzer.NetworkPassword>();
            _hashes = new HashSet<NetworkHash>();
            _files = new List<string>();

            _processor = new PcapProcessor.Processor();
            _analyzer = new PcapAnalyzer.Analyzer();

            // TODO: create command for this.
            _processor.BuildTcpSessions = true;

            // Contract the events.
            _processor.TcpPacketArived += (s, e) => _analyzer.Analyze(CastProcessorTcpPacketToAnalyzerTcpPacket(e.Packet));
            _processor.TcpPacketArived += (s, e) => this.UpdateTcpPacketsCount();
            _processor.TcpSessionArived += (s, e) => this.UpdateTcpSessionsCount();
            _processor.TcpSessionArived += (s, e) => _analyzer.Analyze(CastProcessorTcpSessionToAnalyzerTcpSession(e.TcpSession));
            _analyzer.ParsedItemDetected += OnParsedItemDetected;
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

        private void UpdateTcpPacketsCount()
        {
            if (++_tcpPacketsCount % 10 == 0)
            {
                UpdateAnalyzingStatus();
            }
        }

        private void UpdateAnalyzingStatus()
        {
            lock (_printingLock)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"\r[+] Packets Analyzed: {++_tcpPacketsCount}");
                Console.WriteLine($"\r[+] Sessions Analyzed: {++_tcpSessionsCount}");
                Console.WriteLine($"\r[+] Passwords Found: {_passwords.Count}");
                Console.WriteLine($"\r[+] Hashes Found: {_hashes.Count}");
                Console.SetCursorPosition(0, Console.CursorTop - 4);
                Console.ForegroundColor = ConsoleColor.White;
            }
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
            bool exit = true;

            do
            {
                exit = HandleUserInput();
            }
            while (!exit);
        }

        private void PrintPasswords()
        {
            _passwords.ToDataTable().Print();
        }

        private void PrintHashes()
        {
            this._hashes.ToDataTable().Print();
        }

        private bool HandleUserInput()
        {
            var result = false;
            bool legalInput;

            // TODO: refactor this (verify, organize, catch exceptions).
            do
            {
                PrintCli();
                legalInput = true;
                string userInput = Console.ReadLine();
                string[] inputParts = userInput.Split();

                switch (inputParts[0])
                {
                    case "start":
                        _processor.ProcessPcaps(this._files);
                        Console.SetCursorPosition(0, Console.CursorTop + 4);
                        break;
                    case "add-file":
                        this._files.Add(userInput.Substring(9));
                        break;
                    case "exit":
                        result = true;
                        break;
                    case "show-passwords":
                        PrintPasswords();
                        break;
                    case "show-hashes":
                        PrintHashes();
                        break;
                    default:
                        Console.WriteLine("Illegal Input.");
                        legalInput = false;
                        break;
                }
            }
            while (!legalInput);

            return result;
        }

        private void PrintCli()
        {
            Console.Write("Brute-Shark > ");
        }
    }
}
