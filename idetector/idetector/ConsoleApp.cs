using System;
using idetector.CodeLoader;
using idetector.Collections;
using idetector.Models;
using idetector.Patterns;

namespace idetector
{
    public class ConsoleApp
    {
        public ConsoleApp()
        {
            this.printRequest();
            this.getResult();
        }

        private void printRequest()
        {
            Console.WriteLine("Vul een pad in naar je C# bestand:");
            var path = Console.ReadLine();

            var file = FileReader.ReadSingleFile(path);

            if (!file)
            {
                this.printRequest();
            }
        }

        private void getResult()
        {
            foreach (var item in ClassCollection.GetClasses())
            {
                Singleton s = new Singleton(item.Value);
                s.Scan();
                this.printBar(item.Value, "Singleton", s.Score());
            }
        }

        private void printBar(ClassModel item, string name, int score)
        {
            Console.WriteLine(item.Identifier + ": ");
            Console.WriteLine("\t " +name + " " + score + "%:");
            Console.WriteLine("\t" + new string('-', 102));
            Console.Write("\t|");
                
            Console.Write(new string('█', score));

            if (100 - score != 0)
            {
                Console.Write(new string(' ', 100 - score));
            }
                
            Console.Write("|\n");
            Console.WriteLine("\t" + new string('-', 102));        }
    }
}