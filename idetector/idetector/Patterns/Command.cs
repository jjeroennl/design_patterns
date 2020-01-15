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
        }

        public void Scan()
        {
            foreach (var item in cc.GetClasses())
            {
                _results.Add(item.Value.Identifier, new List<RequirementResult>());
            }

            HasInterfaceOrAbstract();

            /// <summary>
            /// Only if the pattern contains an interface, these can return positive.
            /// </summary>
            if (commandInterface != null) {
                HasCommandClasses();
                HasInvokerClass();
                HasReceiverClasses();

                if (commands.GetClasses().Count > 0)
                {
                    CommandsHavePublicConstructor();
                    CommandsUseReceiver();
                }
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
        /// </summary>
        public void HasInterfaceOrAbstract()
        {
            if (commandInterface == null) {
                foreach (var cls in cc.GetClasses().Values)
                {
                    if (cls.IsInterface || cls.IsAbstract)
                    {
                        commandInterface = cls;
                        _results[cls.Identifier].Add(new RequirementResult("COMMAND-HAS-INTERFACE", true, cls));
                    }
                    else
                    {
                        _results[cls.Identifier].Add(new RequirementResult("COMMAND-HAS-INTERFACE", false, cls));
                    }
                }
            }
        }


        /// <summary>
        /// Checks if command pattern has at least one or more command classes.
        /// </summary>
        public void HasCommandClasses()
        {
            foreach (var cls in cc.GetClasses())
            {
                if (commandInterface == null)
                {
                    _results[cls.Value.Identifier].Add(new RequirementResult("COMMAND-HAS-COMMAND-CLASS", false, cls.Value));
                }
                if (cls.Value.HasParent(commandInterface.Identifier))
                {
                    foreach (var result in _results[commandInterface.Identifier].ToArray())
                    {
                            if (!(result.Id.Equals("COMMAND-HAS-COMMAND-CLASS")))
                            {
                                commands.AddClass(cls.Value);
                                _results[cls.Value.Identifier].Add(new RequirementResult("COMMAND-HAS-COMMAND-CLASS", true, cls.Value));
                            }
                        }
                }
                else
                {
                    foreach (var result in _results[commandInterface.Identifier].ToArray())
                    {
                            if (!(result.Id.Equals("COMMAND-HAS-COMMAND-CLASS")))
                            {
                                _results[cls.Value.Identifier].Add(new RequirementResult("COMMAND-HAS-COMMAND-CLASS", false, cls.Value));
                            }
                        }
                }
            }
        }

        /// <summary>
        /// Checks if a command has a public constructor.
        /// </summary>
        public void CommandsHavePublicConstructor()
        {
            if (commands != null)
            {
                foreach (var command in commands.GetClasses())
                {
                    foreach (var method in command.Value.getMethods())
                    {
                        if (method.isConstructor)
                        {
                            foreach (var modifier in method.Modifiers)
                            {
                                foreach (var result in _results[commandInterface.Identifier].ToArray())
                                {
                                    if (!(result.Id.Equals("COMMAND-HAS-PUBLIC-CONSTRUCTOR")))
                                    {
                                        if (method.Modifiers[0].ToLower().Equals("public"))
                                        {
                                            _results[command.Value.Identifier].Add(new RequirementResult("COMMAND-HAS-PUBLIC-CONSTRUCTOR", true, command.Value));
                                        }
                                        else
                                        {
                                            _results[command.Value.Identifier].Add(new RequirementResult("COMMAND-HAS-PUBLIC-CONSTRUCTOR", false, command.Value));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Checks if command pattern has a receiver class.
        /// </summary>
        public void HasReceiverClasses()
        {
            foreach (var cls in cc.GetClasses())
            {
                int count = 0;
                    foreach (var method in cls.Value.getMethods())
                    {
                        if (method.Modifiers.Length > 0)
                        {
                            if (!method.isConstructor)
                            {
                                if (method.Modifiers[0].ToLower().Equals("public"))
                                {
                                    count += 1;

                                }
                            }

                    }

                    foreach (var result in _results[commandInterface.Identifier].ToArray())
                    {
                        if (count == cls.Value.getMethods().Count)
                        {
                            if (!(result.Id.Equals("COMMAND-HAS-RECEIVER-CLASS")))
                            {
                                receivers.AddClass(cls.Value);
                                _results[cls.Value.Identifier].Add(new RequirementResult("COMMAND-HAS-RECEIVER-CLASS", true, cls.Value));
                            }
                        }
                        else
                        {
                            if (!(result.Id.Equals("COMMAND-HAS-RECEIVER-CLASS")))
                            {
                                _results[cls.Value.Identifier].Add(new RequirementResult("COMMAND-HAS-RECEIVER-CLASS", false, cls.Value));
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Checks if command patterns use the receiver class.
        /// </summary>
        public void CommandsUseReceiver()
        {
            if (commands != null)
            {
              foreach (var command in commands.GetClasses())
               {
                    foreach (var property in command.Value.getProperties())
                    {
                        if (cc.GetClass(property.ValueType.ToString()) != null)
                        {
                            if (receivers.GetClasses() != null)
                            {
                                foreach (var receiver in receivers.GetClasses())
                                {
                                    if ((property.ValueType == receiver.Value.Identifier))
                                    {
                                        _results[command.Value.Identifier].Add(new RequirementResult("COMMAND-USES-RECEIVER", true, command.Value));
                                    }
                                    else
                                    {
                                        _results[command.Value.Identifier].Add(new RequirementResult("COMMAND-USES-RECEIVER", false, command.Value));
                                    }
                                }
                            }
                        }
                    }
                    _results[command.Value.Identifier].Add(new RequirementResult("COMMAND-USES-RECEIVER", false, command.Value));
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
                    foreach (var property in cls.Value.getProperties())
                    {
                        if (cc.GetClass(property.ValueType.ToString()) != null)
                        {
                            if (property.ValueType.ToString() == commandInterface.Identifier)
                            {
                                invoker = cls.Value;
                                _results[cls.Value.Identifier].Add(new RequirementResult("COMMAND-HAS-INVOKER-CLASS", true, cls.Value));
                            } else
                            {
                                _results[cls.Value.Identifier].Add(new RequirementResult("COMMAND-HAS-INVOKER-CLASS", false, cls.Value));
                            }
                            
                        }
                    }

                }

        }

    }
}
