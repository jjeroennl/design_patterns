using idetector.Collections;
using idetector.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace idetector.Patterns
{
    public class FactoryMethod : IPattern
    {
        private int _score;
        private ClassCollection cc;

        public FactoryMethod(ClassCollection _cc)
        {
            cc = _cc;
            PriorityCollection.ClearPriorities();
            PriorityCollection.AddPriority("factorymethod", "ContainsAbstractFactoryClass", Priority.Low);
            PriorityCollection.AddPriority("factorymethod", "ContainsProductInterface", Priority.Low);
            PriorityCollection.AddPriority("factorymethod", "ContainsAbstractProductInterfaceMethod", Priority.High);
            PriorityCollection.AddPriority("factorymethod", "IsInheritingAbstractFactoryClass", Priority.Low);
            PriorityCollection.AddPriority("factorymethod", "IsInheritingProductInterface", Priority.Low);
            PriorityCollection.AddPriority("factorymethod", "ConcreteFactoryIsReturningConcreteProduct", Priority.High);
        }

        public void Scan()
        {
            _score = 0;

            if (ContainsAbstractFactoryClass())
            {
                _score += PriorityCollection.GetPercentage("factorymethod", "ContainsAbstractFactoryClass");
            }
            if (ContainsProductInterface())
            {
                _score += PriorityCollection.GetPercentage("factorymethod", "ContainsProductInterface");
            }
            if (ContainsAbstractProductInterfaceMethod())
            {
                _score += PriorityCollection.GetPercentage("factorymethod", "ContainsAbstractProductInterfaceMethod");
            }
            if (IsInheritingAbstractFactoryClass())
            {
                _score += PriorityCollection.GetPercentage("factorymethod", "IsInheritingAbstractFactoryClass");
            }
            if (IsInheritingProductInterface())
            {
                _score += PriorityCollection.GetPercentage("factorymethod", "IsInheritingProductInterface");
            }
            if (ConcreteFactoryIsReturningConcreteProduct())
            {
                _score += PriorityCollection.GetPercentage("factorymethod", "ConcreteFactoryIsReturningConcreteProduct");
            }
        }

        public int Score()
        {
            return _score;
        }
        #region Lists
        public List<ClassModel> GetAbstractClasses()
        {
            List<ClassModel> abstractClasses = new List<ClassModel>();
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                if (cls.Value.IsAbstract)
                {
                    abstractClasses.Add(cls.Value);
                }
            }
            return abstractClasses;
        }

        public List<ClassModel> GetInterfaces()
        {
            List<ClassModel> interfaces = new List<ClassModel>();
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                if (cls.Value.IsInterface)
                {
                    interfaces.Add(cls.Value);
                }
            }
            return interfaces;
        }

        public List<ClassModel> GetAbstractProductInterfaceClasses()
        {
            List<ClassModel> classes = new List<ClassModel>();
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                foreach (var method in cls.Value.getMethods())
                {
                    foreach (var modifier in method.Modifiers)
                    {
                        if (modifier.ToLower().Equals("abstract"))
                        {
                            foreach (var @interface in GetInterfaces())
                            {
                                if (method.ReturnType == @interface.Identifier)
                                {
                                    classes.Add(cls.Value);
                                }
                            }
                        }
                    }
                }
            }
            return classes;
        }
        #endregion

        #region Checks
        public bool ContainsAbstractFactoryClass()
        {
            if (GetAbstractClasses().Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool ContainsProductInterface()
        {
            if (GetInterfaces().Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool ContainsAbstractProductInterfaceMethod()
        {
            if (GetAbstractProductInterfaceClasses().Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool IsInheritingAbstractFactoryClass()
        {
            if (GetAbstractClasses() != null)
            {
                foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
                {
                    foreach (var abstractClass in GetAbstractClasses())
                    {
                        if (cls.Value.HasParent(abstractClass.Identifier))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool IsInheritingProductInterface()
        {
            foreach (var @interface in GetInterfaces())
            {
                if (@interface != null)
                {
                    foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
                    {
                        if (cls.Value.HasParent(@interface.Identifier))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool ConcreteFactoryIsReturningConcreteProduct()
        {
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                foreach (var @class in GetAbstractProductInterfaceClasses())
                {
                    if (cls.Value.HasParent(@class.Identifier))
                    {
                        foreach (var method in cls.Value.getMethods())
                        {
                            foreach (var @interface in GetInterfaces())
                            {
                                if (method.ReturnType == @interface.Identifier)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        #endregion
    }
}
