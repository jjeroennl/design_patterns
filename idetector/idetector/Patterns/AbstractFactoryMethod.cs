using idetector.Collections;
using idetector.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using idetector.Patterns.Helper;

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
        private HashSet<ClassModel> ifactories = new HashSet<ClassModel>();
        private ClassModel ifactory;

        private Dictionary<string, List<RequirementResult>> _reqs = new Dictionary<string, List<RequirementResult>>();
        private ClassCollection cc;
        private List<ClassModel> abstractClasses = new List<ClassModel>();
        private List<ClassModel> interfaces = new List<ClassModel>();
        private List<ClassModel> parents = new List<ClassModel>();
        private List<ClassModel> possibleFactoryClasses = new List<ClassModel>();
        private List<ClassModel> productInterfaces = new List<ClassModel>();


        public AbstractFactoryMethod(ClassCollection _cc, bool ismethod)
        {
            abstractClasses = API.ListAbstract(cc);
            interfaces = API.ListInterfaces(cc);
            cc = _cc;
            isMethod = ismethod;
            ifactory = null;
        }

        public void Scan()
        {
            SetParents();
            SetIFactoryClass();
            SetPossibleFactoriesAndProductInterfaces();
            foreach (var ifac in ifactories)
            {
                ifactory = ifac;
                _results.Add(ContainsIFactoryClass());
                _results.Add(ContainsProductInterface());
                _results.Add(ContainsAbstractProductInterfaceMethod());
                _results.Add(IsInheritingProductInterface());
                _results.Add(ConcreteFactoryIsReturningConcreteProduct());
                _results.Add(HasMultipleMethods());
                _reqs.Add(ifac.Identifier, _results);
            }
        }

        public Dictionary<string, List<RequirementResult>> GetResults()
        {
            return _reqs;
        }

        public List<RequirementResult> GetResult()
        {
            return _results;
        }
        

        #region Lists

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
                                
                                ifactories.Add(ifctr);
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
                                ifactories.Add(ifctr);
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
            if (ifactory != null) return new RequirementResult("FACTORY-CONTAINS-ABSTRACT-FACTORY-CLASS", true, ifactory);
            return new RequirementResult("FACTORY-CONTAINS-ABSTRACT-FACTORY-CLASS", false, ifactory);
        }

        public RequirementResult ContainsProductInterface()
        {
            if (abstractClasses.Count == 0 && interfaces.Count == 0)
            {
                return new RequirementResult("FACTORY-CONTAINS-PRODUCT-INTERFACE", false, ifactory);
            }
            return new RequirementResult("FACTORY-CONTAINS-PRODUCT-INTERFACE", true, ifactory);
        }

        public RequirementResult ContainsAbstractProductInterfaceMethod()
        {
            if (interfaces.Count == 0)
            {
                return new RequirementResult("FACTORY-CONTAINS-ABSTRACT-PRODUCT-INTERFACE-METHOD", false, ifactory);
            }
            return new RequirementResult("FACTORY-CONTAINS-ABSTRACT-PRODUCT-INTERFACE-METHOD", true, ifactory);
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
                            return new RequirementResult("FACTORY-INHERITING-FACTORY-CLASS", true, ifactory);
                        }
                    }
                }
            }
            return new RequirementResult("FACTORY-INHERITING-ABSTRACT-FACTORY-CLASS", false, ifactory);
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
                            return new RequirementResult("FACTORY-INHERITING-PRODUCT-INTERFACE", true, ifactory);
                        }
                    }
                }
            }
            return new RequirementResult("FACTORY-INHERITING-PRODUCT-INTERFACE", false, ifactory);
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
                                    return new RequirementResult("FACTORY-RETURNS-PRODUCT", true, ifactory);

                                }
                            }
                        }
                    }
                }
            }
            return new RequirementResult("FACTORY-RETURNS-PRODUCT", false, ifactory);
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
                                        return new RequirementResult("FACTORY-ONE-PRODUCT-INTERFACE",false, ifactory);
                                    }
                                    else
                                    {
                                        return new RequirementResult("FACTORY-ONE-PRODUCT-INTERFACE",true, ifactory);
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
                return new RequirementResult("FACTORY-ONE-PRODUCT-INTERFACE", true, ifactory);
            }
            else
            {
                return new RequirementResult("FACTORY-ONE-PRODUCT-INTERFACE", false, ifactory);
            }

            return new RequirementResult("FACTORY-ONE-PRODUCT-INTERFACE", true, ifactory);
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
                    if (isMethod) return new RequirementResult("FACTORY-MULTIPLE-METHODS", false, ifactory);
                    else return new RequirementResult("FACTORY-MULTIPLE-METHODS", true, ifactory);
                }
                if (isMethod) return new RequirementResult("FACTORY-MULTIPLE-METHODS", true, ifactory);
                else return new RequirementResult("FACTORY-MULTIPLE-METHODS", false, ifactory);
            }
            return new RequirementResult("FACTORY-MULTIPLE-METHODS", false, ifactory);
        }
        #endregion
    }
}
