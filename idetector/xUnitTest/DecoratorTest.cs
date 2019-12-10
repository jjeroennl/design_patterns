using idetector.Parser;
using idetector.Patterns;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace xUnitTest
{
    public class DecoratorTest
    {
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

        [Fact]
        public void TestCorrectDecorator()
        {
            var collection = Walker.GenerateModels(setup());

            Decorator d = new Decorator(collection.GetClass("ComponentBase"), collection.GetClasses());
        }
    }
}