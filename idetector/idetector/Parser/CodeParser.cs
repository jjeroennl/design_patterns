using idetector.Collections;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace idetector.Parser
{
    public class CodeParser
    {
        public static ClassCollection Parse(string code)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            
            Walker walker = new Walker();
            walker.Visit(tree.GetRoot());
            ClassCollection result = walker.getCollection();

            return result;
        }
    }
}