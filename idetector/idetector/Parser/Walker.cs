using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace idetector.Parser
{
    class Walker : CSharpSyntaxWalker
    {
        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            base.VisitNamespaceDeclaration(node);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            base.VisitClassDeclaration(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            base.VisitMethodDeclaration(node);
        }
    }
}
