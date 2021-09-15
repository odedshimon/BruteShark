using System;
using System.Collections.Generic;
using System.Text;
using PcapProcessor;
using PcapAnalyzer;
using System.IO;
using System.Linq;
using CommandLine;
using CommonUi;
using System.Reflection;

namespace BruteSharkCli
{
    class SingleCommandRunner
    {
        private SingleCommandFlags _cliFlags;
        private List<string> _files;
        private CommonUi.NetworkContext _networkContext;
        private HashSet<PcapAnalyzer.NetworkFile> _extractedFiles;
        private HashSet<PcapAnalyzer.NetworkPassword> _passwords;
        private HashSet<PcapAnalyzer.NetworkHash> _hashes;
        //private HashSet<PcapAnalyzer.NetworkConnection> _connections;
        private HashSet<CommonUi.VoipCall> _voipCalls;
        private HashSet<PcapAnalyzer.DnsNameMapping> _dnsMappings;

        private Sniffer _sniffer; 
        private PcapProcessor.Processor _processor;
        private PcapAnalyzer.Analyzer _analyzer;

        private readonly Dictionary<string, string> CliModulesNamesToAnalyzerNames = new Dictionary<string, string> {
            { "FileExtracting" , "File Extracting"},
            { "NetworkMap", "Network Map" },
            { "Credentials" ,"Credentials Extractor (Passwords, Hashes)"},
            { "Voip" ,"Voip Calls"},
            { "DNS", "DNS"}
        };

        public SingleCommandRunner(Analyzer analyzer, Processor processor, Sniffer sniffer, string[] args)
        {
            _sniffer = sniffer;
            _analyzer = analyzer;
            _processor = processor;
            _files = new List<string>();

            _networkContext = new NetworkContext();
            _hashes = new HashSet<PcapAnalyzer.NetworkHash>();
            _passwords = new HashSet<NetworkPassword>();
            _extractedFiles = new HashSet<NetworkFile>();
            _voipCalls = new HashSet<CommonUi.VoipCall>();
            _dnsMappings = new HashSet<PcapAnalyzer.DnsNameMapping>();


            _analyzer.ParsedItemDetected += OnParsedItemDetected;
            _analyzer.UpdatedItemProprertyDetected += UpdatedPropertyInItemDetected;

            _processor.ProcessingFinished += (s, e) => this.ExportResults();
            _processor.FileProcessingStatusChanged += (s, e) => this.PrintFileStatusUpdate(s, e);

            // This is done to catch Ctrl + C key press by the user.
            Console.CancelKeyPress += (s, e) => {this.ExportResults(); Environment.Exit(0);};

            // Parse user arguments.
            CommandLine.Parser.Default.ParseArguments<SingleCommandFlags>(args).WithParsed<SingleCommandFlags>((cliFlags) => _cliFlags = cliFlags);
        }

        public void Run()
        {
            try
            {
                SetupRun();

                if (_cliFlags.CaptureDevice != null)
                {
                    SetupSniffer();

                    CliPrinter.Info(_sniffer.PromisciousMode ?
                        $"Started analyzing packets from {_cliFlags.CaptureDevice} device (Promiscious mode) - Press Ctrl + C to stop" :
                        $"Started analyzing packets from {_cliFlags.CaptureDevice} device - Press Ctrl + C to stop");
                    
                    _sniffer.StartSniffing(new System.Threading.CancellationToken());
                }
                else 
                {
                    CliPrinter.Info($"Start analyzing {_files.Count} files");
                    _processor.ProcessPcaps(_files);
                }
            }
            catch (Exception ex)
            {
                CliPrinter.Error(ex);
            }
        }

