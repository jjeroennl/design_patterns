using idetector;
using idetector.Data;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using idetector.Parser;
using idetector.Patterns;
using Xunit;

namespace xUnitTest
{

    // Before running any checks, check if there is an observer interface. if not -> stop checking
    // Get all classes that extend the observer interface
    public class ObserverTests
    {

        SyntaxTree ObserverTree()
        {
            return CSharpSyntaxTree.ParseText(@"
        using System;
        using System.Collections.Generic;
        using System.Threading;

        namespace RefactoringGuru.DesignPatterns.Observer.Conceptual
        {
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
                    Console.WriteLine('Subject: Attached an observer.');
                    this._observers.Add(observer);
                }

                public void Detach(IObserver observer)
                {
                    this._observers.Remove(observer);
                    Console.WriteLine('Subject: Detached an observer.');
                }

                public void Notify()
                {
                    Console.WriteLine('Subject: Notifying observers...');

                    foreach (var observer in _observers)
                    {
                        observer.Update(this);
                    }
                }

                public void SomeBusinessLogic()
                {
                    Console.WriteLine('\nSubject: I'm doing something important.');
                    this.State = new Random().Next(0, 10);

                    Thread.Sleep(15);

                    Console.WriteLine('Subject: My state has just changed to: ' + this.State);
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
            }

            class ConcreteObserverC : IObserver
            {
                public void Update(ISubject subject)
                {
                    if ((subject as Subject).State == 0 || (subject as Subject).State >= 2)
                    {
                        Console.WriteLine('ConcreteObserverC: Reacted to the event.');
                    }
                }
            }

            class ConcreteObserverD : IObserver
            {
                public void Update(ISubject subject)
                {
                    if ((subject as Subject).State == 0 || (subject as Subject).State >= 2)
                    {
                        Console.WriteLine('ConcreteObserverD: Reacted to the event.');
                    }
                }
            }

            class Program
            {
                static void Main(string[] args)
                {
                    // The client code.
                    var subject = new Subject();
                    var observerA = new ConcreteObserverA();
                    subject.Attach(observerA);

                    var observerB = new ConcreteObserverB();
                    subject.Attach(observerB);

                    subject.SomeBusinessLogic();
                    subject.SomeBusinessLogic();

                    subject.Detach(observerB);

                    subject.SomeBusinessLogic();
                }
            }
        }");
        }
        SyntaxTree NoObserverList()
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

        SyntaxTree NoInterfaces()
        {
            return CSharpSyntaxTree.ParseText(@"
                public class Subject
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

                class ConcreteObserverA
                {
                    public void Update(ISubject subject)
                    {
                        if ((subject as Subject).State < 3)
                        {
                            Console.WriteLine('ConcreteObserverA: Reacted to the event.');
                        }
                    }
                }

                class ConcreteObserverB
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

        [Fact]
        public void Test_Observer_HasObserverInterface()
        {
            var tree = ObserverTree();
            var collection = Walker.GenerateModels(tree);

            Observer observer = new Observer(collection);
            Assert.True(observer.HasObserverInterface().Passed);
        }

        [Fact]
        public void Test_Observer_HasObserverRelations()
        {
            var tree = ObserverTree();
            var collection = Walker.GenerateModels(tree);

            Observer observer = new Observer(collection);
            observer.Scan();
            Assert.True(observer.HasObserverRelations().Passed);
        }

    }
}
