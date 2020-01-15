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
         FACTORY-CONCRETE-FACTORY
         FACTORY-CONCRETE-PRODUCTS
         FACTORY-RETURNS-PRODUCT
         FACTORY-ONE-PRODUCT-INTERFACE
         FACTORY-MULTIPLE-METHODS
         */
        private List<RequirementResult> _results = new List<RequirementResult>();

        private bool isMethod;
        private HashSet<ClassModel> ifactories = new HashSet<ClassModel>();
        private ClassModel ifactory;
        private List<ClassModel> _iproducts = new List<ClassModel>();
        private List<ClassModel> _concreteFactories = new List<ClassModel>();
        private Dictionary<string, List<RequirementResult>> _reqs = new Dictionary<string, List<RequirementResult>>();
        private ClassCollection cc;

        private List<ClassModel> parents = new List<ClassModel>();


        public AbstractFactoryMethod(ClassCollection _cc, bool ismethod)
        {
            cc = _cc;
            isMethod = ismethod;
        }

        public void Scan()
        {
            SetParents();
            SetIFactoryClass();

            foreach (var ifac in ifactories)
            {
                _results = new List<RequirementResult>();
                ifactory = ifac;
                FindConcreteFactories();
                FindConcreteProducts();
                ConcreteFactoryIsReturningConcreteProduct();
                ConcreteProductsFollowOneProductInterface();
                HasMultipleMethods();

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


        #region privates

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
                if (ifctr != null)
                {
                    foreach (var method in ifctr.GetMethods())
                    {
                        foreach (var prnt in parents)
                        {
                            if (prnt != ifctr && prnt != null)
                            {
                                var x = prnt.Identifier;
                                if (method.ReturnType.Equals(prnt.Identifier))
                                {
                                    ifactories.Add(ifctr);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void _findConcreteImplementation(List<ClassModel> concretes)
        {
            HashSet<ClassModel> strs = new HashSet<ClassModel>();
            foreach (var concrete in concretes)
            {
                foreach (var inf in API.ListInterfaces(cc).Concat(API.ListAbstract(cc)))
                {
                    if (API.ClassHasMethodOfType(concrete, inf.Identifier))
                    {
                        strs.Add(inf);
                    }
                }
            }

            _iproducts = strs.ToList();
        }

        private RequirementResult checkResultsFailed(List<RequirementResult> results)
        {
            var result = results.FirstOrDefault(e => e.Passed == false);
            if (result == default(RequirementResult))
            {
                result = results.FirstOrDefault(e => e.Passed);
            }

            return result;
        }


        #endregion

        #region Checks

        public void FindConcreteFactories()
        {
            string id = "FACTORY-CONCRETE-FACTORY";
            bool triggered = false;
            foreach (var cls in cc.GetClasses().Values)
            {
                if (cls.HasParent(ifactory.Identifier))
                {
                    _concreteFactories.Add(cls);
                    triggered = true;
                }
            }
            _results.Add(new RequirementResult(id, triggered, ifactory));
        }



        public void FindConcreteProducts()
        {
            _findConcreteImplementation(_concreteFactories);
            List<RequirementResult> results = new List<RequirementResult>();
            string id = "FACTORY-CONCRETE-PRODUCTS";
            foreach (var cls in cc.GetClasses().Values)
            {
                foreach (var iproduct in _iproducts)
                {
                    if (cls.HasParent(iproduct.Identifier))
                    {
                        results.Add(new RequirementResult(id, true, cls));
                    }
                }
            }

            if (!results.Any(e => e.Id.Equals(id)))
            {
                results.Add(new RequirementResult(id, false, ifactory));
            }

            var result = checkResultsFailed(results);
            _results.Add(result);
        }


        /// <summary>
        /// Checks if there is a concrete factory that returns a concrete product.
        /// </summary>
        public void ConcreteFactoryIsReturningConcreteProduct()
        {
            List<RequirementResult> results = new List<RequirementResult>();
            string id = "FACTORY-RETURNS-PRODUCT";
            List<ClassModel> concreteProducts = new List<ClassModel>();
            List<string> crProductsId = new List<string>();
            foreach (var iproduct in _iproducts)
            {
                var save = API.ListChildren(cc, iproduct.Identifier);
                concreteProducts = concreteProducts.Concat(save).ToList();
            }

            foreach (var concreteProduct in concreteProducts)
            {
                crProductsId.Add(concreteProduct.Identifier);
            }


            foreach (var cls in _concreteFactories)
            {
                bool triggered = false;
                foreach (var method in cls.GetMethods())
                {
                    if (crProductsId.Any(e => method.Body.Contains(e)))
                    {
                        triggered = true;
                    }
                }
                results.Add(new RequirementResult(id, triggered, cls));
            }
            var result = checkResultsFailed(results);
            _results.Add(result);
        }

        /// <summary>
        /// Checks if the concrete products follow just one product interface.
        /// </summary>
        public void ConcreteProductsFollowOneProductInterface()
        {
            string id = "FACTORY-ONE-PRODUCT-INTERFACE";
            if (isMethod)
            {
                if (_iproducts.Count == 1)
                {
                    _results.Add(new RequirementResult(id, true, ifactory));
                }
                else
                {
                    _results.Add(new RequirementResult(id, false, ifactory));
                }
            }
            else
            {
                if (_iproducts.Count > 1)
                {
                    _results.Add(new RequirementResult(id, true, ifactory));
                }
                else
                {
                    _results.Add(new RequirementResult(id, false, ifactory));
                }
            }

        }

        /// <summary>
        /// Checking to see if the IFactory has multiple methods
        /// </summary>
        /// <returns>Whether it is allowed to have multiple methods and if it has it</returns>
        public void HasMultipleMethods()
        {
            string id = "FACTORY-MULTIPLE-METHODS";
            HashSet<string> strs = new HashSet<string>();
            foreach (var method in ifactory.GetMethods())
            {
                if (_iproducts.Contains(cc.GetClass(method.ReturnType)))
                {
                    strs.Add(method.ReturnType);
                }
            }

            if (isMethod)
            {
                if (_iproducts.Count == 1)
                {
                    _results.Add(new RequirementResult(id, true, ifactory));
                }
                else
                {
                    _results.Add(new RequirementResult(id, false, ifactory));
                }
            }
            else
            {
                if (_iproducts.Count > 1)
                {
                    _results.Add(new RequirementResult(id, true, ifactory));
                }
                else
                {
                    _results.Add(new RequirementResult(id, false, ifactory));
                }
            }
        }

        #endregion
    }
}