        private void SetupSniffer()
        {
            if (!_sniffer.AvailiableDevicesNames.Contains(_cliFlags.CaptureDevice))
            {
                CliPrinter.Error($"No such device: {_cliFlags.CaptureDevice}");
                Environment.Exit(0);
            }

            _sniffer.SelectedDeviceName = _cliFlags.CaptureDevice;

            if (_cliFlags.PromisciousMode)
            {
                _sniffer.PromisciousMode = true;
            }

            if (_cliFlags.CaptrueFilter != null)
            {
                if (!Sniffer.CheckCaptureFilter(_cliFlags.CaptrueFilter))
                {
                    CliPrinter.Error($"The capture filter: {_cliFlags.CaptrueFilter} is not a valid filter - filters must be in a bpf format");
                    Environment.Exit(0);
                }

                _sniffer.Filter = _cliFlags.CaptrueFilter;
            }
        }

        private void PrintFileStatusUpdate(object sender, FileProcessingStatusChangedEventArgs e)
        {
            if (e.Status == FileProcessingStatus.Started)
            {
                CliPrinter.Info($"Start processing file : {Path.GetFileName(e.FilePath)}");
            }
            else if (e.Status == FileProcessingStatus.Finished)
            {
                CliPrinter.Info($"Finished processing file : {Path.GetFileName(e.FilePath)}");
            }
            else if (e.Status == FileProcessingStatus.Faild)
            {
                CliPrinter.Error($"Failed to process file : {Path.GetFileName(e.FilePath)}");
            }
        }

        private void SetupRun()
        {
            // That can happen when the user enter vesion \ help commad, exit gracefully.
            if (_cliFlags is null)
            {
                Environment.Exit(0);
            }

            // Load modules.
            if (_cliFlags?.Modules?.Any() == true)
            {
                LoadModules(ParseCliModuleNames(_cliFlags.Modules));
            }
            else
            {
                throw new Exception("No mudules selected");
            }

            if (_cliFlags.InputFiles.Count() != 0 && _cliFlags.InputDir != null)
            {
                throw new Exception("Only one of the arguments -i and -d can be presented in a single command mode run");
            }
            else if (_cliFlags.InputFiles.Count() != 0)
            {
                foreach (string filePath in _cliFlags.InputFiles)
                {
                    AddFile(filePath);
                }
            }
            else if (_cliFlags.InputDir != null)
            {
                VerifyDir(_cliFlags.InputDir);
            }
        }

        private void LoadModules(List<string> modules)
        {
            foreach (string m in modules)
            {
                _analyzer.AddModule(m);
            }
        }

        private List<string> ParseCliModuleNames(IEnumerable<string> modules)
        {
            var analyzerModulesToLoad = new List<string>();

            foreach (var cliModuleName in modules)
            {
                string analyzerModuleName = CliModulesNamesToAnalyzerNames.GetValueOrDefault(cliModuleName, defaultValue: null);

                if (analyzerModuleName != null)
                {
                    analyzerModulesToLoad.Add(analyzerModuleName);
                }
            }

            return analyzerModulesToLoad;
        }

        private void VerifyDir(string dirPath)
        {
            FileAttributes attrs = File.GetAttributes(dirPath);
            if ((attrs & FileAttributes.Directory) == FileAttributes.Directory)
            {
                DirectoryInfo dir = new DirectoryInfo(dirPath);
                foreach (var file in dir.GetFiles("*.*"))
                {
                    AddFile(file.FullName);
                }
            }
            else
            {
                throw new IOException($"{dirPath} is not a valid directory path");
            }
        }

        private void ExportResults()
        {
            if (_cliFlags.OutputDir != null)
            { 
                if (_networkContext.Connections.Any())
                {
                    var networkMapFilePath = CommonUi.Exporting.ExportNetworkMap(_cliFlags.OutputDir, _networkContext.Connections);
                    CliPrinter.Info($"Successfully exported network map to json file: {networkMapFilePath}");
                    var nodesDataFilePath = CommonUi.Exporting.ExportNetworkNodesData(_cliFlags.OutputDir, _networkContext.GetAllNodes());
                    CliPrinter.Info($"Successfully exported network nodes data to json file: {nodesDataFilePath}");
                }
                if (_hashes.Any())
                {
                    Utilities.ExportHashes(_cliFlags.OutputDir, _hashes);
                    CliPrinter.Info($"Successfully exported hashes");
                }
                if (_files.Any())
                {
                    var dirPath = CommonUi.Exporting.ExportFiles(_cliFlags.OutputDir, _extractedFiles);
                    CliPrinter.Info($"Successfully exported extracted files to: {dirPath}");
                }
                if (_dnsMappings.Any())
                {
                    var dnsFilePath = CommonUi.Exporting.ExportDnsMappings(_cliFlags.OutputDir, _dnsMappings);
                    CliPrinter.Info($"Successfully exported DNS mappings to file: {dnsFilePath}");
                }
				if(_voipCalls.Any())
                {
                   var dirPath = CommonUi.Exporting.ExportVoipCalls(_cliFlags.OutputDir, _voipCalls);
                    CliPrinter.Info($"Successfully exported voip calls extracted to: {dirPath}");
                }
            }

            CliPrinter.Info("Bruteshark finished processing");
        }

