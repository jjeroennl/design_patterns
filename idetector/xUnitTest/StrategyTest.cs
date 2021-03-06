﻿using System;
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
using idetector.Data;
using idetector;

namespace xUnitTest
{
    public class StrategyTest
    {
        [Fact]
        public void Test_Strategy_Succeed()
        {
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            var tree = SuccessSetup();
            var collection = Walker.GenerateModels(tree);
            int score = 0;

            StateStrategy strategy = new StateStrategy(collection, false);
            strategy.Scan();
            var results = strategy.GetResults();

            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                {
                    int val = calculator.GetScore("STRATEGY", results[cls.Key]);
                    if (val > score) score = val;
                }
            }
            Assert.Equal(100, score);
        }
        [Fact]
        public void Test_State()
        {
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            var tree = StateSetup();
            var collection = Walker.GenerateModels(tree);
            int score = 0;
            bool passed = false;

            StateStrategy strategy = new StateStrategy(collection, false);
            strategy.Scan();
            var results = strategy.GetResults();

            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                    foreach (var result in results[cls.Value.Identifier].ToArray())
                    {
                        if (result.Id.Equals("STRATEGY-CONCRETE-CLASS-RELATIONS"))
                            if(!passed) passed = result.Passed;
                    }
            }
            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                {
                    int val = calculator.GetScore("STRATEGY", results[cls.Key]);
                    if (val > score) score = val;
                }
            }
            Assert.False(passed);
            Assert.NotEqual(100, score);
        }
        [Fact]
        public void Test_Strategy_NoInterfaceOrAbstract()
        {
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            var tree = NoInterface();
            var collection = Walker.GenerateModels(tree);
            int score = 0;
            bool passed = false;

            StateStrategy strategy = new StateStrategy(collection, false);
            strategy.Scan();
            var results = strategy.GetResults();

            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                    foreach (var result in results[cls.Value.Identifier].ToArray())
                    {
                        if (result.Id.Equals("STATE-STRATEGY-INTERFACE-ABSTRACT"))
                            if (!passed) passed = result.Passed;
                    }
            }
            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                {
                    int val = calculator.GetScore("STRATEGY", results[cls.Key]);
                    if (val > score) score = val;
                }
            }
            Assert.False(passed);
            Assert.NotEqual(100, score);
        }
        [Fact]
        public void Test_Strategy_NoContext()
        {
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            var tree = NoContext();
            var collection = Walker.GenerateModels(tree);
            int score = 0;
            bool passed = false;

            StateStrategy strategy = new StateStrategy(collection, false);
            strategy.Scan();
            var results = strategy.GetResults();

            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                    foreach (var result in results[cls.Value.Identifier].ToArray())
                    {
                        if (result.Id.Equals("STATE-STRATEGY-HAS-CONTEXT"))
                            if (!passed) passed = result.Passed;
                    }
            }
            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                {
                    int val = calculator.GetScore("STRATEGY", results[cls.Key]);
                    if (val > score) score = val;
                }
            }
            Assert.False(passed);
            Assert.NotEqual(100, score);
        }
        [Fact]
        public void Test_Strategy_NoConcreteStrategy()
        {
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            var tree = NoConcreteStrategy();
            var collection = Walker.GenerateModels(tree);
            int score = 0;
            bool passed = false;

            StateStrategy strategy = new StateStrategy(collection, false);
            strategy.Scan();
            var results = strategy.GetResults();
            
            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                    foreach (var result in results[cls.Value.Identifier].ToArray())
                    {
                        if (result.Id.Equals("STATE-STRATEGY-CONCRETE-CLASS"))
                            if (!passed) passed = result.Passed;
                    }
            }
            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                {
                    int val = calculator.GetScore("STRATEGY", results[cls.Key]);
                    if (val > score) score = val;
                }
            }
            Assert.False(passed);
            Assert.NotEqual(100, score);
        }
        [Fact]
        public void Test_Strategy_NoPublicConstructor()
        {
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            var tree = NoPublicConstructor();
            var collection = Walker.GenerateModels(tree);
            int score = 0;
            bool passed = false;

            StateStrategy strategy = new StateStrategy(collection, false);
            strategy.Scan();
            var results = strategy.GetResults();
            
            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                    foreach (var result in results[cls.Value.Identifier].ToArray())
                    {
                        if (result.Id.Equals("STATE-STRATEGY-CONTEXT-PUBLIC-CONSTRUCTOR"))
                            if (!passed) passed = result.Passed;
                    }
            }
            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                {
                    int val = calculator.GetScore("STRATEGY", results[cls.Key]);
                    if (val > score) score = val;
                }
            }
            Assert.False(passed);
            Assert.NotEqual(100, score);
        }
        [Fact]
        public void Test_Strategy_NoStrategySetter()
        {
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            var tree = NoStrategySetter();
            var collection = Walker.GenerateModels(tree);
            int score = 0;
            bool passed = false;

            StateStrategy strategy = new StateStrategy(collection, false);
            strategy.Scan();
            var results = strategy.GetResults();

            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                    foreach (var result in results[cls.Value.Identifier].ToArray())
                    {
                        if (result.Id.Equals("STATE-STRATEGY-CONTEXT-STRATEGY-SETTER"))
                            if (!passed) passed = result.Passed;
                    }
            }
            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                {
                    int val = calculator.GetScore("STRATEGY", results[cls.Key]);
                    if (val > score) score = val;
                }
            }
            Assert.False(passed);
            Assert.NotEqual(100, score);
        }
        [Fact]
        public void Test_Strategy_NoPrivateStrategy()
        {
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            var tree = NoPrivateStrategy();
            var collection = Walker.GenerateModels(tree);
            int score = 0;
            bool passed = false;

            StateStrategy strategy = new StateStrategy(collection, false);
            strategy.Scan();
            var results = strategy.GetResults();
            
            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                    foreach (var result in results[cls.Value.Identifier].ToArray())
                    {
                        if (result.Id.Equals("STATE-STRATEGY-CONTEXT-PRIVATE-STRATEGY"))
                            if (!passed) passed = result.Passed;
                    }
            }
            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                {
                    int val = calculator.GetScore("STRATEGY", results[cls.Key]);
                    if (val > score) score = val;
                }
            }
            Assert.False(passed);
            Assert.NotEqual(100, score);
        }
        [Fact]
        public void Test_Strategy_NoMethodContext()
        {
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            var tree = NoMethodContext();
            var collection = Walker.GenerateModels(tree);
            int score = 0;
            bool passed = false;

            StateStrategy strategy = new StateStrategy(collection, false);
            strategy.Scan();
            var results = strategy.GetResults();

            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                    foreach (var result in results[cls.Value.Identifier].ToArray())
                    {
                        if (result.Id.Equals("STATE-STRATEGY-CONTEXT-LOGIC"))
                            if (!passed) passed = result.Passed;
                    }
            }
            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                {
                    int val = calculator.GetScore("STRATEGY", results[cls.Key]);
                    if (val > score) score = val;
                }
            }
            Assert.False(passed);
            Assert.NotEqual(100, score);
        }
        [Fact]
        public void Test_Strategy_RelationsBetweenStrategies()
        {
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            var tree = RelationsBetweenStrategies();
            var collection = Walker.GenerateModels(tree);
            int score = 0;
            bool passed = false;

            StateStrategy strategy = new StateStrategy(collection, false);
            strategy.Scan();
            var results = strategy.GetResults();

            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                    foreach (var result in results[cls.Value.Identifier].ToArray())
                    {
                        if (result.Id.Equals("STRATEGY-CONCRETE-CLASS-RELATIONS"))
                            if (!passed) passed = result.Passed;
                    }
            }
            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                {
                    int val = calculator.GetScore("STRATEGY", results[cls.Key]);
                    if (val > score) score = val;
                }
            }
            Assert.False(passed);
            Assert.NotEqual(100, score);
        }
        [Fact]
        public void Test_Nothing()
        {
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            var tree = FailSetup();
            var collection = Walker.GenerateModels(tree);
            int score = 0;

            StateStrategy strategy = new StateStrategy(collection, false);
            strategy.Scan();
            var results = strategy.GetResults();

            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                {
                    int val = calculator.GetScore("STRATEGY", results[cls.Key]);
                    if (val > score) score = val;
                }
            }
            Assert.InRange(score, 0, 20);
        }
        [Fact]
        public void Test_OnlineCode()
        {
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            var collection = Walker.GenerateModels(CSharpSyntaxTree.ParseText(TestStrings.StrategyExample()));
            int score = 0;

            StateStrategy strategy = new StateStrategy(collection, false);
            strategy.Scan();
            var results = strategy.GetResults();

            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                {
                    int val = calculator.GetScore("STRATEGY", results[cls.Key]);
                    if (val > score) score = val;
                }
            }
            Assert.InRange(score, 75, 95);
        }

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

        SyntaxTree NoContext()
        {
            return CSharpSyntaxTree.ParseText(@"
            namespace RefactoringGuru.DesignPatterns.Strategy.Conceptual
            {
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

        SyntaxTree NoInterface()
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

                public class IStrategy
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

        SyntaxTree NoConcreteStrategy()
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
            }");
        }

        SyntaxTree NoStrategySetter()
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

        SyntaxTree NoPrivateStrategy()
        {
            return CSharpSyntaxTree.ParseText(@"
            namespace RefactoringGuru.DesignPatterns.Strategy.Conceptual
            {
                class Context
                {
                    public IStrategy _strategy;

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

        SyntaxTree NoPublicConstructor()
        {
            return CSharpSyntaxTree.ParseText(@"
            namespace RefactoringGuru.DesignPatterns.Strategy.Conceptual
            {
                class Context
                {
                    private IStrategy _strategy;

                    private Context()
                    { }

                    private Context(IStrategy strategy)
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

        SyntaxTree NoMethodContext()
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

        SyntaxTree RelationsBetweenStrategies()
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
                        var strat = new ConcreteStrategyB;
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
                { }
                
                public class IStrategy
                { }

                class ConcreteStrategyA
                { }

                class ConcreteStrategyB
                { }
            }");
        }
    }
}
