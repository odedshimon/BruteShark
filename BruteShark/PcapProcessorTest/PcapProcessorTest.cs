using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapProcessor;

namespace PcapProcessorTest
{
    [TestClass]
    public class PcapProcessorTest
    {
        public string UdpFilePath { get; set; }
        public string TcpFivePacketsFilePath { get; set; }
        public string HttpSmallFilePath { get; set; }


        public PcapProcessorTest()
        {
            this.UdpFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"Test Files\Kerberos - UDP.pcap");
            this.TcpFivePacketsFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"Test Files\Tcp - 5 Packets.pcap");
            this.HttpSmallFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"Test Files\HTTP - Small File.pcap");
        }

        [TestMethod]
        public void PcapProcessor_ReadUdpPackets_ReadSuccess()
        {
            // Arrange.
            var recievedPackets = new List<PcapProcessor.UdpPacket>();
            var processor = new PcapProcessor.Processor();

            processor.UdpPacketArived +=
                (object sender, UdpPacketArivedEventArgs e) => recievedPackets.Add(e.Packet);

            // Act.
            processor.ProcessPcap(this.UdpFilePath);

            // Assert (the file has 32 packets).
            Assert.AreEqual(32, recievedPackets.Count);
        }
        
        [TestMethod]
        public void PcapProcessor_reconstructUdpStreams_success()
        {
            // Arrange.
            var recievedStreams = new List<UdpSession>();
            var processor = new Processor();
            processor.BuildUdpSessions = true;
            processor.UdpSessionArrived +=
                (object sender, UdpSessionArrivedEventArgs e) => recievedStreams.Add(e.UdpSession);

            // Act.
            processor.ProcessPcap(this.UdpFilePath);


            // Assert 
            Assert.AreEqual(16, recievedStreams.Count);
        }
        
        [TestMethod]
        public void PcapProcessor_reconstructUdpStreams_zero_streams()
        {
            // Arrange.
            var recievedStreams = new List<UdpSession>();
            var processor = new Processor();
            processor.BuildUdpSessions = true;
            processor.UdpSessionArrived +=
                (object sender, UdpSessionArrivedEventArgs e) => recievedStreams.Add(e.UdpSession);

            // Act.
            processor.ProcessPcap(this.TcpFivePacketsFilePath);


            // Assert 
            Assert.AreEqual(0, recievedStreams.Count);
        }

        [TestMethod]
        public void PcapProcessor_ReadTcpPackets_ReadSuccess()
        {
            // Arrange.
            var recievedPackets = new List<PcapProcessor.TcpPacket>();
            var processor = new PcapProcessor.Processor();

            processor.TcpPacketArived +=
                (object sender, TcpPacketArivedEventArgs e) => recievedPackets.Add(e.Packet);

            // Act.
            processor.ProcessPcap(this.TcpFivePacketsFilePath);

            // Assert.
            Assert.AreEqual(5, recievedPackets.Count);
        }

        [TestMethod]
        public void PcapProcessor_BuildTcpSession_BuildSuccess()
        {
            // Arrange.
            var recievedSessions = new List<TcpSession>();
            var processor = new Processor();
            processor.BuildTcpSessions = true;
            processor.TcpSessionArrived +=
                (object sender, TcpSessionArivedEventArgs e) => recievedSessions.Add(e.TcpSession);

            // Act.
            processor.ProcessPcap(this.HttpSmallFilePath);
            string firstSessionText = Encoding.UTF8.GetString(recievedSessions[0].Data);

            // Assert (Check specific session that i know it has real data).
            Assert.AreEqual(18843, recievedSessions[0].Data.Length);
            StringAssert.StartsWith(firstSessionText, @"GET /download.html HTTP/1.1");
        }

        [TestMethod]
        public void PcapProcessor_ReadTcpPacketsMultipleFiles_ReadSuccess()
        {
            // Arrange.
            var recievedPackets = new List<PcapProcessor.TcpPacket>();
            var processor = new PcapProcessor.Processor();

            processor.TcpPacketArived +=
                (object sender, TcpPacketArivedEventArgs e) => recievedPackets.Add(e.Packet);

            // Act.
            processor.ProcessPcaps( new List<string>() {
                this.HttpSmallFilePath,
                this.TcpFivePacketsFilePath });

            // Assert.
            Assert.AreEqual(46, recievedPackets.Count);
        }
    }
}
