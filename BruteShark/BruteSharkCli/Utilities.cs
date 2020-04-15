using System;
using System.Collections.Generic;
using System.Data;
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

    }
}
