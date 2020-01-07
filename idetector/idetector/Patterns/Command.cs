using idetector.Collections;
using System;
using System.Collections.Generic;
using idetector.Models;
using System.Text;

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
        private ClassCollection receivers =  new ClassCollection();

        public Command(ClassCollection _cc)
        {
            cc = _cc;

        }

        public void Scan()
        {
            _score = 0;

            if (HasInterfaceOrAbstract().isTrue)
            {
                _score += PriorityCollection.GetPercentage("command", "HasInterfaceOrAbstract");
            }

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


        public CheckedMessage HasCommandClasses()
        {
            int i = 0;
            foreach (var cls in cc.GetClasses())
            {
                i += 1;
                foreach (string parent in cls.Value.GetParents())
                {
                    if (cc.GetClass(parent) == commandInterface) commands.AddClass(cls.Value);
                }
                if (commands.GetClasses().Count >= 1) return new CheckedMessage(true);
            }
            return new CheckedMessage("There are no commands that implement the abstract class or interface", false);
        }

        public CheckedMessage CommandInheritsInterfaceFunction()
        {
            /*
            int count = 0;
            foreach (var command in commands.GetClasses())
            {
                if (command.)

            }
            */
            return new CheckedMessage("Either the interface does not contain a method, or the commands do not inherit the interface method.", false);
        }

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

        public CheckedMessage HasReceiverClasses()
        {
            foreach (var cls in cc.GetClasses())
            {
                foreach (var method in cls.Value.getMethods())
                {
                   if (method.Modifiers.Length >= 1)
                     {
                        if (method.Modifiers[0].ToLower().Equals("private"))
                        {
                            receivers.AddClass(cls.Value);
                        }
                     }
                }
                if (receivers.GetClasses().Count >= 1) return new CheckedMessage(true);
            }
            return new CheckedMessage("There are no receiver classes implemented", false);
        }

        public CheckedMessage CommandsUseReceiver()
        {
            if (HasCommandClasses().isTrue) {
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
            return new CheckedMessage("None of the commands are controlled by a receiver", false);
        }

            //No clue how to check yet, will have to do more research
            public CheckedMessage HasInvokerClass()
        {
            return new CheckedMessage(true);
        }

        public CheckedMessage InvokerContainsInterfaceParameter()
        {
            return new CheckedMessage(true);
        }
    }
}
