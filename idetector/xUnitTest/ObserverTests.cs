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

                        // Trigger an update in each subscriber.
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

                    class Program
                    {
                        static void Main(string[] args)
                        {
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
                }
");
        }

        SyntaxTree NoObserverInterface()
        {
            return CSharpSyntaxTree.ParseText(@"
                using System;
                using System.Collections.Generic;
                using System.Threading;

                namespace RefactoringGuru.DesignPatterns.Observer.Conceptual
                {
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

                        // Trigger an update in each subscriber.
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
                    }

                    class Program
                    {
                        static void Main(string[] args)
                        {
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
                }
");
        }

        [Fact]
        public void Test_Observer_HasObserverInterface()
        {
            var tree = ObserverTree();
            var collection = Walker.GenerateModels(tree);
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            int score;

            Observer observer = new Observer(collection);
            observer.Scan();

            calculator.GetScore("OBSERVER", observer.GetResults()).TryGetValue("observer", out score);
            Assert.Equal(100, score);
        }

        [Fact]
        public void Test_Observer_HasObserverRelations()
        {
            var tree = ObserverTree();
            var collection = Walker.GenerateModels(tree);
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            int score;

            Observer observer = new Observer(collection);
            observer.Scan();

            calculator.GetScore("OBSERVER", observer.GetResults()).TryGetValue("observer", out score);
            Assert.Equal(100, score);
        }

        [Fact]
        public void Test_Observer_HasSubjectInterface()
        {
            var tree = ObserverTree();
            var collection = Walker.GenerateModels(tree);
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            int score;

            Observer observer = new Observer(collection);
            observer.Scan();

            calculator.GetScore("OBSERVER", observer.GetResults()).TryGetValue("observer", out score);
            Assert.Equal(100, score);
        }


        [Fact]
        public void Test_Observer_HasNoObserverInterface()
        {
            var tree = NoObserverInterface();
            var collection = Walker.GenerateModels(tree);
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            int score;

            Observer observer = new Observer(collection);
            observer.Scan();

            calculator.GetScore("OBSERVER", observer.GetResults()).TryGetValue("observer", out score);
            Assert.Equal(0, score);
        }
    }
}
