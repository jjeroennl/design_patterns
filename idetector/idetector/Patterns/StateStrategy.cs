using System;
using System.Collections.Generic;
using System.Text;
using idetector.Collections;
using idetector.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace idetector.Patterns
{
    public class StateStrategy : IPattern
    {
        private float _score;
        private Dictionary<string, int> _scores = new Dictionary<string, int>();
        private bool IsState = false;
        private ClassCollection cc;
        private ClassCollection concretes = new ClassCollection();
        public ClassModel context;
        private ClassModel interfacer;
        private MethodModel setter;

        /// <summary>
        /// Constructor for StateStrategy
        /// </summary>
        /// <param name="_cc">ClassCollection to check</param>
        /// <param name="state">Bool whether it should check for an state pattern</param>
        public StateStrategy(ClassCollection _cc, bool state)
        {
            cc = _cc;
            IsState = state;
            
            PriorityCollection.ClearPriorities();
            PriorityCollection.AddPriority("strategy", "ContextHasStrategy", Priority.High);
            PriorityCollection.AddPriority("strategy", "ContextHasStrategySetter", Priority.High);
            PriorityCollection.AddPriority("strategy", "ContextHasLogic", Priority.Medium);
            PriorityCollection.AddPriority("strategy", "ContextHasPrivateStrategy", Priority.Low);
            PriorityCollection.AddPriority("strategy", "ContextHasPublicConstructor", Priority.Low);
            PriorityCollection.AddPriority("strategy", "HasInterfaceOrAbstract", Priority.Medium);
            PriorityCollection.AddPriority("strategy", "HasConcreteClasses", Priority.Medium);
            if (IsState == false) PriorityCollection.AddPriority("strategy", "HasRelationsBetweenConcreteClasses", Priority.Low);
        }

        public void Scan()
        {
            _score = 0;

            if (HasInterfaceOrAbstract().isTrue)
            {
                _score += PriorityCollection.GetPercentage("strategy", "HasInterfaceOrAbstract");
            }
            if (HasConcreteClasses().isTrue)
            {
                _score += PriorityCollection.GetPercentage("strategy", "HasConcreteClasses");
                if (IsState == false && HasRelationsBetweenConcreteClasses().isTrue)
                {
                    _score += PriorityCollection.GetPercentage("strategy", "HasRelationsBetweenConcreteClasses");
                }
            }
            if (ContextChecks().isTrue)
            {
                if (ContextHasStrategy(context).isTrue)
                {
                    _score += PriorityCollection.GetPercentage("strategy", "ContextHasStrategy");

                    if (ContextHasPrivateStrategy(context).isTrue)
                    {
                        _score += PriorityCollection.GetPercentage("strategy", "ContextHasPrivateStrategy");
                    }
                    if (ContextHasStrategySetter(context).isTrue)
                    {
                        _score += PriorityCollection.GetPercentage("strategy", "ContextHasStrategySetter");
                    }
                }
                if (ContextHasPublicConstructor(context).isTrue)
                {
                    _score += PriorityCollection.GetPercentage("strategy", "ContextHasPublicConstructor");
                }
                if (ContextHasLogic(context).isTrue)
                {
                    _score += PriorityCollection.GetPercentage("strategy", "ContextHasLogic");
                }
            }
        }

        public int Score()
        {
            return (int) _score;
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
        /// Checking if there is a class which suffises as an 'Context' class
        /// </summary>
        /// <returns>CheckedMessage</returns>
        public CheckedMessage ContextChecks()
        {
            int score = 0;
            foreach (var cls in cc.GetClasses())
            {
                this._scores[cls.Value.Identifier] = 0;

                if (ContextHasStrategy(cls.Value).isTrue)
                {
                    score += 1;
                    if (ContextHasPrivateStrategy(cls.Value).isTrue) score += 1;
                    if (ContextHasPrivateStrategy(cls.Value).isTrue) this._scores[cls.Value.Identifier] += 1;
                }
                if (ContextHasPublicConstructor(cls.Value).isTrue) score += 1;
                if (ContextHasPublicConstructor(cls.Value).isTrue) this._scores[cls.Value.Identifier] += 1;

                if (ContextHasStrategySetter(cls.Value).isTrue) score += 1;
                if (ContextHasStrategySetter(cls.Value).isTrue) this._scores[cls.Value.Identifier] += 1;

                if (ContextHasLogic(cls.Value).isTrue) score += 1;
                if (ContextHasLogic(cls.Value).isTrue) this._scores[cls.Value.Identifier] += 1;


                if (score >= 3)
                {
                    context = cls.Value;
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
                    if (cc.GetClass(property.ValueType.ToString()) == interfacer) return new CheckedMessage(true);
                }
            }
            return new CheckedMessage("There is not an 'Context' class, which contains an strategy", false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cls">ClassModel to check</param>
        /// <returns>CheckedMessage</returns>
        public CheckedMessage ContextHasPrivateStrategy(ClassModel cls)
        {
            foreach (var property in cls.getProperties())
            {
                if (cc.GetClass(property.ValueType.ToString()) != null)
                {
                    if (cc.GetClass(property.ValueType.ToString()) == interfacer)
                    {
                        if (property.Modifiers.Length >= 1)
                        {
                            if (property.Modifiers[0].ToLower().Equals("private")) return new CheckedMessage(true);
                        }
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
        public CheckedMessage ContextHasPublicConstructor(ClassModel cls)
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
        public CheckedMessage ContextHasStrategySetter(ClassModel cls)
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
                        if (interfacer != null)
                        {
                            if (property.ValueType.ToString() == interfacer.Identifier)
                            {
                                if (method.Parameters.Contains(property.ValueType.ToString()) && method.Body.Contains(property.Identifier))
                                {
                                    setter = method;
                                    return new CheckedMessage(true);
                                }
                            }
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
        public CheckedMessage ContextHasLogic(ClassModel cls)
        {
            foreach (var method in cls.getMethods())
            {
                if (!method.isConstructor)
                {
                    foreach (var property in cls.getProperties())
                    {
                        if (cc.GetClass(property.ValueType.ToString()) != null)
                        {
                            if (cc.GetClass(property.ValueType.ToString()) == interfacer)
                            {
                                if (setter == null || method != setter)
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
        public CheckedMessage HasInterfaceOrAbstract()
        {
            foreach (var cls in cc.GetClasses())
            {
                if (cls.Value.IsInterface || cls.Value.IsAbstract)
                {
                    interfacer = cls.Value;
                    return new CheckedMessage(true);
                }
            }
            return new CheckedMessage("There is not an interface or abstract class for the strategies/patterns", false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>CheckedMessage</returns>
        public CheckedMessage HasConcreteClasses()
        {
            int i = 0;
            foreach (var cls in cc.GetClasses())
            {
                i += 1;
                foreach (string parent in cls.Value.GetParents())
                {
                    if (cc.GetClass(parent) == interfacer) concretes.AddClass(cls.Value);
                }
                if (cc.GetClasses().Count == i && concretes.GetClasses().Count >= 1) return new CheckedMessage(true);
            }
            return new CheckedMessage("There are no classes that implement the abstract class or interface", false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>CheckedMessage</returns>
        public CheckedMessage HasRelationsBetweenConcreteClasses()
        {
            foreach (var cls in concretes.GetClasses())
            {
                foreach (var method in cls.Value.getMethods())
                {
                    if (!method.isConstructor)
                    {
                        foreach (var cs in concretes.GetClasses())
                        {
                            if (cs.Value.Identifier != cls.Value.Identifier)
                            {
                                if (method.Body.Contains(cs.Value.Identifier)) return new CheckedMessage("There are relations between the strategies, which aren't allowed", false);
                            }
                        }
                    }
                }
            }
            return new CheckedMessage(true);
        }
    }
}
