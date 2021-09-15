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
            var connections = new HashSet<PcapAnalyzer.NetworkConnection>() {
                new PcapAnalyzer.NetworkConnection()
                {
                    Source = "1.1.1.1",
                    Destination = "2.2.2.2", 
                    Protocol = "TCP", 
                    SrcPort = 3009, 
                    DestPort = 80
                }
            };
            
            
            string expectedJson = @"[
  {
    ""Source"": ""1.1.1.1"",
    ""Destination"": ""2.2.2.2"",
    ""Protocol"": ""TCP"",
    ""SrcPort"": 3009,
    ""DestPort"": 80
  }
]";

            // Act.
            string jsonString = CommonUi.Exporting.GetIndentdJson(connections);

            // Assert.
            Assert.AreEqual(expectedJson, jsonString);

        }
    }
}
