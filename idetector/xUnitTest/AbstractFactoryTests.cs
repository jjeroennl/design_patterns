using idetector;
using idetector.Data;
using idetector.Parser;
using idetector.Patterns;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace xUnitTest
{
    public class AbstractFactoryTests
    {
        #region Syntax Trees
        SyntaxTree SuccessSetup()
        {
            return CSharpSyntaxTree.ParseText(@" 
                namespace RefactoringGuru.DesignPatterns.AbstractFactory.Conceptual
                {
                    public interface IAbstractFactory
                    {
                        IAbstractProductA CreateProductA();

                        IAbstractProductB CreateProductB();
                    }

                    class ConcreteFactory1 : IAbstractFactory
                    {
                        public IAbstractProductA CreateProductA()
                        {
                            return new ConcreteProductA1();
                        }

                        public IAbstractProductB CreateProductB()
                        {
                            return new ConcreteProductB1();
                        }
                    }

                    class ConcreteFactory2 : IAbstractFactory
                    {
                        public IAbstractProductA CreateProductA()
                        {
                            return new ConcreteProductA2();
                        }

                        public IAbstractProductB CreateProductB()
                        {
                            return new ConcreteProductB2();
                        }
                    }

                    public interface IAbstractProductA
                    {
                        string UsefulFunctionA();
                    }

                    class ConcreteProductA1 : IAbstractProductA
                    {
                        public string UsefulFunctionA()
                        {
                            return 'The result of the product A1.';
                        }
                    }

                    class ConcreteProductA2 : IAbstractProductA
                    {
                        public string UsefulFunctionA()
                        {
                            return 'The result of the product A2.';
                        }
                    }

                    public interface IAbstractProductB
                    {
                        string UsefulFunctionB();

                        string AnotherUsefulFunctionB(IAbstractProductA collaborator);
                    }

                    class ConcreteProductB1 : IAbstractProductB
                    {
                        public string UsefulFunctionB()
                        {
                            return 'The result of the product B1.';
                        }

                        public string AnotherUsefulFunctionB(IAbstractProductA collaborator)
                        {
                            var result = collaborator.UsefulFunctionA();

                            return $'The result of the B1 collaborating with the ({result})';
                        }
                    }

                    class ConcreteProductB2 : IAbstractProductB
                    {
                        public string UsefulFunctionB()
                        {
                            return 'The result of the product B2.';
                        }

                        public string AnotherUsefulFunctionB(IAbstractProductA collaborator)
                        {
                            var result = collaborator.UsefulFunctionA();

                            return $'The result of the B2 collaborating with the ({result})';
                        }
                    }

                    class Client
                    {
                        public void Main()
                        {
                            Console.WriteLine('Client: Testing client code with the first factory type...');
                            ClientMethod(new ConcreteFactory1());
                            Console.WriteLine();

                            Console.WriteLine('Client: Testing the same client code with the second factory type...');
                            ClientMethod(new ConcreteFactory2());
                        }

                        public void ClientMethod(IAbstractFactory factory)
                        {
                            var productA = factory.CreateProductA();
                            var productB = factory.CreateProductB();

                            Console.WriteLine(productB.UsefulFunctionB());
                            Console.WriteLine(productB.AnotherUsefulFunctionB(productA));
                        }
                    }

                    class Program
                    {
                        static void Main(string[] args)
                        {
                            new Client().Main();
                        }
                    }
                }");
        }

        SyntaxTree ContainsOneMethod()
        {
            return CSharpSyntaxTree.ParseText(@" 
                namespace RefactoringGuru.DesignPatterns.AbstractFactory.Conceptual
                {
                    public interface IAbstractFactory
                    {
                        IAbstractProductA CreateProductA();
                    }

                    class ConcreteFactory1 : IAbstractFactory
                    {
                        public IAbstractProductA CreateProductA()
                        {
                            return new ConcreteProductA1();
                        }
                    }

                    class ConcreteFactory2 : IAbstractFactory
                    {
                        public IAbstractProductA CreateProductA()
                        {
                            return new ConcreteProductA2();
                        }
                    }

                    public interface IAbstractProductA
                    {
                        string UsefulFunctionA();
                    }

                    class ConcreteProductA1 : IAbstractProductA
                    {
                        public string UsefulFunctionA()
                        {
                            return 'The result of the product A1.';
                        }
                    }

                    class ConcreteProductA2 : IAbstractProductA
                    {
                        public string UsefulFunctionA()
                        {
                            return 'The result of the product A2.';
                        }
                    }

                    public interface IAbstractProductB
                    {
                        string UsefulFunctionB();

                        string AnotherUsefulFunctionB(IAbstractProductA collaborator);
                    }

                    class ConcreteProductB1 : IAbstractProductB
                    {
                        public string UsefulFunctionB()
                        {
                            return 'The result of the product B1.';
                        }

                        public string AnotherUsefulFunctionB(IAbstractProductA collaborator)
                        {
                            var result = collaborator.UsefulFunctionA();

                            return $'The result of the B1 collaborating with the ({result})';
                        }
                    }

                    class ConcreteProductB2 : IAbstractProductB
                    {
                        public string UsefulFunctionB()
                        {
                            return 'The result of the product B2.';
                        }

                        public string AnotherUsefulFunctionB(IAbstractProductA collaborator)
                        {
                            var result = collaborator.UsefulFunctionA();

                            return $'The result of the B2 collaborating with the ({result})';
                        }
                    }

                    class Client
                    {
                        public void Main()
                        {
                            Console.WriteLine('Client: Testing client code with the first factory type...');
                            ClientMethod(new ConcreteFactory1());
                            Console.WriteLine();

                            Console.WriteLine('Client: Testing the same client code with the second factory type...');
                            ClientMethod(new ConcreteFactory2());
                        }

                        public void ClientMethod(IAbstractFactory factory)
                        {
                            var productA = factory.CreateProductA();
                            var productB = factory.CreateProductB();

                            Console.WriteLine(productB.UsefulFunctionB());
                            Console.WriteLine(productB.AnotherUsefulFunctionB(productA));
                        }
                    }

                    class Program
                    {
                        static void Main(string[] args)
                        {
                            new Client().Main();
                        }
                    }
                }");

        }
        #endregion

        [Fact]
        public void Test_AbstractFactory_Succes()
        {
            var tree = SuccessSetup();
            var collection = Walker.GenerateModels(tree);
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());

            AbstractFactoryMethod abstractFactory = new AbstractFactoryMethod(collection, false);
            abstractFactory.Scan();
            int score = calculator.GetScore("FACTORY", abstractFactory.GetResult());

            Assert.True(abstractFactory.ContainsIFactoryClass().Passed);
            Assert.True(abstractFactory.ContainsProductInterface().Passed);
            Assert.True(abstractFactory.ContainsAbstractProductInterfaceMethod().Passed);
            Assert.True(abstractFactory.IsInheritingFactoryClass().Passed);
            Assert.True(abstractFactory.IsInheritingProductInterface().Passed);
            Assert.True(abstractFactory.ConcreteFactoryIsReturningConcreteProduct().Passed);
            Assert.True(abstractFactory.HasMultipleMethods().Passed);
            Assert.True(abstractFactory.ConcreteProductsFollowOneProductInterface().Passed);

            Assert.Equal(100, score);
        }

        #region Tests for individual failing checks.

        [Fact]
        public void Test_AbstractFactory_ContainsOneMethod()
        {
            var tree = ContainsOneMethod();
            var collection = Walker.GenerateModels(tree);

            AbstractFactoryMethod abstractFactory = new AbstractFactoryMethod(collection, false);
            abstractFactory.Scan();

            Assert.False(abstractFactory.HasMultipleMethods().Passed);
        }

        #endregion
    }
    }
