using System;
using System.Collections.Generic;
using System.Text;
using PcapAnalyzer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace PcapAnalyzerTest
{
    [TestClass]
    public class NetworkMapModuleTest
    {
        public string NetworkConnectionsListJson { get; set; }
        public NetworkMapModuleTest()
        {
            this.NetworkConnectionsListJson = Path.Combine(Directory.GetCurrentDirectory(), @"TestFiles\NetworkConnectionsListJson.json");
        }

        [TestMethod]
        public void NetworkMapAsJsonString_Test()
        {
            // Arrange
            var connections = new List<PcapAnalyzer.NetworkConnection>();
            var connection = new PcapAnalyzer.NetworkConnection(source: "1.1.1.1",destination: "2.2.2.2", protocol: "TCP", srcPort: 3009, dstPort: 80);

            connections.Add(connection);
            
            var expected = File.ReadAllText(this.NetworkConnectionsListJson);

            // Act.
            string jsonString = PcapAnalyzer.NetwrokMapJsonExporter.GetNetworkMapAsJsonString(connections);

            // Assert.
            Assert.AreEqual(expected, jsonString);

        }
    }
}
