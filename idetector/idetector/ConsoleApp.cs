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
            Decorator d = new Decorator(collection);
            d.Scan();

            Observer obs = new Observer(collection);
            obs.Scan();

            //foreach (var item in this.collection.GetClasses())
            //{
            //    Console.WriteLine(item.Value.Identifier + ": ");
            //    Singleton s = new Singleton(item.Value);
            //    s.Scan();

            //    foreach (var result in s.GetResults())
            //    {
            //        printBar(item.Value, "Singleton: " + result.Key, Calculator.GetScore("SINGLETON", result.Value));
            //    }
            //    // printBar(item.Value,"Facade", f.Score(item.Value));
            //}

            //foreach (var result in d.GetResults())
            //{
            //    printBar(collection.GetClass(result.Key),"Decorator", Calculator.GetScore("DECORATOR", result.Value));
            //}
            
            printBar("Factory Method", Calculator.GetScore("FACTORY", fm.GetResult()));

            printBar("Abstract Factory", Calculator.GetScore("ABSTRACT-FACTORY", af.GetResult()));

            foreach (var result in state.GetResults())
            {
                Console.WriteLine(result.Key + ": ");
                printBar(collection.GetClass(result.Key), "State", Calculator.GetScore("STATE", result.Value));
            }
            foreach (var result in strat.GetResults())
            {
                Console.WriteLine(result.Key + ": ");
                printBar(collection.GetClass(result.Key), "Strategy", Calculator.GetScore("STRATEGY", result.Value));
            }

            foreach (var result in obs.GetResults())
            {
                Console.WriteLine(result.Key + ": ");
                printBar(collection.GetClass(result.Key), "Observer", Calculator.GetScore("OBSERVER", result.Value));
            }
        }

        private void printBar(string name, int score)
        {
            Console.WriteLine(name + ": ");
            Console.WriteLine("\t " + score + "%:");
            Console.WriteLine("\t" + new string('-', 102));
            Console.Write("\t|");

            Console.Write(new string('█', score));

            if (200 - score != 0)
            {
                Console.Write(new string(' ', 200 - score));
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