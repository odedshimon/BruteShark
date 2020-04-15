using System;
using System.Collections.Generic;
using System.Text;

namespace PcapProcessor
{
    public class PcapFileProcessingException : Exception
    {
        public string FilePath { get; set; }

        public PcapFileProcessingException(string filePath)
        {
            this.FilePath = filePath;
        }
    }
}
