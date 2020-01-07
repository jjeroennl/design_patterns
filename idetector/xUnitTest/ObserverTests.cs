using idetector;
using idetector.Data;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using idetector.Parser;
using idetector.Patterns;
using Xunit;

namespace xUnitTest
{
    // The sole responsibility of a subject is to maintain a list of observers 
    // and to notify them of state changes by calling their update() operation.

    // Check that subject loops through list of observers and calls a known function

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

        SyntaxTree ObserverTree()
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
        public void Test_Observer_HasObserverInterfaceWithUpdateFunction()
        {
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            var tree = ObserverTree();
            var collection = Walker.GenerateModels(tree);

            Observer observer = new Observer(collection);
            observer.Scan();
            var score = calculator.GetScore("OBSERVER", observer.GetResult());
            Assert.Equal(100, score);
        }

        [Fact]
        public void Test_Observer_HasObserverInterface()
        {
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            var tree = ObserverTree();
            var collection = Walker.GenerateModels(tree);

            Observer observer = new Observer(collection);
            observer.Scan();
            var score = calculator.GetScore("OBSERVER", observer.GetResult());
            Assert.Equal(100, score);
        }

        [Fact]
        public void Test_Observer_HasSubjectFunctions()
        {
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            var tree = ObserverTree();
            var collection = Walker.GenerateModels(tree);

            Observer observer = new Observer(collection);
            observer.Scan();
            var score = calculator.GetScore("OBSERVER", observer.GetResult());
            Assert.Equal(100, score);
        }

        [Fact]
        public void Test_Observer_HasSubjectWithObserverList()
        {
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            var tree = ObserverTree();
            var collection = Walker.GenerateModels(tree);

            Observer observer = new Observer(collection);
            observer.Scan();
            var score = calculator.GetScore("OBSERVER", observer.GetResult());
            Assert.Equal(100, score);
        }

        [Fact]
        public void Test_Observer_ConcreteObserverExtendsIObserver()
        {
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            var tree = ObserverTree();
            var collection = Walker.GenerateModels(tree);

            Observer observer = new Observer(collection);
            observer.Scan();
            var score = calculator.GetScore("OBSERVER", observer.GetResult());
            Assert.Equal(100, score);
        }

        [Fact]
        public void Test_Observer_NoIObserverParent()
        {
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            var tree = NoInterfaces();
            var collection = Walker.GenerateModels(tree);

            Observer observer = new Observer(collection);
            var score = calculator.GetScore("OBSERVER", observer.GetResult());
            Assert.NotEqual(100, score);
        }

        [Fact]
        public void Test_Observer_HasNoObserverInterface()
        {
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            var tree = NoInterfaces();
            var collection = Walker.GenerateModels(tree);

            Observer observer = new Observer(collection);
            var score = calculator.GetScore("OBSERVER", observer.GetResult());
            Assert.NotEqual(100, score);
        }

        [Fact]
        public void Test_Observer_SubjectHasNoObserverList()
        {
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            var tree = NoObserverList();
            var collection = Walker.GenerateModels(tree);

            Observer observer = new Observer(collection);
            var score = calculator.GetScore("OBSERVER", observer.GetResult());
            Assert.NotEqual(100, score);
        }
    }
}
