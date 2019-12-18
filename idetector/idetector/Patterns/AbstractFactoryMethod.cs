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
    public class AbstractFactoryMethod : IPattern
    {
        private float _score;
        private Dictionary<ClassModel, int> _scores = new Dictionary<ClassModel, int>();
        private ClassCollection cc;
        private List<ClassModel> abstractClasses = new List<ClassModel>();
        private List<ClassModel> interfaces = new List<ClassModel>();
        private List<ClassModel> possibleFactoryClasses = new List<ClassModel>();
        private List<ClassModel> productInterfaces = new List<ClassModel>();

        /// <summary>
        /// Constructor for FactoryMethod.
        /// </summary>
        /// <param name="_cc">ClassCollection to check.</param>
        public AbstractFactoryMethod(ClassCollection _cc)
        {
            cc = _cc;
            
            PriorityCollection.ClearPriorities();
            PriorityCollection.AddPriority("factorymethod", "ContainsAbstractFactoryClass", Priority.Low);
            PriorityCollection.AddPriority("factorymethod", "ContainsProductInterface", Priority.Low);
            PriorityCollection.AddPriority("factorymethod", "ContainsAbstractProductInterfaceMethod", Priority.High);
            PriorityCollection.AddPriority("factorymethod", "IsInheritingAbstractFactoryClass", Priority.Low);
            PriorityCollection.AddPriority("factorymethod", "IsInheritingProductInterface", Priority.Low);
            PriorityCollection.AddPriority("factorymethod", "ConcreteFactoryIsReturningConcreteProduct", Priority.High);
            PriorityCollection.AddPriority("factorymethod", "ConcreteFactoriesHaveOneMethod", Priority.Low);
            PriorityCollection.AddPriority("factorymethod", "ConcreteProductsFollowOneProductInterface", Priority.Low);
        }

        /// <summary>
        /// Scan method to calculate score based on check methods.
        /// </summary>
        public void Scan()
        {
            SetAbstractClasses();
            SetInterfaces();
            SetPossibleFactoriesAndProductInterfaces();

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
                _score += PriorityCollection.GetPercentage("factorymethod", "ConcreteFactoriesHaveOneMethod");
            }
            if (ConcreteProductsFollowOneProductInterface().isTrue)
            {
                _score += PriorityCollection.GetPercentage("factorymethod", "ConcreteProductsFollowOneProductInterface");
            }

            foreach (var cls in possibleFactoryClasses)
            {
                _scores[cls] = (int)_score;
            }
            foreach (var acls in abstractClasses)
            {
                _scores[acls] = (int)_score;
            }
            foreach (var inter in interfaces)
            {
                _scores[inter] = (int)_score;
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

        /// <summary>
        /// Total score calculated by scan based on class.
        /// </summary>
        /// <param name="clsModel">ClassModel of class.</param>
        /// <returns></returns>
        public int Score(ClassModel clsModel)
        {
            if (this._scores.ContainsKey(clsModel)) return this._scores[clsModel];

            return 0;
        }

        #region Lists
        /// <summary>
        /// Method to set a list of abstract classes.
        /// </summary>
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

        /// <summary>
        /// Method to set a list of interfaces.
        /// </summary>
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

        /// <summary>
        /// Method to set a list of possible factories and a list of product interfaces.
        /// </summary>
        public void SetPossibleFactoriesAndProductInterfaces()
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
                                    possibleFactoryClasses.Add(cls.Value);
                                    productInterfaces.Add(cc.GetClass(method.ReturnType));
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
            if (possibleFactoryClasses.Count != 0)
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
            foreach (var @interface in productInterfaces)
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
                foreach (var @class in possibleFactoryClasses)
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
                foreach (var @class in possibleFactoryClasses)
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

        /// <summary>
        /// Method that checks if the concrete products follow just one product interface.
        /// </summary>
        /// <returns>Whether or not the check passes.</returns>
        public CheckedMessage ConcreteProductsFollowOneProductInterface()
        {
            if (productInterfaces.Count != 1)
            {
                ClassModel temp = null;
                foreach (var @interface in productInterfaces)
                {
                    if (@interface != null)
                    {
                        foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
                        {
                            if (cls.Value.HasParent(@interface.Identifier))
                            {
                                if (temp != null && temp != @interface)
                                {
                                    return new CheckedMessage(false);
                                }
                                else
                                {
                                    temp = @interface;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                return new CheckedMessage(true);
            }

            return new CheckedMessage(true);
        }
        #endregion
    }
}
