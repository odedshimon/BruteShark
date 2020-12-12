using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
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
        public string PcapNGFile { get; set; }


        public PcapProcessorTest()
        {
            this.UdpFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"Test Files\Kerberos - UDP.pcap");
            this.TcpFivePacketsFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"Test Files\Tcp - 5 Packets.pcap");
            this.HttpSmallFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"Test Files\HTTP - Small File.pcap");
            this.PcapNGFile = Path.Combine(Directory.GetCurrentDirectory(), @"Test Files\HTTP - Small File.pcapng");
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
        public void PcapProcessor_ReconstructUdpStreams_ReconstructSuccess()
        {
            // Arrange.
            var recievedStreams = new List<UdpSession>();
            var recievedStreamsFromPcapNG = new List<UdpSession>();
            var processor = new Processor();
            var pcapNGprocessor = new Processor();
            processor.BuildUdpSessions = true;
            processor.UdpSessionArrived += (object sender, UdpSessionArrivedEventArgs e) => recievedStreams.Add(e.UdpSession);

            pcapNGprocessor.BuildUdpSessions = true;
            pcapNGprocessor.UdpSessionArrived += (object sender, UdpSessionArrivedEventArgs e) => recievedStreamsFromPcapNG.Add(e.UdpSession);

            byte[] firstUdpStreamExpectedData = new byte[] {
                0x00, 0x23, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0x70, 0x61, 0x67,
                0x65, 0x61, 0x64, 0x32, 0x11, 0x67, 0x6f, 0x6f, 0x67, 0x6c, 0x65, 0x73, 0x79, 0x6e, 0x64, 0x69,
                0x63, 0x61, 0x74, 0x69, 0x6f, 0x6e, 0x03, 0x63, 0x6f, 0x6d, 0x00, 0x00, 0x01, 0x00, 0x01 ,0x00,
                0x23, 0x81, 0x80, 0x00, 0x01, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x07, 0x70, 0x61, 0x67, 0x65,
                0x61, 0x64, 0x32, 0x11, 0x67, 0x6f, 0x6f, 0x67, 0x6c, 0x65, 0x73, 0x79, 0x6e, 0x64, 0x69, 0x63,
                0x61, 0x74, 0x69, 0x6f, 0x6e, 0x03, 0x63, 0x6f, 0x6d, 0x00, 0x00, 0x01, 0x00, 0x01, 0xc0, 0x0c,
                0x00, 0x05, 0x00, 0x01, 0x00, 0x00, 0xbc, 0xc1, 0x00, 0x11, 0x07, 0x70, 0x61, 0x67, 0x65, 0x61,
                0x64, 0x32, 0x06, 0x67, 0x6f, 0x6f, 0x67, 0x6c, 0x65, 0xc0, 0x26, 0xc0, 0x3b, 0x00, 0x05, 0x00,
                0x01, 0x00, 0x00, 0x00, 0x7a, 0x00, 0x1a, 0x06, 0x70, 0x61, 0x67, 0x65, 0x61, 0x64, 0x06, 0x67,
                0x6f, 0x6f, 0x67, 0x6c, 0x65, 0x06, 0x61, 0x6b, 0x61, 0x64, 0x6e, 0x73, 0x03, 0x6e, 0x65, 0x74,
                0x00, 0xc0, 0x58, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x7b, 0x00, 0x04, 0xd8, 0xef, 0x3b,
                0x68, 0xc0, 0x58, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x7b, 0x00, 0x04, 0xd8, 0xef, 0x3b,
                0x63 };

            // Act.
            processor.ProcessPcap(this.HttpSmallFilePath);
            pcapNGprocessor.ProcessPcap(this.PcapNGFile);
            // Assert - check if we succeeded reconstructing the expected amount of sessions
            Assert.AreEqual(1, recievedStreams.Count);
            Assert.AreEqual(1, recievedStreamsFromPcapNG.Count);

            // Assert - check if we succeeded reconstructing the data of the udp sessions by checking if we recieved the exact amount of bytes
            byte[] firstSessionBytes = recievedStreams[0].Data;
            byte[] firstSessionBytesFromPcapNG = recievedStreamsFromPcapNG[0].Data;

            Assert.AreEqual(193, firstSessionBytes.Length);
            CollectionAssert.AreEqual(firstUdpStreamExpectedData, firstSessionBytes);


            Assert.AreEqual(193, firstSessionBytesFromPcapNG.Length);
            CollectionAssert.AreEqual(firstUdpStreamExpectedData, firstSessionBytesFromPcapNG);




        }
        
        [TestMethod]
        public void PcapProcessor_ReconstructUdpStreams_ZeroStreams()
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
            var recievedSessionsFromPcapNG = new List<TcpSession>();

            var processor = new Processor();
            var processorPcapNG = new Processor();

            processorPcapNG.BuildTcpSessions = true;
            processor.BuildTcpSessions = true;

            processorPcapNG.TcpSessionArrived += 
                (object sender, TcpSessionArivedEventArgs e) => recievedSessionsFromPcapNG.Add(e.TcpSession);
            processor.TcpSessionArrived +=
                (object sender, TcpSessionArivedEventArgs e) => recievedSessions.Add(e.TcpSession);

            // Act.
            processorPcapNG.ProcessPcap(this.PcapNGFile);
            processor.ProcessPcap(this.HttpSmallFilePath);
            string firstSessionText = Encoding.UTF8.GetString(recievedSessions[0].Data);
            string firstSessionFromPcapNGText = Encoding.UTF8.GetString(recievedSessionsFromPcapNG[0].Data);

            // Assert (Check specific session that i know it has real data).
            Assert.AreEqual(18843, recievedSessions[0].Data.Length);
            Assert.AreEqual(18843, recievedSessionsFromPcapNG[0].Data.Length);
            StringAssert.StartsWith(firstSessionText, @"GET /download.html HTTP/1.1");
            StringAssert.StartsWith(firstSessionFromPcapNGText, @"GET /download.html HTTP/1.1");
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

        [TestMethod]
        public void PcapProcessor_identifyPcapFileFormat()
        {
            var processor = new PcapProcessor.Processor();
            Assert.AreEqual(true, processor.IsPcapFile(this.HttpSmallFilePath));
            Assert.AreEqual(false, processor.IsPcapFile(this.PcapNGFile));
        }
    }
}
