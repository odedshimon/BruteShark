using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace CommonUi
{
    public static class Exporting
    {
        public static string GetUniqueFilePath(string filePath)
        {
            string dir = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string fileExt = Path.GetExtension(filePath);

            for (int i = 1; ; ++i)
            {
                if (!File.Exists(filePath))
                    return new FileInfo(filePath).FullName;

                filePath = Path.Combine(dir, fileName + " " + i + fileExt);
            }
        }

        public static string GetIndentdJson(IEnumerable<object> connections)
        {
            return JsonSerializer.Serialize(connections, new JsonSerializerOptions() { WriteIndented = true });
        }

        public static string ExportToFile(string dirPath, string fileName, IEnumerable<object> dataToExport)
        {
            var filePath = GetUniqueFilePath(Path.Combine(dirPath, fileName));
            File.WriteAllText(filePath, GetIndentdJson(dataToExport));
            return filePath;
        }

        public static string ExportNetworkMap(string dirPath, HashSet<PcapAnalyzer.NetworkConnection> connections)
        {
            return ExportToFile(dirPath, "BruteShark Network Map.json", connections);
        }

        public static string ExportNetworkNodesData(string dirPath, List<NetworkNode> networkNodes)
        {
            return ExportToFile(dirPath, "BruteShark Network Nodes Data.json", networkNodes);
        }

        public static string ExportFiles(string dirPath, HashSet<PcapAnalyzer.NetworkFile> networkFiles)
        {
            var extractedFilesDir = Path.Combine(dirPath, "Files");
            Directory.CreateDirectory(extractedFilesDir);

            foreach (var file in networkFiles)
            {
                var filePath = GetUniqueFilePath(Path.Combine(extractedFilesDir, $"{file.Source} - {file.Destination}.{file.Extention}"));
                File.WriteAllBytes(filePath, file.FileData);
            }

            return extractedFilesDir;
        }

        public static string ExportVoipCalls(string dirPath, HashSet<VoipCall> voipCalls )
        {
            var VoipCallsDir = Path.Combine(dirPath, "VoipCalls");
            Directory.CreateDirectory(VoipCallsDir);

            foreach (var call in voipCalls)
            {
                if (call.RTPStream.Length > 0)
                {
                    var filepath = GetUniqueFilePath(Path.Combine(VoipCallsDir, $"{call.ToFilename()}.media"));
                    File.WriteAllBytes(filepath, call.RTPStream);
                }
            }

            return VoipCallsDir;
        }

        public static string ExportDnsMappings(string dirPath, HashSet<PcapAnalyzer.DnsNameMapping> dnsMappings)
        {
            var filePath = GetUniqueFilePath(Path.Combine(dirPath, "BruteShark DNS Mappings.json"));

            File.WriteAllLines(
                filePath, 
                dnsMappings.Select(d => d.ToString()));
            
            return filePath;
        }

        public static string ReplaceInvalidFileNameChars(string filename, char newChar)
        {
            return string.Join(newChar.ToString(), filename.Split(Path.GetInvalidFileNameChars()));
        }

    }
}
