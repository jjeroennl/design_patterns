using System;
using idetector.CodeLoader;
using idetector.Collections;
using idetector.Models;
using idetector.Patterns;
using idetector.Patterns.Facade;

namespace idetector
{
    public class ConsoleApp
    {
        private ClassCollection collection;

        public ConsoleApp()
        {
            Console.WriteLine(@" _     _      _            _             
(_) __| | ___| |_ ___  ___| |_ ___  _ __ 
| |/ _` |/ _ \ __/ _ \/ __| __/ _ \| '__|
| | (_| |  __/ ||  __/ (__| || (_) | |   
|_|\__,_|\___|\__\___|\___|\__\___/|_|");
            Console.WriteLine();
            this.printRequest();
            this.getResult();
        }

        private void printRequest()
        {
            Console.WriteLine("Vul een pad in naar je C# bestand:");
            var path = Console.ReadLine();

            var file = FileReader.ReadSingleFile(path);


            if (file == null)
            {
                this.printRequest();
            }
            else
            {
                this.collection = file;
            }
        }

        private void getResult()
        {
            Facade f = new Facade(collection);
            f.Scan();

            foreach (var item in this.collection.GetClasses())
            {
                Singleton s = new Singleton(item.Value);
                s.Scan();
                Decorator d = new Decorator(item.Value, collection.GetClasses());
                d.Scan();
             
                Console.WriteLine(item.Value.Identifier + ": ");

                this.printBar(item.Value, "Singleton", s.Score());
                this.printBar(item.Value,"Decorator", d.Score());
                this.printBar(item.Value,"Facade", f.Score(item.Value));
            }
        }

        private void printBar(ClassModel item, string name, int score)
        {
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