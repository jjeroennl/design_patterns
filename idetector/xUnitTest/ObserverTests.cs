using idetector.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using idetector.Collections;
using idetector.Parser;
using idetector.Patterns;
using Xunit;

namespace xUnitTest
{
    // The sole responsibility of a subject is to maintain a list of observers 
    // and to notify them of state changes by calling their update() operation.

    // The responsibility of observers is to register (and unregister) themselves on a subject (to get notified of state changes) 
    // and to update their state (synchronize their state with subject's state) when they are notified.


    // Observer interface has Update function
    // ConcreteObservers inherit update func from Observer interface

    // Subject Interface usually has 3 functions:
    // Subscribe (Register), unsubscribe (unregister) and notify 

    // (Concrete)Subject has a list or similar 
    // containing observers that subject is subscribed to

    // (Concrete)Subject sub/unsub have same parameters
    public class ObserverTests
    {

        SyntaxTree SuccessTree()
        {
            return CSharpSyntaxTree.ParseText(@" 
                public interface IObserver
                {
                    void Update(ISubject subject);
                }

                public interface ISubject
                {
                    void Attach(IObserver observer);
                    void Detach(IObserver observer);
                    void Notify();
                }

                public class Subject : ISubject
                {

                    public int State { get; set; } = -0;
                    private List<IObserver> _observers = new List<IObserver>();

                    public void Attach(IObserver observer)
                    {
                        this._observers.Add(observer);
                    }

                    public void Detach(IObserver observer)
                    {
                        this._observers.Remove(observer);
                    }

                    public void Notify()
                    {
                        foreach (var observer in _observers)
                        {
                            observer.Update(this);
                        }
                    }

                    public void SomeBusinessLogic()
                    {
                        this.State = new Random().Next(0, 10);
                        Thread.Sleep(15);
                        this.Notify();
                    }
                }

                class ConcreteObserverA : IObserver
                {
                    public void Update(ISubject subject)
                    {
                        if ((subject as Subject).State < 3)
                        {
                            Console.WriteLine('ConcreteObserverA: Reacted to the event.');
                        }
                    }
                }

                class ConcreteObserverB : IObserver
                {
                    public void Update(ISubject subject)
                    {
                        if ((subject as Subject).State == 0 || (subject as Subject).State >= 2)
                        {
                            Console.WriteLine('ConcreteObserverB: Reacted to the event.');
                        }
                    }
                }");
        }

        SyntaxTree BrokenTree()
        {
            return CSharpSyntaxTree.ParseText(@" 
                public class IObserver
                {
                    void Update(ISubject subject);
                }");
        }

        [Fact]
        public void Test_Observer_Succeed()
        {
            var tree = SuccessTree();
            var collection = Walker.GenerateModels(tree);

            Observer oInterface = new Observer(collection);
            Assert.True(oInterface.HasInterfaceWithVoidFunction());
        }

        [Fact]
        public void Test_Observer_InterfaceWithSubscribeMethodsExists()
        {
            var tree = SuccessTree();
            var collection = Walker.GenerateModels(tree);

            Observer oInterface = new Observer(collection);
            Assert.True(oInterface.Has_Abstract_Class_Or_Interface_With_Subscriber_Functions());
        }

        [Fact]
        public void Test_Observer_FailToFindObserverInterface()
        {
            var tree = BrokenTree();
            var collection = Walker.GenerateModels(tree);

            Observer oInterface = new Observer(collection);
            Assert.False(oInterface.HasInterfaceWithVoidFunction());
        }

        [Fact]
        public void Test_Observer_SubjectHasList()
        {
            var tree = SuccessTree();
            var collection = Walker.GenerateModels(tree);

            Observer sClass = new Observer(collection);
            Assert.True(sClass.HasSubjectWithList());
        }

        [Fact]
        public void Test_Observer_SubjectHasNoList()
        {
            var tree = BrokenTree();
            var collection = Walker.GenerateModels(tree);

            Observer sClass = new Observer(collection);
            Assert.False(sClass.HasSubjectWithList());
        }
    }
}
