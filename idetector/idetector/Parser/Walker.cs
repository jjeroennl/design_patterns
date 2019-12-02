using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using idetector.Collections;
using idetector.Models;
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

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            MethodModel methodModel = new MethodModel(node);
            base.VisitConstructorDeclaration(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {

            MethodModel methodModel = new MethodModel(node);

            var n = node.Parent;

            var shouldLoop = true;
            var loops = 0;

            while (!n.GetType().ToString().Equals("Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax") && shouldLoop)
            {
                n = n.Parent;

                loops++;
                if (loops > 500)
                {
                    shouldLoop = false;
                }
            }

            if (shouldLoop)
            {
                // If parent class found
                var cls = (ClassDeclarationSyntax)n;
                var model = ClassCollection.GetClass(cls.Identifier.ToString());
                model.AddMethod(methodModel);
            }

            base.VisitMethodDeclaration(node);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            PropertyModel propertyModel = new PropertyModel(node);
            
            base.VisitPropertyDeclaration(node);
        }

        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            var n = node.Parent;

            var shouldLoop = true;
            var loops = 0;

            while (!n.GetType().ToString().Equals("Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax") && shouldLoop)
            {
                n = n.Parent;

                loops++;
                if (loops > 500)
                {
                    shouldLoop = false;
                }
            }

            if (shouldLoop)
            {
                var _class = (ClassDeclarationSyntax) n;
                var member = ClassCollection.GetClass(node.Type.ToString());
                
                ClassCollection.GetClass(_class.Identifier.ToString()).AddObjectCreation(member);
            }
            else
            {
                throw new ClassNotException();
            }
            base.VisitObjectCreationExpression(node);
        }



        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            PropertyModel propertyModel = new PropertyModel(node);



            base.VisitFieldDeclaration(node);
        }  
        
        public ClassCollection getCollection()
        {
            return new ClassCollection();
        }
    }
}
