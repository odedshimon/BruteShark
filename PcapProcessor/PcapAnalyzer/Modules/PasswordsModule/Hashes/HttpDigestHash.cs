using System;
using System.Collections.Generic;
using System.Text;

namespace PcapAnalyzer
{
    public class HttpDigestHash : NetworkHash
    {
        public string Qop { get; set; }
        public string Realm { get; set; }
        public string Nonce { get; set; }
        public string Uri { get; set; }
        public string Cnonce { get; set; }
        public string Nc { get; set; }
        public string Username { get; set; }
        public string Method { get; set; }
        public string Response { get; set; }
    }
}
