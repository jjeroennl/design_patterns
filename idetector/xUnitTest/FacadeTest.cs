using System;
using idetector.Collections;
using idetector.Parser;
using idetector.Patterns.Facade;
using idetector.Patterns.Helper;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace xUnitTest
{
    public class FacadeTest
    {
        private ClassCollection facade;
        private ClassCollection notFacade;
        private ClassCollection reallyNotFacade;

        [Fact]
        void FacadeProject()
        {
            Facade f = new Facade(this.notFacade);
            Assert.Equal(100, f.Score("GasPipes"));
            Assert.Equal(100, f.Score("HouseBuilderFacade"));
            Assert.Equal(0, f.Score("User"));
        }
        
        [Fact]
        void NotFacade()
        {
            Facade f = new Facade(this.notFacade);
            Assert.Equal(0, f.Score("GasPipes"));
            Assert.Equal(0, f.Score("HouseBuilderFacade"));
            Assert.Equal(0, f.Score("User"));
        }
        
        [Fact]
        void ReallyNotFacade()
        {
            Facade f = new Facade(this.reallyNotFacade);
            Assert.Equal(0, f.Score("Current"));
            Assert.Equal(0, f.Score("NoteBs"));
        }


        public void registerFacade()
        {
            var facade = CSharpSyntaxTree.ParseText(@"
namespace idetector.Patterns
{ 
    class User{
            public static User me; 

            private static User(){

            }
                    
            public  User getUser(){
                if(User.me == null){
                    User.me = new User();
                }
                return User.me;
            }
        }

        class Current
        {
            private static Current me; 

            private static Current(){

            }
                    
            public static Current getCurrent(){
                if(Current.me == null){
                    Current.me = new Current();
                }
                return Current.me;
            }
        }

        class Data
        {
            public static Data me; 

            private Data(){

            }
                    
            public static Data getData(){
                if(Data.me == null){
                    Data.me = new Data();
                }
                return Data.me;
            }
        }
}
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
namespace PianoTrainer.Controller{
public interface INote {
   INote getNote();
    
   void setNote(INote note):
    }
}

namespace PianoTrainer.Controller {
public class NoteA : INote{
        INote _note;

        public INote getNote(){
            return note;
        }
        public void SetNote(INote note){
            _note = note;
        }
    }
}
namespace PianoTrainer.Controller {
public class NoteB : INote{
        INote _note;

        public INote getNote(){
            return note;
        }
        public void SetNote(INote note){
            _note = note;
        }
    }
}
public class NoteBs : NoteB{
        INote _note;

        public INote getNote(){
            return note;
        }
        public void SetNote(INote note){
            _note = note;
        }
    }
}
            ");

            this.facade = Walker.GenerateModels(facade);

            var notFacade = CSharpSyntaxTree.ParseText(@"
 
            ");

            this.notFacade = Walker.GenerateModels(notFacade);
        }
    }
}