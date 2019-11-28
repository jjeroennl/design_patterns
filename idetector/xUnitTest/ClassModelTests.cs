using idetector.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using Xunit;

namespace xUnitTest
{
    public class ClassModelTests
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
                    static void Main(string[] args , string foo){
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

            ClassModel classModel = new ClassModel(classDeclaration);

            Assert.Equal("class", classModel.Keyword);
            Assert.Equal("Program", classModel.Identifier);
            Assert.Equal(@"static void Main(string[] args , string foo){
                        Console.WriteLine('Hello, World!');
                    }", classModel.Members[0]);
            Assert.Equal(@"public void Foo(){
                        return""Bar"";
                    }", classModel.Members[1]);
            Assert.Null(classModel.Modifiers);
            Assert.Null(classModel.Attributes);
        }
    }
}
