using System.Collections.Generic;
using System.Diagnostics;
using idetector;
using idetector.Data;
using idetector.Models;
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
                public class Subject : ISubject
{
  private List<Observer> observers = new List<Observer>();
  private int _int;
  public int Inventory
  {
    get
    {
       return _int;
    }
    set
    {
       // Just to make sure that if there is an increase in inventory then only we are notifying 
          the observers.
          if (value > _int)
             Notify();
          _int = value;
    }
  }
  public void Subscribe(Observer observer)
  {
     observers.Add(observer);
  }

  public void Unsubscribe(Observer observer)
  {
     observers.Remove(observer);
  }

  public void Notify()
  {
     observers.ForEach(x => x.Update());
  }
}

public interface IObserver
{
  void Update();
}

public interface ISubject
{
   void Subscribe(Observer observer);
   void Unsubscribe(Observer observer);
   void Notify();
}

public class ConcreteObserver : IObserver
{
  public string ObserverName { get;private set; }
  public Observer(string name)
  {
    this.ObserverName = name;
  }
  public void Update()
  {
    Console.WriteLine('{0}: A new product has arrived at the
    store',this.ObserverName);
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

        SyntaxTree NoSubjectInterface()
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

        SyntaxTree NoInterfaces()
        {
            return CSharpSyntaxTree.ParseText(@"
                using System;
                using System.Collections.Generic;
                using System.Threading;

                namespace RefactoringGuru.DesignPatterns.Observer.Conceptual
                {
                    public class Subject
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
            bool passed = false;

            Observer observer = new Observer(collection);
            observer.Scan();
            var results = observer.GetResults();

            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                {
                    foreach (var result in results[cls.Value.Identifier].ToArray())
                    {
                        if (result.Id.Equals("OBSERVER-HAS-OBSERVER-INTERFACE"))
                        {
                            if(!passed) passed = result.Passed;
                        }
                    }
                }
            }
            Assert.True(passed);

            //var score = calculator.GetScore("OBSERVER", observer.GetResults()["IObserver"]);
            //Assert.Equal(100, score);
        }

        [Fact]
        public void Test_Observer_HasSubjectInterface()
        {
            var tree = ObserverTree();
            var collection = Walker.GenerateModels(tree);
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            bool passed = false;

            Observer observer = new Observer(collection);
            observer.Scan();
            var results = observer.GetResults();

            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                {
                    foreach (var result in results[cls.Value.Identifier].ToArray())
                    {
                        if (result.Id.Equals("OBSERVER-HAS-SUBJECT-INTERFACE"))
                        {
                            if (!passed) passed = result.Passed;
                        }
                    }
                }
            }
            Assert.True(passed);
        }

        [Fact]
        public void Test_Observer_HasObserverRelations()
        {
            var tree = ObserverTree();
            var collection = Walker.GenerateModels(tree);
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            bool passed = false;

            Observer observer = new Observer(collection);
            observer.Scan();
            var results = observer.GetResults();

            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                {
                    foreach (var result in results[cls.Value.Identifier].ToArray())
                    {
                        if (result.Id.Equals("OBSERVER-HAS-OBSERVER-RELATIONS"))
                        {
                            if (!passed) passed = result.Passed;
                        }
                    }
                }
            }
            Assert.True(passed);
        }

        [Fact]
        public void Test_Observer_HasObserversAndSubjects()
        {
            var tree = ObserverTree();
            var collection = Walker.GenerateModels(tree);
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            bool passed = false;

            Observer observer = new Observer(collection);
            observer.Scan();
            var results = observer.GetResults();

            foreach (var cls in collection.GetClasses())
            {
                if (results.ContainsKey(cls.Key))
                {
                    foreach (var result in results[cls.Value.Identifier].ToArray())
                    {
                        if (result.Id.Equals("OBSERVER-HAS-OBSERVERS-AND-SUBJECTS"))
                        {
                            if (!passed) passed = result.Passed;
                        }
                    }
                }
            }
            Assert.True(passed);
        }

        [Fact]
        public void Test_Observer_HasNoObserverInterface()
        {
            var tree = NoObserverInterface();
            var collection = Walker.GenerateModels(tree);
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());

            Observer observer = new Observer(collection);
            observer.Scan();
            Assert.False(observer.GetResults().ContainsKey("IObserver"));
        }

        [Fact]
        public void Test_Observer_HasNoSubjectInterface()
        {
            var tree = NoSubjectInterface();
            var collection = Walker.GenerateModels(tree);
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());

            Observer observer = new Observer(collection);
            observer.Scan();
            
            Assert.False(observer.GetResults().ContainsKey("ISubject"));
        }

        [Fact]
        public void Test_Observer_HasNoInterfaces()
        {
            var tree = NoInterfaces();
            var collection = Walker.GenerateModels(tree);
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());

            Observer observer = new Observer(collection);
            observer.Scan();

            bool present = false;
            bool sPresent = false;
            bool oPresent = false;

            if (observer.GetResults().ContainsKey("IObserver"))
            {
                sPresent = true;
            }

            if (observer.GetResults().ContainsKey("ISubject"))
            {
                oPresent = true;
            }

            if (sPresent == oPresent)
            {
                present = true;
            }

            Assert.True(present);
        }
    }
}
