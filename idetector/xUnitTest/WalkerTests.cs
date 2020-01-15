using idetector.Collections;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Walker = idetector.Parser.Walker;

using  Xunit;

namespace xUnitTest
{
    public class WalkerTests
    {
        private ClassCollection prepare()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"using System;
            using System.Collections;
            using System.Linq;
            using System.Text;
 
            namespace HelloWorld
            {
                interface Controller{
                    int x {get; set;}
                }

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

            return Walker.GenerateModels(tree);

        }
        
        [Fact]
        public void Test_Construction()
        {
            var collection = this.prepare();
            
            var cls = collection.GetClass("User");

            Assert.Equal("User", cls.Identifier);
            Assert.Single(cls.GetProperties());
            Assert.Equal(2, cls.GetMethods().Count);
            Assert.False(cls.IsInterface);
        }
        [Fact]
        public void Test_Interface()
        {
            var collection = this.prepare();

            var intface = collection.GetClass("Controller");
            Assert.Equal("Controller", intface.Identifier);
            Assert.Single(intface.GetProperties());
            Assert.True(intface.IsInterface);
        }
    }
}