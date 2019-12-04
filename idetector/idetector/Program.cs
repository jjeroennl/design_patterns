using System;
using System.Threading.Tasks;
using idetector.Collections;
using idetector.Models;
using idetector.Parser;
using idetector.Patterns;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace idetector
{
    class Program
    {
        static async Task Main(string[] args)
        {
            PriorityCollection.AddPriority("singleton", "IsPrivateConstructor", Priority.High);
            PriorityCollection.AddPriority("andere", "Method", Priority.Low);
            PriorityCollection.AddPriority("singleton", "IsGetInstance", Priority.Medium);


            Console.WriteLine(PriorityCollection.GetPercentage("singleton", "IsPrivateConstructor"));


            ConsoleApp app = new ConsoleApp();

            while (true) ;

        }
    }
}
