using idetector.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using Xunit;

namespace xUnitTest
{
    public class MethodModelTests
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
                    static void Main(string[] args , string foo)
                    {
Console.WriteLine('Hello, World!');
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
            MethodDeclarationSyntax methodDeclaration = (MethodDeclarationSyntax)classDeclaration.Members[0];

            MethodModel methodModel = new MethodModel(methodDeclaration);

            Assert.Equal("static", methodModel.Modifiers[0]);
            Assert.Equal("void", methodModel.ReturnType);
            Assert.Equal("Main", methodModel.Identifier);
            Assert.Equal("string[] args , string foo", methodModel.Parameters);
            Assert.Equal("{Console.WriteLine('Hello, World!');}", methodModel.Body.Replace(System.Environment.NewLine, ""));
            
            Assert.True(methodModel.HasModifier("static"));
        }

    }
}
