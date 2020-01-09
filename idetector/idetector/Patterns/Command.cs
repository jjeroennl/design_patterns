using idetector.Collections;
using System;
using System.Collections.Generic;
using idetector.Models;
using System.Text;
using System.Diagnostics;

namespace idetector.Patterns
{
    public class Command : IPattern
    {
        private float _score;
        private Dictionary<string, int> _scores = new Dictionary<string, int>();
        private ClassCollection cc;

        private ClassModel commandInterface;
        private ClassCollection commands = new ClassCollection();
        private ClassModel invoker;
        private ClassCollection receivers = new ClassCollection();


        /// <summary>
        /// Constructor for Command
        /// </summary>
        /// <param name="_cc">ClassCollection to check</param>
        public Command(ClassCollection _cc)
        {
            cc = _cc;

            PriorityCollection.ClearPriorities();
            PriorityCollection.AddPriority("command", "HasInterfaceOrAbstract", Priority.High);
            PriorityCollection.AddPriority("command", "HasCommandClasses", Priority.High);
            PriorityCollection.AddPriority("command", "CommandsHavePublicConstructor", Priority.High);
            PriorityCollection.AddPriority("command", "HasReceiverClasses", Priority.High);
        }

        public void Scan()
        {
            _score = 0;

            if (HasInterfaceOrAbstract().isTrue)
            {
                _score += PriorityCollection.GetPercentage("command", "HasInterfaceOrAbstract");
            }
            if (HasCommandClasses().isTrue)
            {
                _score += PriorityCollection.GetPercentage("command", "HasCommandClasses");
            }
            if (CommandsHavePublicConstructor().isTrue)
            {
                _score += PriorityCollection.GetPercentage("command", "CommandsHavePublicConstructor");
            }
            if (HasReceiverClasses().isTrue)
            {
                _score += PriorityCollection.GetPercentage("command", "HasReceiverClasses");
            }
            /*
            if (CommandsUseReceiver().isTrue)
            {
                _score += PriorityCollection.GetPercentage("command", "CommandsUseReceiver");
            }
            if (HasInvokerClass().isTrue)
            {
                _score += PriorityCollection.GetPercentage("command", "HasInvokerClass");
            }*/

        }

        public int Score()
        {
            return (int)_score;
        }


        public int Score(string className)
        {
            if (this._scores.ContainsKey(className))
            {
                return this._scores[className];
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Checks if command pattern has an interface or an abstract class.
        /// If this is true, adds the interface as a Classmodel to defined "commandInterface"
        /// </summary>
        public CheckedMessage HasInterfaceOrAbstract()
        {
            foreach (var cls in cc.GetClasses())
            {
                if (cls.Value.IsInterface || cls.Value.IsAbstract)
                {
                    commandInterface = cls.Value;
                    return new CheckedMessage(true);
                }
            }
            return new CheckedMessage("The command pattern does not contain either an interface or an abstract class.", false);
        }


        /// <summary>
        /// Checks if command pattern has at least one or more command classes.
        /// If a command is found, the command is added to the defined "commands" ClassCollection. If there is 1 or more commands, the check returns true.
        /// </summary>
        public CheckedMessage HasCommandClasses()
        {
            foreach (var cls in cc.GetClasses())
            {
                if (cls.Value.HasParent(commandInterface.Identifier))
                {
                    commands.AddClass(cls.Value);
                }
             }
            if (commands.GetClasses().Count >= 1) return new CheckedMessage(true);

            return new CheckedMessage("There are no commands that implement the abstract class or interface", false, commandInterface.Identifier);
        }

        /// <summary>
        /// Checks if a command has a public constructor.
        /// Only returns true if the count of command classes is the same as the amount of those checked for having a public constructor.
        /// </summary>
        public CheckedMessage CommandsHavePublicConstructor()
        {
            int count = 0;
            if (HasCommandClasses().isTrue)
            {
                foreach (var command in commands.GetClasses())
                {
                    foreach (var method in command.Value.getMethods())
                    {
                        if (method.isConstructor)
                        {
                            foreach (var modifier in method.Modifiers)
                            {
                                count += 1;
                            }
                        }
                    }
                }
            }
            if (commands.GetClasses().Count == count)
            {
                return new CheckedMessage(true);
            }
            else return new CheckedMessage("Either the interface does not contain a method, or the commands do not inherit the interface method.", false);
        }


        /// <summary>
        /// Checks if command pattern has a receiver class.
        /// If the amount of methods with private modifiers is the same as the amount of methods within a class, the class is added to the defined receivers ClassCollection.
        /// If there is at least one receiver class, returns true.
        /// </summary>
        public CheckedMessage HasReceiverClasses()
        {
            foreach (var cls in cc.GetClasses())
            {
                int count = 0;
                foreach (var method in cls.Value.getMethods())
                {
                    if (method.Modifiers[0].ToLower().Equals("private"))
                    {
                        count += 1;
                    }
                }
                if (count == cls.Value.getMethods().Count) receivers.AddClass(cls.Value);
            }
            if (receivers.GetClasses().Count >= 1)
            {
                return new CheckedMessage(true);
            }
            else return new CheckedMessage("There are no receiver classes implemented", false);
        }


        /// <summary>
        /// Checks if command patterns use the receiver class.
        /// If at least one of the commands use the receiver class, returns true.
        /// </summary>
        public CheckedMessage CommandsUseReceiver()
        {
            if (HasCommandClasses().isTrue)
            {
                if (HasReceiverClasses().isTrue)
                {
                    foreach (var command in commands.GetClasses())
                    {
                        foreach (var property in command.Value.getProperties())
                        {
                            if (cc.GetClass(property.ValueType.ToString()) != null)
                            {
                                foreach (var receiver in receivers.GetClasses())
                                {
                                    if (cc.GetClass(property.ValueType.ToString()) == receiver.Value)
                                    {
                                        return new CheckedMessage(true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return new CheckedMessage("None of the commands are controlled by a receiver", false);
        }


        /// <summary>
        /// Checks if command pattern has an invoker class.
        /// If this is true, adds the interface as a Classmodel to defined "commandInterface"
        /// </summary>
        public CheckedMessage HasInvokerClass()
        {
            if (HasInterfaceOrAbstract().isTrue)
            {
                foreach (var cls in cc.GetClasses())
                {
                    foreach (var property in cls.Value.getProperties())
                    {
                        if (cc.GetClass(property.ValueType.ToString()) != null)
                        {
                            if (property.ValueType.ToString() == commandInterface.ToString())
                            {
                                return new CheckedMessage(true);
                            }

                        }
                    }

                }

            }
            return new CheckedMessage("The code does not contain an Invoker class", false);
        }

    }
}
