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
        private ClassModel Context;
        private MethodModel Setter;

        public Strategy(ClassCollection _cc)
        {
            cc = _cc;
            PriorityCollection.ClearPriorities();
            PriorityCollection.AddPriority("strategy", "ContextHasStrategy", Priority.Medium);
            PriorityCollection.AddPriority("strategy", "ContextHasPrivateStrategy", Priority.Low);
            PriorityCollection.AddPriority("strategy", "ContextHasPublicConstructor", Priority.Low);
            PriorityCollection.AddPriority("strategy", "ContextHasStrategySetter", Priority.Medium);
            PriorityCollection.AddPriority("strategy", "ContextHasLogic", Priority.Low);
            PriorityCollection.AddPriority("strategy", "HasInterface", Priority.Low);
            PriorityCollection.AddPriority("strategy", "HasConcreteClasses", Priority.Low);
            PriorityCollection.AddPriority("strategy", "HasNoRelationsBetweenConcreteClasses", Priority.Low);
        }

        public void Scan()
        {
            if (ContextChecks().isTrue)
            {
                if (ContextHasStrategy(Context).isTrue)
                {
                    _score += PriorityCollection.GetPercentage("strategy", "ContextHasStrategy");

                    if (ContextHasPrivateStrategy(Context).isTrue)
                    {
                        _score += PriorityCollection.GetPercentage("strategy", "ContextHasPrivateStrategy");
                    }
                }
                if (ContextHasPublicConstructor(Context).isTrue)
                {
                    _score += PriorityCollection.GetPercentage("strategy", "ContextHasPublicConstructor");
                }
                if (ContextHasStrategySetter(Context).isTrue)
                {
                    _score += PriorityCollection.GetPercentage("strategy", "ContextHasStrategySetter");
                }
                if (ContextHasLogic(Context).isTrue)
                {
                    _score += PriorityCollection.GetPercentage("strategy", "ContextHasLogic");
                }
            }
            if (HasInterface().isTrue)
            {
                _score += PriorityCollection.GetPercentage("strategy", "HasInterface");
            }
            if (HasConcreteClasses().isTrue)
            {
                _score += PriorityCollection.GetPercentage("strategy", "HasConcreteClasses");
            }
            if (HasNoRelationsBetweenConcreteClasses().isTrue)
            {
                _score += PriorityCollection.GetPercentage("strategy", "HasNoRelationsBetweenConcreteClasses");
            }
        }

        public int Score()
        {
            return _score;
        }

        /// <summary>
        /// Checking if there is a class which suffises as an 'Context' class
        /// </summary>
        /// <returns>CheckedMessage</returns>
        private CheckedMessage ContextChecks()
        {
            int score = 0;
            foreach (var cls in cc.GetClasses())
            {
                if (ContextHasStrategy(cls.Value).isTrue)
                {
                    score += 1;
                    if (ContextHasPrivateStrategy(cls.Value).isTrue) score += 1;
                }
                if (ContextHasPublicConstructor(cls.Value).isTrue) score += 1;
                if (ContextHasStrategySetter(cls.Value).isTrue) score += 1;
                if (ContextHasLogic(cls.Value).isTrue) score += 1;

                if (score >= 3)
                {
                    Context = cls.Value;
                    return new CheckedMessage(true);
                }
            }
            return new CheckedMessage("There is not an class which suffises as an 'Context' class", false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cls">ClassModel to check</param>
        /// <returns>CheckedMessage</returns>
        private CheckedMessage ContextHasStrategy(ClassModel cls)
        {
            foreach (var property in cls.getProperties())
            {
                if (cc.GetClass(property.ValueType.ToString()) != null)
                {
                    if (cc.GetClass(property.ValueType.ToString()).IsInterface) return new CheckedMessage(true);
                }
            }
            return new CheckedMessage("There is not an 'Context' class, which contains an strategy", false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cls">ClassModel to check</param>
        /// <returns>CheckedMessage</returns>
        private CheckedMessage ContextHasPrivateStrategy(ClassModel cls)
        {
            foreach (var property in cls.getProperties())
            {
                if (cc.GetClass(property.ValueType.ToString()) != null)
                {
                    if (cc.GetClass(property.ValueType.ToString()).IsInterface)
                    {
                        if (property.Modifiers[0].ToLower().Equals("private")) return new CheckedMessage(true);
                    }
                }
            }
            return new CheckedMessage("There is not an 'Context' class, which contains an private strategy, but an public one", false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cls">ClassModel to check</param>
        /// <returns>CheckedMessage</returns>
        private CheckedMessage ContextHasPublicConstructor(ClassModel cls)
        {
            foreach (var method in cls.getMethods())
            {
                if (method.isConstructor)
                {
                    foreach (var modifier in method.Modifiers)
                    {
                        if (modifier.ToLower().Equals("public")) return new CheckedMessage(true);
                    }
                }
            }
            return new CheckedMessage("There is not an public constructor in the 'Context' class", false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cls">ClassModel to check</param>
        /// <returns>CheckedMessage</returns>
        private CheckedMessage ContextHasStrategySetter(ClassModel cls)
        {
            foreach (var property in cls.getProperties())
            {
                if (property.Type.Equals(Models.Type.PropertySyntax))
                {
                    var node = (PropertyDeclarationSyntax)property.GetNode();
                    if (node.AccessorList.ToString().Contains("set")) return new CheckedMessage(true);
                }
                foreach (var method in cls.getMethods())
                {
                    if (!method.isConstructor)
                    {
                        if (method.Parameters.Contains(property.ValueType.ToString()) && method.Body.Contains(property.Identifier))
                        {
                            Setter = method;
                            return new CheckedMessage(true);
                        }
                    }
                }
            }
            return new CheckedMessage("There is not an setter for the strategy/state in the 'Context' class", false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cls">ClassModel to check</param>
        /// <returns>CheckedMessage</returns>
        private CheckedMessage ContextHasLogic(ClassModel cls)
        {
            foreach (var method in cls.getMethods())
            {
                if (!method.isConstructor)
                {
                    foreach (var property in cls.getProperties())
                    {
                        if (cc.GetClass(property.ValueType.ToString()) != null)
                        {
                            if (cc.GetClass(property.ValueType.ToString()).IsInterface)
                            {
                                if (Setter == null || method != Setter)
                                {
                                    if (method.Body.Contains(property.Identifier)) return new CheckedMessage(true);
                                }
                            }
                        }
                    }
                }
            }
            return new CheckedMessage("The 'Context' class doesn't contain any logic to call the strategies/states", false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>CheckedMessage</returns>
        private CheckedMessage HasInterface()
        {
            foreach (var cls in cc.GetClasses())
            {
                if(cls.Value.IsInterface) return new CheckedMessage(true);
            }
            return new CheckedMessage("There is not an interface or abstract class for the strategies/patterns", false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>CheckedMessage</returns>
        private CheckedMessage HasConcreteClasses()
        {
            foreach (var cls in cc.GetClasses())
            {
                foreach (string parent in cls.Value.GetParents())
                {
                    if (cc.GetClass(parent).IsInterface) return new CheckedMessage(true);
                }
            }
            return new CheckedMessage("There are no classes that implement the abstract class or interface", false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>CheckedMessage</returns>
        private CheckedMessage HasNoRelationsBetweenConcreteClasses()
        {
            foreach (var cls in cc.GetClasses())
            {
                return new CheckedMessage(true);
            }
            return new CheckedMessage(false);
        }
    }
}
