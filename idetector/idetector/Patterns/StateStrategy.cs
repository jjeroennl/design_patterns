using System;
using System.Collections.Generic;
using System.Text;
using idetector.Collections;
using idetector.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace idetector.Patterns
{
    /*ID's:
     * STATE-STRATEGY-HAS-CONTEXT
     * STATE-STRATEGY-CONTEXT-HAS-STRATEGY
     * STATE-STRATEGY-CONTEXT-PRIVATE-STRATEGY
     * STATE-STRATEGY-CONTEXT-PUBLIC-CONSTRUCTOR
     * STATE-STRATEGY-CONTEXT-STRATEGY-SETTER
     * STATE-STRATEGY-CONTEXT-LOGIC
     * STATE-STRATEGY-INTERFACE-ABSTRACT
     * STATE-STRATEGY-CONCRETE-CLASS
     * STATE-CONCRETE-CLASS-RELATIONS
     */
    public class StateStrategy : IPattern
    {
        private float _score;
        private Dictionary<string, int> _scores = new Dictionary<string, int>();
        private bool IsState = false;
        private ClassCollection cc;

        private ClassCollection Concretes = new ClassCollection();
        public ClassModel Context;
        private List<ClassModel> interfaces;
        private MethodModel Setter;
        private  Dictionary<string, RequirementResult> _results = new  Dictionary<string, RequirementResult>();


        public StateStrategy(ClassCollection _cc, bool isState)
        {
            cc = _cc;
            IsState = isState;
            Concretes = new ClassCollection();
            interfaces = new List<ClassModel>();

        }

        public void Scan()
        {
            HasInterfaceOrAbstract();
            _results.Add(ContextChecks());
            _results.Add(HasConcreteClasses());
            if (!IsState)
            {
                _results.Add(HasRelationsBetweenConcreteClasses());
            }
            _results.Add(ContextHasStrategy(Context));
            _results.Add(ContextHasPrivateStrategy(Context));
            _results.Add(ContextHasStrategySetter(Context));
            _results.Add(ContextHasPublicConstructor(Context));
            _results.Add(ContextHasLogic(Context));
        }

        public Dictionary<string, RequirementResult> GetResult()
        {
            return _results;
        }

        public int Score()
        {
            return (int) _score;
        }

        /// <summary>
        /// Checking if there is a class which suffises as an 'Context' class
        /// </summary>
        /// <returns>CheckedMessage</returns>
        public RequirementResult ContextChecks()
        {
            int score = 0;
            int i = 100 / 5;

            foreach (var cls in cc.GetClasses())
            {
                if (ContextHasStrategy(cls.Value).Passed)
                {
                    score += i;
                    if (ContextHasPrivateStrategy(cls.Value).Passed) score += i;
                }
                if (ContextHasPublicConstructor(cls.Value).Passed) score += i;

                if (ContextHasStrategySetter(cls.Value).Passed) score += i;

                if (ContextHasLogic(cls.Value).Passed) score += i;

                if (score >= 50)
                {
                    Context = cls.Value;
                    return new RequirementResult("STATE-STRATEGY-HAS-CONTEXT", true);
                }
            }
            return new RequirementResult("STATE-STRATEGY-HAS-CONTEXT", false);
        }
        private RequirementResult ContextHasStrategy(ClassModel cls)
        {
            if (cls != null)
            {
                foreach (var property in cls.getProperties())
                {
                    foreach (ClassModel @interface in interfaces)
                    {
                        if (cc.GetClass(property.ValueType.ToString()) == @interface)
                        {
                            return new RequirementResult("STATE-STRATEGY-CONTEXT-HAS-STRATEGY", true);
                        }

                    }

                }
            }

            return new RequirementResult("STATE-STRATEGY-CONTEXT-HAS-STRATEGY", false);
          }

        public RequirementResult ContextHasPrivateStrategy(ClassModel cls)
        {
            if (cls != null)
            {
                foreach (var property in cls.getProperties())
                {
                    foreach (ClassModel @interface in interfaces)
                    {
                        if (cc.GetClass(property.ValueType.ToString()) == @interface)
                        {
                            if (property.Modifiers.Length >= 1)
                            {
                                if (property.Modifiers[0].ToLower().Equals("private"))
                                {
                                    return new RequirementResult("STATE-STRATEGY-CONTEXT-PRIVATE-STRATEGY", true);
                                }
                            }
                        }
                    }
                }
            }

            return new RequirementResult("STATE-STRATEGY-CONTEXT-PRIVATE-STRATEGY", false);
        }


        public RequirementResult ContextHasPublicConstructor(ClassModel cls)
        {
            if (cls != null)
            {
                foreach (var method in cls.getMethods())
                {
                    if (method.isConstructor)
                    {
                        foreach (var modifier in method.Modifiers)
                        {
                            if (modifier.ToLower().Equals("public"))
                            {
                                return new RequirementResult("STATE-STRATEGY-CONTEXT-PUBLIC-CONSTRUCTOR", true);
                            }
                        }
                    }
                }
            }

            return new RequirementResult("STATE-STRATEGY-CONTEXT-PUBLIC-CONSTRUCTOR", false);
        }


        public RequirementResult ContextHasStrategySetter(ClassModel cls)
        {
            if (cls != null)
            {
                foreach (var property in cls.getProperties())
                {
                    if (property.Type.Equals(Models.Type.PropertySyntax))
                    {
                        var node = (PropertyDeclarationSyntax)property.GetNode();
                        if (node.AccessorList.ToString().Contains("set"))
                        {
                            return new RequirementResult("STATE-STRATEGY-CONTEXT-STRATEGY-SETTER", true);
                        }
                    }

                    foreach (var method in cls.getMethods())
                    {
                        foreach (ClassModel @interface in interfaces)
                        {
                            if (property.ValueType.ToString() == @interface.Identifier)
                            {
                                if (method.Parameters.Contains(property.ValueType.ToString()) &&
                                    method.Body.Contains(property.Identifier))
                                {
                                    if (!method.isConstructor)
                                    {
                                        Setter = method;
                                        return new RequirementResult("STATE-STRATEGY-CONTEXT-STRATEGY-SETTER", true);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return  new RequirementResult("STATE-STRATEGY-CONTEXT-STRATEGY-SETTER", false);
        }

        public RequirementResult ContextHasLogic(ClassModel cls)
        {
            if (cls != null)
            {
                foreach (var method in cls.getMethods())
                {
                    if (!method.isConstructor)
                    {
                        foreach (var property in cls.getProperties())
                        {
                            foreach(ClassModel @interface in interfaces)
                            {
                                if (cc.GetClass(property.ValueType.ToString()) == @interface)
                                {
                                    if (Setter == null || method != Setter)
                                    {
                                        if (method.Body.Contains(property.Identifier))
                                        {
                                            return new RequirementResult("STATE-STRATEGY-CONTEXT-LOGIC", true);
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
            return new RequirementResult("STATE-STRATEGY-CONTEXT-LOGIC", false);
        }

        public void HasInterfaceOrAbstract()
        {
            foreach (var cls in cc.GetClasses())
            {
                if (cls.Value.IsInterface || cls.Value.IsAbstract)
                {
                    interfaces.Add(cls.Value);
                    _results.Add(cls.Value.Identifier, new RequirementResult("STATE-STRATEGY-INTERFACE-ABSTRACT", true));
                }
                else
                {
                    _results.Add(cls.Value.Identifier, new RequirementResult("STATE-STRATEGY-INTERFACE-ABSTRACT", false));
                }
            }
        }


        public RequirementResult HasConcreteClasses()
        {
            int i = 0;
            foreach (var cls in cc.GetClasses())
            {
                i += 1;
                foreach (string parent in cls.Value.GetParents())
                {
                    foreach (ClassModel @interface in interfaces)
                    if (cc.GetClass(parent) == @interface) Concretes.AddClass(cls.Value);
                }

                if (cc.GetClasses().Count == i && Concretes.GetClasses().Count >= 1)
                {
                    return new RequirementResult( "STATE-STRATEGY-CONCRETE-CLASS",true);
                }
            }

            return new RequirementResult("STATE-STRATEGY-CONCRETE-CLASS", false);
        }

        public RequirementResult HasRelationsBetweenConcreteClasses()
        {
            foreach (var cls in Concretes.GetClasses())
            {
                foreach (var method in cls.Value.getMethods())
                {
                    if (!method.isConstructor)
                    {
                        foreach (var cs in Concretes.GetClasses())
                        {
                            if (cs.Value.Identifier != cls.Value.Identifier)
                            {
                                if (method.Body.Contains(cs.Value.Identifier))
                                {
                                    return new RequirementResult("STRATEGY-CONCRETE-CLASS-RELATIONS", false);

                                }
                            }
                        }
                    }
                }
            }
            return new RequirementResult("STRATEGY-CONCRETE-CLASS-RELATIONS", true);
        }
    }
}
