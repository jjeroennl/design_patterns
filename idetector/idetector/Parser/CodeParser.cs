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
            
            ClassWalker w = new ClassWalker();
            w.Visit(tree.GetRoot());
            
            Walker w2 = new Walker();
            w2.Visit(tree.GetRoot()); 
        }
    }
}