using System.Collections.Generic;
using idetector.Collections;
using idetector.Models;
using idetector.Parser;
using idetector.Patterns;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace xUnitTest
{
    public class DecoratorTest
    {
        [Fact]
        public void TestCorrectDecorator()
        {
            var collection = Walker.GenerateModels(setup());

            Decorator d = new Decorator(collection.GetClass("ComponentBase"), collection.GetClasses());
            d.Scan();
            Assert.Equal(100, d.Score());
        }

        [Fact]
        public void TestNoBaseParam()
        {
            var collection = Walker.GenerateModels(setupNoBaseParam());
            Decorator d = new Decorator(collection.GetClass("ComponentBase"), collection.GetClasses());
            d.Scan();
            Assert.Equal(80, d.Score());
        }
        [Fact]
        public void testRandomClasses()
        {
            List<string> decorators = new List<string>();
            var collection = Walker.GenerateModels(CSharpSyntaxTree.ParseText(TestStrings.RandomClasses()));
            foreach (var cls in collection.GetClasses())
            {
                Decorator d = new Decorator(cls.Value, collection.GetClasses());
                d.Scan();
                if (d.Score() > 0)
                {
                    decorators.Add(cls.Key);
                }
            }
            Assert.Empty(decorators);
        }
        [Fact]
        public void TestFullDecorator()
        {
            List<string> decorators = new List<string>();
            var collection = Walker.GenerateModels(setup());
            foreach (var cls in collection.GetClasses())
            {
                Decorator d = new Decorator(cls.Value, collection.GetClasses());
                d.Scan();
                if (d.Score() > 0)
                {

                    decorators.Add(cls.Key);
                }
            }
            Assert.Single(decorators);
            Assert.Equal("ComponentBase", collection.GetClass(decorators[0]).Identifier);
        }

        [Fact]
        public void TestNoBaseProperty()
        {
            var collection = Walker.GenerateModels(setupNoBaseProperty());
            Decorator d = new Decorator(collection.GetClass("ComponentBase"), collection.GetClasses());
            d.Scan();
            Assert.Equal(80, d.Score());
        }
        [Fact]
        public void TestNoAbstractChild()
        {
            var collection = Walker.GenerateModels(setupNoAbstract());
            Decorator d = new Decorator(collection.GetClass("ComponentBase"), collection.GetClasses());
            d.Scan();
            Assert.Equal(0, d.Score());
        }
        [Fact]
        public void TestDecoratorWithRandomClasses()
        {
            var collection = Walker.GenerateModels(CSharpSyntaxTree.ParseText(TestStrings.RandomClasses() + TestStrings.CorrectDecoratorString()));
            List<string> decorators = new List<string>();
            foreach (var cls in collection.GetClasses())
            {
                Decorator d = new Decorator(cls.Value, collection.GetClasses());
                d.Scan();
                if (d.Score() > 0)
                {

                    decorators.Add(cls.Key);
                }
            }
            Assert.Single(decorators);
            Assert.Equal("ComponentBase", collection.GetClass(decorators[0]).Identifier);
        }


        public SyntaxTree setup()
        {
            return CSharpSyntaxTree.ParseText(@"
public abstract class ComponentBase
{
    public abstract void Operation();
}
 
 
public class ConcreteComponent : ComponentBase
{
    public override void Operation()
    {
        Console.WriteLine(""Component Operation"");
    }
}
 
 
public abstract class DecoratorBase : ComponentBase
{
    private ComponentBase _component;
 
    public DecoratorBase(ComponentBase component)
    {
        _component = component;
    }
 
    public override void Operation()
    {
        _component.Operation();
    }
}
 
 
public class ConcreteDecorator : DecoratorBase
{
    public ConcreteDecorator(ComponentBase component) : base(component) { }
 
    public override void Operation()
    {
        base.Operation();
        Console.WriteLine(""(modified)"");
    }
}
");
        }





        public SyntaxTree setupNoBaseParam()
        {
            return CSharpSyntaxTree.ParseText(@"
public abstract class ComponentBase
{
    public abstract void Operation();
}
 
 
public class ConcreteComponent : ComponentBase
{
    public override void Operation()
    {
        Console.WriteLine(""Component Operation"");
    }
}
 
 
public abstract class DecoratorBase : ComponentBase
{
    private ComponentBase _component;
 
    public DecoratorBase()
    {
        
    }
 
    public override void Operation()
    {
        _component.Operation();
    }
}
 
 
public class ConcreteDecorator : DecoratorBase
{
    public ConcreteDecorator(ComponentBase component) : base(component) { }
 
    public override void Operation()
    {
        base.Operation();
        Console.WriteLine(""(modified)"");
    }
}
");
        }

        public SyntaxTree setupNoBaseProperty()
        {
            return CSharpSyntaxTree.ParseText(@"
public abstract class ComponentBase
{
    public abstract void Operation();
}
 
 
public class ConcreteComponent : ComponentBase
{
    public override void Operation()
    {
        Console.WriteLine(""Component Operation"");
    }
}
 
 
public abstract class DecoratorBase : ComponentBase
{
    
 
    public DecoratorBase(ComponentBase _component)
    {
        
    }
 
    public override void Operation()
    {
        _component.Operation();
    }
}
 
 
public class ConcreteDecorator : DecoratorBase
{
    public ConcreteDecorator(ComponentBase component) : base(component) { }
 
    public override void Operation()
    {
        base.Operation();
        Console.WriteLine(""(modified)"");
    }
}
");
        }


        public SyntaxTree setupNoAbstract()
        {
            return CSharpSyntaxTree.ParseText(@"
public abstract class ComponentBase
{
    public abstract void Operation();
}
 
 
public class ConcreteComponent : ComponentBase
{
    public override void Operation()
    {
        Console.WriteLine(""Component Operation"");
    }
}
 
 
public class DecoratorBase : ComponentBase
{
    
 
    public DecoratorBase(ComponentBase _component)
    {
        
    }
 
    public override void Operation()
    {
        _component.Operation();
    }
}
 
 
public class ConcreteDecorator : DecoratorBase
{
    public ConcreteDecorator(ComponentBase component) : base(component) { }
 
    public override void Operation()
    {
        base.Operation();
        Console.WriteLine(""(modified)"");
    }
}
");
        }
    }
}