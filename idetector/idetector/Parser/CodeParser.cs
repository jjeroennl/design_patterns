using idetector.Collections;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace idetector.Parser
{
    public class CodeParser
    {
        public static void Parse(string code)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            Walker.GenerateModels(tree);
        }
    }
}