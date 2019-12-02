using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using idetector.CodeLoader;
using idetector.Collections;
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
                        x = new AnotherOne();
                    }
                    public void Foo(){
                        return""Bar"";
                    }
                }

                class User{
                    private static User me;

                    private User(){

                    }

                    public static User getUser(){
                        if(this.me == null){
                            this.me = new User();
                        }

                        return this.me;
                    }
                }
            }");

            Walker.GenerateModels(tree);

            int score = 0;
            string name = "";
            foreach (var item in ClassCollection.GetClasses())
            {
                var cls = item.Value;
                Singleton singleton = new Singleton(cls);
                singleton.Scan();
                if (score < singleton.Score())
                {
                    score = singleton.Score();
                    name = cls.Identifier;
                }
            }
            Console.WriteLine(name + " Scoort: " + score + " punten");
            while (true) ;

        }
    }
}
