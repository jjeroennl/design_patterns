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
        private float _score;
        private ClassCollection cc;

        /// <summary>
        /// Constructor for FactoryMethod.
        /// </summary>
        /// <param name="_cc">ClassCollection to check.</param>
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
            PriorityCollection.AddPriority("factorymethod", "ConcreteFactoryHasOneMethod", Priority.Low);
        }

        /// <summary>
        /// Scan method to calculate score based on check methods.
        /// </summary>
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
            if (ConcreteFactoriesHaveOneMethod())
            {
                _score += PriorityCollection.GetPercentage("factorymethod", "ConcreteFactoryHasOneMethod");
            }
        }

        /// <summary>
        /// Total score calculated by scan.
        /// </summary>
        /// <returns>Float of total score cast to integer.</returns>
        public int Score()
        {
            return (int) _score;
        }

        #region Lists
        /// <summary>
        /// Retrieve all abstract classes in ClassCollection.
        /// </summary>
        /// <returns>List of abstract classes as ClassModels.</returns>
        private List<ClassModel> GetAbstractClasses()
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

        /// <summary>
        /// Retrieve all interfaces in ClassCollection.
        /// </summary>
        /// <returns>List of interfaces as ClassModels.</returns>
        private List<ClassModel> GetInterfaces()
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

        /// <summary>
        /// Retrieve all classes with an abstract product interface method in them in ClassCollection.
        /// </summary>
        /// <returns>List of ClassModels.</returns>
        private List<ClassModel> GetAbstractProductInterfaceMethodClasses()
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
        /// <summary>
        /// Method that checks if there's any abstract (factory) classes present.
        /// </summary>
        /// <returns>Whether or not the check passes.</returns>
        public bool ContainsAbstractFactoryClass()
        {
            if (GetAbstractClasses().Count != 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Method that checks if there's any (product) interfaces present.
        /// </summary>
        /// <returns>Whether or not the check passes.</returns>
        public bool ContainsProductInterface()
        {
            if (GetInterfaces().Count != 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Method that checks if there's any classes present that have an abstract method with the return type of a product interface.
        /// </summary>
        /// <returns>Whether or not the check passes.</returns>
        public bool ContainsAbstractProductInterfaceMethod()
        {
            if (GetAbstractProductInterfaceMethodClasses().Count != 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Method that checks if there's any classes that inherit an abstract (factory) class.
        /// </summary>
        /// <returns>Whether or not the check passes.</returns>
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

        /// <summary>
        /// Method that checks if there's any classes that inherit a (product) interface.
        /// </summary>
        /// <returns>Whether or not the check passes.</returns>
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

        /// <summary>
        /// Method that checks if there's a concrete factory that returns a concrete product.
        /// </summary>
        /// <returns>Whether or not the check passes.</returns>
        public bool ConcreteFactoryIsReturningConcreteProduct()
        {
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                foreach (var @class in GetAbstractProductInterfaceMethodClasses())
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

        /// <summary>
        /// Method that checks if the concrete factories have just one method.
        /// </summary>
        /// <returns>Whether or not the check passes.</returns>
        public bool ConcreteFactoriesHaveOneMethod()
        {
            bool ret = false;
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                foreach (var @class in GetAbstractProductInterfaceMethodClasses())
                {
                    if (cls.Value.HasParent(@class.Identifier))
                    {
                        if (cls.Value.getMethods().Count != 1)
                        {
                            return false;
                        }
                        else
                        {
                            ret = true;
                        }
                    }
                }
            }

            return ret;
        }
        #endregion
    }
}
