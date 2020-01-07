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
        private ClassModel Invoker;


        public Command(ClassCollection _cc)
        {
            cc = _cc;

            PriorityCollection.ClearPriorities();
            PriorityCollection.AddPriority("command", "HasInterfaceOrAbstract", Priority.Medium);
            PriorityCollection.AddPriority("command", "HasCommandClasses", Priority.Medium);
            PriorityCollection.AddPriority("command", "CommandInheritsInterfaceFunction", Priority.Medium);
            PriorityCollection.AddPriority("command", "CommandHasPublicConstructor", Priority.Medium);
            PriorityCollection.AddPriority("command", "HasReceiverClass", Priority.Medium);
            PriorityCollection.AddPriority("command", "HasInvokerClass", Priority.Medium);
            PriorityCollection.AddPriority("command", "InvokerContainsInterfaceParameter", Priority.Medium);

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
            return new CheckedMessage(true);
        }

        public CheckedMessage CommandHasPublicConstructor()
        {
            return new CheckedMessage(true);
        }

        //Implement public functions
        public CheckedMessage HasReceiverClass()
        {
            return new CheckedMessage(true);
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
