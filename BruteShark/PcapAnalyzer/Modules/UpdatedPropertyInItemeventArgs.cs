using System;
using System.Reflection;

namespace PcapAnalyzer
{
    public class UpdatedPropertyInItemeventArgs : EventArgs
    {
        public object ParsedItem { get; set; }
        public PropertyInfo PropertyChanged { get; set; }
        public object NewPropertyValue { get; set; }
    }
}