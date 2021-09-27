using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace BruteSharkCli
{
    public class SingleCommandFlags
    {
        [Option('d', "input-dir", Required=false, SetName="dir_input",  HelpText="The input directory containing the files to be processed.")]
        public string InputDir { get; set; }

        [Option('i', "input", Required=false, SetName="files_input", Separator=',', HelpText ="The files to be processed separated by comma.")]
        public IEnumerable<string> InputFiles { get; set; }

        [Option('m', "modules", Required=false, Separator=',', HelpText="The modules to be separated by comma: Credentials, FileExtracting, NetworkMap, DNS, Voip.")]
        public IEnumerable<string> Modules { get; set; }

        [Option('o', "output", Required=false, HelpText="Output directory for the results files.")]
        public string OutputDir { get; set; }

        [Option('p', "promiscuous", Required=false, HelpText="Configures whether to start live capture with promiscuous mode (sometimes needs super user privileges to do so),use along with -l for live capture.")]
        public bool PromisciousMode { get; set; }

        [Option('l', "live-capture", Required=false, Default=null, HelpText="Capture and process packets live from a network interface.")]
        public string CaptureDevice { get; set; }
        
        [Option('f', "filter", Required=false, Default=null, HelpText="Set a capture BPF filter to the live traffic processing.")]
        public string CaptrueFilter { get; set; }
    }
}
