using System;

namespace PcapProcessor
{
    public class ProcessingPrecentsChangedEventArgs : EventArgs
    {
        public int Precents { get; set; }
    }
}