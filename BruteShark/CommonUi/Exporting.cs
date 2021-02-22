using System;
using System.Collections.Generic;
using System.IO;
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

        public static string GetNetworkMapAsJsonString(HashSet<PcapAnalyzer.NetworkConnection> connections)
        {
            string connectionsJsonSerialized = JsonSerializer.Serialize(connections, new JsonSerializerOptions() { WriteIndented = true });
            return connectionsJsonSerialized;
        }

        public static string ExportNetworkMap(string dirPath, HashSet<PcapAnalyzer.NetworkConnection> connections)
        {
            var filePath = GetUniqueFilePath(Path.Combine(dirPath, "BruteShark Network Map.json"));
            File.WriteAllText(filePath, GetNetworkMapAsJsonString(connections));
            return filePath;
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
        public static string ExportVoipCalls(string dirPath, HashSet<VoipCallPresentation> voipCalls )
        {
            var VoipCallsDir = Path.Combine(dirPath, "Voip Calls");
            foreach(var call in voipCalls)
            {
                if (call.GetRTPStream().Length > 0)
                {
                    var filepath = GetUniqueFilePath(Path.Combine(VoipCallsDir, $"{call}.media"));
                    File.WriteAllBytes(filepath, call.GetRTPStream());
                }
            }

            return VoipCallsDir;
        }

    }
}
