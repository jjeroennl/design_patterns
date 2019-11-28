using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace idetector.Models
{
    public class MethodModel
    {
        /// <summary>
        /// Getters/setters for method data.
        /// </summary>
        private MethodDeclarationSyntax _node { get; set; }
        public string[] Modifiers { get; set; }
        public string ReturnType { get; set; }
        public string Identifier { get; set; }
        public string Parameters { get; set; }
        public string Body { get; set; }

        /// <summary>
        /// Constructor for extracting method data from given node.
        /// </summary>
        /// <param name="node">Node to extract data from.</param>
        public MethodModel(MethodDeclarationSyntax node)
        {
            _node = node; 
            Modifiers = new string[_node.Modifiers.Count];
            for (int i = 0; i < _node.Modifiers.Count; i++)
            {
                Modifiers[i] = _node.Modifiers[i].ToString();
            }

            ReturnType = _node.ReturnType.ToString();
            Identifier = _node.Identifier.ToString();
            Parameters = _node.ParameterList.ToString();
            Body = _node.Body.ToString();
        }
    }
}
