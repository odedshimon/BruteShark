using System;
using System.Collections.Generic;
using System.Linq;

namespace PcapAnalyzer
{
    public class Analyzer
    {
        private List<IModule> _loadedModules;
        private List<IModule> _availbleModules;

        public event EventHandler<ParsedItemDetectedEventArgs> ParsedItemDetected;

        public List<string> AvailableModulesNames => _availbleModules.Select(m => m.Name).ToList();
        public List<string> LoadedModulesNames => _loadedModules.Select(m => m.Name).ToList();


        public Analyzer()
        {
            InitilyzeModulesList();
        }

        public void RemoveModule(string module_name)
        {
            _loadedModules.Remove(
                _loadedModules.Where(
                    m => m.Name == module_name).First());
        }

        public void AddModule(string module_name)
        {
            if (!this.AvailableModulesNames.Contains(module_name))
            {
                throw new Exception($"No module named {module_name}");
            }

            var module = _availbleModules.Where(m => m.Name == module_name).First();
            _loadedModules.Add(module);
        }

        private void InitilyzeModulesList()
        {
            _loadedModules = new List<IModule>();

            // Create an instance for any available modules by looking for every class that 
            // implements IModule.
            this._availbleModules = AppDomain.CurrentDomain.GetAssemblies()
                                    .SelectMany(s => s.GetTypes())
                                    .Where(p => typeof(IModule).IsAssignableFrom(p) && !p.IsInterface)
                                    .Select(t => (IModule)Activator.CreateInstance(t))
                                    .ToList();

            // Register to each module event.
            foreach(var m in _availbleModules)
            {
                m.ParsedItemDetected += (s, e) => this.ParsedItemDetected(s, e);
            }
            
        }

        // TODO: use template instead this 3 functions (or change all design)
        // TODO: try catch so if one module will fail..
        public void Analyze(UdpPacket udpPacket)
        {
            foreach (var module in _loadedModules)
            {
                module.Analyze(udpPacket);
            }
        }

        public void Analyze(TcpPacket tcpPacket)
        {
            foreach (var module in _loadedModules)
            {
                module.Analyze(tcpPacket);
            }
        }

        public void Analyze(TcpSession tcpSession)
        {
            foreach (var module in _loadedModules)
            {
                module.Analyze(tcpSession);
            }
        }


    }
}
