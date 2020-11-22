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


            var connection = new PcapAnalyzer.NetworkConnection();
            connection.Protocol = "TCP";
            connection.Source= "1.1.1.1";
            connection.Destination = "2.2.2.2";
            connection.SrcPort = 3009;
            connection.DestPort = 80;

            connections.Add(connection);
            
            var expected = File.ReadAllText(this.NetworkConnectionsListJson);

            // Act.
            string jsonString = PcapAnalyzer.Neo4jJsonExporter.GetNetworkMapAsJsonString(connections);

            // Assert.
            Assert.AreEqual(expected, jsonString);

        }
    }
}
