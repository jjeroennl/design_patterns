using System.Collections.Generic;
using idetector.Collections;
using idetector.Models;
using System.Linq;
using idetector.Patterns.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace idetector.Patterns
{
    public class Decorator : IPattern
    {
        //ID's:
        //DECORATOR-BASE-HAS-CHILDREN
        //DECORATOR-BASE-CHILDREN-TYPES
        //DECORATOR-HAS-CHILDREN
        //DECORATOR-HAS-BASE-PROPERTY
        //DECORATOR-CONSTRUCTOR-SETS-COMPONENT
        //DECORATOR-CONCRETE-CALLS-BASE

        private ClassModel _cls;
        private ClassCollection _collection;
        private List<ClassModel> _children = new List<ClassModel>();
        private ClassModel _decoratorInterface;
        private List<ClassModel> _decorators = new List<ClassModel>();
        private List<ClassModel> _bases = new List<ClassModel>();
        private bool isDecorator = false;
        private Dictionary<string, List<RequirementResult>> _results = new Dictionary<string, List<RequirementResult>>();

        

        public Decorator(ClassModel cls, ClassCollection collection)
        {
            _collection = collection;
            _cls = cls;
        }

        public Decorator(ClassCollection collection)
        {
            _collection = collection;
        }

        public void Scan()
        {
            var classes = findDecoratorInterface(_collection);
            foreach (var cls in classes)
            {
                List<RequirementResult> results = new List<RequirementResult>();
                _cls = cls;
                results.Add(CheckChildren());
                results.Add(CheckAbstractChild());
                results.Add(FindDecorators());
                results.Add(DecoratorHasBaseProperty());
                results.Add(ConstructorSetsComponent());
                results.Add(DecoratorCallsBase());
                _results.Add(cls.Identifier, results);
            }
        }

        public List<RequirementResult> GetResult()
        {
            throw new System.NotImplementedException();
        }

        public Dictionary<string, List<RequirementResult>> GetResults()
        {
            return _results;
        }

        private List<ClassModel> findDecoratorInterface(ClassCollection cc)
        {
            var interfaces = API.ListInterfaces(cc);
            var abstracts = API.ListAbstract(cc);
            List<ClassModel> abstractDecorators = new List<ClassModel>();
            foreach (var inf in interfaces)
            {
                abstractDecorators.Add(abstracts.FirstOrDefault(e => e.HasParent(inf.Identifier)));
            }

            return abstractDecorators;
        }


        public RequirementResult CheckChildren()
        {

            var classes =
                _collection.GetClassListExcept(API.ListAbstract(_collection).Concat(API.ListInterfaces(_collection)).ToList());

            foreach (var cls in classes)
            {
                return new RequirementResult("DECORATOR-BASE-HAS-CHILDREN", true);
            }

            
        }

        public RequirementResult CheckAbstractChild()
        {
            var i = 0;
            foreach (var child in _children)
            {
                if (child.IsAbstract)
                {
                    i++;
                    _decoratorInterface = child;
                }
                else
                {
                    _bases.Add(child);
                }
            }

            if (i != 1)
            {
                return new RequirementResult("DECORATOR-BASE-CHILDREN-TYPES", false);
            }

            return new RequirementResult("DECORATOR-BASE-CHILDREN-TYPES", true);
        }

        public RequirementResult DecoratorHasBaseProperty()
        {
            if (_decoratorInterface != null)
            {
                int i = 0;
                foreach (var property in _decoratorInterface.getProperties())
                {
                    if (property.ValueType.Equals(_cls.Identifier))
                    {
                        i++;
                    }
                }

                if (i < 1)
                {
                    return new RequirementResult("DECORATOR-HAS-BASE-PROPERTY", false);
                }
            }


            return new RequirementResult("DECORATOR-HAS-BASE-PROPERTY", true);
        }

        public RequirementResult ConstructorSetsComponent()
        {
            if (_decoratorInterface != null)
            {
                foreach (var item in _decoratorInterface.getMethods())
                {
                    if (item.Parameters.Contains(_cls.Identifier))
                    {
                        int count = 0;
                        foreach (var decorator in _decorators)
                        {
                            bool triggered = false;

                            foreach (var method in decorator.getMethods())
                            {
                                if (method.isConstructor)
                                {
                                    count++;
                                    if (!method.Parameters.Contains(_cls.Identifier))
                                    {
                                        triggered = true;
                                    }
                                }
                            }

                            if (!triggered && (count >= _decorators.Count))
                            {
                                return new RequirementResult("DECORATOR-CONSTRUCTOR-SETS-COMPONENT", true);
                            }

                            return new RequirementResult("DECORATOR-CONSTRUCTOR-SETS-COMPONENT", false);
                        }
                    }
                }
            }

            return new RequirementResult("DECORATOR-CONSTRUCTOR-SETS-COMPONENT", false);
        }

        public RequirementResult FindDecorators()
        {
            if (_decoratorInterface != null)
            {
                foreach (var item in _collection.Values)
                {
                    if (item.HasParent(_decoratorInterface.Identifier))
                    {
                        _decorators.Add(item);
                    }
                }

                if (_decorators.Count < 1)
                {
                    return new RequirementResult("DECORATOR-HAS-CHILDREN", false);
                }
            }

            return new RequirementResult("DECORATOR-HAS-CHILDREN", true);
        }

        public RequirementResult DecoratorCallsBase()
        {
            if (_decorators.Count > 0)
            {
                foreach (var decorator in _decorators)
                {
                    foreach (var method in decorator.getMethods())
                    {
                        if (method.isConstructor)
                        {
                            var node = (ConstructorDeclarationSyntax) method.getNode();
                            if (!node.Initializer.ThisOrBaseKeyword.ToString().ToLower().Equals("base"))
                            {
                                return new RequirementResult("DECORATOR-CONCRETE-CALLS-BASE", false);
                            }
                        }
                    }
                }
            }

            return new RequirementResult("DECORATOR-CONCRETE-CALLS-BASE", true);
        }
    }
}