using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace idetector.Models
{
    class PropertyModel
    {
        private PropertyDeclarationSyntax _node { get; set; }
        public ClassModel Parent { get; set; }
        public string[] Modifier { get; set; }
        public string Type { get; set; }
        public string Identifier { get; set; }

        public PropertyModel(PropertyDeclarationSyntax node)
        {
            _node = node;

            Modifier = new string[_node.Modifiers.Count];
            for (int i = 0; i < _node.Modifiers.Count; i++)
            {
                Modifier[i] = _node.Modifiers[i].ToString();
            }

            Type = _node.Type.ToString();
            Identifier = _node.Identifier.ToString();
        }
    }
}
