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
            PriorityCollection.AddPriority("strategy", "HasPublicConstructor", Priority.Low);
            PriorityCollection.AddPriority("strategy", "ContextHasStrategySetter", Priority.Medium);
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
            if (HasPublicConstructor())
            {
                _score += PriorityCollection.GetPercentage("strategy", "HasPublicConstructor");
            }
            if (ContextHasStrategySetter())
            {
                _score += PriorityCollection.GetPercentage("strategy", "ContextHasStrategySetter");
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
                    if(cc.GetClass(property.ValueType.ToString()).IsInterface)
                    {
                        return true;
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
                    if (cc.GetClass(property.ValueType.ToString()).IsInterface)
                    {
                        if (property.Modifiers[0].ToLower().Equals("private"))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool HasPublicConstructor()
        {
            foreach (var cls in cc.GetClasses())
            {
                foreach (var method in cls.Value.getMethods())
                {
                    if (method.isConstructor)
                    {
                        foreach (var modifier in method.Modifiers)
                        {
                            if (modifier.ToLower().Equals("public"))
                            {
                                return true;
                            }
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
                        if (node.AccessorList.ToString().Contains("set"))
                        {
                            return true;
                        }
                    }
                    foreach (var method in cls.Value.getMethods())
                    {
                        if (method.Parameters.Contains(property.ValueType.ToString()) && method.Body.Contains(property.Identifier))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
