using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using idetector.CodeLoader;
using idetector.Models;
using idetector.Parser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace idetector
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string title;
            int percentage;

            Console.WriteLine(@" _     _      _            _             
(_) __| | ___| |_ ___  ___| |_ ___  _ __ 
| |/ _` |/ _ \ __/ _ \/ __| __/ _ \| '__|
| | (_| |  __/ ||  __/ (__| || (_) | |   
|_|\__,_|\___|\__\___|\___|\__\___/|_|");
            StringBuilder sb = new StringBuilder("", 2);
            sb.Append(@"
                 RESULTS
+------------------+-------------+------+
|     Pattern      | Probability | Info |
+------------------+-------------+------+");

            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"using System;
            using System.Collections;
            using System.Linq;
            using System.Text;
 
            namespace HelloWorld
            {
                class Program : Controller
                {
                    static void Main(string[] args , string foo){
                        Console.WriteLine('Hello, World!');
                        
                        var anotherOne = new AnotherOne();
                    }
                    public void Foo(){
                        return""Bar"";
                    }
                }

                class AnotherOne : Controller
                {
                    AnotherOne();
                        Console.WriteLine('Hello, World!');
                    }
                    public void Foo(){
                        return""Bar"";
                    }
                }
            }");

            /*
            ClassWalker w = new ClassWalker();
            w.Visit(tree.GetRoot());
            
            Walker w2 = new Walker();
            w2.Visit(tree.GetRoot()); 
            */

            // Singleton
            title = "Singleton";
            percentage = 80;
            sb.Append(@"
| " + title.PadRight(16) + " | " + percentage.ToString().PadRight(3) + " %       | Link |");

            sb.Append(@"
+------------------+-------------+------+");
            Console.WriteLine(sb.ToString());
        }
    }
}