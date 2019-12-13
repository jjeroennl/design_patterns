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
        SyntaxTree SuccessSetup()
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
        SyntaxTree StateSetup()
        {
            return CSharpSyntaxTree.ParseText(@"
            namespace RefactoringGuru.DesignPatterns.State.Conceptual
                {
                class Context
                {
                    private State _state;

                    public Context(State state)
                    {
                        _state = null;
                        this.TransitionTo(state);
                    }

                    public void TransitionTo(State state)
                    {
                        this._state = state;
                        this._state.SetContext(this);
                    }
                    public void Request1()
                    {
                        this._state.Handle1();
                    }

                    public void Request2()
                    {
                        this._state.Handle2();
                    }
                }

                abstract class State
                {
                    protected Context _context;

                    public void SetContext(Context context)
                    {
                        this._context = context;
                    }

                    public abstract void Handle1();

                    public abstract void Handle2();
                }

                class ConcreteStateA : State
                {
                    public override void Handle1()
                    {
                        this._context.TransitionTo(new ConcreteStateB());
                    }

                    public override void Handle2()
                    {
                    }
                }

                class ConcreteStateB : State
                {
                    public override void Handle1()
                    { }

                    public override void Handle2()
                    {
                        this._context.TransitionTo(new ConcreteStateA());
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
            var tree = SuccessSetup();
            var collection = Walker.GenerateModels(tree);

            StateStrategy strategy = new StateStrategy(collection);
            strategy.Scan();
            /*
            bool hundred = false;
            if (strategy.Score() >= 95)
            {
                hundred = true;
            }
            Assert.True(hundred);
            */
            Assert.Equal(100, strategy.Score());
        }
        [Fact]
        public void Test_State()
        {
            var tree = StateSetup();
            var collection = Walker.GenerateModels(tree);

            StateStrategy strategy = new StateStrategy(collection);
            strategy.Scan();
            Assert.InRange(strategy.Score(), 85, 95);
        }
        [Fact]
        public void Test_Strategy_NoContext()
        {
            var tree = FailSetup();
            var collection = Walker.GenerateModels(tree);

            StateStrategy strategy = new StateStrategy(collection);
            strategy.Scan();
            Assert.Equal(0, strategy.Score());
        }
        [Fact]
        public void Test_Strategy_NoInterface()
        {
            var tree = FailSetup();
            var collection = Walker.GenerateModels(tree);

            StateStrategy strategy = new StateStrategy(collection);
            strategy.Scan();
            Assert.Equal(0, strategy.Score());
        }
        [Fact]
        public void Test_Strategy_NoConcreteStrategy()
        {
            var tree = FailSetup();
            var collection = Walker.GenerateModels(tree);

            StateStrategy strategy = new StateStrategy(collection);
            strategy.Scan();
            Assert.Equal(0, strategy.Score());
        }
        [Fact]
        public void Test_Strategy_NoStrategySetter()
        {
            var tree = FailSetup();
            var collection = Walker.GenerateModels(tree);

            StateStrategy strategy = new StateStrategy(collection);
            strategy.Scan();
            Assert.Equal(0, strategy.Score());
        }
        [Fact]
        public void Test_Strategy_NoPrivateStrategy()
        {
            var tree = FailSetup();
            var collection = Walker.GenerateModels(tree);

            StateStrategy strategy = new StateStrategy(collection);
            strategy.Scan();
            Assert.Equal(0, strategy.Score());
        }
        [Fact]
        public void Test_Strategy_NoMethodContext()
        {
            var tree = FailSetup();
            var collection = Walker.GenerateModels(tree);

            StateStrategy strategy = new StateStrategy(collection);
            strategy.Scan();
            Assert.Equal(0, strategy.Score());
        }
        [Fact]
        public void Test_Strategy_RelationsBetweenStrategies ()
        {
            var tree = FailSetup();
            var collection = Walker.GenerateModels(tree);

            StateStrategy strategy = new StateStrategy(collection);
            strategy.Scan();
            Assert.Equal(0, strategy.Score());
        }
    }
}
