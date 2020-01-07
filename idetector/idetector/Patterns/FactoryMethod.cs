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
         */
        private int _score;
        private ClassCollection cc;
        private List<RequirementResult> _results = new List<RequirementResult>();

        public FactoryMethod(ClassCollection _cc)
        {
            cc = _cc;
        }

        public void Scan()
        {
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
        public RequirementResult ContainsAbstractFactoryClass()
        {
            if (GetAbstractClasses().Count != 0)
            {
                return new RequirementResult("FACTORY-CONTAINS-ABSTRACT-FACTORY-CLASS", true);
            }
            return new RequirementResult("FACTORY-CONTAINS-ABSTRACT-FACTORY-CLASS", false);
        }

        public RequirementResult ContainsProductInterface()
        {
            if (GetInterfaces().Count == 0)
            {
                return new RequirementResult("FACTORY-CONTAINS-PRODUCT-INTERFACE", false);
            }
            return new RequirementResult("FACTORY-CONTAINS-PRODUCT-INTERFACE", true);
        }

        public RequirementResult ContainsAbstractProductInterfaceMethod()
        {
            if (GetAbstractProductInterfaceClasses().Count == 0)
            {
                return new RequirementResult("FACTORY-CONTAINS-ABSTRACT-PRODUCT-INTERFACE-METHOD", false);
            }

            return new RequirementResult("FACTORY-CONTAINS-ABSTRACT-PRODUCT-INTERFACE-METHOD", true);
        }

        public RequirementResult IsInheritingAbstractFactoryClass()
        {
            if (GetAbstractClasses() != null)
            {
                foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
                {
                    foreach (var abstractClass in GetAbstractClasses())
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
            foreach (var @interface in GetInterfaces())
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
                                    return new RequirementResult("FACTORY-RETURNS-PRODUCT", true);
                                }
                            }
                        }
                    }
                }
            }
            return new RequirementResult("FACTORY-RETURNS-PRODUCT", false);
        }
        #endregion
    }
}
