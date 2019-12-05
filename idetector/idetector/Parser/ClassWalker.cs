using System;
using idetector.Collections;
using idetector.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace idetector.Parser
{
    public class ClassWalker: CSharpSyntaxWalker
    {
        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            ClassModel classModel = new ClassModel(node);
            ClassCollection.AddClass(classModel);
            base.VisitClassDeclaration(node);
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            ClassModel cls = new ClassModel(node);
            ClassCollection.AddClass(cls);
            base.VisitInterfaceDeclaration(node);
        }
    }
}