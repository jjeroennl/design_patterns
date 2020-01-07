using System;
using System.Collections.Generic;
using System.Text;
using idetector.Collections;
using idetector.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace idetector.Patterns
{
    /*ID's:
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
        private ClassModel Interface;
        private MethodModel Setter;
        private List<RequirementResult> _results = new List<RequirementResult>();


        public StateStrategy(ClassCollection _cc, bool state)
        {
            cc = _cc;
            IsState = state;
            Concretes = new ClassCollection();

        }

        public void Scan()
        {
            _results.Add(HasInterfaceOrAbstract());
            _results.Add(HasConcreteClasses());
            if (IsState)
            {
                _results.Add(HasRelationsBetweenConcreteClasses());
            }
            _results.Add(ContextHasStrategy(Context));
            _results.Add(ContextHasPrivateStrategy(Context));
            _results.Add(ContextHasStrategySetter(Context));
            _results.Add(ContextHasPublicConstructor(Context));
            _results.Add(ContextHasLogic(Context));
        }

        public List<RequirementResult> GetResult()
        {
            return _results;
        }

        public int Score()
        {
            return (int) _score;
        }

        private RequirementResult ContextHasStrategy(ClassModel cls)
        {
            if (cls != null)
            {
                foreach (var property in cls.getProperties())
                {

                    if (cc.GetClass(property.ValueType.ToString()) != null)
                    {
                        if (cc.GetClass(property.ValueType.ToString()) == Interface)
                        {
                            if (cc.GetClass(property.ValueType.ToString()) == Interface)
                            {
                                return new RequirementResult("STATE-STRATEGY-CONTEXT-HAS-STRATEGY", true);
                            }
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
                    if (cc.GetClass(property.ValueType.ToString()) != null)
                    {
                        if (cc.GetClass(property.ValueType.ToString()) == Interface)
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
                        if (Interface != null)
                        {
                            if (property.ValueType.ToString() == Interface.Identifier)
                            {
                                if (property.ValueType.ToString() == Interface.Identifier)
                                {
                                    if (method.Parameters.Contains(property.ValueType.ToString()) &&
                                        method.Body.Contains(property.Identifier))
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
                            if (cc.GetClass(property.ValueType.ToString()) != null)
                            {
                                if (cc.GetClass(property.ValueType.ToString()) == Interface)
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

        public RequirementResult HasInterfaceOrAbstract()
        {
            foreach (var cls in cc.GetClasses())
            {
                if (cls.Value.IsInterface || cls.Value.IsAbstract)
                {
                    Interface = cls.Value;
                    return new RequirementResult("STATE-STRATEGY-INTERFACE-ABSTRACT",true);
                }
            }

            return new RequirementResult("STATE-STRATEGY-INTERFACE-ABSTRACT", false);
        }


        public RequirementResult HasConcreteClasses()
        {
            int i = 0;
            foreach (var cls in cc.GetClasses())
            {
                i += 1;
                foreach (string parent in cls.Value.GetParents())
                {
                    if (cc.GetClass(parent) == Interface) Concretes.AddClass(cls.Value);
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
                                    return new RequirementResult("STATE-CONCRETE-CLASS-RELATIONS", false);

                                }
                            }
                        }
                    }
                }
            }
            return new RequirementResult("STATE-CONCRETE-CLASS-RELATIONS", true);
        }
    }
}
