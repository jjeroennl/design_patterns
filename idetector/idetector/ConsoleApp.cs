using System;
using System.Collections.Generic;
using idetector.CodeLoader;
using idetector.Collections;
using idetector.Data;
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
            Requirements reqs = new Requirements();
            ScoreCalculator Calculator = new ScoreCalculator(reqs.GetRequirements());

            Facade f = new Facade(collection);
            f.Scan();

            StateStrategy state = new StateStrategy(collection, true);
            state.Scan();

            StateStrategy strat = new StateStrategy(collection, false);
            strat.Scan();

            AbstractFactoryMethod fm = new AbstractFactoryMethod(collection, true);
            fm.Scan();

            AbstractFactoryMethod af = new AbstractFactoryMethod(collection, false);
            af.Scan();

            foreach (var item in this.collection.GetClasses())
            {                
              
                Console.WriteLine(item.Value.Identifier + ": ");
                Singleton s = new Singleton(item.Value);
                s.Scan();
                Decorator d = new Decorator(item.Value, collection.GetClasses());
                d.Scan();

                this.printBar(item.Value, "Singleton", Calculator.GetScore("SINGLETON",s.GetResult()));
                this.printBar(item.Value,"Decorator", Calculator.GetScore("DECORATOR", d.GetResult()));
                this.printBar(item.Value,"Facade", f.Score(item.Value));
            }

            printBar("Factory Method", Calculator.GetScore("FACTORY", fm.GetResult()));
            printBar("Abstract Factory", Calculator.GetScore("FACTORY", af.GetResult()));
            printBar("Strategy", Calculator.GetScore("STRATEGY", strat.GetResult()));
            printBar("State", Calculator.GetScore("STATE", state.GetResult()));
        }

        private void printBar(string name, int score)
        {
            Console.WriteLine(name + ": ");
            Console.WriteLine("\t " + score + "%:");
            Console.WriteLine("\t" + new string('-', 102));
            Console.Write("\t|");

            Console.Write(new string('█', score));

            if (100 - score != 0)
            {
                Console.Write(new string(' ', 100 - score));
            }

            Console.Write("|\n");
            Console.WriteLine("\t" + new string('-', 102));
        }

     
        private void printBar(ClassModel item, string name, int score)
        {
            printBar(name, score);       
        }
    }
}