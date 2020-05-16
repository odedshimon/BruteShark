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

        // TODO: jpeg is just for test, i will implement this module genericly
        private readonly byte[] jpg_header = new byte[] { 0xff, 0xd8 };
        private readonly byte[] jpg_footer = new byte[] { 0xff, 0xd9 };


        public void Analyze(UdpPacket udpPacket) { }

        public void Analyze(TcpPacket tcpPacket) { }

        public void Analyze(TcpSession tcpSession)
        {
            // TODO: remove this try-except (implement at Analyzer class)
            try
            {
                // TODO: add session object one side stream data getter
                var file_data = GetDataBetweenHeaderAndFooter(tcpSession.Data, jpg_header, jpg_footer);

                if (file_data != null)
                {

                }
            }
            catch (Exception e)
            {

            }


        }

        public byte[] GetDataBetweenHeaderAndFooter(byte[] data, byte[] header, byte[] footer)
        {
            int header_position = Utilities.SearchForSubarray(data, header);

            if (header_position > 0)
            {
                // TODO: check if this skip is memory inefficient, if not refactor SearchForSubarray to get optional
                // parameter of start index
                int footer_position = Utilities.SearchForSubarray(data.Skip(header_position).ToArray(), jpg_footer);

                if (footer_position > 0)
                {
                    return data.SubArray(index: header_position, length: footer_position);
                }
            }

            return null;
        }


    }
}
