using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapAnalyzer;

namespace PcapAnalyzerTest
{
    [TestClass]
    public class FileExtractingModuleTest
    {
        public TcpSession SingleJpgSession { get; set; }
        public TcpSession TwoJpgSession { get; set; }

        public FileExtractingModuleTest()
        {
            // Dummy jpg data wrapped by two 0xff from each side
            var dummyJpgData = new byte[] { 0xff, 0xff, 0xff, 0xd8, 0xff, 0xe0, 0x00, 0x10, 0x4a, 0x46, 0x49, 0xff, 0xd9, 0xff, 0xff };

            // Create a sesion with dummy jpg data.
            this.SingleJpgSession = new PcapAnalyzer.TcpSession
            {
                SourceIp = "1.1.1.1",
                DestinationIp = "2.2.2.2",
                Data = dummyJpgData
            };

            this.TwoJpgSession = new PcapAnalyzer.TcpSession
            {
                SourceIp = "1.1.1.1",
                DestinationIp = "2.2.2.2",
                Data = dummyJpgData.Concat(dummyJpgData).ToArray()
            };
        }


        [TestMethod]
        public void Utilities_GetDataBetweenHeaderAndFooter_ParseSuccess()
        {
            // Arrange
            var data = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a };
            var header = new byte[] { 0x02, 0x03 };
            var footer = new byte[] { 0x06, 0x07 };
            var expected = new byte[] { 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 };

            // Act
            var res = PcapAnalyzer.Utilities.GetDataBetweenHeaderAndFooter(data, header, footer);

            // Assert
            Assert.IsTrue(res.SequenceEqual(expected));
        }

        [TestMethod]
        public void FileExtractingModule_ParseSingleJpgFile_ParseSuccess()
        {
            // Arrange. Create e.
            var fileExtractingModule = new PcapAnalyzer.FileExtactingModule();
            var parsedFiles = new List<PcapAnalyzer.NetworkFile>();
            fileExtractingModule.ParsedItemDetected += 
                (object sender, ParsedItemDetectedEventArgs e) => parsedFiles.Add(e.ParsedItem as PcapAnalyzer.NetworkFile);

            // Act.
            fileExtractingModule.Analyze(this.SingleJpgSession);

            // Assert.
            Assert.AreEqual(1, parsedFiles.Count);
        }

        [TestMethod]
        public void FileExtractingModule_ParseTwoJpgFile_ParseSuccess()
        {
            // Arrange. Create e.
            var fileExtractingModule = new PcapAnalyzer.FileExtactingModule();
            var parsedFiles = new List<PcapAnalyzer.NetworkFile>();
            fileExtractingModule.ParsedItemDetected +=
                (object sender, ParsedItemDetectedEventArgs e) => parsedFiles.Add(e.ParsedItem as PcapAnalyzer.NetworkFile);

            // Act.
            fileExtractingModule.Analyze(this.TwoJpgSession);

            // Assert.
            Assert.AreEqual(2, parsedFiles.Count);
        }


    }
}
