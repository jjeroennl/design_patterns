using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;


namespace idetector.Models
{
    public class MethodModel
    {
        /// <summary>
        /// Getters/setters for method data.
        /// </summary>
        private BaseMethodDeclarationSyntax _node { get; set; }

        public bool isConstructor = false;
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
            Modifiers = new string[node.Modifiers.Count];
            for (int i = 0; i <node.Modifiers.Count; i++)
            {
                Modifiers[i] = node.Modifiers[i].ToString();
            }

            ReturnType = node.ReturnType.ToString();
            Identifier = node.Identifier.ToString();
            Parameters = node.ParameterList.ToString();
            if (node.Body != null)
            {
                Body = node.Body.ToString();
            }
        }

        public MethodModel(ConstructorDeclarationSyntax node, string type)
        {
            _node = node;
            Modifiers = new string[node.Modifiers.Count];
            for (int i = 0; i < node.Modifiers.Count; i++)
            {
                Modifiers[i] = node.Modifiers[i].ToString();
            }

            isConstructor = true;
            ReturnType = type;
            Identifier = node.Identifier.ToString();
            Parameters = node.ParameterList.ToString();
            if (node.Body != null)
            {
                Body = node.Body.ToString();
            }
        }

        public BaseMethodDeclarationSyntax getNode()
        {
            return _node;
        }
    }
}
