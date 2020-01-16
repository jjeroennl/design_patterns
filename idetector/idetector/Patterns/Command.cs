using idetector.Collections;
using System;
using System.Collections.Generic;
using idetector.Models;
using System.Text;
using System.Diagnostics;

namespace idetector.Patterns
{
    /*ID's:
     * COMMAND-HAS-INTERFACE
     * COMMAND-HAS-COMMAND-CLASS
     * COMMAND-HAS-PUBLIC-CONSTRUCTOR
     * COMMAND-HAS-RECEIVER-CLASS
     * COMMAND-USES-RECEIVER
     * COMMAND-HAS-INVOKER-CLASS
     */
    public class Command : IPattern
    {
        private float _score;
        private Dictionary<string, int> _scores = new Dictionary<string, int>();
        private ClassCollection cc;
        private ClassModel _cls;

        private ClassModel commandInterface;
        private List<ClassModel> publicCommandConstructors;
        private ClassCollection commands;
        private ClassModel invoker;
        private ClassCollection receivers;

        private Dictionary<string, List<RequirementResult>> _results;

        /// <summary>
        /// Constructor for Command
        /// </summary>
        /// <param name="_cc">ClassCollection to check</param>
        public Command(ClassCollection _cc)
        {
            cc = _cc;
            commands = new ClassCollection();
            receivers = new ClassCollection();
            _results = new Dictionary<string, List<RequirementResult>>();
            publicCommandConstructors = new List<ClassModel>();
        }

        public void Scan()
        {
            foreach (var item in cc.GetClasses())
            {
                _results.Add(item.Value.Identifier, new List<RequirementResult>());
            }

            HasCommandInterface();

            if (commandInterface != null)
            {
                HasCommandClasses();
                HasReceiverClasses();

                if (commands.GetClasses().Count > 0)
                {
                    CommandsHavePublicConstructor();
                    CommandsUseReceiver();
                }
                HasInvokerClass();

            }
        }

        public Dictionary<string, List<RequirementResult>> GetResults()
        {
            return _results;
        }

        public int Score()
        {
            return (int)_score;
        }

        /// <summary>
        /// Checks if command pattern has an interface or an abstract class.
        /// If this is true, adds the interface as a Classmodel to defined "commandInterface"
        /// </summary>
        public void HasCommandInterface()
        {
            if (commandInterface == null)
            {
                foreach (var cls in cc.GetClasses().Values)
                {
                    if (cls.IsInterface || cls.IsAbstract)
                    {
                        commandInterface = cls;
                        _results[commandInterface.Identifier].Add(new RequirementResult("COMMAND-HAS-INTERFACE", true, cls));
                    }
                }
            }
        }


        /// <summary>
        /// Checks if command pattern has at least one or more command classes.
        /// If a command is found, the command is added to the defined "commands" ClassCollection. If there is 1 or more commands, the check returns true.
        /// </summary>
        public void HasCommandClasses()
        {
            foreach (var cls in cc.GetClasses())
            {
                if (cls.Value.HasParent(commandInterface.Identifier))
                {
                    commands.AddClass(cls.Value);
                }
            }

            if (commands.GetClasses().Count > 0)
            {
                _results[commandInterface.Identifier]
                    .Add(new RequirementResult("COMMAND-HAS-COMMAND-CLASS", true, commandInterface));
            }
            else
            {
                _results[commandInterface.Identifier].Add(new RequirementResult("COMMAND-HAS-COMMAND-CLASS", false, commandInterface));
            }
        }

        /// <summary>
        /// Checks if a command has a public constructor.
        /// Only returns true if the count of command classes is the same as the amount of those checked for having a public constructor.
        /// </summary>
        public void CommandsHavePublicConstructor()
        {
            if (commands != null)
            {
                foreach (var command in commands.GetClasses())
                {
                    foreach (var method in command.Value.GetMethods())
                    {
                        if (method.isConstructor)
                        {
                            foreach (var modifier in method.Modifiers)
                            {
                                if (method.Modifiers[0].ToLower().Equals("public"))
                                {
                                    publicCommandConstructors.Add(command.Value);
                                }
                            }
                        }
                    }
                }

                if (commands.GetClasses().Count == publicCommandConstructors.Count)
                {
                    _results[commandInterface.Identifier]
                        .Add(new RequirementResult("COMMAND-HAS-PUBLIC-CONSTRUCTOR", true, commandInterface));

                }
                else
                {
                    _results[commandInterface.Identifier]
                        .Add(new RequirementResult("COMMAND-HAS-PUBLIC-CONSTRUCTOR", false, commandInterface));
                }
            }
        }


        /// <summary>
        /// Checks if command pattern has a receiver class.
        /// If the amount of methods with private modifiers is the same as the amount of methods within a class, the class is added to the defined receivers ClassCollection.
        /// If there is at least one receiver class, returns true.
        /// </summary>
        public void HasReceiverClasses()
        {
            foreach (var cls in cc.GetClasses())
            {
                int privateMethods = 0;
                foreach (var method in cls.Value.GetMethods())
                {
                    if (method.Modifiers.Length > 0)
                    {
                        if (!method.isConstructor)
                        {
                            if (method.Modifiers[0].ToLower().Equals("public"))
                            {
                                privateMethods += 1;

                            }
                        }

                    }
                }

                if (privateMethods == cls.Value.GetMethods().Count)
                {
                    receivers.AddClass(cls.Value);
                }
            }

            if (receivers.GetClasses().Count > 0)
            {
                _results[commandInterface.Identifier]
                    .Add(new RequirementResult("COMMAND-HAS-RECEIVER-CLASS", true, commandInterface));

            }

            else
            {
                _results[commandInterface.Identifier]
                    .Add(new RequirementResult("COMMAND-HAS-RECEIVER-CLASS", false, commandInterface));
            }

        }



        /// <summary>
        /// Checks if command patterns use the receiver class.
        /// If at least one of the commands use the receiver class, returns true.
        /// </summary>
        public void CommandsUseReceiver()
        {
            bool itdoes = false;
            if (commands != null)
            {
                if (receivers != null)
                {
                    foreach (var command in commands.GetClasses())
                    {
                        foreach (var property in command.Value.GetProperties())
                        {
                            if (cc.GetClass(property.ValueType) != null)
                            {
                                foreach (var receiver in receivers.GetClasses())
                                {
                                    if (property.ValueType == receiver.Value.Identifier)
                                    {
                                        itdoes = true;
                                        _results[commandInterface.Identifier].Add(new RequirementResult("COMMAND-USES-RECEIVER", true, command.Value));
                                    }

                                }
                            }
                        }
                    }

                    if (!itdoes)
                    {
                        _results[commandInterface.Identifier].Add(new RequirementResult("COMMAND-USES-RECEIVER", false, commandInterface));
                    }
                }
            }
        }


        /// <summary>
        /// Checks if command pattern has an invoker class.
        /// </summary>
        public void HasInvokerClass()
        {
            foreach (var cls in cc.GetClasses())
            {
                foreach (var property in cls.Value.GetProperties())
                {
                    if (cc.GetClass(property.ValueType) != null)
                    {
                        if (property.ValueType == commandInterface.Identifier)
                        {
                            invoker = cls.Value;
                            _results[commandInterface.Identifier].Add(new RequirementResult("COMMAND-HAS-INVOKER-CLASS", true, cls.Value));
                            break;
                        }
                    }
                }
            }

            if (invoker == null)
            {
                _results[commandInterface.Identifier].Add(new RequirementResult("COMMAND-HAS-INVOKER-CLASS", false, commandInterface));
            }

        }

    }
}
