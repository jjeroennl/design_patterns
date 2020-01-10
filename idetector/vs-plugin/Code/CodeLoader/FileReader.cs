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

        public static void ReadDirectory(string path)
        {
            var files = SearchDirectoryForCSFiles(path, new List<string>());
            string dataStream = "";

            foreach (var file in files)
            {
                dataStream += FileReader.ReadFile(file);
            }

            CodeParser.Parse(dataStream);
        }

        private static List<string> SearchDirectoryForCSFiles(string sDir, List<string> s)
        {
            try
            {
                foreach (string dir in Directory.GetDirectories(sDir))
                {
                    foreach (string file in Directory.GetFiles(dir))
                    {
                        if (file.EndsWith(".cs"))
                        {
                            s.Add((file));
                        }
                    }

                    SearchDirectoryForCSFiles(dir, s);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }

            return s;
        }
    }
}