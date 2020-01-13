using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using idetector.Collections;
using idetector.Parser;

namespace idetector.CodeLoader
{
    public class FileReader
    {
        public static ClassCollection ReadSingleFile(string path)
        {
            try
            {
                var dataStream = System.IO.File.ReadAllText(path);
                return CodeParser.Parse(dataStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        
        private static string ReadFile(string path)
        {
            return System.IO.File.ReadAllText(path);
        }

        public static ClassCollection ReadDirectory(string path)
        {
            var files = SearchDirectoryForCSFiles(path, new List<string>());
            string dataStream = "";

            foreach (var file in files)
            {
                dataStream += FileReader.ReadFile(file);
            }

            return CodeParser.Parse(dataStream);
        }

        private static List<string> SearchDirectoryForCSFiles(string sDir, List<string> s)
        {
            try
            {
                foreach (string dir in Directory.GetDirectories(sDir))
                {
                    if (dir.Contains(".vs") || dir.Contains("\\obj\\") || dir.Contains("\\bin\\"))
                    {
                        continue;
                    }
                    

                    SearchDirectoryForCSFiles(dir, s);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }

            foreach (string file in Directory.GetFiles(sDir))
            {
                if (file.EndsWith(".cs") && !file.ToLower().Contains("assemblyinfo.cs"))
                {
                    s.Add((file));
                }
            }

            return s;
        }
    }
}