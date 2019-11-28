using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using idetector.CodeLoader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace idetector
{
    class Program
    {
        static async Task Main(string[] args)
        {
            FileReader.ReadDirectory("/Users/jeroentissink/Github/design_patterns/idetector/");
            
        }
    }
}