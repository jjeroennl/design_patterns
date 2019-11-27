using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace idetector.Models
{
    public class PropertyModel
    {
        private PropertyDeclarationSyntax _node { get; set; }
        public ClassModel Parent { get; set; }
        public string[] Modifiers { get; set; }
        public string Type { get; set; }
        public string Identifier { get; set; }

        public PropertyModel(PropertyDeclarationSyntax node)
        {
            _node = node;
            Modifiers = new string[_node.Modifiers.Count];
            for (int i = 0; i < _node.Modifiers.Count; i++)
            {
                Modifiers[i] = _node.Modifiers[i].ToString();
            }

            Type = _node.Type.ToString();
            Identifier = _node.Identifier.ToString();
        }
    }
}