        private void AddFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                _files.Add(filePath);
            }
            else
            {
                CliPrinter.Error($"File does not exist - {filePath}");
            }
        }

        private void UpdatedPropertyInItemDetected(object sender, UpdatedPropertyInItemeventArgs e)
        {
            if (e.ParsedItem is PcapAnalyzer.VoipCall)
            {
                var voipCall = CommonUi.Casting.CastAnalyzerVoipCallToPresentationVoipCall(e.ParsedItem as PcapAnalyzer.VoipCall);

                if (_voipCalls.Contains(voipCall))
                {
                    voipCall.GetType()
                        .GetProperty(e.PropertyChanged.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                        .SetValue(_voipCalls
                            .Where(c => c.Equals(voipCall))
                            .FirstOrDefault(), e.NewPropertyValue);

                    if (e.PropertyChanged.Name == "CallState" || e.PropertyChanged.Name == "RTPPort")
                    {
                        PrintUpdatedItem(_voipCalls.Where(c => c.Equals(voipCall)).First(), e.PropertyChanged.Name);
                    }
                }
            }
        }

        private void OnParsedItemDetected(object sender, ParsedItemDetectedEventArgs e)
        {
            if (e.ParsedItem is PcapAnalyzer.NetworkPassword)
            {
                if (_passwords.Add(e.ParsedItem as PcapAnalyzer.NetworkPassword))
                {
                    PrintDetectedItem(e.ParsedItem);
                }
            }
            else if (e.ParsedItem is PcapAnalyzer.NetworkHash)
            {
                if (_hashes.Add(e.ParsedItem as PcapAnalyzer.NetworkHash))
                {
                    PrintDetectedItem(e.ParsedItem);
                }
            }
            else if (e.ParsedItem is PcapAnalyzer.NetworkFile)
            {
                if (_extractedFiles.Add(e.ParsedItem as PcapAnalyzer.NetworkFile))
                {
                    PrintDetectedItem(e.ParsedItem);
                }
            }
            else if (e.ParsedItem is PcapAnalyzer.NetworkConnection)
            {
                var networkConnection = e.ParsedItem as NetworkConnection;
                _networkContext.Connections.Add(networkConnection);
            }
            else if (e.ParsedItem is PcapAnalyzer.VoipCall)
            {
                var voipCall = e.ParsedItem as PcapAnalyzer.VoipCall;
                CommonUi.VoipCall callPresentation = CommonUi.Casting.CastAnalyzerVoipCallToPresentationVoipCall(voipCall);
                PrintDetectedItem(callPresentation);
                _voipCalls.Add(callPresentation);
			}
            else if (e.ParsedItem is PcapAnalyzer.DnsNameMapping)
            {
                if (_dnsMappings.Add(e.ParsedItem as DnsNameMapping))
                {
                    PrintDetectedItem(e.ParsedItem);
                }
            }
        }

        private void PrintDetectedItem(object item)
        {
            CliPrinter.WriteLine(ConsoleColor.Blue, $"Found: {item}");
        }

        private void PrintUpdatedItem(object item, string propertyUpdatedName)
        {
            CliPrinter.WriteLine(ConsoleColor.Blue, $"Updated {propertyUpdatedName} for: {item}");
        }

    }
}
