using System;

namespace PcapAnalyzer
{
    public class ParsedItemDetectedEventArgs : EventArgs
    {
        public object ParsedItem { get; set; }
    }
}