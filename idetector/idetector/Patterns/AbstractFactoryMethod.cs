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

        /*ID's:
         *FACTORY-CONTAINS-ABSTRACT-FACTORY-CLASS
         *FACTORY-CONTAINS-PRODUCT-INTERFACE
         *FACTORY-CONTAINS-ABSTRACT-PRODUCT-INTERFACE-METHOD
         *FACTORY-INHERITING-PRODUCT-INTERFACE
         *FACTORY-RETURNS-PRODUCT
         *FACTORY-MULTIPLE-METHODS
         */
        private List<RequirementResult> _results = new List<RequirementResult>();

        private bool isMethod;
        private ClassModel ifactory;

        private Dictionary<ClassModel, int> _scores = new Dictionary<ClassModel, int>();
        private ClassCollection cc;
        private List<ClassModel> abstractClasses = new List<ClassModel>();
        private List<ClassModel> interfaces = new List<ClassModel>();
        private List<ClassModel> parents = new List<ClassModel>();
        private List<ClassModel> possibleFactoryClasses = new List<ClassModel>();
        private List<ClassModel> productInterfaces = new List<ClassModel>();


        public AbstractFactoryMethod(ClassCollection _cc, bool ismethod)
        {
            cc = _cc;
            isMethod = ismethod;
            ifactory = null;
        }

        public void Scan()
        {
            SetAbstractClasses();
            SetInterfaces();
            SetParents();
            SetIFactoryClass();
            SetPossibleFactoriesAndProductInterfaces();

            _results.Add(ContainsIFactoryClass());
            _results.Add(ContainsProductInterface());
            _results.Add(ContainsAbstractProductInterfaceMethod());
            _results.Add(IsInheritingProductInterface());
            _results.Add(ConcreteFactoryIsReturningConcreteProduct());
            _results.Add(HasMultipleMethods());
        }

        public List<RequirementResult> GetResult()
        {
            return _results;
        }


        public int Score(ClassModel clsModel)
        {
            if (this._scores.ContainsKey(clsModel)) return this._scores[clsModel];

            return 0;
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
        private void SetParents()
        {
            foreach (var cls in cc.GetClasses())
            {
                if (cls.Value.GetParents().Count > 0)
                {
                    foreach (var prnt in cls.Value.GetParents())
                    {
                        if (!parents.Contains(cc.GetClass(prnt)))
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
        #endregion

        #region Checks
        /// <summary>
        /// Method that checks if there's any (abstract) factory classes present.
        /// </summary>
        /// <returns>Whether or not the check passes.</returns>
        public RequirementResult ContainsIFactoryClass()
        {
            if (ifactory != null) return new RequirementResult("FACTORY-CONTAINS-ABSTRACT-FACTORY-CLASS", true);
            return new RequirementResult("FACTORY-CONTAINS-ABSTRACT-FACTORY-CLASS", false);
        }

        public RequirementResult ContainsProductInterface()
        {
            if (abstractClasses.Count == 0 && interfaces.Count == 0)
            {
                return new RequirementResult("FACTORY-CONTAINS-PRODUCT-INTERFACE", false);
            }
            return new RequirementResult("FACTORY-CONTAINS-PRODUCT-INTERFACE", true);
        }

        public RequirementResult ContainsAbstractProductInterfaceMethod()
        {
            if (interfaces.Count == 0)
            {
                return new RequirementResult("FACTORY-CONTAINS-ABSTRACT-PRODUCT-INTERFACE-METHOD", false);
            }
            return new RequirementResult("FACTORY-CONTAINS-ABSTRACT-PRODUCT-INTERFACE-METHOD", true);
        }

        public RequirementResult IsInheritingFactoryClass()
        {
            if (parents != null)
            {
                foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
                {
                    foreach (var prnt in parents)
                    {
                        if (cls.Value.HasParent(prnt.Identifier))
                        {
                            return new RequirementResult("FACTORY-INHERITING-FACTORY-CLASS", true);
                        }
                    }
                }
            }
            return new RequirementResult("FACTORY-INHERITING-ABSTRACT-FACTORY-CLASS", false);
        }

        public RequirementResult IsInheritingProductInterface()
        {
            foreach (var @interface in productInterfaces)
            {
                if (@interface != null)
                {
                    foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
                    {
                        if (cls.Value.HasParent(@interface.Identifier))
                        {
                            return new RequirementResult("FACTORY-INHERITING-PRODUCT-INTERFACE", true);
                        }
                    }
                }
            }
            return new RequirementResult("FACTORY-INHERITING-PRODUCT-INTERFACE", false);
        }

        public RequirementResult ConcreteFactoryIsReturningConcreteProduct()
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
                                    return new RequirementResult("FACTORY-RETURNS-PRODUCT", true);

                                }
                            }
                        }
                    }
                }
            }
            return new RequirementResult("FACTORY-RETURNS-PRODUCT", false);
        }

        /// <summary>
        /// Method that checks if the concrete products follow just one product interface.
        /// </summary>
        /// <returns>Whether or not the check passes.</returns>
        public RequirementResult ConcreteProductsFollowOneProductInterface()
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
                                        return new RequirementResult("FACTORY-ONE-PRODUCT-INTERFACE",false);
                                    }
                                    else
                                    {
                                        return new RequirementResult("FACTORY-ONE-PRODUCT-INTERFACE",true);
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
                return new RequirementResult("FACTORY-ONE-PRODUCT-INTERFACE", true);
            }
            else
            {
                return new RequirementResult("FACTORY-ONE-PRODUCT-INTERFACE", false);
            }

            return new RequirementResult("FACTORY-ONE-PRODUCT-INTERFACE", true);
        }

        /// <summary>
        /// Checking to see if the IFactory has multiple methods
        /// </summary>
        /// <returns>Whether it is allowed to have multiple methods and if it has it</returns>
        public RequirementResult HasMultipleMethods()
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
                    if (isMethod) return new RequirementResult("FACTORY-MULTIPLE-METHODS", false);
                    else return new RequirementResult("FACTORY-MULTIPLE-METHODS", true);
                }
                if (isMethod) return new RequirementResult("FACTORY-MULTIPLE-METHODS", true);
                else return new RequirementResult("FACTORY-MULTIPLE-METHODS", false);
            }
            return new RequirementResult("FACTORY-MULTIPLE-METHODS", false);
        }
        #endregion
    }
}
