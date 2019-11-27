using idetector.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using Xunit;

namespace xUnitTest
{
    public class PropertyModelTests
    {
        public SyntaxTree setup()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"using System;
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
            }");
            return tree;
        }

        [Fact]
        public void Test_Construction()
        {
            var tree = setup();
            var root = (CompilationUnitSyntax)tree.GetRoot();
            MemberDeclarationSyntax firstMember = root.Members[0];
            NamespaceDeclarationSyntax namespaceDeclaration = (NamespaceDeclarationSyntax)firstMember;
            ClassDeclarationSyntax classDeclaration = (ClassDeclarationSyntax)namespaceDeclaration.Members[0];
            PropertyDeclarationSyntax propertyDeclaration = (PropertyDeclarationSyntax)classDeclaration.Members[0];

            PropertyModel propertyModel = new PropertyModel(propertyDeclaration);

            Assert.Equal("private", propertyModel.Modifiers[0]);
            Assert.Equal("override", propertyModel.Modifiers[1]);
            Assert.Equal("int", propertyModel.Type);
            Assert.Equal("getal", propertyModel.Identifier);
        }

    }
}
