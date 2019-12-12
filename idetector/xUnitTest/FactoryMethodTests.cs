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
                class Creator
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

                public class IProduct
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

        SyntaxTree NoAbstractProductSetup()
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

        [Fact]
        public void Test_FactoryMethod_Success()
        {
            var tree = SuccessSetup();
            var collection = Walker.GenerateModels(tree);

            FactoryMethod factoryMethod = new FactoryMethod(collection);
            
            Assert.True(factoryMethod.IsAbstractCreatorClass());
            Assert.True(factoryMethod.IsAbstractProductInterface());
            Assert.True(factoryMethod.IsInheritingAbstractCreatorClass());

            Assert.True(factoryMethod.IsProductInterface());
            Assert.True(factoryMethod.IsInheritingProductInterface());
        }

        [Fact]
        public void Test_FactoryMethod_Failure()
        {
            var tree = FailureSetup();
            var collection = Walker.GenerateModels(tree);

            FactoryMethod factoryMethod = new FactoryMethod(collection);

            Assert.False(factoryMethod.IsAbstractCreatorClass());
            Assert.False(factoryMethod.IsAbstractProductInterface());
            Assert.False(factoryMethod.IsInheritingAbstractCreatorClass());

            Assert.False(factoryMethod.IsProductInterface());
            Assert.False(factoryMethod.IsInheritingProductInterface());
        }

        [Fact]
        public void Test_FactoryMethod_IsNotAbstractProductInterface()
        {
            var tree = NoAbstractProductSetup();
            var collection = Walker.GenerateModels(tree);

            FactoryMethod factoryMethod = new FactoryMethod(collection);

            Assert.False(factoryMethod.IsAbstractProductInterface());
        }
    }
}
