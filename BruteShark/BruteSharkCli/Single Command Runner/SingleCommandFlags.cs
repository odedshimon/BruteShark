using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace BruteSharkCli
{
    public class SingleCommandFlags
    {
        [Option('i', "input", Required = false, Separator = ',', HelpText = "The files to be processed seperated by comma")]
        public IEnumerable<string> InputFiles { get; set; }

        [Option('d', "input-dir", Required = false, HelpText = "The input directory containing the files to be processed.")]
        public string InputDir{ get; set; }

        [Option('m', "modules", Required = false , Separator = ',', HelpText = "The modules to be separterd by comma: Credentials, FileExtracting, NetworkMap")]
        public IEnumerable<string> Modules { get; set; }

        // TODO - merge parallel processing feature branch to make this flag really work
        [Option('p', "parallel", Required = false, HelpText = "Whether to process the files in parallel, default value is false.")]
        public bool ParallelProcessing { get; set; }

        [Option('o', "output", Required = false, HelpText = "Output direcorty for the results files.")]
        public string OutputDir { get; set; }
    }
}
