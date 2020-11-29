using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace PcapAnalyzer
{
    public static class JsonExporter
    {
        public static string GetNetworkMapAsJsonString(List<NetworkConnection> connections)
        {
            return JsonSerializer.Serialize(
                connections, 
                new JsonSerializerOptions { WriteIndented = true });
        }

        public static void ExportFile(List<NetworkConnection> connections,  string filename)
        {
            string connectionsJsonSerialized = GetNetworkMapAsJsonString(connections);
            File.WriteAllText(filename, connectionsJsonSerialized);
        }
    }

    
}
