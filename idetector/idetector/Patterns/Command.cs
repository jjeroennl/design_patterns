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

            HasCommandInterface();

            if (commandInterface != null)
            {
                HasInvokerClass();
                HasCommandClasses();
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
        public void HasCommandInterface()
        {
            if (commandInterface == null)
            {
                foreach (var cls in cc.GetClasses().Values)
                {
                    if (cls.IsInterface || cls.IsAbstract)
                    {
                        foreach (var method in cls.GetMethods())
                        {
                            if (method.ReturnType == "void" && cls.GetMethods().Count == 1)
                            {
                                commandInterface = cls;
                                Debug.WriteLine("Interface added: " + cls.Identifier + " true");
                                _results[cls.Identifier].Add(new RequirementResult("COMMAND-HAS-INTERFACE", true, cls));
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Checks if command pattern has at least one or more command classes.
        /// </summary>
        public void HasCommandClasses()
        {
            if (commandInterface != null)
            {
                foreach (var cls in cc.GetClasses())
                {
                    if (cls.Value.HasParent(commandInterface.Identifier))
                    {
                        commands.AddClass(cls.Value);
                        Debug.WriteLine("Command added: " + cls.Value.Identifier + " true");
                    }
                }
            }
            if (commands != null)
            {
                    _results[commandInterface.Identifier].Add(new RequirementResult("COMMAND-HAS-COMMAND-CLASS", true, commandInterface));
            } else
            {
                _results[commandInterface.Identifier].Add(new RequirementResult("COMMAND-HAS-COMMAND-CLASS", false, commandInterface));
            }
        }

        /// <summary>
        /// Checks if a command has a public constructor.
        /// </summary>
        public void CommandsHavePublicConstructor()
        {
            if (commands != null)
            {
                int count = 0;
                foreach (var command in commands.GetClasses())
                {
                    bool hasPublicConstructor = false;
                    foreach (var method in command.Value.GetMethods())
                    {
                        if (method.isConstructor)
                        {
                            foreach (var modifier in method.Modifiers)
                            {
                                if (method.Modifiers[0].ToLower().Equals("public"))
                                {
                                    Debug.WriteLine("Command has public constructor: " + command.Value.Identifier + " true");
                                    hasPublicConstructor = true;
                                }
                            }
                        }
                    }

                    if (hasPublicConstructor)
                    {
                        count += 1;
                    }
                }

                if (commands.GetClasses().Count == count)
                {
                    _results[commandInterface.Identifier].Add(new RequirementResult("COMMAND-HAS-PUBLIC-CONSTRUCTOR", true, commandInterface));
                } else
                {
                    _results[commandInterface.Identifier].Add(new RequirementResult("COMMAND-HAS-PUBLIC-CONSTRUCTOR", false, commandInterface));
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
                bool isReceiver = false;
                foreach (var method in cls.Value.GetMethods())
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
                }

                if (count == cls.Value.GetMethods().Count)
                {
                    isReceiver = true;
                }

                if (isReceiver)
                {
                    receivers.AddClass(cls.Value);
                    Debug.WriteLine("Receiver added: " + cls.Value.Identifier + " true");
                }
            }
            
            if (receivers.GetClasses().Count > 0)
            {
                _results[commandInterface.Identifier].Add(new RequirementResult("COMMAND-HAS-RECEIVER-CLASS", true, commandInterface));
            }
        }


        /// <summary>
        /// Checks if command patterns use the receiver class.
        /// </summary>
        public void CommandsUseReceiver()
        {
            if (commands != null)
            {
                int receiverCommandCount = 0;
                foreach (var command in commands.GetClasses())
                {
                    foreach (var property in command.Value.GetProperties())
                    {
                        if (cc.GetClass(property.ValueType.ToString()) != null)
                        {
                            if (receivers.GetClasses() != null)
                            {
                                foreach (var receiver in receivers.GetClasses())
                                {
                                    if ((property.ValueType == receiver.Value.Identifier))
                                    {
                                        Debug.WriteLine("Command using receiver: " + command.Value.Identifier + " true");
                                        receiverCommandCount += 1;
                                    }
                                }
                            }
                        }
                    }
                }

                if (receiverCommandCount > 0)
                {
                    _results[commandInterface.Identifier].Add(new RequirementResult("COMMAND-USES-RECEIVER", true, commandInterface));
                }
                else _results[commandInterface.Identifier].Add(new RequirementResult("COMMAND-USES-RECEIVER", false, commandInterface));
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
                    if (cc.GetClass(property.ValueType.ToString()) != null)
                    {
                        if (property.ValueType.ToString() == commandInterface.Identifier)
                        {
                            invoker = cls.Value;
                            Debug.WriteLine("Invoker added: " + cls.Value.Identifier + " true");
                        }
                    }
                }
            }

            if (invoker != null)
            {
                _results[commandInterface.Identifier].Add(new RequirementResult("COMMAND-HAS-INVOKER-CLASS", true, commandInterface));
            }
            else
                _results[commandInterface.Identifier].Add(new RequirementResult("COMMAND-HAS-INVOKER-CLASS", false, commandInterface));
            }
    }
}
