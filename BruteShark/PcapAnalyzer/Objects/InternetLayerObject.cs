using System;
using System.Collections.Generic;
using System.Text;

namespace PcapAnalyzer
{
    public class InternetLayerObject
    {
        public string Source { get; set; }
        public string Destination { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is InternetLayerObject))
            {
                return false;
            }

            var InternetLayerObject = obj as InternetLayerObject;

            return this.Source == InternetLayerObject.Source &&
                   this.Destination == InternetLayerObject.Destination;
        }

        public override int GetHashCode()
        {
            return this.Source.GetHashCode() ^
                   this.Destination.GetHashCode();
        }
    }
}
