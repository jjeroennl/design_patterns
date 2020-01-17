using System;
using System.Collections.Generic;
using System.Text;
using idetector.Collections;
using idetector.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using idetector.Patterns.Helper;
using System.Linq;

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
        private bool IsState;
        private ClassCollection cc;
        private Dictionary<string, List<RequirementResult>> _results = new Dictionary<string, List<RequirementResult>>();

        private ClassCollection Concretes = new ClassCollection();
        public ClassModel Context;
        private List<ClassModel> interfaces;
        private MethodModel Setter;

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

        public Dictionary<string, List<RequirementResult>> GetResults()
        {
            return _results;
        }

        private void HasInterfaceOrAbstract()
        {
            foreach (ClassModel cls in cc.GetClasses().Values)
            {
                if (API.ClassIsAbstractOrInterface(cls))
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
            int score, oldScore = 0;
            int i = 100 / 5;
            bool a2 = false, b2 = false, c2 = false, d2 = false, e2 = false;

            foreach (var cls in cc.GetClasses().Values)
            {
                score = 0;
                bool a = false, b = false, c = false, d = false, e = false;
                if (ContextHasStrategy(cls))
                {
                    a = true;
                    score += i;
                    if (ContextHasPrivateStrategy(cls))
                    {
                        b = true;
                        score += i;
                    }
                }
                if (ContextHasPublicConstructor(cls))
                {
                    c = true;
                    score += i;
                }
                if (ContextHasStrategySetter(cls))
                {
                    d = true;
                    score += i;
                }
                if (ContextHasLogic(cls))
                {
                    e = true;
                    score += i;
                }

                if (score >= 50 && score > oldScore)
                {
                    a2 = a;
                    b2 = b;
                    c2 = c;
                    d2 = d;
                    e2 = e;
                    oldScore = score;
                    Context = cls;
                }
            }
            if (Context != null)
            {
                foreach (ClassModel @interface in interfaces)
                {
                    bool addStrategy = true, addPrivate = true, addPublic = true, addSetter = true, addLogic = true;
                    foreach (var result in _results[@interface.Identifier].ToArray())
                    {
                        if (result.Id.Equals("STATE-STRATEGY-CONTEXT-HAS-STRATEGY")) addStrategy = false;
                        if (result.Id.Equals("STATE-STRATEGY-CONTEXT-PRIVATE-STRATEGY")) addPrivate = false;
                        if (result.Id.Equals("STATE-STRATEGY-CONTEXT-PUBLIC-CONSTRUCTOR")) addPublic = false;
                        if (result.Id.Equals("STATE-STRATEGY-CONTEXT-STRATEGY-SETTER")) addSetter = false;
                        if (result.Id.Equals("STATE-STRATEGY-CONTEXT-LOGIC")) addLogic = false;
                    }
                    if (addStrategy && a2) _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONTEXT-HAS-STRATEGY", true, Context));
                    else _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONTEXT-HAS-STRATEGY", false, Context));

                    if (addPrivate && b2) _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONTEXT-PRIVATE-STRATEGY", true, Context));
                    else _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONTEXT-PRIVATE-STRATEGY", false, Context));

                    if (addPublic && c2) _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONTEXT-PUBLIC-CONSTRUCTOR", true, Context));
                    else _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONTEXT-PUBLIC-CONSTRUCTOR", false, Context));

                    if (addSetter && d2) _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONTEXT-STRATEGY-SETTER", true, Context));
                    else _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONTEXT-STRATEGY-SETTER", false, Context));

                    if (addLogic && e2) _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONTEXT-LOGIC", true, Context));
                    else _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONTEXT-LOGIC", false, Context));

                    _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-HAS-CONTEXT", true, Context));
                }
            }
            else
            {
                foreach (ClassModel @interface in interfaces)
                {
                    bool add = true;
                    foreach (var result in _results[@interface.Identifier].ToArray())
                    {
                        if (result.Id.Equals("STATE-STRATEGY-HAS-CONTEXT")) add = false;
                    }
                    foreach (ClassModel cls in cc.GetClasses().Values)
                    {
                        _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-HAS-CONTEXT", false, cls));
                        break;
                    }
                }
            }
        }
        private bool ContextHasStrategy(ClassModel cls)
        {
            if (cls != null)
            {
                foreach (ClassModel @interface in interfaces)
                {
                    if (API.ClassHasPropertyOfType(cls, @interface.Identifier))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool ContextHasPrivateStrategy(ClassModel cls)
        {
            if (API.ClassHasPropertyOfType(cls, null, new[] { "private" }))
            {
                return true;
            }
            return false;
        }


        private bool ContextHasPublicConstructor(ClassModel cls)
        {
            if (cls != null)
            {
                if (API.ClassHasConstructorOfType(cls, null, new[] { "public" }))
                {
                    foreach (ClassModel @interface in interfaces)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool ContextHasStrategySetter(ClassModel cls)
        {
            if (cls != null)
            {
                if (API.ClassHasPropertySyntaxSetter(cls, Models.Type.PropertySyntax.ToString()))
                {
                    foreach (ClassModel @interface in interfaces)
                    {
                        return true;
                    }
                }
                else
                {
                    foreach (var property in cls.GetProperties())
                    {
                        foreach (ClassModel @interface in interfaces)
                        {
                            if (property.ValueType.ToString() == @interface.Identifier)
                            {
                                foreach (var method in cls.GetMethods())
                                {
                                    if (method.Parameters.Contains(property.ValueType.ToString()) &&
                                        method.Body.Contains(property.Identifier))
                                    {
                                        if (!method.isConstructor)
                                        {
                                            Setter = method;
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool ContextHasLogic(ClassModel cls)
        {
            if (cls != null)
            {
                foreach (var method in cls.GetMethods())
                {
                    if (!method.isConstructor)
                    {
                        foreach (var property in cls.GetProperties())
                        {
                            foreach (ClassModel @interface in interfaces)
                            {
                                bool add = true;
                                if (cc.GetClass(property.ValueType.ToString()) == @interface)
                                {
                                    if (Setter == null || method != Setter)
                                    {
                                        if (method.Body.Contains(property.Identifier))
                                        {
                                            return true;
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
            return false;
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
                            bool add = true;
                            foreach (var result in _results[@interface.Identifier].ToArray())
                            {
                                if (result.Id.Equals("STATE-STRATEGY-CONCRETE-CLASS")) add = false;
                            }
                            if (add) _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONCRETE-CLASS", true, cls));
                        }
                    }
                }
            }
            foreach (var cls in cc.GetClasses().Values)
            {
                foreach (ClassModel @interface in interfaces)
                {
                    bool add = true;
                    foreach (var result in _results[@interface.Identifier].ToArray())
                    {
                        if (result.Id.Equals("STATE-STRATEGY-CONCRETE-CLASS")) add = false;
                    }
                    if (add) _results[@interface.Identifier].Add(new RequirementResult("STATE-STRATEGY-CONCRETE-CLASS", false, cls));
                }
            }
        }

        private void HasRelationsBetweenConcreteClasses()
        {
            foreach (var cls in Concretes.GetClasses().Values)
            {
                foreach (var method in cls.GetMethods())
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
                                        bool add = true;
                                        foreach (var result in _results[@interface.Identifier].ToArray())
                                        {
                                            if (result.Id.Equals("STRATEGY-CONCRETE-CLASS-RELATIONS")) add = false;
                                        }
                                        if (add) _results[@interface.Identifier].Add(new RequirementResult("STRATEGY-CONCRETE-CLASS-RELATIONS", false, cls));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            foreach (var cls in cc.GetClasses().Values)
            {
                foreach (ClassModel @interface in interfaces)
                {
                    bool add = true;
                    foreach (var result in _results[@interface.Identifier].ToArray())
                    {
                        if (result.Id.Equals("STRATEGY-CONCRETE-CLASS-RELATIONS")) add = false;
                    }
                    if (add) _results[@interface.Identifier].Add(new RequirementResult("STRATEGY-CONCRETE-CLASS-RELATIONS", true, cls));
                }
            }
        }
    }
}
