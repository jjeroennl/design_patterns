using idetector.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using idetector.Collections;
using idetector.Parser;
using idetector.Patterns;
using Xunit;

namespace xUnitTest
{
    public class SingletonTest
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
        SyntaxTree noStaticGetInstancesetup()
        {
            return CSharpSyntaxTree.ParseText(@" 
               class User{
                    private static User me;

                    private User(){

                    }
                    
                    public  User getUser(){
                        if(this.me == null){
                            this.me = new User();
                        }
                        
                        return this.me;
                    }
                }");
        }
        SyntaxTree noPrivateStaticSelfsetup()
        {
            return CSharpSyntaxTree.ParseText(@" 
               class User{
                    public static User me;

                    private static User(){

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
        public void Test_Singleton_Succeed()
        {
            var tree = Successsetup();
            Walker.GenerateModels(tree);

            Singleton singleton = new Singleton(ClassCollection.GetClass("User"));
            singleton.Scan();
            Assert.Equal(100, singleton.Score());
            
        }
        [Fact]
        public void Test_Singleton_NoStaticInstance()
        {
            var tree = noStaticGetInstancesetup();
            Walker.GenerateModels(tree);

            Singleton singleton = new Singleton(ClassCollection.GetClass("User"));
            singleton.Scan();
            Assert.Equal(75, singleton.Score());

        }
        [Fact]
        public void Test_Singleton_noPrivateStatic()
        {
            var tree = noPrivateStaticSelfsetup();
            Walker.GenerateModels(tree);

            Singleton singleton = new Singleton(ClassCollection.GetClass("User"));
            singleton.Scan();
            Assert.Equal(75, singleton.Score());

        }
    }
}

