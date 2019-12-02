using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using idetector.CodeLoader;
using idetector.Models;
using idetector.Parser;
using idetector.Patterns;
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
                    private static AnotherOne x;
                    AnotherOne(){
                        Console.WriteLine('Hello, World!');
                    }
                    public void Foo(){
                        return""Bar"";
                    }
                }
            }");

            ClassWalker w = new ClassWalker();
            w.Visit(tree.GetRoot());
            
            Walker w2 = new Walker();
            w2.Visit(tree.GetRoot());

            Singleton singleton = new Singleton();
            singleton.Scan();
            Console.WriteLine(singleton.Score());
            while (true) ;

        }
    }
}