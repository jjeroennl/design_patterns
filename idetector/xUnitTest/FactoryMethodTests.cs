﻿using idetector;
using idetector.Collections;
using idetector.Data;
using idetector.Parser;
using idetector.Patterns;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace xUnitTest
{
    public class FactoryMethodTests
    {
        #region Syntax Trees
        SyntaxTree SuccessSetup()
        {
            return CSharpSyntaxTree.ParseText(@"
                abstract class Creator
                {
                    public abstract IProduct FactoryMethod();

                    public string SomeOperation()
                    {
                        var product = FactoryMethod();
                        var result = 'Creator: The same creator's code has just worked with '
                            + product.Operation();

                        return result;
                    }
                }

                class ConcreteCreator1 : Creator
                {
                    public override IProduct FactoryMethod()
                    {
                        return new ConcreteProduct1();
                    }
                }

                class ConcreteCreator2 : Creator
                {
                    public override IProduct FactoryMethod()
                    {
                        return new ConcreteProduct2();
                    }
                }

                public interface IProduct
                {
                    string Operation();
                }

                class ConcreteProduct1 : IProduct
                {
                    public string Operation()
                    {
                        return '{ Result of ConcreteProduct1}
                        ';
                    }
                }

                class ConcreteProduct2 : IProduct
                {
                    public string Operation()
                    {
                        return '{ Result of ConcreteProduct2}
                        ';
                    }
                }
        ");
        }

        SyntaxTree FailureSetup()
        {
            return CSharpSyntaxTree.ParseText(@"
                abstract class Creator
                {
                    public string SomeOperation()
                    {
                        var product = FactoryMethod();
                        var result = 'Creator: The same creator's code has just worked with '
                            + product.Operation();

                        return result;
                    }
                }
        ");
        }

        SyntaxTree MissingAbstractFactoryClassSetup()
        {
            return CSharpSyntaxTree.ParseText(@"
                class Creator
                { }
        ");
        }

        SyntaxTree MissingAbstractProductInterfaceMethodSetup()
        {
            return CSharpSyntaxTree.ParseText(@"
                abstract class Creator
                {
                    public string SomeOperation()
                    {
                        var product = FactoryMethod();
                        var result = 'Creator: The same creator's code has just worked with '
                            + product.Operation();

                        return result;
                    }
                }
        ");
        }

        SyntaxTree IsNotInheritingAbstractFactoryClassSetup()
        {
            return CSharpSyntaxTree.ParseText(@"
                abstract class Creator
                {
                    public abstract IProduct FactoryMethod();

                    public string SomeOperation()
                    {
                        var product = FactoryMethod();
                        var result = 'Creator: The same creator's code has just worked with '
                            + product.Operation();

                        return result;
                    }
                }

                class ConcreteCreator1
                {
                    public IProduct FactoryMethod()
                    {
                        return new ConcreteProduct1();
                    }
                }

        ");
        }

        SyntaxTree MissingProductInterfaceSetup()
        {
            return CSharpSyntaxTree.ParseText(@"
                abstract class Creator
                {
                    public abstract IProduct FactoryMethod();

                    public string SomeOperation()
                    {
                        var product = FactoryMethod();
                        var result = 'Creator: The same creator's code has just worked with '
                            + product.Operation();

                        return result;
                    }
                }

                public class IProduct
                {
                    string Operation();
                }
        ");
        }

        SyntaxTree IsNotInheritingProductInterfaceSetup()
        {
            return CSharpSyntaxTree.ParseText(@"
                abstract class Creator
                {
                    string Operation();
                }

                public interface IProduct
                {
                    string Operation();
                }

                class ConcreteProduct1 : Creator
                {
                    public string Operation()
                    {
                        return '{ Result of ConcreteProduct1}
                        ';
                    }
                }

        ");
        }

        SyntaxTree ConcreteFactoryIsNotReturningConcreteProductSetup()
        {
            return CSharpSyntaxTree.ParseText(@"
                abstract class Creator
                {
                    public abstract IProduct FactoryMethod();

                    public string SomeOperation()
                    {
                        var product = FactoryMethod();
                        var result = 'Creator: The same creator's code has just worked with '
                            + product.Operation();

                        return result;
                    }
                }

                class ConcreteCreator1 : Creator
                {
                    //
                }

                public interface IProduct
                {
                    string Operation();
                }

                class ConcreteProduct1 : IProduct
                {
                    public string Operation()
                    {
                        return '{ Result of ConcreteProduct1}
                        ';
                    }
                }

        ");
        }

        SyntaxTree ConcreteFactoriesDoNotHaveOneMethodSetup()
        {
            return CSharpSyntaxTree.ParseText(@"
                abstract class Creator
                {
                    public abstract IProduct FactoryMethod();

                    public string SomeOperation()
                    {
                        var product = FactoryMethod();
                        var result = 'Creator: The same creator's code has just worked with '
                            + product.Operation();

                        return result;
                    }
                }

                public interface IProduct
                {
                    string Operation();
                }

        ");
        }

        SyntaxTree ConcreteProductsDontFollowOneProductInterfaceSetup()
        {
            return CSharpSyntaxTree.ParseText(@"
                abstract class Creator
                {
                    public abstract IProduct FactoryMethod();
                    public abstract IProduct2 FactoryMethod();

                    public string SomeOperation()
                    {
                        var product = FactoryMethod();
                        var result = 'Creator: The same creator's code has just worked with '
                            + product.Operation();

                        return result;
                    }
                }

                class ConcreteCreator1 : Creator
                {
                    public override IProduct FactoryMethod()
                    {
                        return new ConcreteProduct1();
                    }
                }

                class ConcreteCreator2 : Creator
                {
                    public override IProduct FactoryMethod()
                    {
                        return new ConcreteProduct2();
                    }
                }

                public interface IProduct
                {
                    string Operation();
                }

                public interface IProduct2
                {
                    string Operation();
                }

                class ConcreteProduct1 : IProduct
                {
                    public string Operation()
                    {
                        return '{ Result of ConcreteProduct1}
                        ';
                    }
                }

                class ConcreteProduct2 : IProduct2
                {
                    public string Operation()
                    {
                        return '{ Result of ConcreteProduct2}
                        ';
                    }
                }
        ");
        }
        #endregion


        #region Tests for individual succeeding checks.

        [Fact]
        public void Test_FactoryMethod_ContainsAbstractFactoryClass()
        {
            var tree = SuccessSetup();
            var collection = Walker.GenerateModels(tree);
            
            AbstractFactoryMethod factoryMethod = new AbstractFactoryMethod(collection, true);
            factoryMethod.Scan();
            Assert.True(factoryMethod.ContainsIFactoryClass().Passed);
        }

        [Fact]
        public void Test_FactoryMethod_ContainsAbstractProductInterfaceMethod()
        {
            var tree = SuccessSetup();
            var collection = Walker.GenerateModels(tree);
            
            AbstractFactoryMethod factoryMethod = new AbstractFactoryMethod(collection, true);
            factoryMethod.Scan();
            Assert.True(factoryMethod.ContainsAbstractProductInterfaceMethod().Passed);
        }

        [Fact]
        public void Test_FactoryMethod_IsInheritingAbstractFactoryClass()
        {
            var tree = SuccessSetup();
            var collection = Walker.GenerateModels(tree);

            AbstractFactoryMethod factoryMethod = new AbstractFactoryMethod(collection, true);
            factoryMethod.Scan();
            Assert.True(factoryMethod.IsInheritingFactoryClass().Passed);
        }

        [Fact]
        public void Test_FactoryMethod_IsInheritingProductInterface()
        {
            var tree = SuccessSetup();
            var collection = Walker.GenerateModels(tree);

            AbstractFactoryMethod factoryMethod = new AbstractFactoryMethod(collection, true);
            factoryMethod.Scan();
            Assert.True(factoryMethod.IsInheritingProductInterface().Passed);
        }

        [Fact]
        public void Test_FactoryMethod_ConcreteFactoryIsReturningProduct()
        {
            var tree = SuccessSetup();
            var collection = Walker.GenerateModels(tree);

            AbstractFactoryMethod factoryMethod = new AbstractFactoryMethod(collection, true);
            factoryMethod.Scan();

            Assert.True(factoryMethod.ConcreteFactoryIsReturningConcreteProduct().Passed);
        }

        [Fact]
        public void Test_FactoryMethod_ConcreteFactoryHasOneMethod()
        {
            var tree = SuccessSetup();
            var collection = Walker.GenerateModels(tree);

            AbstractFactoryMethod factoryMethod = new AbstractFactoryMethod(collection, true);
            factoryMethod.Scan();
            Assert.True(factoryMethod.HasMultipleMethods().Passed);
        }

        [Fact]
        public void Test_FactoryMethod_ConcreteProductsFollowOneProductInterface()
        {
            var tree = SuccessSetup();
            var collection = Walker.GenerateModels(tree);

            AbstractFactoryMethod factoryMethod = new AbstractFactoryMethod(collection, true);
            factoryMethod.Scan();
            Assert.True(factoryMethod.ConcreteProductsFollowOneProductInterface().Passed);
        }
        #endregion

        #region Tests for individual failing checks.

        [Fact]
        public void Test_FactoryMethod_MissingFactoryClassSetup()
        {
            var tree = MissingAbstractFactoryClassSetup();
            var collection = Walker.GenerateModels(tree);

            AbstractFactoryMethod factoryMethod = new AbstractFactoryMethod(collection, true);
            factoryMethod.Scan();
            Assert.False(factoryMethod.ContainsIFactoryClass().Passed);
        }

        [Fact]
        public void Test_FactoryMethod_MissingAbstractProductInterfaceMethod()
        {
            var tree = MissingAbstractProductInterfaceMethodSetup();
            var collection = Walker.GenerateModels(tree);

            AbstractFactoryMethod factoryMethod = new AbstractFactoryMethod(collection, true);
            factoryMethod.Scan();
            Assert.True(factoryMethod.ContainsIFactoryClass().Passed);
            Assert.False(factoryMethod.ContainsAbstractProductInterfaceMethod().Passed);
        }

        [Fact]
        public void Test_FactoryMethod_IsNotInheritingAbstractFactoryClass()
        {
            var tree = IsNotInheritingAbstractFactoryClassSetup();
            var collection = Walker.GenerateModels(tree);

            AbstractFactoryMethod factoryMethod = new AbstractFactoryMethod(collection, true);
            factoryMethod.Scan();
            var results = factoryMethod.GetResults()["Creator"];
            Assert.True(results[0].Passed);
            Assert.False(results[4].Passed);
        }

        [Fact]
        public void Test_FactoryMethod_MissingProductInterface()
        {
            var tree = MissingProductInterfaceSetup();
            var collection = Walker.GenerateModels(tree);
            
            AbstractFactoryMethod factoryMethod = new AbstractFactoryMethod(collection, true);
            factoryMethod.Scan();
            var results = factoryMethod.GetResults()["Creator"];
            Assert.False(results[1].Passed);
        }

        [Fact]
        public void Test_FactoryMethod_IsNotInheritingProductInterface()
        {
            var tree = IsNotInheritingProductInterfaceSetup();
            var collection = Walker.GenerateModels(tree);

            AbstractFactoryMethod factoryMethod = new AbstractFactoryMethod(collection, true);
            factoryMethod.Scan();
            var results = factoryMethod.GetResults()["Creator"];
            Assert.True(results[1].Passed);
            Assert.False(results[2].Passed);
        }

        [Fact]
        public void Test_FactoryMethod_ConcreteFactoryIsNotReturningConcreteProduct()
        {
            var tree = ConcreteFactoryIsNotReturningConcreteProductSetup();
            var collection = Walker.GenerateModels(tree);

            AbstractFactoryMethod factoryMethod = new AbstractFactoryMethod(collection, true);
            factoryMethod.Scan();
            var results = factoryMethod.GetResults()["Creator"];
            Assert.False(results[4].Passed);
        }

        [Fact]
        public void Test_FactoryMethod_ConcreteFactoryHasNotOneMethod()
        {
            var tree = ConcreteFactoriesDoNotHaveOneMethodSetup();
            var collection = Walker.GenerateModels(tree);

            AbstractFactoryMethod factoryMethod = new AbstractFactoryMethod(collection, true);
            factoryMethod.Scan();
            var results = factoryMethod.GetResults()["Creator"];
            Assert.True(results[5].Passed);
        }
            
        [Fact]
        public void Test_FactoryMethod_ConcreteProductsDontFollowOneProductInterface()
        {
            var tree = ConcreteProductsDontFollowOneProductInterfaceSetup();
            var collection = Walker.GenerateModels(tree);

            AbstractFactoryMethod factoryMethod = new AbstractFactoryMethod(collection, true);
            factoryMethod.Scan();
            var results = factoryMethod.GetResults()["Creator"];
            Assert.False(results[5].Passed);
        }
        #endregion

        #region Tests for scores.
        [Fact]
        public void Test_FactoryMethod_ScorePass()
        {
            var tree = SuccessSetup();
            var collection = Walker.GenerateModels(tree);
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());

            AbstractFactoryMethod factoryMethod = new AbstractFactoryMethod(collection, true);
            factoryMethod.Scan();

            var score = calculator.GetScore("FACTORY", factoryMethod.GetResults());

            Assert.Equal(100, score["Creator"]);
        }
        
        [Fact]
        public void Test_FactoryMethod_Score33()
        {
            var tree = FailureSetup();
            var collection = Walker.GenerateModels(tree);
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());

            AbstractFactoryMethod factoryMethod = new AbstractFactoryMethod(collection, true);
            factoryMethod.Scan();

            var score = calculator.GetScore("FACTORY", factoryMethod.GetResults());

            Assert.Equal(33, score["Creator"]);
        }
        #endregion
    }
}
