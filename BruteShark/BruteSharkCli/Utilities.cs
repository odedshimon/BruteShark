using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BruteSharkCli
{
    public static class Utilities
    {
        public static DataTable ToDataTable<T>(this IEnumerable<T> items, int itemLengthLimit = -1)
        {
            var dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                dataTable.Columns.Add(prop.Name, prop.PropertyType);
            }

            foreach (var item in items)
            {
                var values = new object[props.Length];

                for (var i = 0; i < props.Length; i++)
                {
                    var cellData = props[i].GetValue(item, null).ToString();

                    if (itemLengthLimit > -1 && cellData.Length > itemLengthLimit)
                    {
                        cellData = cellData.Substring(0, itemLengthLimit) + "...";
                    }

                    values[i] = cellData;
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public static void PrintBruteSharkAsciiArt()
        {
            var bruteSharkAscii =@" 

      ,|               ________  ________  ___  ___  _________  _______             ________  ___  ___  ________  ________  ___  __       
     / ;               |\   __  \|\   __  \|\  \|\  \|\___   ___\\  ___ \          |\   ____\|\  \|\  \|\   __  \|\   __  \|\  \|\  \     
    /  \               \ \  \|\ /\ \  \|\  \ \  \\\  \|___ \  \_\ \   __/|   ______\ \  \_____ \  \\\  \ \  \|\  \ \  \|\  \ \  \/  /|_   
   : ,'(                \ \   __  \ \   _  _\ \  \\\  \   \ \  \ \ \  \_|/__|\______\ \_____  \ \   __  \ \   __  \ \   _  _\ \   ___  \  
   |( `.\                \ \  \|\  \ \  \\  \\ \  \\\  \   \ \  \ \ \  \_|\ \|______|\|____|\  \ \  \ \  \ \  \ \  \ \  \\  \\ \  \\ \  \ 
   : \  `\       \.       \ \_______\ \__\\ _\\ \_______\   \ \__\ \ \_______\         ____\_\  \ \__\ \__\ \__\ \__\ \__\\ _\\ \__\\ \__\
    \ `.         | `.      \|_______|\|__|\|__|\|_______|    \|__|  \|_______|        |\_________\|__|\|__|\|__|\|__|\|__|\|__|\|__| \|__|
     \  `-._     ;   \                                                                \|_________|                                        
      \     ``-.'.. _ `._
       `. `-.            ```-...__                                (Network Forensic Analysis Tool)
        .'`.        --..          ``-..____	
      ,'.-'`,_-._            ((((   <o.   ,'							
           `' `-.)``-._-...__````  ____.-'
               ,'    _,'.--,---------'
           _.-' _..-'   ),'
          ``--''        `       ";

            Console.WriteLine(bruteSharkAscii);
        }

        internal static void ExportHashes(string dirPath, HashSet<PcapAnalyzer.NetworkHash> hashes)
        {
            // Run on each Hash Type we found.
            string hashesPath = Path.Combine(dirPath, "Hasehs");
            Directory.CreateDirectory(hashesPath);

            foreach (string hashType in hashes.Select(h => h.HashType).Distinct())
            {
                try
                {
                    // Convert all hashes from that type to Hashcat format.
                    var hashesToExport = hashes.Where(h => (h as PcapAnalyzer.NetworkHash).HashType == hashType)
                                                .Select(h => BruteForce.Utilities.ConvertToHashcatFormat(
                                                             CommonUi.Casting.CastAnalyzerHashToBruteForceHash(h)));

                    var outputFilePath = CommonUi.Exporting.GetUniqueFilePath(Path.Combine(hashesPath, $"Brute Shark - {hashType} Hashcat Export.txt"));

                    using (var streamWriter = new StreamWriter(outputFilePath, true))
                    {
                        foreach (var hash in hashesToExport)
                        {
                            streamWriter.WriteLine(hash);
                        }
                    }

                    Console.WriteLine("Hashes file created: " + outputFilePath);
                }
                catch (Exception ex)
                {
                    // In case Casting.CastAnalyzerHashToBruteForceHash(h) fails and throws exception for not supported hash type
                    continue;
                }
            }
        }

        internal static void ExportNetworkMap(string dirPath, HashSet<PcapAnalyzer.NetworkConnection> connections)
        {
            string netowrkMapPath = Path.Combine(Path.Combine(dirPath, "NetworkMap"), "networkmap.json");
            Directory.CreateDirectory(netowrkMapPath);

            PcapAnalyzer.NetwrokMapJsonExporter.FileExport(connections.ToList<PcapAnalyzer.NetworkConnection>(), netowrkMapPath);

            Console.WriteLine($"Successfully exported network map to json file:  {netowrkMapPath}");
        }

    }
}
