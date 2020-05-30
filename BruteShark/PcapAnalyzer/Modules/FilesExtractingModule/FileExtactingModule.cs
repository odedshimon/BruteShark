using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;


namespace PcapAnalyzer
{
    public class FileExtactingModule : IModule
    {
        public string Name => "File Extracintg";
        public event EventHandler<ParsedItemDetectedEventArgs> ParsedItemDetected;

        // File signitures examples:
        // 1. Scalpel (https://github.com/machn1k/Scalpel-2.0/blob/master/conf/scalpel.conf)
        // 2. OpenForensics (https://github.com/ethanbayne/OpenForensics/blob/master/OpenForensics/FileTypes.xml)
        private List<(string, string, string)> _filesSignitures = new List<(string header, string footer, string extention)>
        {
            (header: "FFD8FF", footer: "FFD9", extention: "jpg"),
            (header: "89504E470D0A1A0A", footer: "49454E44AE426082", "png"),
            (header: "504E47", footer: "FFFCFDFE", "png"),
            (header: "474946383761", footer: "003B", "gif"),
            (header: "474946383961", footer: "00003B", "gif"),
            (header: "000001BA", footer: "000001B9", "mpg"),
            (header: "000001B3", footer: "000001B7", "mpg"),
            (header: "504B030414", footer: "504B050600", "zip")
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
                    var startIndex = 0;
                    var footerPosition = 0;
                    var dummy = 0;

                    // We need a while loop incase there are more than on file from each type in the session data.
                    while (startIndex != -1)
                    {
                        // TODO: add session one side data stream getter.
                        // Algorythm: each iteration search from the last footer position (if found any file).
                        var file_data = Utilities.GetDataBetweenHeaderAndFooter
                        (
                            data: tcpSession.Data.Skip(startIndex).ToArray(),
                            header: Utilities.StringToByteArray(header),
                            footer: Utilities.StringToByteArray(footer), 
                            headerPosition: ref dummy, 
                            footerPosition: ref footerPosition
                        );

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

                            // If file was found, update the search start index.
                            startIndex += footerPosition + footer.Length;
                            continue;
                        }

                        startIndex = -1;
                    }
                }
                
            }
            catch (Exception e)
            {

            }
        }


    }
}
