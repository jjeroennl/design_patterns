using idetector;
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
        SyntaxTree noIProductSetup()
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
        SyntaxTree NoConcreteProductReturned()
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
        SyntaxTree MultipleInterfaceProducts()
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
                    public override IProduct2 FactoryMethod()
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

                class ConcreteProduct1 : IProduct2
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
        SyntaxTree MultipleMethods()
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
                    public IProduct2 FactoryMethod2(){
                        return new ConcreteProduct3();
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
                class ConcreteProduct3 : IProduct2(){
                    public string Operation()
                    {
                        return '{ Result of ConcreteProduct3}
                        ';
                    }
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

            var score = calculator.GetScore("FACTORY", factoryMethod.GetResults()["Creator"]);

            Assert.Equal(100, score);
        }
        [Fact]
        public void Test_FactoryMethod_NoIProduct()
        {
            //if there is no product interface, factory cannot return products at all.
            var tree = noIProductSetup();
            var collection = Walker.GenerateModels(tree);
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());

            AbstractFactoryMethod factoryMethod = new AbstractFactoryMethod(collection, true);
            factoryMethod.Scan();



            Assert.Empty((factoryMethod.GetResults()));
        }
        [Fact]
        public void Test_FactoryMethod_NoConcreteProductReturned()
        {
            var tree = NoConcreteProductReturned();
            var collection = Walker.GenerateModels(tree);
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());

            AbstractFactoryMethod factoryMethod = new AbstractFactoryMethod(collection, true);
            factoryMethod.Scan();

            var score = calculator.GetScore("FACTORY", factoryMethod.GetResults()["Creator"]);

            Assert.Equal(80, score);
        }
        [Fact]
        public void Test_FactoryMethod_MultipleProductInterfaces()
        {
            var tree = MultipleInterfaceProducts();
            var collection = Walker.GenerateModels(tree);
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());

            AbstractFactoryMethod factoryMethod = new AbstractFactoryMethod(collection, true);
            factoryMethod.Scan();

            var score = calculator.GetScore("FACTORY", factoryMethod.GetResults()["Creator"]);
            //should fail both multiple products and multiple methods (since concrete factories return more then one type of interface)
            Assert.Equal(60, score);
        }
        [Fact]
        public void Test_FactoryMethod_MultipleMethods()
        {
            var tree = MultipleInterfaceProducts();
            var collection = Walker.GenerateModels(tree);
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());

            AbstractFactoryMethod factoryMethod = new AbstractFactoryMethod(collection, true);
            factoryMethod.Scan();

            var score = calculator.GetScore("FACTORY", factoryMethod.GetResults()["Creator"]);
            //should fail both multiple products and multiple methods (since concrete factories return more then one type of interface)
            Assert.Equal(60, score);
        }
        #endregion
    }
}
