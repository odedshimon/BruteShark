using PcapAnalyzer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BruteSharkCli
{
    class CliShell
    {
        private int _tcpSessionsCount;
        private int _udpStreamsCount;
        private ulong _tcpPacketsCount;
        private ulong _udpPacketsCount;
        private object _printingLock;
        private bool _exit;

        private List<string> _files;
        private List<CliShellCommand> _commands;
        private HashSet<PcapAnalyzer.NetworkHash> _hashes;
        private HashSet<PcapAnalyzer.NetworkPassword> _passwords;
        private HashSet<PcapAnalyzer.NetworkConnection> _connections;
        private List<VoipCallPresentation> _voipCalls;

        private PcapAnalyzer.Analyzer _analyzer;
        private PcapProcessor.Processor _processor;

        public string Seperator { get; set; }


        public CliShell(PcapAnalyzer.Analyzer analyzer, PcapProcessor.Processor processor, string seperator = ">")
        {
            _tcpPacketsCount = 0;
            _udpPacketsCount = 0;
            _udpStreamsCount = 0;
            _tcpSessionsCount = 0;

            this.Seperator = seperator;
            _printingLock = new object();
            _files = new List<string>();

            _processor = processor;
            _analyzer = analyzer;

            _analyzer.ParsedItemDetected += OnParsedItemDetected;
            _processor.TcpPacketArived += (s, e) => this.UpdateTcpPacketsCount();
            _processor.UdpPacketArived += (s, e) => this.UpdateUdpPacketsCount();
            _processor.TcpSessionArrived += (s, e) => this.UpdateTcpSessionsCount();
            _processor.UdpSessionArrived += (s, e) => this.UpdateUdpStreamsCount();

            _hashes = new HashSet<PcapAnalyzer.NetworkHash>();
            _passwords = new HashSet<PcapAnalyzer.NetworkPassword>();
            _connections = new HashSet<PcapAnalyzer.NetworkConnection>();
            _voipCalls = new List<VoipCallPresentation>();
            
            this._commands = new List<CliShellCommand>();
            AddCommand(new CliShellCommand("add-file", p => AddFile(p), "Add file to analyze. Usage: add-file <FILE-PATH>"));
            AddCommand(new CliShellCommand("start", p => StartAnalyzing(), "Start analyzing"));
            AddCommand(new CliShellCommand("show-passwords", p => PrintPasswords(), "Print passwords."));
            AddCommand(new CliShellCommand("show-modules", p => PrintModules(), "Print modules."));
            AddCommand(new CliShellCommand("show-hashes", p => PrintHashes(), "Print Hashes"));
            AddCommand(new CliShellCommand("show-networkmap", p => PrintNetworkMap(), "Prints the network map as a json string. Usage: show-networkmap"));
            AddCommand(new CliShellCommand("export-hashes", p => Utilities.ExportHashes(p, _hashes), "Export all Hashes to Hascat format input files. Usage: export-hashes <OUTPUT-DIRECTORY>"));
            AddCommand(new CliShellCommand("export-networkmap", p => CommonUi.Exporting.ExportNetworkMap(p, _connections), "Export network map to a json file for neo4j. Usage: export-networkmap <OUTPUT-file>"));
            AddCommand(new CliShellCommand("show-voip-calls", p => PrintVoipCalls(), "Prints the detected VoIP calls"));

            // Add the help command
            this.AddCommand(new CliShellCommand(
                "help",
                 param => this.PrintCommandsWithDescription(),
                 "Print help menu"));

            // Add the exit command
            this.AddCommand(new CliShellCommand(
                "exit",
                 param => this._exit = true,
                 "Exit CLI"));

            LoadModules(_analyzer.AvailableModulesNames);
        }

        private void PrintVoipCalls()
        {
            this._voipCalls.ToDataTable().Print();
        }

        private void LoadModules(List<string> modules)
        {
            foreach (string m in modules)
            {
                _analyzer.AddModule(m);
            }
        }

        private void PrintModules()
        {
            foreach (string module in this._analyzer.AvailableModulesNames)
            {
                Console.WriteLine($" - {module}");
            }
        }

        public void AddCommand(CliShellCommand cliShellCommand)
        {
            this._commands.Add(cliShellCommand);
        }

        public void PrintCommandsWithDescription()
        {
            this._commands.ToDataTable().Print();
        }

        internal void Start()
        {

            Utilities.PrintBruteSharkAsciiArt();
            RunCommand("help");
            _exit = false;

            do
            {
                HandleUserInput();
            }
            while (!_exit);
        }

        private void HandleUserInput()
        {
            bool legalInput;

            do
            {
                Console.Write(this.Seperator);
                string userInput = Console.ReadLine();
                legalInput = RunCommand(userInput);
            }
            while (!legalInput);
        }

        public bool RunCommand(string userInput)
        {
            bool legalInput = true;
            string[] inputParts = userInput.Split();

            CliShellCommand wanted_command = this._commands.Where(c => c.Keyword == inputParts[0]).FirstOrDefault();

            if (wanted_command != null)
            {
                // Run command.
                string commandData = userInput.Substring(userInput.IndexOf(" ") + 1);
                wanted_command.Action(commandData);
            }
            else
            {
                Console.WriteLine("Illegal Input.");
                legalInput = false;
            }

            return legalInput;
        }

        private void PrintPasswords()
        {
            _passwords.ToDataTable().Print();
        }

        private void PrintHashes()
        {
            this._hashes.ToDataTable(itemLengthLimit: 15).Print();
        }

        private void PrintNetworkMap()
        {
            Console.WriteLine(CommonUi.Exporting.GetNetworkMapAsJsonString(this._connections));
        }

        private void StartAnalyzing()
        {
            _processor.ProcessPcaps(this._files);
            Console.SetCursorPosition(0, Console.CursorTop + 5);
        }

        private void UpdateTcpSessionsCount()
        {
            ++_tcpSessionsCount;
            UpdateAnalyzingStatus();
        }

        private void UpdateUdpStreamsCount()
        {
            ++_udpStreamsCount;
            UpdateAnalyzingStatus();
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
                Console.WriteLine($"\r[+] Packets Analyzed: {_tcpPacketsCount + _udpPacketsCount}, " + $"TCP: {_tcpPacketsCount} " + $"UDP: {_udpPacketsCount}");
                Console.WriteLine($"\r[+] TCP Sessions Analyzed: {_tcpSessionsCount}" + $" UDP Streams Analyzed: {_udpStreamsCount}");
                Console.WriteLine($"\r[+] Passwords Found: {_passwords.Count}");
                Console.WriteLine($"\r[+] Hashes Found: {_hashes.Count}");
                Console.WriteLine($"\r[+] Network Connections Found: {_connections.Count}");
                Console.SetCursorPosition(0, Console.CursorTop - 5);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private void AddFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                _files.Add(filePath);
            }
            else
            {
                Console.WriteLine("File does not exist.");
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
            if (e.ParsedItem is PcapAnalyzer.NetworkConnection)
            {
                var networkConnection = e.ParsedItem as NetworkConnection;
                _connections.Add(networkConnection);
            }
            if (e.ParsedItem is PcapAnalyzer.VoipCall)
            {
                var voipCall = e.ParsedItem as VoipCall;
                _voipCalls.Add(VoipCallPresentation.FromAnalyzerVoipCall(voipCall));
            }

                UpdateAnalyzingStatus();
        }
       
    } 
}



