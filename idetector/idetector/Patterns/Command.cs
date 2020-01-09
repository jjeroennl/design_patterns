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

        private ClassModel commandInterface;
        private ClassCollection commands = new ClassCollection();
        private ClassModel invoker;
        private ClassCollection receivers = new ClassCollection();

        private List<RequirementResult> _results = new List<RequirementResult>();

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
            _results.Add(HasInterfaceOrAbstract());
            _results.Add(HasCommandClasses());
            _results.Add(CommandsHavePublicConstructor());
            _results.Add(CommandsUseReceiver());

        }
        public List<RequirementResult> GetResult()
        {
            return _results;
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
        public RequirementResult HasInterfaceOrAbstract()
        {
            foreach (var cls in cc.GetClasses())
            {
                if (cls.Value.IsInterface || cls.Value.IsAbstract)
                {
                    commandInterface = cls.Value;
                    return new RequirementResult("COMMAND-HAS-INTERFACE",true);
                }
            }
            return new RequirementResult("COMMAND-HAS-INTERFACE", false);
        }


        /// <summary>
        /// Checks if command pattern has at least one or more command classes.
        /// If a command is found, the command is added to the defined "commands" ClassCollection. If there is 1 or more commands, the check returns true.
        /// </summary>
        public RequirementResult HasCommandClasses()
        {
            foreach (var cls in cc.GetClasses())
            {
                    if (cls.Value.HasParent(commandInterface.Identifier))
                    {
                        commands.AddClass(cls.Value);
                    }
             }
            if (commands.GetClasses().Count >= 1) return new RequirementResult("COMMAND-HAS-COMMAND-CLASS",true);

            return new RequirementResult("COMMAND-HAS-COMMAND-CLASS", false);
        }

        /// <summary>
        /// Checks if a command has a public constructor.
        /// Only returns true if the count of command classes is the same as the amount of those checked for having a public constructor.
        /// </summary>
        public RequirementResult CommandsHavePublicConstructor()
        {
            int count = 0;
            if (HasCommandClasses().Passed)
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
                                    count += 1;
                                }
                            }
                        }
                    }
                }
            }
            if (commands.GetClasses().Count == count)
            {
                return new RequirementResult("COMMAND-HAS-PUBLIC-CONSTRUCTOR",true);
            }
            else return new RequirementResult("COMMAND-HAS-PUBLIC-CONSTRUCTOR", false);
        }


        /// <summary>
        /// Checks if command pattern has a receiver class.
        /// If the amount of methods with private modifiers is the same as the amount of methods within a class, the class is added to the defined receivers ClassCollection.
        /// If there is at least one receiver class, returns true.
        /// </summary>
        public RequirementResult HasReceiverClasses()
        {
            foreach (var cls in cc.GetClasses())
            {
                int count = 0;
                foreach (var method in cls.Value.getMethods())
                {
                    if (method.Modifiers.Length > 0)
                    {
                        if (method.Modifiers[0].ToLower().Equals("private"))
                        {
                            count += 1;
                        }
                    }
                }
                if (count == cls.Value.getMethods().Count) receivers.AddClass(cls.Value);
            }
            if (receivers.GetClasses().Count >= 1)
            {
                return new RequirementResult("COMMAND-HAS-RECEIVER-CLASS",true);
            }
            else return new RequirementResult("COMMAND-HAS-RECEIVER-CLASS", false);
        }


        /// <summary>
        /// Checks if command patterns use the receiver class.
        /// If at least one of the commands use the receiver class, returns true.
        /// </summary>
        public RequirementResult CommandsUseReceiver()
        {
            if (HasCommandClasses().Passed)
            {
                if (HasReceiverClasses().Passed)
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
                                        return new RequirementResult("COMMAND-USES-RECEIVER",true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return new RequirementResult("COMMAND-USES-RECEIVER", false);
        }


        /// <summary>
        /// Checks if command pattern has an invoker class.
        /// If this is true, adds the interface as a Classmodel to defined "commandInterface"
        /// </summary>
        public RequirementResult HasInvokerClass()
        {
            if (HasInterfaceOrAbstract().Passed)
            {
                foreach (var cls in cc.GetClasses())
                {
                    foreach (var property in cls.Value.getProperties())
                    {
                        if (cc.GetClass(property.ValueType.ToString()) != null)
                        {
                            if (property.ValueType.ToString() == commandInterface.ToString())
                            {
                                return new RequirementResult("COMMAND-HAS-INVOKER-CLASS",true);
                            }

                        }
                    }

                }

            }
            return new RequirementResult("COMMAND-HAS-INVOKER-CLASS", false);
        }

    }
}
