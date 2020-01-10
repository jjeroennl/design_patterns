using System.Collections.Generic;
using idetector;
using idetector.Collections;
using idetector.Data;
using idetector.Models;
using idetector.Parser;
using idetector.Patterns;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace xUnitTest
{
    public class CommandTest
    {
        [Fact]
        public void Test_Command_Succeed()
        {
            Requirements r = new Requirements();
            ScoreCalculator calculator = new ScoreCalculator(r.GetRequirements());
            var tree = SuccessSetup();
            var collection = Walker.GenerateModels(tree);

            Command command = new Command(collection);
            command.Scan();
            var score = calculator.GetScore("COMMAND", command.GetResult());
            Assert.Equal(100, score);
        }

        [Fact]
        public void Test_NoInterface()
        {
            var tree = NoInterface();
            var collection = Walker.GenerateModels(tree);

            Command command = new Command(collection);
            Assert.False(command.HasInterfaceOrAbstract().Passed);
        }

        [Fact]
        public void Test_NoCommandClass()
        {
            var tree = NoCommand();
            var collection = Walker.GenerateModels(tree);

            Command command = new Command(collection);
            command.HasInterfaceOrAbstract();
            Assert.False(command.HasCommandClasses().Passed);
        }
        

        [Fact]
        public void Test_NoPublicConstructorCommand()
        {
            var tree = NoPublicConstructor();
            var collection = Walker.GenerateModels(tree);

            Command command = new Command(collection);
            command.HasInterfaceOrAbstract();
            Assert.False(command.CommandsHavePublicConstructor().Passed);
        }

        [Fact]
        public void Test_NoReceivers()
        {
            var tree = NoReceiver();
            var collection = Walker.GenerateModels(tree);

            Command command = new Command(collection);

            Assert.False(command.HasReceiverClasses().Passed);
        }

        [Fact]
        public void Test_NoCommandsUsingReceiver()
        {
            var tree = NoCommandUsingReceiver();
            var collection = Walker.GenerateModels(tree);

            Command command = new Command(collection);
            command.HasInterfaceOrAbstract();
            command.HasCommandClasses();
            command.HasReceiverClasses();
            Assert.False(command.CommandsUseReceiver().Passed);
        }

        [Fact]
        public void Test_NoInvoker()
        {
            var tree = NoInvoker();
            var collection = Walker.GenerateModels(tree);

            Command command = new Command(collection);

            Assert.False(command.HasInvokerClass().Passed);
        }

        [Fact]
        public void Test_Interface()
        {
            var tree = SuccessSetup();
            var collection = Walker.GenerateModels(tree);

            Command command = new Command(collection);

            Assert.True(command.HasInterfaceOrAbstract().Passed);
        }

        [Fact]
        public void Test_CommandClasses()
        {
            var tree = SuccessSetup();
            var collection = Walker.GenerateModels(tree);

            Command command = new Command(collection);

            command.HasInterfaceOrAbstract();
            Assert.True(command.HasCommandClasses().Passed);
        }

        [Fact]
        public void Test_PublicCommandConstructor()
        {
            var tree = SuccessSetup();
            var collection = Walker.GenerateModels(tree);

            Command command = new Command(collection);

            command.HasInterfaceOrAbstract();
            command.HasCommandClasses();
            Assert.True(command.CommandsHavePublicConstructor().Passed);
        }

        [Fact]
        public void Test_Receivers()
        {
            var tree = SuccessSetup();
            var collection = Walker.GenerateModels(tree);

            Command command = new Command(collection);

            Assert.True(command.HasReceiverClasses().Passed);
        }

        [Fact]
        public void Test_CommandUsesReceiver()
        {
            var tree = SuccessSetup();
            var collection = Walker.GenerateModels(tree);

            Command command = new Command(collection);
            command.HasInterfaceOrAbstract();
            command.HasCommandClasses();
            command.HasReceiverClasses();
            Assert.True(command.CommandsUseReceiver().Passed);
        }

        [Fact]
        public void Test_Invoker()
        {
            var tree = SuccessSetup();
            var collection = Walker.GenerateModels(tree);

            Command command = new Command(collection);
            command.HasInterfaceOrAbstract();
            Assert.True(command.HasInvokerClass().Passed);
        }

        public SyntaxTree SuccessSetup()
        {
            return CSharpSyntaxTree.ParseText(@"
        namespace RefactoringGuru.DesignPatterns.Command.Conceptual
        {
            // The Command interface declares a method for executing a command.
            public interface ICommand
            {
                void Execute();
            }

            // Some commands can implement simple operations on their own.
            class SimpleCommand : ICommand
            {
                private string _payload = string.Empty;

                public SimpleCommand(string payload)
                {
                    this._payload = payload;
                }

                public void Execute()
                {
                    Console.WriteLine($'SimpleCommand: See, I can do simple things like printing({ this._payload})');
                }
            }

            // However, some commands can delegate more complex operations to other
            // objects, called 'receivers.'
            class ComplexCommand : ICommand
            {
                private Receiver _receiver;

                // Context data, required for launching the receiver's methods.
                private string _a;

                private string _b;

                // Complex commands can accept one or several receiver objects along
                // with any context data via the constructor.
                public ComplexCommand(Receiver receiver, string a, string b)
                {
                    this._receiver = receiver;
                    this._a = a;
                    this._b = b;
                }

                // Commands can delegate to any methods of a receiver.
                public void Execute()
                {
                    Console.WriteLine('ComplexCommand: Complex stuff should be done by a receiver object');
                    this._receiver.DoSomething(this._a);
                    this._receiver.DoSomethingElse(this._b);
                }
            }

            // The Receiver classes contain some important business logic. They know how
            // to perform all kinds of operations, associated with carrying out a
            // request. In fact, any class may serve as a Receiver.

            class Receiver
            {
                public void DoSomething(string a)
                {
                    Console.WriteLine($'Receiver: Working on ({a}.)');
                }

                public void DoSomethingElse(string b)
                {
                    Console.WriteLine($'Receiver: Also working on ({b}.)');
                }
            }

            // The Invoker is associated with one or several commands. It sends a
            // request to the command.

            class Invoker
            {
                private ICommand _onStart;

                private ICommand _onFinish;

                // Initialize commands.
                public void SetOnStart(ICommand command)
                {
                    this._onStart = command;
                }

                public void SetOnFinish(ICommand command)
                {
                    this._onFinish = command;
                }

                // The Invoker does not depend on concrete command or receiver classes.
                // The Invoker passes a request to a receiver indirectly, by executing a
                // command.
                public void DoSomethingImportant()
                {
                    Console.WriteLine('Invoker: Does anybody want something done before I begin?');
                    if (this._onStart is ICommand)
                    {
                        this._onStart.Execute();
                    }

                    Console.WriteLine('Ínvoker: ...doing something really important...');

                    Console.WriteLine('Invoker: Does anybody want something done after I finish?');
                    if (this._onFinish is ICommand)
                    {
                        this._onFinish.Execute();
                    }
                }
            }");
        }

        public SyntaxTree NoInterface()
        {
            return CSharpSyntaxTree.ParseText(@"
            namespace RefactoringGuru.DesignPatterns.Command.Conceptual
        {
            // Some commands can implement simple operations on their own.
            class SimpleCommand : ICommand
            {
                private string _payload = string.Empty;

                public SimpleCommand(string payload)
                {
                    this._payload = payload;
                }

                public void Execute()
                {
                    Console.WriteLine($'SimpleCommand: See, I can do simple things like printing({ this._payload})');
                }
            }

            // However, some commands can delegate more complex operations to other
            // objects, called 'receivers.'
            class ComplexCommand : ICommand
            {
                private Receiver _receiver;

                // Context data, required for launching the receiver's methods.
                private string _a;

                private string _b;

                // Complex commands can accept one or several receiver objects along
                // with any context data via the constructor.
                public ComplexCommand(Receiver receiver, string a, string b)
                {
                    this._receiver = receiver;
                    this._a = a;
                    this._b = b;
                }

                // Commands can delegate to any methods of a receiver.
                public void Execute()
                {
                    Console.WriteLine('ComplexCommand: Complex stuff should be done by a receiver object');
                    this._receiver.DoSomething(this._a);
                    this._receiver.DoSomethingElse(this._b);
                }
            }

            // The Receiver classes contain some important business logic. They know how
            // to perform all kinds of operations, associated with carrying out a
            // request. In fact, any class may serve as a Receiver.
            class Receiver
            {
                public void DoSomething(string a)
                {
                    Console.WriteLine($'Receiver: Working on ({a}.)');
                }

                public void DoSomethingElse(string b)
                {
                    Console.WriteLine($'Receiver: Also working on ({b}.)');
                }
            }

            // The Invoker is associated with one or several commands. It sends a
            // request to the command.
            class Invoker
            {
                private ICommand _onStart;

                private ICommand _onFinish;

                // Initialize commands.
                public void SetOnStart(ICommand command)
                {
                    this._onStart = command;
                }

                public void SetOnFinish(ICommand command)
                {
                    this._onFinish = command;
                }

                // The Invoker does not depend on concrete command or receiver classes.
                // The Invoker passes a request to a receiver indirectly, by executing a
                // command.
                public void DoSomethingImportant()
                {
                    Console.WriteLine('Invoker: Does anybody want something done before I begin?');
                    if (this._onStart is ICommand)
                    {
                        this._onStart.Execute();
                    }

                    Console.WriteLine('Ínvoker: ...doing something really important...');

                    Console.WriteLine('Invoker: Does anybody want something done after I finish?');
                    if (this._onFinish is ICommand)
                    {
                        this._onFinish.Execute();
                    }
                }

            }");
        }
        public SyntaxTree NoCommand()
        {
            return CSharpSyntaxTree.ParseText(@"
        namespace RefactoringGuru.DesignPatterns.Command.Conceptual
        {
            // The Command interface declares a method for executing a command.
            public interface ICommand
            {
                void Execute();
            }

            // The Receiver classes contain some important business logic. They know how
            // to perform all kinds of operations, associated with carrying out a
            // request. In fact, any class may serve as a Receiver.
            class Receiver
            {
                public void DoSomething(string a)
                {
                    Console.WriteLine($'Receiver: Working on ({a}.)');
                }

                public void DoSomethingElse(string b)
                {
                    Console.WriteLine($'Receiver: Also working on ({b}.)');
                }
            }

            // The Invoker is associated with one or several commands. It sends a
            // request to the command.
            class Invoker
            {
                private ICommand _onStart;

                private ICommand _onFinish;

                // Initialize commands.
                public void SetOnStart(ICommand command)
                {
                    this._onStart = command;
                }

                public void SetOnFinish(ICommand command)
                {
                    this._onFinish = command;
                }

                // The Invoker does not depend on concrete command or receiver classes.
                // The Invoker passes a request to a receiver indirectly, by executing a
                // command.
                public void DoSomethingImportant()
                {
                    Console.WriteLine('Invoker: Does anybody want something done before I begin?');
                    if (this._onStart is ICommand)
                    {
                        this._onStart.Execute();
                    }

                    Console.WriteLine('Ínvoker: ...doing something really important...');

                    Console.WriteLine('Invoker: Does anybody want something done after I finish?');
                    if (this._onFinish is ICommand)
                    {
                        this._onFinish.Execute();
                    }
                }
            }");
        }
        public SyntaxTree NoPublicConstructor()
        {
            return CSharpSyntaxTree.ParseText(@"
        namespace RefactoringGuru.DesignPatterns.Command.Conceptual
        {
            // The Command interface declares a method for executing a command.
            public interface ICommand
            {
                void Execute();
            }

            // Some commands can implement simple operations on their own.
            class SimpleCommand : ICommand
            {
                private string _payload = string.Empty;

                private SimpleCommand(string payload)
                {
                    this._payload = payload;
                }

                public void Execute()
                {
                    Console.WriteLine($'SimpleCommand: See, I can do simple things like printing({ this._payload})');
                }
            }

            // However, some commands can delegate more complex operations to other
            // objects, called 'receivers.'
            class ComplexCommand : ICommand
            {
                private Receiver _receiver;

                // Context data, required for launching the receiver's methods.
                private string _a;

                private string _b;

                // Complex commands can accept one or several receiver objects along
                // with any context data via the constructor.
                private ComplexCommand(Receiver receiver, string a, string b)
                {
                    this._receiver = receiver;
                    this._a = a;
                    this._b = b;
                }

                // Commands can delegate to any methods of a receiver.
                public void Execute()
                {
                    Console.WriteLine('ComplexCommand: Complex stuff should be done by a receiver object');
                    this._receiver.DoSomething(this._a);
                    this._receiver.DoSomethingElse(this._b);
                }
            }

            // The Receiver classes contain some important business logic. They know how
            // to perform all kinds of operations, associated with carrying out a
            // request. In fact, any class may serve as a Receiver.
            class Receiver
            {
                public void DoSomething(string a)
                {
                    Console.WriteLine($'Receiver: Working on ({a}.)');
                }

                public void DoSomethingElse(string b)
                {
                    Console.WriteLine($'Receiver: Also working on ({b}.)');
                }
            }

            // The Invoker is associated with one or several commands. It sends a
            // request to the command.
            class Invoker
            {
                private ICommand _onStart;

                private ICommand _onFinish;

                // Initialize commands.
                public void SetOnStart(ICommand command)
                {
                    this._onStart = command;
                }

                public void SetOnFinish(ICommand command)
                {
                    this._onFinish = command;
                }

                // The Invoker does not depend on concrete command or receiver classes.
                // The Invoker passes a request to a receiver indirectly, by executing a
                // command.
                public void DoSomethingImportant()
                {
                    Console.WriteLine('Invoker: Does anybody want something done before I begin?');
                    if (this._onStart is ICommand)
                    {
                        this._onStart.Execute();
                    }

                    Console.WriteLine('Ínvoker: ...doing something really important...');

                    Console.WriteLine('Invoker: Does anybody want something done after I finish?');
                    if (this._onFinish is ICommand)
                    {
                        this._onFinish.Execute();
                    }
                }
            }");
        }
        public SyntaxTree NoReceiver()
        {
            return CSharpSyntaxTree.ParseText(@"
        namespace RefactoringGuru.DesignPatterns.Command.Conceptual
        {
            // The Command interface declares a method for executing a command.
            public interface ICommand
            {
                void Execute();
            }

            // Some commands can implement simple operations on their own.
            class SimpleCommand : ICommand
            {
                private string _payload = string.Empty;

                private SimpleCommand(string payload)
                {
                    this._payload = payload;
                }

            }

            // However, some commands can delegate more complex operations to other
            // objects, called 'receivers.'
            class ComplexCommand : ICommand
            {
                private Receiver _receiver;

                // Context data, required for launching the receiver's methods.
                private string _a;

                private string _b;

                // Complex commands can accept one or several receiver objects along
                // with any context data via the constructor.
                private ComplexCommand(Receiver receiver, string a, string b)
                {
                    this._receiver = receiver;
                    this._a = a;
                    this._b = b;
                }

                // Commands can delegate to any methods of a receiver.
                private void Execute()
                {
                    Console.WriteLine('ComplexCommand: Complex stuff should be done by a receiver object');
                    this._receiver.DoSomething(this._a);
                    this._receiver.DoSomethingElse(this._b);
                }
            }


            // The Invoker is associated with one or several commands. It sends a
            // request to the command.
            class Invoker
            {
                private ICommand _onStart;

                private ICommand _onFinish;

                // Initialize commands.
                private void SetOnStart(ICommand command)
                {
                    this._onStart = command;
                }

                private void SetOnFinish(ICommand command)
                {
                    this._onFinish = command;
                }

                // The Invoker does not depend on concrete command or receiver classes.
                // The Invoker passes a request to a receiver indirectly, by executing a
                // command.
                private void DoSomethingImportant()
                {
                    Console.WriteLine('Invoker: Does anybody want something done before I begin?');
                    if (this._onStart is ICommand)
                    {
                        this._onStart.Execute();
                    }

                    Console.WriteLine('Ínvoker: ...doing something really important...');

                    Console.WriteLine('Invoker: Does anybody want something done after I finish?');
                    if (this._onFinish is ICommand)
                    {
                        this._onFinish.Execute();
                    }
                }
            }");
        }

        public SyntaxTree NoCommandUsingReceiver()
        {
            return CSharpSyntaxTree.ParseText(@"
        namespace RefactoringGuru.DesignPatterns.Command.Conceptual
        {
            // The Command interface declares a method for executing a command.
            public interface ICommand
            {
                void Execute();
            }

            // Some commands can implement simple operations on their own.
            class SimpleCommand : ICommand
            {
                private string _payload = string.Empty;

                private SimpleCommand(string payload)
                {
                    this._payload = payload;
                }

                public void Execute()
                {
                    Console.WriteLine($'SimpleCommand: See, I can do simple things like printing({ this._payload})');
                }
            }

            // However, some commands can delegate more complex operations to other
            // objects, called 'receivers.'
            class ComplexCommand : ICommand
            {

                // Context data, required for launching the receiver's methods.
                private string _a;

                private string _b;

                // Complex commands can accept one or several receiver objects along
                // with any context data via the constructor.

                // Commands can delegate to any methods of a receiver.
                public void Execute()
                {
                    Console.WriteLine('ComplexCommand: Complex stuff should be done by a receiver object');
                    this._receiver.DoSomething(this._a);
                    this._receiver.DoSomethingElse(this._b);
                }
            }

            // The Receiver classes contain some important business logic. They know how
            // to perform all kinds of operations, associated with carrying out a
            // request. In fact, any class may serve as a Receiver.
            class Receiver
            {
                public void DoSomething(string a)
                {
                    Console.WriteLine($'Receiver: Working on ({a}.)');
                }

                public void DoSomethingElse(string b)
                {
                    Console.WriteLine($'Receiver: Also working on ({b}.)');
                }
            }

            // The Invoker is associated with one or several commands. It sends a
            // request to the command.
            class Invoker
            {
                private ICommand _onStart;

                private ICommand _onFinish;

                // Initialize commands.
                public void SetOnStart(ICommand command)
                {
                    this._onStart = command;
                }

                public void SetOnFinish(ICommand command)
                {
                    this._onFinish = command;
                }

                // The Invoker does not depend on concrete command or receiver classes.
                // The Invoker passes a request to a receiver indirectly, by executing a
                // command.
                public void DoSomethingImportant()
                {
                    Console.WriteLine('Invoker: Does anybody want something done before I begin?');
                    if (this._onStart is ICommand)
                    {
                        this._onStart.Execute();
                    }

                    Console.WriteLine('Ínvoker: ...doing something really important...');

                    Console.WriteLine('Invoker: Does anybody want something done after I finish?');
                    if (this._onFinish is ICommand)
                    {
                        this._onFinish.Execute();
                    }
                }
            }");
        }
        public SyntaxTree NoInvoker()
        {
            return CSharpSyntaxTree.ParseText(@"
        namespace RefactoringGuru.DesignPatterns.Command.Conceptual
        {
            // The Command interface declares a method for executing a command.
            public interface ICommand
            {
                void Execute();
            }

            // Some commands can implement simple operations on their own.
            class SimpleCommand : ICommand
            {
                private string _payload = string.Empty;

                private SimpleCommand(string payload)
                {
                    this._payload = payload;
                }

                public void Execute()
                {
                    Console.WriteLine($'SimpleCommand: See, I can do simple things like printing({ this._payload})');
                }
            }

            // However, some commands can delegate more complex operations to other
            // objects, called 'receivers.'
            class ComplexCommand : ICommand
            {
                private Receiver _receiver;

                // Context data, required for launching the receiver's methods.
                private string _a;

                private string _b;

                // Complex commands can accept one or several receiver objects along
                // with any context data via the constructor.
                private ComplexCommand(Receiver receiver, string a, string b)
                {
                    this._receiver = receiver;
                    this._a = a;
                    this._b = b;
                }

                // Commands can delegate to any methods of a receiver.
                public void Execute()
                {
                    Console.WriteLine('ComplexCommand: Complex stuff should be done by a receiver object');
                    this._receiver.DoSomething(this._a);
                    this._receiver.DoSomethingElse(this._b);
                }
            }

            // The Receiver classes contain some important business logic. They know how
            // to perform all kinds of operations, associated with carrying out a
            // request. In fact, any class may serve as a Receiver.
            class Receiver
            {
                public void DoSomething(string a)
                {
                    Console.WriteLine($'Receiver: Working on ({a}.)');
                }

                public void DoSomethingElse(string b)
                {
                    Console.WriteLine($'Receiver: Also working on ({b}.)');
                }
            }
            }");
        }
    }
}