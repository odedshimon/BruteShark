using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;


namespace PcapAnalyzer.Modules.FilesExtractingModule
{
    class FileExtactingModule : IModule
    {
        public string Name => "File Extracintg";
        public event EventHandler<ParsedItemDetectedEventArgs> ParsedItemDetected;


        private List<(string, string, string)> _filesSignitures = new List<(string header, string footer, string extention)>
        {
            (header: "FFD8FF", footer: "FFD9", extention: "jpg"),
            (header: "89504E470D0A1A0A", footer: "49454E44AE426082", "png"),
            (header: "474946383761", footer: "003B", "gif"),
            (header: "000001BA", footer: "000001B7", "mpg"),
            (header: "000001B3", footer: "000001B7", "mpg")
        };


        public void Analyze(UdpPacket udpPacket) { }

        public void Analyze(TcpPacket tcpPacket) { }

        public void Analyze(TcpSession tcpSession)
        {
            // TODO: remove this try-except (implement at Analyzer class)
            try
            {
                foreach (var (header, footer, extention) in _filesSignitures)
                {
                    // TODO: add session object one side stream data getter
                    var file_data = Utilities.GetDataBetweenHeaderAndFooter(
                        tcpSession.Data, 
                        Utilities.StringToByteArray(header), 
                        Utilities.StringToByteArray(footer));

                    if (file_data != null)
                    {
                        var file = new NetworkFile()
                        {
                            Source = tcpSession.SourceIp,
                            Destination = tcpSession.DestinationIp,
                            FileData = file_data,
                            Extention = extention,
                            Protocol = "TCP",
                            Algorithm = "Header-Footer Carving"
                        };

                        // Raise event.
                        this.ParsedItemDetected(this, new ParsedItemDetectedEventArgs()
                        {
                            ParsedItem = file
                        });
                    }
                }
                
            }
            catch (Exception e)
            {

            }
        }


    }
}
