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
    public class StateTest
    {
        [Fact]
        public void Test_State_Succeed()
        {
            var tree = SuccessSetup();
            var collection = Walker.GenerateModels(tree);
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            int score = 0;

            StateStrategy state= new StateStrategy(collection, true);
            state.Scan();
            var results = state.GetResults();

            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                {
                    int val = calculator.GetScore("STATE", results[cls.Key]);
                    if (val > score) score = val;
                }
            }
            Assert.Equal(100, score);

        }
        [Fact]

        public void Test_Nothing()
        {
            var tree = FailSetup();
            var collection = Walker.GenerateModels(tree);
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            int score = 0;

            StateStrategy state= new StateStrategy(collection, true);
            state.Scan();
            var results = state.GetResults();

            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                {
                    int val = calculator.GetScore("STATE", results[cls.Key]);
                    if (val > score) score = val;
                }
            }
            Assert.InRange(score, 0, 20);

        }
        [Fact]
        public void Test_State_RelationsBetweenStates()
        {
            var tree = RelationsBetweenStates();
            var collection = Walker.GenerateModels(tree);
            bool hasKey = false;

            StateStrategy state = new StateStrategy(collection, true);
            state.Scan();
            var results = state.GetResults();

            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                {
                    foreach (var result in results[cls.Value.Identifier].ToArray())
                    {
                        if (result.Id.Equals("STRATEGY-CONCRETE-CLASS-RELATIONS")) hasKey = true;
                    }
                }
            }
            Assert.False(hasKey);
        }
        [Fact]
        public void Test_StrategySetup()
        {
            var tree = StrategySetup();
            var collection = Walker.GenerateModels(tree);
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            int score = 0;

            StateStrategy state = new StateStrategy(collection, true);
            state.Scan();
            var results = state.GetResults();

            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                {
                    int val = calculator.GetScore("STATE", results[cls.Key]);
                    if (val > score) score = val;
                }
            }
            Assert.Equal(100, score);
        }
        [Fact]
        public void Test_OnlineCode()
        {
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            var collection = Walker.GenerateModels(CSharpSyntaxTree.ParseText(TestStrings.StateExample()));
            int score = 0;

            StateStrategy state = new StateStrategy(collection, true);
            state.Scan();
            var results = state.GetResults();

            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                {
                    int val = calculator.GetScore("STATE", results[cls.Key]);
                    if (val > score) score = val;
                }
            }
            Assert.InRange(score, 70, 90);
        }

        SyntaxTree SuccessSetup()
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
        SyntaxTree StrategySetup()
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
            namespace RefactoringGuru.DesignPatterns.State.Conceptual
            {
                class Context
                {
                    private IState _state;

                    public Context()
                    { }

                    public Context()
                    { }

                    public void SetState()
                    { }

                    public void DoSomeBusinessLogic()
                    { }
                }

                public class IState
                {
                    object DoAlgorithm(object data);
                }

                class ConcreteStateA
                { }

                class ConcreteStateB
                { }
            }");
        }

        SyntaxTree RelationsBetweenStates()
        {
            return CSharpSyntaxTree.ParseText(@"
            namespace RefactoringGuru.DesignPatterns.State.Conceptual
            {
                class Context
                {
                    private IState _state;

                    public Context()
                    { }

                    public Context(IState state)
                    {
                        this._state = state;
                    }

                    public void SetState(IState state)
                    {
                        this._state = state;
                    }

                    public void DoSomeBusinessLogic()
                    {
                        var result = this._state.DoAlgorithm(new List<string> { 'a', 'b', 'c', 'd', 'e' });

                        string resultStr = string.Empty;
                        foreach (var element in result as List<string>)
                        {
                            resultStr += element + ',';
                        }
                    }
                }

                public interface IState
                {
                    object DoAlgorithm(object data);
                }

                class ConcreteStateA : IState
                {
                    public object DoAlgorithm(object data)
                    {
                        var strat = new ConcreteStrategyB;
                        var list = data as List<string>;
                        list.Sort();

                        return list;
                    }
                }

                class ConcreteStateB : IState
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
    }
}
