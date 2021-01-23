using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
    }
}
