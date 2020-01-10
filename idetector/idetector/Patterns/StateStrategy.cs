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
        private Dictionary<string, List<RequirementResult>> _results = new Dictionary<string, List<RequirementResult>>();


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
            ContextChecks();
            HasConcreteClasses();
            if (!IsState)
            {
                HasRelationsBetweenConcreteClasses();
            }
        }

        public bool GetResult(string Iidentifier, string req)
        {
            foreach (ClassModel cls in cc.GetClasses().Values)
            {
                if (_results[Iidentifier].Contains(new RequirementResult(req, true, cls)))
                {
                    return true;
                }
            }
            return false;
        }

        public Dictionary<string, List<RequirementResult>> GetResults()
        {
            return _results;
        }

        public int Score()
        {
            return (int) _score;
        }

        private void HasInterfaceOrAbstract()
        {
            foreach (var cls in cc.GetClasses().Values)
            {
                if (cls.IsInterface || cls.IsAbstract)
                {
                    interfaces.Add(cls);
                    _results.Add(cls.Identifier, new List<RequirementResult>());
                    _results[cls.Identifier].Add(new RequirementResult("STATE-STRATEGY-INTERFACE-ABSTRACT", true, cls));
                }
                else
                {
                    _results.Add(cls.Identifier, new List<RequirementResult>());
                    _results[cls.Identifier].Add(new RequirementResult("STATE-STRATEGY-INTERFACE-ABSTRACT", false, cls));
                }
            }
        }

        /// <summary>
        /// Checking if there is a class which suffises as an 'Context' class
        /// </summary>
        private void ContextChecks()
        {
            int score = 0;
            int i = 100 / 5;

            foreach (var cls in cc.GetClasses().Values)
            {
                ContextHasStrategy(cls);
                ContextHasPublicConstructor(cls);
                ContextHasStrategySetter(cls);
                ContextHasLogic(cls);
                
                if (score >= 50)
                {
                    Context = cls;

                    foreach (ClassModel @interface in interfaces)
                    {
                        _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-HAS-CONTEXT", true, Context));
                    }
                }
                else
                {
                    foreach (ClassModel @interface in interfaces)
                    {
                        _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-HAS-CONTEXT", false, cls));
                    }
                }
            }
        }
        private void ContextHasStrategy(ClassModel cls)
        {
            if (cls != null)
            {
                foreach (var property in cls.getProperties())
                {
                    foreach (ClassModel @interface in interfaces)
                    {
                        if (cc.GetClass(property.ValueType.ToString()) == @interface)
                        {
                            _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONTEXT-HAS-STRATEGY", true, cls));

                            ContextHasPrivateStrategy(cls, @interface, property);
                        }
                    }
                }
            }
            foreach (ClassModel @interface in interfaces)
            {
                if (!_results[@interface.Identifier].Contains(new RequirementResult("STATE-STRATEGY-CONTEXT-HAS-STRATEGY", true, cls)))
                {
                    _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONTEXT-HAS-STRATEGY", false, cls));
                    _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONTEXT-PRIVATE-STRATEGY", false, cls));
                }
            }
        }

        private void ContextHasPrivateStrategy(ClassModel cls, ClassModel @interface ,PropertyModel property)
        {
            if (property.Modifiers.Length > 0)
            {
                if (property.Modifiers[0].ToLower().Equals("private"))
                {
                    _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONTEXT-PRIVATE-STRATEGY", true, cls));
                }
            }
            if (!_results[@interface.Identifier].Contains(new RequirementResult("STATE-STRATEGY-CONTEXT-PRIVATE-STRATEGY", true, cls)))
            {
                _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONTEXT-PRIVATE-STRATEGY", false, cls));
            }
        }


        private void ContextHasPublicConstructor(ClassModel cls)
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
                                foreach (ClassModel @interface in interfaces)
                                {
                                    _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONTEXT-PUBLIC-CONSTRUCTOR", true, cls));
                                }
                            }
                        }
                    }
                }
            }
            foreach (ClassModel @interface in interfaces)
            {
                if (!_results[@interface.Identifier].Contains(new RequirementResult("STATE-STRATEGY-CONTEXT-PUBLIC-CONSTRUCTOR", true, cls)))
                {
                    _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONTEXT-PUBLIC-CONSTRUCTOR", false, cls));
                }
            }
        }

        private void ContextHasStrategySetter(ClassModel cls)
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
                            foreach (ClassModel @interface in interfaces)
                            {
                                _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONTEXT-STRATEGY-SETTER", true, cls));
                            }
                        }
                    }

                    foreach (ClassModel @interface in interfaces)
                    {
                        if (property.ValueType.ToString() == @interface.Identifier)
                        {
                            foreach (var method in cls.getMethods())
                            {
                                if (method.Parameters.Contains(property.ValueType.ToString()) &&
                                    method.Body.Contains(property.Identifier))
                                {
                                    if (!method.isConstructor)
                                    {
                                        Setter = method;
                                        _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONTEXT-STRATEGY-SETTER", true, cls));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            foreach (ClassModel @interface in interfaces)
            {
                if (!_results[@interface.Identifier].Contains(new RequirementResult("STATE-STRATEGY-CONTEXT-STRATEGY-SETTER", true, cls)))
                {
                    _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONTEXT-STRATEGY-SETTER", false, cls));
                }
            }
        }

        private void ContextHasLogic(ClassModel cls)
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
                                            _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONTEXT-LOGIC", true, cls));
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
            foreach (ClassModel @interface in interfaces)
            {
                if (!_results[@interface.Identifier].Contains(new RequirementResult("STATE-STRATEGY-CONTEXT-LOGIC", true, cls)))
                {
                    _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONTEXT-LOGIC", false, cls));
                }
            }
        }

        private void HasConcreteClasses()
        {
            foreach (var cls in cc.GetClasses().Values)
            {
                foreach (string parent in cls.GetParents())
                {
                    foreach (ClassModel @interface in interfaces)
                    {
                        if (cc.GetClass(parent) == @interface)
                        {
                            Concretes.AddClass(cls);
                            _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONCRETE-CLASS", true, cls));
                        }
                    }
                }
                foreach (ClassModel @interface in interfaces)
                {
                    if (!_results[@interface.Identifier].Contains(new RequirementResult("STATE-STRATEGY-CONCRETE-CLASS", true, cls)))
                    {
                        _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONCRETE-CLASS", false, cls));
                    }
                }
            }
        }

        private void HasRelationsBetweenConcreteClasses()
        {
            foreach (var cls in Concretes.GetClasses().Values)
            {
                foreach (var method in cls.getMethods())
                {
                    if (!method.isConstructor)
                    {
                        foreach (var cs in Concretes.GetClasses().Values)
                        {
                            if (cs.Identifier != cls.Identifier)
                            {
                                if (method.Body.Contains(cs.Identifier))
                                {
                                    foreach (ClassModel @interface in interfaces)
                                    {
                                        _results[@interface.Identifier].Add(new RequirementResult("STRATEGY-CONCRETE-CLASS-RELATIONS", false, cls));
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (ClassModel @interface in interfaces)
                {
                    if (!_results[@interface.Identifier].Contains(new RequirementResult("STRATEGY-CONCRETE-CLASS-RELATIONS", true, cls)))
                    {
                        _results[@interface.Identifier].Add(new RequirementResult("STRATEGY-CONCRETE-CLASS-RELATIONS", false, cls));
                    }
                }
            }
        }
    }
}
