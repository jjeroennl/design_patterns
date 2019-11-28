using idetector.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using Xunit;
using Type = idetector.Models.Type;

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
                    private override string str;
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
        public void Test_Property_Construction()
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
            Assert.Equal("int", propertyModel.ValueType);
            Assert.Equal(Type.PropertySyntax, propertyModel.Type);
            Assert.Equal("getal", propertyModel.Identifier);
            Assert.Equal(classDeclaration.Identifier.ToString(), propertyModel.Parent);
            Assert.Equal(Type.ClassSyntax, propertyModel.ParentType);
        }
        [Fact]
        public void Test_Field_Construction()
        {
            var tree = setup();
            var root = (CompilationUnitSyntax)tree.GetRoot();
            MemberDeclarationSyntax firstMember = root.Members[0];
            NamespaceDeclarationSyntax namespaceDeclaration = (NamespaceDeclarationSyntax)firstMember;
            ClassDeclarationSyntax classDeclaration = (ClassDeclarationSyntax)namespaceDeclaration.Members[0];
            FieldDeclarationSyntax fieldDeclaration = (FieldDeclarationSyntax) classDeclaration.Members[1];

            PropertyModel p = new PropertyModel(fieldDeclaration);

            Assert.Equal("private", p.Modifiers[0]);
            Assert.Equal("string", p.ValueType);
            Assert.Equal(Type.FieldSyntax, p.Type);
            Assert.Equal("str", p.Identifier);
            Assert.Equal(fieldDeclaration, p.GetNode());
            Assert.Equal(classDeclaration.Identifier.ToString(), p.Parent);
            Assert.Equal(Type.ClassSyntax, p.ParentType);
        }

    }
}
