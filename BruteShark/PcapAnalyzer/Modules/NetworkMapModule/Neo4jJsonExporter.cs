using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace PcapAnalyzer
{
    public class Neo4jJsonExporter
    {

        public static string GetNetworkMapAsJsonString(List<NetworkConnection> connections)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string connectionsJsonSerialized = JsonSerializer.Serialize(connections, options);
            return connectionsJsonSerialized;
        }
        public static void FileExport(List<NetworkConnection> connections,  string filename)
        {
            string connectionsJsonSerialized = GetNetworkMapAsJsonString(connections);
            File.WriteAllText(filename, connectionsJsonSerialized);
            
        }
    }

    
}
