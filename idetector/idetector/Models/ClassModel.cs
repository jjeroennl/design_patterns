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
        private TypeDeclarationSyntax _node { get; set; }
        public string[] Modifiers { get; set; }
        public string[] Members { get; set; }
        public string[] Attributes { get; set; }
        public string Keyword { get; set; }
        public string Identifier { get; set; }
        public bool IsInterface { get; set; } = false;
        public bool IsAbstract { get; set; } = false;

        private List<MethodModel> Methods = new List<MethodModel>();
        private List<PropertyModel> Properties = new List<PropertyModel>();
        public HashSet<ClassModel> Parents = new HashSet<ClassModel>();
        public HashSet<ClassModel> ObjectCreations = new HashSet<ClassModel>();
        public HashSet<string> UnknownParent = new HashSet<string>();
        public string Namespace;

        public ClassModel(ClassDeclarationSyntax node)
        {
            _node = node;
            Keyword = node.Keyword.ToString();
            Identifier = node.Identifier.ToString();

            _setNamespace(node);
            _setMembers();
            _setAttributes();
            _setModifiers();
        }
        public ClassModel(InterfaceDeclarationSyntax node)
        {
            _node = node;
            Keyword = node.Keyword.ToString();
            Identifier = node.Identifier.ToString();
            IsInterface = true;
            
            _setMembers();
            _setAttributes();
            _setModifiers();
        }

        private void _setNamespace(TypeDeclarationSyntax node)
        {
            SyntaxNode parent = node;
            while (!parent.GetType().ToString().Equals("Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax") && parent.Parent != null)
            {
                parent = node.Parent;
            }

            if (parent.GetType().ToString().Equals("Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax"))
            {
                var @namespace = (NamespaceDeclarationSyntax)parent;
                this.Namespace = @namespace.Name.ToString();
            }
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
                    if (Modifiers[i].ToLower().Equals("abstract"))
                    {
                        IsAbstract = true;
                    }
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

        public TypeDeclarationSyntax GetNode()
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

        public void AddParent(ClassModel classModel)
        {
            Parents.Add(classModel);
        }

        public void RemoveProperty(PropertyModel property)
        {
            Properties.Remove(property);
        }

        public void AddObjectCreation(ClassModel node)
        {
            ObjectCreations.Add(node);
        }

        public List<MethodModel> getConstructors()
        {
            List<MethodModel> constructors = new List<MethodModel>();
            foreach (var methods in Methods)
            {
                if (methods.isConstructor)
                {
                    constructors.Add(methods);
                }
            }
            return constructors;
        }

        public List<MethodModel> GetMethods()
        {
            return Methods;
        }

        public List<PropertyModel> GetProperties()
        {
            return Properties;
        }

        public bool HasParent(string name)
        {
            var hasUnknownParent = false;
            
            if(this.UnknownParent.Count > 0)
            {
                hasUnknownParent = this.UnknownParent.Any(e => e.Equals(name));
            }

            var hasKnownParent = false;
            if (this.Parents.Count > 0)
            {
                hasKnownParent = this.Parents.Any(e => e.Identifier.Equals(name));
            }
            return hasUnknownParent || hasKnownParent;
        }

        public List<string> GetParents()
        {
            List<string> returnValue = new List<string>();

            foreach (var parent in this.Parents)
            {
                returnValue.Add(parent.Identifier.ToString());
            }
            foreach (var parent in this.UnknownParent)
            {
                returnValue.Add(parent);
            }

            return returnValue;
        }

        public void AddExternalParent(string parent)
        {
            UnknownParent.Add(parent);
        }
    }
}
