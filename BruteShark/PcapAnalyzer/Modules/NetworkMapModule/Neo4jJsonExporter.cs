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


        public static void FileExport(List<NetworkConnection> connections,  string filename)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string connectionsJsonSerialized = JsonSerializer.Serialize(connections, options);
            File.WriteAllText(filename, connectionsJsonSerialized);
            
        }
    }

    
}
