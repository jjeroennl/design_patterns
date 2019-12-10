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
        SyntaxTree Successsetup()
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
        ");
        }




        [Fact]
        public void Test_FactoryMethod__Success()
        {
            var tree = Successsetup();
            var collection = Walker.GenerateModels(tree);

            FactoryMethod factoryMethod = new FactoryMethod(collection);
            factoryMethod.Scan();

            Assert.Equal("Creator", factoryMethod.GetAbstractCreatorClass().Identifier);
            Assert.True(factoryMethod.IsAbstractCreatorClass());
            Assert.True(factoryMethod.IsAbstractProductInterface());
            Assert.True(factoryMethod.IsInheritingAbstractCreatorClass());
        }
    }
}
