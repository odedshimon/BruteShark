using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapAnalyzer;
using System.Collections.Generic;
using System.IO;


namespace PcapAnalyzerTest
{
    [TestClass]
    public class DnsModuleTest
    {
        [TestMethod]
        public void DnsModule_ParseTwoRecords_ParseSuccess()
        {
            // Arrange
            var dnsModule = new PcapAnalyzer.DnsModule();
            var parsedRecords = new List<PcapAnalyzer.DnsNameMapping>();
            dnsModule.ParsedItemDetected +=
                (object sender, ParsedItemDetectedEventArgs e) => parsedRecords.Add(e.ParsedItem as PcapAnalyzer.DnsNameMapping);

            var dnsPacket = new PcapAnalyzer.UdpPacket
            {
                SourceIp = "2.2.2.2",
                DestinationIp = "1.1.1.1",
                DestinationPort = 53,
                SourcePort = 100,
                Data = new byte[]
                {
                    0x79, 0x56, 0x81, 0x80, 0x00, 0x01, 0x00, 0x02, 0x00, 0x02, 0x00, 0x00, 0x04, 0x6d, 0x61, 0x69,
                    0x6c, 0x08, 0x70, 0x61, 0x74, 0x72, 0x69, 0x6f, 0x74, 0x73, 0x02, 0x69, 0x6e, 0x00, 0x00, 0x01,
                    0x00, 0x01, 0xc0, 0x0c, 0x00, 0x05, 0x00, 0x01, 0x00, 0x00, 0x2a, 0x4b, 0x00, 0x02, 0xc0, 0x11,
                    0xc0, 0x11, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x2a, 0x4c, 0x00, 0x04, 0x4a, 0x35, 0x8c, 0x99,
                    0xc0, 0x11, 0x00, 0x02, 0x00, 0x01, 0x00, 0x01, 0x43, 0x8c, 0x00, 0x06, 0x03, 0x6e, 0x73, 0x32,
                    0xc0, 0x11, 0xc0, 0x11, 0x00, 0x02, 0x00, 0x01, 0x00, 0x01, 0x43, 0x8c, 0x00, 0x06, 0x03, 0x6e,
                    0x73, 0x31, 0xc0, 0x11
                }
            };

            // Act.
            dnsModule.Analyze(dnsPacket);

            // Assert.
            Assert.AreEqual(2, parsedRecords.Count);
            Assert.AreEqual(parsedRecords[0].Destination, "patriots.in");
            Assert.AreEqual(parsedRecords[0].Query, "mail.patriots.in");
            Assert.AreEqual(parsedRecords[1].Destination, "74.53.140.153");
            Assert.AreEqual(parsedRecords[1].Query, "mail.patriots.in");
        }
    }
}
