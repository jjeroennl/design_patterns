using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace idetector.Models
{
    public class ClassModel
    {
        public List<MethodModel> methods = new List<MethodModel>();
        public List<PropertyModel> properties = new List<PropertyModel>();

        public ClassModel(ClassDeclarationSyntax node)
        {

        }
    }
}
