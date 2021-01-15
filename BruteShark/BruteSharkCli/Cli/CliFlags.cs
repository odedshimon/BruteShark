using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace BruteSharkCli.Cli
{
    public class CliFlags
    {
        [Option('i', "input", Required = false, Separator = ',', HelpText = "The files to be processed seperated by ,")]
        public IEnumerable<string> InputFiles { get; set; }

        [Option('d', "input-dir", Required = false, HelpText = "The input directory containing the files to be processed.")]
        public string InputDir{ get; set; }

        [Option('m', "modules", Required = false , Separator = ',', HelpText = "The modules to be included in the processing of the files i.e. filesExtracting, Hashes, etc'.")]
        public IEnumerable<string> Modules { get; set; }

        // TODO - merge parallel processing feature branch to make this flag really work
        [Option('p', "parallel", Required = false, Default = false, HelpText = "Whether to process the files in parallel, default value is false.")]
        public bool ParallelProcessing { get; set; }

        [Option('c', "single-command", Required = false, Default = false, HelpText = "Run BruteShark cli in single command mode using the given command line arguments as input and configuration for the run.")]
        public bool SingleCommandMode { get; set; }
        
        [Option('o', "output", Required = false,Default = "", HelpText = "The output direcorty for the results files.")]
        public string OutputDir{ get; set; }
    }
}
