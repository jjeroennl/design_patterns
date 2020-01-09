using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace idetector.Models
{
    public class PropertyModel
    {
        private MemberDeclarationSyntax _node;
        public string Parent { get; set; }
        public Type ParentType { get; set; }
        public string[] Modifiers { get; set; }
        public bool isOverride { get; set; }
        public string ValueType { get; set; }
        public Type Type { get; set; }
        public string Identifier { get; set; }


        public PropertyModel(PropertyDeclarationSyntax node)
        {
            _node = node;
            Modifiers = new string[node.Modifiers.Count];
            for (int i = 0; i < node.Modifiers.Count; i++)
            {
                Modifiers[i] = node.Modifiers[i].ToString();
                if (node.Modifiers[i].ToString().Equals("override"))
                {
                    isOverride = true;
                }
            }

            ValueType = node.Type.ToString();
            _setParent(node.Parent);
            Type = Type.PropertySyntax;
            Identifier = node.Identifier.ToString();
        }
        public PropertyModel(FieldDeclarationSyntax node)
        {
            _node = node;
            Modifiers = new string[node.Modifiers.Count];
            for (int i = 0; i < node.Modifiers.Count; i++)
            {
                Modifiers[i] = node.Modifiers[i].ToString();
            }
            _setParent(node.Parent);
            ValueType = node.Declaration.Type.ToString();
            Type = Type.FieldSyntax;
            Identifier = node.Declaration.Variables.ToString().Split(' ')[0];
        }

        private void _setParent(SyntaxNode node)
        {
            if (node is ClassDeclarationSyntax)
            {
                var temp = (ClassDeclarationSyntax) node;
                Parent = temp.Identifier.ToString();
                ParentType = Type.ClassSyntax;
            }
            if (node is MethodDeclarationSyntax)
            {
                var temp = (MethodDeclarationSyntax) node;
                Parent = temp.Identifier.ToString();
                ParentType = Type.MethodSyntax;
            }
            

        }

        public MemberDeclarationSyntax GetNode()
        {
            if (Type == Type.FieldSyntax)
            {
                return (FieldDeclarationSyntax) _node;
            }
            return (PropertyDeclarationSyntax) _node;
        }
        
        public bool HasModifier(string modifier)
        {
            return Modifiers.Any(e => e.Equals(modifier));
        }
        
    }
}
