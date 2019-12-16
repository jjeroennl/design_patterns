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
        SyntaxTree noPrivateConstructor()
        {
            return CSharpSyntaxTree.ParseText(@" 
               class User{
                    private static User me;

                    public User(){

                    }
                    
                    public static User getUser(){
                        if(this.me == null){
                            this.me = new User();
                        }
                        
                        return this.me;
                    }
                }");
        }
        SyntaxTree noPrivateSelf()
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
        SyntaxTree NoStaticSelf()
        {
            return CSharpSyntaxTree.ParseText(@" 
               class User{
                    private User me;

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
        SyntaxTree noGetInstance()
        {
            return CSharpSyntaxTree.ParseText(@" 
               class User{
                    private static User me;

                    private User(){

                    }
                    
                    public void User getUser(){
                        if(this.me == null){
                            this.me = new User();
                        }
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
                    
                    public User getUser(){
                        if(this.me == null){
                            this.me = new User();
                        }
                        
                        return this.me;
                    }
                }");
        }
        SyntaxTree noCreationOfSelf()
        {
            return CSharpSyntaxTree.ParseText(@" 
               class User{
                    private static User me;

                    private User(){

                    }
                    
                    public static User getUser(){
                        if(this.me == null);
                        
                        return this.me;
                    }
                }");
        }

        [Fact]
        public void Test_Singleton_Succeed()
        {
            var tree = Successsetup();
            var collection = Walker.GenerateModels(tree);

            Singleton singleton = new Singleton(collection.GetClass("User"));
            singleton.Scan();
            Assert.Equal(100, singleton.Score());
        }
      
        [Fact]
        public void Test_Singleton_NoPrivateConstructor()
        {
            var tree = noPrivateConstructor();
            var collection = Walker.GenerateModels(tree);

            Singleton singleton = new Singleton(collection.GetClass("User"));
            singleton.Scan();
            Assert.NotEqual(100, singleton.Score());
        }
        [Fact]
        public void Test_Singleton_noPrivateStatic()
        {
            var tree = noPrivateSelf();
            var collection = Walker.GenerateModels(tree);

            Singleton singleton = new Singleton(collection.GetClass("User"));
            singleton.Scan();
            Assert.NotEqual(100, singleton.Score());
        }
        [Fact]
        public void Test_Singleton_NoStaticSelf()
        {
            var tree = NoStaticSelf();
            var collection = Walker.GenerateModels(tree);

            Singleton singleton = new Singleton(collection.GetClass("User"));
            singleton.Scan();
            Assert.NotEqual(100, singleton.Score());
        }
        [Fact]
        public void Test_Singleton_NoGetInstance()
        {
            var tree = noGetInstance();
            var collection = Walker.GenerateModels(tree);

            Singleton singleton = new Singleton(collection.GetClass("User"));
            singleton.Scan();
            Assert.NotEqual(100, singleton.Score());
        }
        [Fact]
        public void Test_Singleton_NoStaticGetInstance()
        {
            var tree = noStaticGetInstancesetup();
            var collection = Walker.GenerateModels(tree);

            Singleton singleton = new Singleton(collection.GetClass("User"));
            singleton.Scan();
            Assert.NotEqual(100, singleton.Score());
        }
        [Fact]
        public void Test_Singleton_NoCreationOfSelf()
        {
            var tree = noCreationOfSelf();
            var collection = Walker.GenerateModels(tree);

            Singleton singleton = new Singleton(collection.GetClass("User"));
            singleton.Scan();
            Assert.NotEqual(100, singleton.Score());
        }
    }
}

