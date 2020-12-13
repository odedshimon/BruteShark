using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PcapAnalyzerTest
{
    [TestClass]
    public class AnalyzerTest
    {
        [TestMethod]
        public void Analyzer_LoadModules_LoadSuccess()
        {
            // Arrange.
            var analyzer = new PcapAnalyzer.Analyzer();

            // Act.
            var modulesList = analyzer.AvailableModulesNames;

            // Assert.
            Assert.AreEqual(4, modulesList.Count);
        }

        [TestMethod]
        public void Analyzer_AddModule_AddSuccess()
        {
            // Arrange.
            var analyzer = new PcapAnalyzer.Analyzer();

            // Act (Add one module).
            analyzer.AddModule(analyzer.AvailableModulesNames.First());

            // Assert.
            Assert.AreEqual(1, analyzer.LoadedModulesNames.Count);
        }

        [TestMethod]
        public void Analyzer_RemoveModule_LoadSuccess()
        {
            // Arrange.
            var analyzer = new PcapAnalyzer.Analyzer();

            // Act (Add two modulem, remove one).
            analyzer.AddModule(analyzer.AvailableModulesNames[0]);
            analyzer.AddModule(analyzer.AvailableModulesNames[1]);
            analyzer.RemoveModule(analyzer.LoadedModulesNames[0]);

            // Assert.
            Assert.AreEqual(1, analyzer.LoadedModulesNames.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Analyzer_AddWrangModuleName_AddFail()
        {
            // Arrange.
            var analyzer = new PcapAnalyzer.Analyzer();

            // Act.
            analyzer.AddModule("not-a-real-module-name");
        }

    }
}
