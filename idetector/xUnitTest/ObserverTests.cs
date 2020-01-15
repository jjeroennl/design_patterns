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
               using System;
using System.Collections.Generic;//We have used List<Observer> here
namespace ObserverPattern
{
    interface IObserver
    {
        void Update(int i);
    }
    class ObserverType1 : IObserver
    {
        string nameOfObserver;
        public ObserverType1(String name)
        {
            this.nameOfObserver = name;
        }
        public void Update(int i)
        {
            Console.WriteLine(' {0} has received an alert: Someone has updated myValue in Subject to: {1}', nameOfObserver,i);
        }
    }
    class ObserverType2 : IObserver
    {
        string nameOfObserver;
        public ObserverType2(String name)
        {
            this.nameOfObserver = name;
        }
        public void Update(int i)
        {
            Console.WriteLine(' {0} notified: myValue in Subject at present: {1}', nameOfObserver, i);
        }
    }

    interface ISubject
    {
        void Register(IObserver o);
        void Unregister(IObserver o);
        void NotifyRegisteredUsers(int i);
    }
    class Subject:ISubject
    {
        List<IObserver> observerList = new List<IObserver>();
        private int flag;
        public int Flag
        {
            get 
            { 
                return flag;
            }
            set
            {
                flag = value;
                //Flag value changed.So notify observer/s.
                NotifyRegisteredUsers(flag);
            }
        }
        public void Register(IObserver anObserver)
        { 
            observerList.Add(anObserver);
        }
        public void Unregister(IObserver anObserver)
        {
            observerList.Remove(anObserver);
        }
        public void NotifyRegisteredUsers(int i) 
        {
            foreach (IObserver observer in observerList)
            {
                observer.Update(i);
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(' ***Observer Pattern Demo***\n');
            //We have 3 observers- 2 of them are ObserverType1, 1 of them is of ObserverType2
            IObserver myObserver1 = new ObserverType1('Roy');
            IObserver myObserver2 = new ObserverType1('Kevin');
            IObserver myObserver3 = new ObserverType2('Bose');
            Subject subject = new Subject();
            //Registering the observers-Roy,Kevin,Bose
            subject.Register(myObserver1);
            subject.Register(myObserver2);
            subject.Register(myObserver3);
            Console.WriteLine(' Setting Flag = 5 ');
            subject.Flag = 5;           
            //Unregistering an observer(Roy))
            subject.Unregister(myObserver1);
            //No notification this time Roy.Since it is unregistered.
            Console.WriteLine('\n Setting Flag = 50 ');            
            subject.Flag = 50;
            //Roy is registering himself again
            subject.Register(myObserver1);
            Console.WriteLine('\n Setting Flag = 100 ');
            subject.Flag = 100;
            Console.ReadKey();
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
            
            // Checks if there is either an observer or subject, not both
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
