using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace idetector.Models
{
    public class ClassModel
    {
        private ClassDeclarationSyntax _node { get; set; }
        public string[] Modifiers { get; set; }
        public string[] Members { get; set; }
        public string[] Attributes { get; set; }
        public string Keyword { get; set; }
        public string Identifier { get; set; }

        private List<MethodModel> Methods = new List<MethodModel>();
        private List<PropertyModel> Properties = new List<PropertyModel>();
        public HashSet<ClassModel> ObjectCreations = new HashSet<ClassModel>();

        public ClassModel(ClassDeclarationSyntax node)
        {
            _node = node;
            Keyword = _node.Keyword.ToString();
            Identifier = _node.Identifier.ToString();
            
            _setMembers();
            _setAttributes();
            _setModifiers();
        }

        private void _setModifiers()
        {
            int ModifierCount = _node.Modifiers.Count;
            if (ModifierCount != 0)
            {
                Modifiers = new string[ModifierCount];
                for (int i = 0; i < ModifierCount; i++)
                {
                    Modifiers[i] = _node.Modifiers[i].ToString();
                }
            }
            else Modifiers = null;
        }

        private void _setAttributes()
        {
            int AttributeCount = _node.AttributeLists.Count;
            if (AttributeCount != 0)
            {
                Attributes = new string[AttributeCount];
                for (int i = 0; i < AttributeCount; i++)
                {
                    Attributes[i] = _node.AttributeLists[i].ToString();
                }
            }
            else Attributes = null;
        }

        private void _setMembers()
        {
            int MemberCount = _node.Members.Count;
            if (MemberCount != 0)
            {
                Members = new string[MemberCount];
                for (int i = 0; i < MemberCount; i++)
                {
                    Members[i] = _node.Members[i].ToString();
                }
            }
            else Members = null;
        }

        public ClassDeclarationSyntax GetNode()
        {
            return _node;
        }

        public void AddMethod(MethodModel method)
        {
            Methods.Add(method);
        }

        public void RemoveMethod(MethodModel method)
        {
            Methods.Remove(method);
        }

        public void AddProperty(PropertyModel property)
        {
            Properties.Add(property);
        }

        public void RemoveProperty(PropertyModel property)
        {
            Properties.Remove(property);
        }

        public void AddObjectCreation(ClassModel node)
        {
            ObjectCreations.Add(node);
        }

        public List<MethodModel> getMethods()
        {
            return Methods;
        }

        public List<PropertyModel> getProperties()
        {
            return Properties;
        }
    }
}
