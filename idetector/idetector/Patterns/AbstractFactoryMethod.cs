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
        private bool isMethod;
        private ClassModel ifactory;

        private Dictionary<ClassModel, int> _scores = new Dictionary<ClassModel, int>();
        private ClassCollection cc;
        private List<ClassModel> abstractClasses = new List<ClassModel>();
        private List<ClassModel> interfaces = new List<ClassModel>();
        private List<ClassModel> parents = new List<ClassModel>();
        private List<ClassModel> possibleFactoryClasses = new List<ClassModel>();
        private List<ClassModel> productInterfaces = new List<ClassModel>();

        /// <summary>
        /// Constructor for FactoryMethod.
        /// </summary>
        /// <param name="_cc">ClassCollection to check.</param>
        public AbstractFactoryMethod(ClassCollection _cc, bool ismthd)
        {
            cc = _cc;
            isMethod = ismthd;
            ifactory = null;

            PriorityCollection.ClearPriorities();
            PriorityCollection.AddPriority("factory", "ContainsIFactoryClass", Priority.Low);
            PriorityCollection.AddPriority("factory", "ContainsProductInterface", Priority.Low);
            PriorityCollection.AddPriority("factory", "ContainsAbstractProductInterfaceMethod", Priority.High);
            PriorityCollection.AddPriority("factory", "IsInheritingFactoryClass", Priority.Low);
            PriorityCollection.AddPriority("factory", "IsInheritingProductInterface", Priority.Low);
            PriorityCollection.AddPriority("factory", "ConcreteFactoryIsReturningConcreteProduct", Priority.High);
            PriorityCollection.AddPriority("factory", "ConcreteProductsFollowOneProductInterface", Priority.Low);
            PriorityCollection.AddPriority("factory", "HasMultipleMethods", Priority.Low);
        }

        /// <summary>
        /// Scan method to calculate score based on check methods.
        /// </summary>
        public void Scan()
        {
            SetAbstractClasses();
            SetInterfaces();
            SetParents();
            SetIFactoryClass();
            SetPossibleFactoriesAndProductInterfaces();

            _score = 0;

            if (ContainsIFactoryClass().isTrue)
            {
                _score += PriorityCollection.GetPercentage("factory", "ContainsIFactoryClass");
            }
            if (ContainsProductInterface().isTrue)
            {
                _score += PriorityCollection.GetPercentage("factory", "ContainsProductInterface");
            }
            if (ContainsAbstractProductInterfaceMethod().isTrue)
            {
                _score += PriorityCollection.GetPercentage("factory", "ContainsAbstractProductInterfaceMethod");
            }
            if (IsInheritingFactoryClass().isTrue)
            {
                _score += PriorityCollection.GetPercentage("factory", "IsInheritingFactoryClass");
            }
            if (IsInheritingProductInterface().isTrue)
            {
                _score += PriorityCollection.GetPercentage("factory", "IsInheritingProductInterface");
            }
            if (ConcreteFactoryIsReturningConcreteProduct().isTrue)
            {
                _score += PriorityCollection.GetPercentage("factory", "ConcreteFactoryIsReturningConcreteProduct");
            }
            if (ConcreteProductsFollowOneProductInterface().isTrue)
            {
                _score += PriorityCollection.GetPercentage("factory", "ConcreteProductsFollowOneProductInterface");
            }
            if (HasMultipleMethods().isTrue)
            {
                _score += PriorityCollection.GetPercentage("factory", "HasMultipleMethods");
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

        private void SetParents()
        {
            foreach (var cls in cc.GetClasses())
            {
                if (cls.Value.GetParents().Count > 0)
                {
                    foreach (var prnt in cls.Value.GetParents())
                    {
                        if(!parents.Contains(cc.GetClass(prnt)))
                        {
                            parents.Add(cc.GetClass(prnt));
                        }
                    }
                }
            }
        }

        public void SetIFactoryClass()
        {
            foreach (var ifctr in parents)
            {
                foreach (var property in ifctr.getProperties())
                {
                    foreach (var prnt in parents)
                    {
                        if (prnt != ifctr)
                        {
                            if (property.ValueType.Equals(prnt.Identifier))
                            {
                                ifactory = ifctr;
                            }
                        }
                    }
                }
                foreach (var method in ifctr.getMethods())
                {
                    foreach (var prnt in parents)
                    {
                        if (prnt != ifctr)
                        {
                            if (method.ReturnType.Equals(prnt.Identifier))
                            {
                                ifactory = ifctr;
                            }
                        }
                    }
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
                    if (cls.Value.IsAbstract || cls.Value.IsInterface)
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
        #endregion

        #region Checks
        /// <summary>
        /// Method that checks if there's any (abstract) factory classes present.
        /// </summary>
        /// <returns>Whether or not the check passes.</returns>
        public CheckedMessage ContainsIFactoryClass()
        {
            if (ifactory != null) return new CheckedMessage(true);
            return new CheckedMessage("There is no factory interface detected", false);
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
            return new CheckedMessage("There are no interfaces detected for the products", false);
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
        /// Method that checks if there's any classes that inherit an (abstract) factory class.
        /// </summary>
        /// <returns>Whether or not the check passes.</returns>
        public CheckedMessage IsInheritingFactoryClass()
        {
            if (parents != null)
            {
                foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
                {
                    foreach (var prnt in parents)
                    {
                        if (cls.Value.HasParent(prnt.Identifier))
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
                                    if (isMethod)
                                    {
                                        return new CheckedMessage("Concrete products can't have multiple product interfaces in Factory Method", false);
                                    }
                                    else
                                    {
                                        return new CheckedMessage(true);
                                    }
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
            else if (isMethod)
            {
                return new CheckedMessage(true);
            }
            else
            {
                return new CheckedMessage("Must have multiple product interfaces for Abstract Factory", false);
            }

            return new CheckedMessage(true);
        }

        /// <summary>
        /// Checking to see if the IFactory has multiple methods
        /// </summary>
        /// <returns>Whether it is allowed to have multiple methods and if it has it</returns>
        public CheckedMessage HasMultipleMethods()
        {
            int count = 0;

            if(ifactory != null)
            {
                if (ifactory.getMethods().Count() > 1)
                {
                    foreach (var method in ifactory.getMethods())
                    {
                        foreach (var prnt in parents)
                        {
                            if (method.ReturnType.Equals(prnt.Identifier))
                            {
                                count += 1;
                            }
                        }
                    }
                }
                if (count > 1)
                {
                    if (isMethod) return new CheckedMessage("There are multiple methods in the factory interface, which isn't allowed in Factory Method", false);
                    else return new CheckedMessage(true);
                }
                if (isMethod) return new CheckedMessage(true);
                else return new CheckedMessage("There is only one method which should only be the case in a Factory Method pattern", false);
            }
            return new CheckedMessage("There is no factory interface detected", false);
        }
        #endregion
    }
}
