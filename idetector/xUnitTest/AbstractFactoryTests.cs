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

        [Fact]
        public void Test_AbstractFactory_Succes()
        {
            var tree = SuccessSetup();
            var collection = Walker.GenerateModels(tree);
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            int score = 0;

            AbstractFactoryMethod abstractFactory = new AbstractFactoryMethod(collection, false);
            abstractFactory.Scan();
            var results = abstractFactory.GetResults();

            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                {
                    int val = calculator.GetScore("ABSTRACT-FACTORY", results[cls.Key]);
                    if (val > score) score = val;
                }
            }
            Assert.Equal(100, score);
        }
        [Fact]
        public void Test_FactoryMethod_MultipleProductInterfaces()
        {
            var tree = MultipleInterfaceProducts();
            var collection = Walker.GenerateModels(tree);
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());

            AbstractFactoryMethod factoryMethod = new AbstractFactoryMethod(collection, false);
            factoryMethod.Scan();

            var score = calculator.GetScore("ABSTRACT-FACTORY", factoryMethod.GetResults()["Creator"]);
            //should fail both multiple products and multiple methods (since concrete factories return more then one type of interface)
            Assert.Equal(100, score);
        }
        [Fact]
        public void Test_FactoryMethod_MultipleMethods()
        {
            var tree = MultipleInterfaceProducts();
            var collection = Walker.GenerateModels(tree);
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());

            AbstractFactoryMethod factoryMethod = new AbstractFactoryMethod(collection, false);
            factoryMethod.Scan();

            var score = calculator.GetScore("ABSTRACT-FACTORY", factoryMethod.GetResults()["Creator"]);
            //should fail both multiple products and multiple methods (since concrete factories return more then one type of interface)
            Assert.Equal(100, score);
        }
    }

}
