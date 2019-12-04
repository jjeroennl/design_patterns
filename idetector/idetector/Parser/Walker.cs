using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using idetector.Collections;
using idetector.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace idetector.Parser
{
    public class Walker : CSharpSyntaxWalker
    {
        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            base.VisitNamespaceDeclaration(node);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var cls = ClassCollection.GetClass(node.Identifier.ToString());
            if(node.BaseList != null)
            {
                foreach (var n in node.BaseList.Types)
                {
                    try
                    {
                        var parentClass = ClassCollection.GetClass(n.Type.ToString());
                        if (parentClass != null)
                        {
                            cls.AddParent(parentClass);
                        }
                        else
                        {
                            cls.AddExternalParent(n.Type.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        cls.AddExternalParent(n.Type.ToString());
                    }
                   
                }
            }
            base.VisitClassDeclaration(node);
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            var parentClass = getParentClass(node);
            MethodModel methodModel = new MethodModel(node, parentClass.Identifier);
            
            parentClass.AddMethod(methodModel);
            
            base.VisitConstructorDeclaration(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {

            MethodModel methodModel = new MethodModel(node);

            var parentClass = getParentClass(node);
            parentClass.AddMethod(methodModel);

            base.VisitMethodDeclaration(node);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            var parentClass = getParentClass(node);
            
            PropertyModel propertyModel = new PropertyModel(node);
            parentClass.AddProperty(propertyModel);
            
            base.VisitPropertyDeclaration(node);
        }

        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            var me = ClassCollection.GetClass(node.Type.ToString());

            var parentClass = getParentClass(node);
            parentClass.AddObjectCreation(me);
            
            base.VisitObjectCreationExpression(node);
        }
        
        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            PropertyModel propertyModel = new PropertyModel(node);

            var parentClass = getParentClass(node);
            parentClass.AddProperty(propertyModel);

            base.VisitFieldDeclaration(node);
        }  
        
        public ClassCollection getCollection()
        {
            return new ClassCollection();
        }
        
        private ClassModel getParentClass(SyntaxNode node){
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
                var member = ClassCollection.GetClass(_class.Identifier.ToString());

                return member;
            }

            return null;
        }

        public static void GenerateModels (SyntaxTree tree)
        {
            ClassWalker w = new ClassWalker();
            w.Visit(tree.GetRoot());
            
            Walker w2 = new Walker();
            w2.Visit(tree.GetRoot());
        }
    }
}
