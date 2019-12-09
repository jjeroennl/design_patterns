using System;
using idetector.Collections;
using idetector.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace idetector.Parser
{
    public class ClassWalker: CSharpSyntaxWalker
    {
        private ClassCollection _collection;
        public ClassWalker(ClassCollection collection)
        {
            _collection = collection;
        }
        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            ClassModel classModel = new ClassModel(node);
            _collection.AddClass(classModel);
            base.VisitClassDeclaration(node);
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            ClassModel cls = new ClassModel(node);
            _collection.AddClass(cls);
            base.VisitInterfaceDeclaration(node);
        }
    }
}