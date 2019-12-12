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
    public class StrategyTest
    {
        SyntaxTree Successsetup()
        {
            return CSharpSyntaxTree.ParseText(@"
            namespace RefactoringGuru.DesignPatterns.Strategy.Conceptual
            {
                class Context
                {
                    private IStrategy _strategy;

                    public Context()
                    { }

                    public Context(IStrategy strategy)
                    {
                        this._strategy = strategy;
                    }

                    public void SetStrategy(IStrategy strategy)
                    {
                        this._strategy = strategy;
                    }

                    public void DoSomeBusinessLogic()
                    {
                        var result = this._strategy.DoAlgorithm(new List<string> { 'a', 'b', 'c', 'd', 'e' });

                        string resultStr = string.Empty;
                        foreach (var element in result as List<string>)
                        {
                            resultStr += element + ',';
                        }
                    }
                }
                public interface IStrategy
                {
                    object DoAlgorithm(object data);
                }

                class ConcreteStrategyA : IStrategy
                {
                    public object DoAlgorithm(object data)
                    {
                        var list = data as List<string>;
                        list.Sort();

                        return list;
                    }
                }

                class ConcreteStrategyB : IStrategy
                {
                    public object DoAlgorithm(object data)
                    {
                        var list = data as List<string>;
                        list.Sort();
                        list.Reverse();

                        return list;
                    }
                }
            }");
        }

        SyntaxTree FailSetup()
        {
            return CSharpSyntaxTree.ParseText(@"
            namespace RefactoringGuru.DesignPatterns.Strategy.Conceptual
            {
                class Context
                {
                    private IStrategy _strategy;

                    public Context()
                    { }

                    public Context(IStrategy strategy)
                    { }

                    public void SetStrategy(IStrategy strategy)
                    { }

                    public void DoSomeBusinessLogic()
                    { }
                }
                public class IStrategy
                {
                    object DoAlgorithm(object data);
                }

                class ConcreteStrategyA
                { }

                class ConcreteStrategyB
                { }
            }");
        }
        [Fact]
        public void Test_Strategy_Succeed()
        {
            var tree = Successsetup();
            var collection = Walker.GenerateModels(tree);

            Strategy strategy = new Strategy(collection);
            strategy.Scan();
            bool hundred = false;
            if (strategy.Score() >= 95)
            {
                hundred = true;
            }
            Assert.True(hundred);
        }
        [Fact]
        public void Test_Strategy_NoContext()
        {
            var tree = FailSetup();
            var collection = Walker.GenerateModels(tree);

            Strategy strategy = new Strategy(collection);
            strategy.Scan();
            Assert.Equal(0, strategy.Score());
        }
        [Fact]
        public void Test_Strategy_NoInterface()
        {
            var tree = FailSetup();
            var collection = Walker.GenerateModels(tree);

            Strategy strategy = new Strategy(collection);
            strategy.Scan();
            Assert.Equal(0, strategy.Score());
        }
        [Fact]
        public void Test_Strategy_NoConcreteStrategy()
        {
            var tree = FailSetup();
            var collection = Walker.GenerateModels(tree);

            Strategy strategy = new Strategy(collection);
            strategy.Scan();
            Assert.Equal(0, strategy.Score());
        }
        [Fact]
        public void Test_Strategy_NoStrategySetter()
        {
            var tree = FailSetup();
            var collection = Walker.GenerateModels(tree);

            Strategy strategy = new Strategy(collection);
            strategy.Scan();
            Assert.Equal(0, strategy.Score());
        }
        [Fact]
        public void Test_Strategy_NoPrivateStrategy()
        {
            var tree = FailSetup();
            var collection = Walker.GenerateModels(tree);

            Strategy strategy = new Strategy(collection);
            strategy.Scan();
            Assert.Equal(0, strategy.Score());
        }
        [Fact]
        public void Test_Strategy_NoMethodContext()
        {
            var tree = FailSetup();
            var collection = Walker.GenerateModels(tree);

            Strategy strategy = new Strategy(collection);
            strategy.Scan();
            Assert.Equal(0, strategy.Score());
        }
        [Fact]
        public void Test_Strategy_RelationsBetweenStrategies ()
        {
            var tree = FailSetup();
            var collection = Walker.GenerateModels(tree);

            Strategy strategy = new Strategy(collection);
            strategy.Scan();
            Assert.Equal(0, strategy.Score());
        }
    }
}
