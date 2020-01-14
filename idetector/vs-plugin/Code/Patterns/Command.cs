﻿using idetector.Collections;
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
        private ClassCollection commands = new ClassCollection();
        private ClassModel invoker;
        private ClassCollection receivers = new ClassCollection();

        private Dictionary<string, List<RequirementResult>> _results = new Dictionary<string, List<RequirementResult>>();

        /// <summary>
        /// Constructor for Command
        /// </summary>
        /// <param name="_cc">ClassCollection to check</param>
        public Command(ClassCollection _cc)
        {
            cc = _cc;
        }

        public void Scan()
        {
            HasInterfaceOrAbstract();
            HasCommandClasses();
            CommandsHavePublicConstructor();
            HasReceiverClasses();
            CommandsUseReceiver();
            HasInvokerClass();
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
        public void HasInterfaceOrAbstract()
        {
            if (commandInterface == null)
            {
                foreach (var cls in cc.GetClasses().Values)
                {
                    if (cls.IsInterface || cls.IsAbstract)
                    {
                        commandInterface = cls;
                        _results.Add(cls.Identifier, new List<RequirementResult>());
                        _results[cls.Identifier].Add(new RequirementResult("COMMAND-HAS-INTERFACE", true, cls));
                    }
                    else
                    {
                        _results.Add(cls.Identifier, new List<RequirementResult>());
                        _results[cls.Identifier].Add(new RequirementResult("COMMAND-HAS-INTERFACE", false, cls));
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
                foreach (var result in _results[commandInterface.Identifier].ToArray())
                {
                    if (cls.Value.HasParent(commandInterface.Identifier))
                    {
                        if (!(result.Id.Equals("COMMAND-HAS-COMMAND-CLASS")))
                        {
                            if (!(result.Id.Equals("COMMAND-HAS-INTERFACE")))
                            {
                                commands.AddClass(cls.Value);
                                _results.Add(cls.Value.Identifier, new List<RequirementResult>());
                                _results[cls.Value.Identifier].Add(new RequirementResult("COMMAND-HAS-COMMAND-CLASS", true, cls.Value));
                            }
                        }
                    }
                    else
                    {
                        if (!(result.Id.Equals("COMMAND-HAS-COMMAND-CLASS")))
                        {
                            if (!(result.Id.Equals("COMMAND-HAS-INTERFACE")))
                            {
                                _results.Add(cls.Value.Identifier, new List<RequirementResult>());
                                _results[cls.Value.Identifier].Add(new RequirementResult("COMMAND-HAS-COMMAND-CLASS", false, cls.Value));
                            }
                        }
                    }
                }
            }
            Debug.WriteLine(commands.GetClasses().Values);
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
                    foreach (var method in command.Value.getMethods())
                    {
                        if (method.isConstructor)
                        {
                            foreach (var modifier in method.Modifiers)
                            {
                                if (method.Modifiers[0].ToLower().Equals("public"))
                                {
                                    _results.Add(command.Value.Identifier, new List<RequirementResult>());
                                    _results[command.Value.Identifier].Add(new RequirementResult("COMMAND-HAS-PUBLIC-CONSTRUCTOR", true, command.Value));
                                }
                                else
                                {
                                    _results.Add(command.Value.Identifier, new List<RequirementResult>());
                                    _results[command.Value.Identifier].Add(new RequirementResult("COMMAND-HAS-PUBLIC-CONSTRUCTOR", false, command.Value));
                                }
                            }
                        }
                    }
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
                            if (!(result.Id.Equals("COMMAND-HAS-RECEIVER-CLASS") || (result.Id.Equals("COMMAND-HAS-INTERFACE"))))
                            {
                                receivers.AddClass(cls.Value);
                                _results.Add(cls.Value.Identifier, new List<RequirementResult>());
                                _results[cls.Value.Identifier].Add(new RequirementResult("COMMAND-HAS-RECEIVER-CLASS", true, cls.Value));
                            }
                        }
                        else
                        {
                            if (!(result.Id.Equals("COMMAND-HAS-RECEIVER-CLASS") || (result.Id.Equals("COMMAND-HAS-INTERFACE"))))
                            {
                                _results.Add(cls.Value.Identifier, new List<RequirementResult>());
                                _results[cls.Value.Identifier].Add(new RequirementResult("COMMAND-HAS-RECEIVER-CLASS", false, cls.Value));
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Checks if command patterns use the receiver class.
        /// If at least one of the commands use the receiver class, returns true.
        /// </summary>
        public void CommandsUseReceiver()
        {
            if (commands != null)
            {
                if (receivers != null)
                {
                    foreach (var command in commands.GetClasses())
                    {
                        foreach (var property in command.Value.getProperties())
                        {
                            if (cc.GetClass(property.ValueType.ToString()) != null)
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
                }
            }
        }


        /// <summary>
        /// Checks if command pattern has an invoker class.
        /// </summary>
        public void HasInvokerClass()
        {
            if (commandInterface != null)
            {
                foreach (var cls in cc.GetClasses())
                {
                    foreach (var property in cls.Value.getProperties())
                    {
                        if (cc.GetClass(property.ValueType.ToString()) != null)
                        {
                            if (property.ValueType.ToString() == commandInterface.Identifier)
                            {
                                _results[cls.Value.Identifier].Add(new RequirementResult("COMMAND-HAS-INVOKER-CLASS", true, cls.Value));
                            }
                            else
                            {
                                _results[cls.Value.Identifier].Add(new RequirementResult("COMMAND-HAS-INVOKER-CLASS", true, cls.Value));
                            }

                        }
                    }

                }

            }
        }

    }
}