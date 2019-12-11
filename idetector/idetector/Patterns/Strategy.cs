using System;
using System.Collections.Generic;
using System.Text;
using idetector.Collections;
using idetector.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace idetector.Patterns
{
    public class Strategy : IPattern
    {
        private int _score;
        private ClassCollection cc;

        public Strategy(ClassCollection _cc)
        {
            cc = _cc;
            PriorityCollection.ClearPriorities();
            PriorityCollection.AddPriority("strategy", "ContextHasStrategy", Priority.Medium);
            PriorityCollection.AddPriority("strategy", "ContextHasPrivateStrategy", Priority.Low);
            PriorityCollection.AddPriority("strategy", "ContextHasPublicConstructor", Priority.Low);
            PriorityCollection.AddPriority("strategy", "ContextHasStrategySetter", Priority.Medium);
            PriorityCollection.AddPriority("strategy", "HasInterface", Priority.Low);
            PriorityCollection.AddPriority("strategy", "HasConcreteClasses", Priority.Low);
        }

        public void Scan()
        {
            if (ContextHasStrategy())
            {
                _score += PriorityCollection.GetPercentage("strategy", "ContextHasStrategy");
            }
            if (ContextHasPrivateStrategy())
            {
                _score += PriorityCollection.GetPercentage("strategy", "ContextHasPrivateStrategy");
            }
            if (ContextHasPublicConstructor())
            {
                _score += PriorityCollection.GetPercentage("strategy", "ContextHasPublicConstructor");
            }
            if (ContextHasStrategySetter())
            {
                _score += PriorityCollection.GetPercentage("strategy", "ContextHasStrategySetter");
            }
            if (HasInterface())
            {
                _score += PriorityCollection.GetPercentage("strategy", "HasInterface");
            }
            if (HasConcreteClasses())
            {
                _score += PriorityCollection.GetPercentage("strategy", "HasConcreteClasses");
            }
        }

        public int Score()
        {
            return _score;
        }

        public bool ContextHasStrategy()
        {
            foreach (var cls in cc.GetClasses())
            {
                foreach (var property in cls.Value.getProperties())
                {
                    if(cc.GetClass(property.ValueType.ToString()) != null)
                    {
                        if (cc.GetClass(property.ValueType.ToString()).IsInterface) return true;
                    }
                }
            }
            return false;
        }

        public bool ContextHasPrivateStrategy()
        {
            foreach (var cls in cc.GetClasses())
            {
                foreach (var property in cls.Value.getProperties())
                {
                    if (cc.GetClass(property.ValueType.ToString()) != null)
                    {
                        if (cc.GetClass(property.ValueType.ToString()).IsInterface)
                        {
                            if (property.Modifiers[0].ToLower().Equals("private")) return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool ContextHasPublicConstructor()
        {
            foreach (var cls in cc.GetClasses())
            {
                foreach (var method in cls.Value.getMethods())
                {
                    if (method.isConstructor)
                    {
                        foreach (var modifier in method.Modifiers)
                        {
                            if (modifier.ToLower().Equals("public")) return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool ContextHasStrategySetter()
        {
            foreach (var cls in cc.GetClasses())
            {
                foreach (var property in cls.Value.getProperties())
                {
                    if (property.Type.Equals(Models.Type.PropertySyntax))
                    {
                        var node = (PropertyDeclarationSyntax)property.GetNode();
                        if (node.AccessorList.ToString().Contains("set")) return true;
                    }
                    foreach (var method in cls.Value.getMethods())
                    {
                        if (method.Parameters.Contains(property.ValueType.ToString()) && method.Body.Contains(property.Identifier)) return true;
                    }
                }
            }
            return false;
        }

        public bool ContextHasLogic()
        {
            foreach (var cls in cc.GetClasses())
            {
                foreach (var property in cls.Value.getProperties())
                {
                    if (cc.GetClass(property.ValueType.ToString()) != null)
                    {
                        if (cc.GetClass(property.ValueType.ToString()).IsInterface)
                        {

                        }
                    }
                }
            }
            return false;
        }

        public bool HasInterface()
        {
            foreach (var cls in cc.GetClasses())
            {
                if(cls.Value.IsInterface) return true;
            }
            return false;
        }

        public bool HasConcreteClasses()
        {
            foreach (var cls in cc.GetClasses())
            {
                foreach (string parent in cls.Value.GetParents())
                {
                    if (cc.GetClass(parent).IsInterface) return true;
                }
            }
            return false;
        }
    }
}
