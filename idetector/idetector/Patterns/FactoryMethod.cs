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

        /*ID's:
         *FACTORY-CONTAINS-ABSTRACT-FACTORY-CLASS
         *FACTORY-CONTAINS-PRODUCT-INTERFACE
         *FACTORY-CONTAINS-ABSTRACT-PRODUCT-INTERFACE-METHOD
         *FACTORY-INHERITING-ABSTRACT-FACTORY-CLASS
         *FACTORY-INHERITING-PRODUCT-INTERFACE
         *FACTORY-RETURNS-PRODUCT
         *FACTORY-ONE-METHOD
         *FACTORY-ONE-PRODUCT-INTERFACE
         */
        private List<RequirementResult> _results = new List<RequirementResult>();

        private float _score;
        private Dictionary<ClassModel, int> _scores = new Dictionary<ClassModel, int>();
        private ClassCollection cc;
        private List<ClassModel> abstractClasses = new List<ClassModel>();
        private List<ClassModel> interfaces = new List<ClassModel>();
        private List<ClassModel> possibleFactoryClasses = new List<ClassModel>();
        private List<ClassModel> productInterfaces = new List<ClassModel>();


        public FactoryMethod(ClassCollection _cc)
        {
            cc = _cc;
        }

        public void Scan()
        {
            SetAbstractClasses();
            SetInterfaces();
            SetPossibleFactoriesAndProductInterfaces();

            _results.Add(ContainsAbstractFactoryClass());
            _results.Add(ContainsProductInterface());
            _results.Add(ContainsAbstractProductInterfaceMethod());
            _results.Add(IsInheritingAbstractFactoryClass());
            _results.Add(IsInheritingProductInterface());
            _results.Add(ConcreteFactoryIsReturningConcreteProduct());

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
        public RequirementResult ContainsAbstractFactoryClass()
        {
            if (abstractClasses.Count != 0)
            {
                return new RequirementResult("FACTORY-CONTAINS-ABSTRACT-FACTORY-CLASS", true);
            }
            return new RequirementResult("FACTORY-CONTAINS-ABSTRACT-FACTORY-CLASS", false);
        }

        public RequirementResult ContainsProductInterface()
        {
            if (abstractClasses.Count == 0)
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

        public RequirementResult IsInheritingAbstractFactoryClass()
        {
            if (abstractClasses != null)
            {
                foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
                {
                    foreach (var abstractClass in abstractClasses)
                    {
                        if (cls.Value.HasParent(abstractClass.Identifier))
                        {
                            return new RequirementResult("FACTORY-INHERITING-ABSTRACT-FACTORY-CLASS", true);
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


        public RequirementResult ConcreteFactoriesHaveOneMethod()
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
                            return new RequirementResult("FACTORY-ONE-METHOD",false);
                        }
                        else
                        {
                            ret = true;
                        }
                    }
                }
            }
            return new RequirementResult("FACTORY-ONE-METHOD", ret);
        }


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
                                    return new RequirementResult("FACTORY-ONE-PRODUCT-INTERFACE",false);
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
            return new RequirementResult("FACTORY-ONE-PRODUCT-INTERFACE", true);
        }
        #endregion
    }
}
