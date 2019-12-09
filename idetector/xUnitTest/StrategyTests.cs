using System;
using System.Collections.Generic;
using System.Text;
using idetector.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using idetector.Collections;
using idetector.Parser;
using idetector.Patterns;
using Xunit;

namespace xUnitTest
{
    public class StrategyTests
    {
        SyntaxTree Successsetup()
        {
            return CSharpSyntaxTree.ParseText(@" 
               class User{
                    private static User me;

                    private User(){

                    }
                    
                    public static User getUser(){
                        if(this.me == null){
                            this.me = new User();
                        }
                        
                        return this.me;
                    }
                }");
        }
        [Fact]
        public void Test_Strategy_Succeed()
        {
            var tree = Successsetup();
            Walker.GenerateModels(tree);

            Strategy strategy = new Strategy(ClassCollection.GetClass("User"));
            strategy.Scan();
            Assert.Equal(100, strategy.Score());
        }
    }
}
