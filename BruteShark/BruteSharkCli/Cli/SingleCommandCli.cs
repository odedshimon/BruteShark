using System;
using System.Collections.Generic;
using System.Text;
using PcapProcessor;
using PcapAnalyzer;
using System.IO;
using System.Linq;

namespace BruteSharkCli
{
    class SingleCommandCli
    {
        private PcapProcessor.Processor _processor;
        private PcapAnalyzer.Analyzer _analyzer;
        private CliFlags _cliFlags;
        private List<string> _files;
        private HashSet<PcapAnalyzer.NetworkPassword> _passwords;
        private HashSet<PcapAnalyzer.NetworkHash> _hashes;
        private HashSet<PcapAnalyzer.NetworkConnection> _connections;
        public SingleCommandCli(Analyzer analyzer, Processor processor, CliFlags cliFlags)
        { 
            _analyzer = analyzer;
            _processor = processor;
            _cliFlags = cliFlags;
            _hashes = new HashSet<NetworkHash>();
            _connections = new HashSet<PcapAnalyzer.NetworkConnection>();
            _passwords = new HashSet<NetworkPassword>();
            _files = new List<string>();
            _analyzer.ParsedItemDetected += OnParsedItemDetected;
        }

        public void Run()
        {
            try
            {      
                verifyPath(_cliFlags);
                _processor.ProcessingFinished += (s, e) => this.ExportResults(_cliFlags);
                _processor.FileProcessingStatusChanged += (s, e) => this.printFileStatusUpdate(s, e);
                Console.WriteLine("[+] Started analyzing pcap files");
                _processor.ProcessPcaps(_files);
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

        private void printFileStatusUpdate(object sender, FileProcessingStatusChangedEventArgs e)
        {
            if (e.Status == FileProcessingStatus.Started || e.Status == FileProcessingStatus.Finished)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Console.WriteLine($"File : {Path.GetFileName(e.FilePath)} Processing {e.Status}");
            Console.ForegroundColor = ConsoleColor.White;
        }
        private void verifyPath(CliFlags cliFlags)
        {
            if (_cliFlags.InputFiles.Count() != 0 && _cliFlags.InputDir != null)
            {
                throw new Exception("Only one of the arguments -i and -d can be presented in a single command mode run");
            }
            else if (_cliFlags.InputFiles.Count() != 0)
            {
                foreach (string filePath in cliFlags.InputFiles)
                {
                    AddFile(filePath);
                }
            }
            else
            {
                verifyDir(cliFlags.InputDir);
            }
        }
        private void verifyDir(string dirPath)
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

        private void ExportResults(CliFlags cliFlags)
        {
            if (cliFlags.OutputDir != null)
            { 
                foreach (string moduleName in cliFlags.Modules)
                {
                    if (moduleName.Contains("NetworkMap"))
                    {
                        Utilities.ExportNetworkMap(cliFlags.OutputDir, _connections);
                    }
                    else if (moduleName.Contains("Credentials"))
                    {
                        Utilities.ExportHashes(cliFlags.OutputDir, _hashes);
                    }
                    else if (moduleName.Contains("FileExtracting"))
                    {
                        // Todo - extract files to output
                    }
                    // Todo - add exporting of dns module results
                }
           }

            Console.WriteLine("[X] Bruteshark finished processing");
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
                printDetectedItem(e.ParsedItem);
            }
            if (e.ParsedItem is PcapAnalyzer.NetworkHash)
            {
                _hashes.Add(e.ParsedItem as PcapAnalyzer.NetworkHash);
                printDetectedItem(e.ParsedItem);
            }
            if (e.ParsedItem is PcapAnalyzer.NetworkConnection)
            {
                var networkConnection = e.ParsedItem as NetworkConnection;
                _connections.Add(networkConnection);
            }
        }
        private void printDetectedItem(object item)
        {
            Console.WriteLine($"Found: {item}");
        }
    }

}
