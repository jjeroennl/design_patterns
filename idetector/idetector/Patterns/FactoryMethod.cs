﻿using idetector.Collections;
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
        private List<ClassModel> abstractClasses = new List<ClassModel>();
        private List<ClassModel> interfaces = new List<ClassModel>();
        private List<ClassModel> classes = new List<ClassModel>();

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
            SetAbstractClasses();
            SetInterfaces();
            SetAbstractProductInterfaceClasses();
            _score = 0;

            if (ContainsAbstractFactoryClass().isTrue)
            {
                _score += PriorityCollection.GetPercentage("factorymethod", "ContainsAbstractFactoryClass");
            }
            if (ContainsProductInterface().isTrue)
            {
                _score += PriorityCollection.GetPercentage("factorymethod", "ContainsProductInterface");
            }
            if (ContainsAbstractProductInterfaceMethod().isTrue)
            {
                _score += PriorityCollection.GetPercentage("factorymethod", "ContainsAbstractProductInterfaceMethod");
            }
            if (IsInheritingAbstractFactoryClass().isTrue)
            {
                _score += PriorityCollection.GetPercentage("factorymethod", "IsInheritingAbstractFactoryClass");
            }
            if (IsInheritingProductInterface().isTrue)
            {
                _score += PriorityCollection.GetPercentage("factorymethod", "IsInheritingProductInterface");
            }
            if (ConcreteFactoryIsReturningConcreteProduct().isTrue)
            {
                _score += PriorityCollection.GetPercentage("factorymethod", "ConcreteFactoryIsReturningConcreteProduct");
            }
            if (ConcreteFactoriesHaveOneMethod().isTrue)
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
        private void SetAbstractClasses()
        {
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                if (cls.Value.IsAbstract)
                {
                    abstractClasses.Add(cls.Value);
                }
            }
        }

        private void SetInterfaces()
        {
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                if (cls.Value.IsInterface)
                {
                    interfaces.Add(cls.Value);
                }
            }
        }

        public void SetAbstractProductInterfaceClasses()
        {
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                foreach (var method in cls.Value.getMethods())
                {
                    foreach (var modifier in method.Modifiers)
                    {
                        if (modifier.ToLower().Equals("abstract"))
                        {
                            foreach (var @interface in interfaces)
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
        }
        #endregion

        #region Checks
        /// <summary>
        /// Method that checks if there's any abstract (factory) classes present.
        /// </summary>
        /// <returns>Whether or not the check passes.</returns>
        public CheckedMessage ContainsAbstractFactoryClass()
        {
            if (abstractClasses.Count != 0)
            {
                return new CheckedMessage(true);
            }
            return new CheckedMessage(false);
        }

        /// <summary>
        /// Method that checks if there's any (product) interfaces present.
        /// </summary>
        /// <returns>Whether or not the check passes.</returns>
        public CheckedMessage ContainsProductInterface()
        {
            if (interfaces.Count != 0)
            {
                return new CheckedMessage(true);
            }
            return new CheckedMessage(false);
        }

        /// <summary>
        /// Method that checks if there's any classes present that have an abstract method with the return type of a product interface.
        /// </summary>
        /// <returns>Whether or not the check passes.</returns>
        public CheckedMessage ContainsAbstractProductInterfaceMethod()
        {
            if (classes.Count != 0)
            {
                return new CheckedMessage(true);
            }
            return new CheckedMessage(false);
        }

        /// <summary>
        /// Method that checks if there's any classes that inherit an abstract (factory) class.
        /// </summary>
        /// <returns>Whether or not the check passes.</returns>
        public CheckedMessage IsInheritingAbstractFactoryClass()
        {
            if (abstractClasses != null)
            {
                foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
                {
                    foreach (var abstractClass in abstractClasses)
                    {
                        if (cls.Value.HasParent(abstractClass.Identifier))
                        {
                            return new CheckedMessage(true);
                        }
                    }
                }
            }
            return new CheckedMessage(false);
        }

        /// <summary>
        /// Method that checks if there's any classes that inherit a (product) interface.
        /// </summary>
        /// <returns>Whether or not the check passes.</returns>
        public CheckedMessage IsInheritingProductInterface()
        {
            foreach (var @interface in interfaces)
            {
                if (@interface != null)
                {
                    foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
                    {
                        if (cls.Value.HasParent(@interface.Identifier))
                        {
                            return new CheckedMessage(true);
                        }
                    }
                }
            }
            return new CheckedMessage(false);
        }

        /// <summary>
        /// Method that checks if there's a concrete factory that returns a concrete product.
        /// </summary>
        /// <returns>Whether or not the check passes.</returns>
        public CheckedMessage ConcreteFactoryIsReturningConcreteProduct()
        {
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                foreach (var @class in classes)
                {
                    if (cls.Value.HasParent(@class.Identifier))
                    {
                        foreach (var method in cls.Value.getMethods())
                        {
                            foreach (var @interface in interfaces)
                            {
                                if (method.ReturnType == @interface.Identifier)
                                {
                                    return new CheckedMessage(true);
                                }
                            }
                        }
                    }
                }
            }
            return new CheckedMessage(false);
        }

        /// <summary>
        /// Method that checks if the concrete factories have just one method.
        /// </summary>
        /// <returns>Whether or not the check passes.</returns>
        public CheckedMessage ConcreteFactoriesHaveOneMethod()
        {
            bool ret = false;
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                foreach (var @class in classes)
                {
                    if (cls.Value.HasParent(@class.Identifier))
                    {
                        if (cls.Value.getMethods().Count != 1)
                        {
                            return new CheckedMessage(false);
                        }
                        else
                        {
                            ret = true;
                        }
                    }
                }
            }
            return new CheckedMessage(ret);
        }
        #endregion
    }
}
