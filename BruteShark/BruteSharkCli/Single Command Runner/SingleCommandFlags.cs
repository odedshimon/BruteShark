using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace BruteSharkCli
{
    public class SingleCommandFlags
    {
        [Option('d', "input-dir", Required = false, SetName ="dir_input",  HelpText = "The input directory containing the files to be processed.")]
        public string InputDir { get; set; }

        [Option('i', "input", Required = false, SetName = "files_input", Separator = ',', HelpText = "The files to be processed seperated by comma")]
        public IEnumerable<string> InputFiles { get; set; }

        [Option('m', "modules", Required = false , Separator = ',', HelpText = "The modules to be separterd by comma: Credentials, FileExtracting, NetworkMap, DNS")]
        public IEnumerable<string> Modules { get; set; }

        // TODO - merge parallel processing feature branch to make this flag really work
        // [Option('p', "parallel", Required = false, HelpText = "Whether to process the files in parallel, default value is false.")]
        // public bool ParallelProcessing { get; set; }

        [Option('o', "output", Required = false, HelpText = "Output direcorty for the results files.")]
        public string OutputDir { get; set; }

        [Option('P', "promiscious", Required = false, HelpText = "Configures whether to start live capture on normal or promiscious mode (sometimes needs super user privileges to to do so),use along with -l for live catpure.")]
        public bool PromisciousMode { get; set; }

        [Option('l', "live-capture", Required = false, Default = null, HelpText = "Caputre and process packets live from a network interface.")]
        public string CaptureDevice { get; set; }
    }
}
