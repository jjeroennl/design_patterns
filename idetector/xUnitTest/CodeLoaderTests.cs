using idetector.Collections;
using idetector.Parser;
using Xunit;

namespace xUnitTest
{
    public class CodeLoaderTests
    {
        [Fact]
        public void TestCodeParser()
        {
            var code = @"using System;
            using System.Collections;
            using System.Linq;
            using System.Text;
 
            namespace HelloWorld
            {
                class Program : Controller
                {
                    private override int getal {get; set;}
                    static void Main(string[] args , string foo)
                    {
                        Console.WriteLine(""Hello, World!"");
                    }
                    public void Foo(){
                        return""Bar"";
                    }
                }
            }";

            var collection = CodeParser.Parse(code);
            
            Assert.IsType<ClassCollection>(collection);
            
            //COLLECTION NOT YET IMPLEMENTED, ADD UNIT TESTS HERE WHEN CREATING IT.
        }
    }
}