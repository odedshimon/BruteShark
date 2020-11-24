using System.Collections.Generic;
using System.Text.Json;
using System.IO;

namespace PcapAnalyzer
{
    public class NetwrokMapJsonExporter
    {

        public static string GetNetworkMapAsJsonString(List<NetworkConnection> connections)
        {
            string connectionsJsonSerialized = JsonSerializer.Serialize(connections, new JsonSerializerOptions() { WriteIndented = true });
            return connectionsJsonSerialized;
        }
        public static void FileExport(List<NetworkConnection> connections,  string filename)
        {
            string connectionsJsonSerialized = GetNetworkMapAsJsonString(connections);
            File.WriteAllText(filename, connectionsJsonSerialized);
            
        }
    }

    
}
