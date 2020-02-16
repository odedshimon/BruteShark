using System;
using System.Collections.Generic;
using System.Linq;

namespace PcapAnalyzer
{
    public class Analyzer
    {
        private List<IModule> _modules;

        public event EventHandler<ParsedItemDetectedEventArgs> ParsedItemDetected;

        public List<string> AvailableModulesNames => this._modules.Select(m => m.Name).ToList();


        public Analyzer()
        {
            _iniitilyzeModulesList();
        }

        private void _iniitilyzeModulesList()
        {
            // Create an instance for any available modules by looking for every class that 
            // implements IModule.
            this._modules = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(s => s.GetTypes())
                            .Where(p => typeof(IModule).IsAssignableFrom(p) && !p.IsInterface)
                            .Select(t => (IModule)Activator.CreateInstance(t))
                            .ToList();

            // Register to each module event.
            foreach(var m in _modules)
            {
                m.ParsedItemDetected += (s, e) => this.ParsedItemDetected(s, e);
            }
            
        }

        public void Analyze(TcpPacket tcpPacket)
        {
            foreach (var module in _modules)
            {
                module.Analyze(tcpPacket);
            }
        }

        public void Analyze(TcpSession tcpSession)
        {
            foreach (var module in _modules)
            {
                module.Analyze(tcpSession);
            }
        }

        

    }
}
