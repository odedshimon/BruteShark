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
        public IEnumerable<IModule> AvailableModules => _availbleModules.ToList();


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

        public void Analyze(object item)
        {
            foreach (var module in _loadedModules)
            {
                if (item is UdpPacket)
                {
                    Utilities.SafeRun(() => module.Analyze(item as UdpPacket));
                }
                else if (item is UdpStream)
                {
                    Utilities.SafeRun(() => module.Analyze(item as UdpStream));
                }
                else if (item is TcpPacket)
                {
                    Utilities.SafeRun(() => module.Analyze(item as TcpPacket));
                }
                else if (item is TcpSession)
                {
                    Utilities.SafeRun(() => module.Analyze(item as TcpSession));
                }
                else
                {
                    throw new Exception("Unsupported type for analyzer");
                }
            }
        }
        
    }
}
