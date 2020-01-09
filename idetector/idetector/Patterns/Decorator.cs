using System.Collections.Generic;
using idetector.Collections;
using idetector.Models;
using System.Linq;
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

        private List<RequirementResult> _results = new List<RequirementResult>();

        public Decorator(ClassModel cls, ClassCollection collection)
        {
            _collection = collection;
            _cls = cls;
        }
        public Decorator(ClassCollection collection)
        {
            _collection = collection;
            _cls = getInterfaces(_collection);
        }

        public void Scan()
        {
            _cls = getInterfaces(_collection);
            _results.Add(CheckChildren());
            _results.Add(CheckAbstractChild());
            _results.Add(FindDecorators());
            _results.Add(DecoratorHasBaseProperty());
            _results.Add(ConstructorSetsComponent());
            _results.Add(DecoratorCallsBase());
        }

        public List<RequirementResult> GetResult()
        {
            return _results;
        }

        private ClassModel getInterfaces(ClassCollection cc)
        {
            
        } 
        


        public RequirementResult CheckChildren()
        {
            List<ClassModel> children = new List<ClassModel>();
            foreach (var item in _collection.Values)
            {
                if (item.HasParent(_cls.Identifier))
                {
                    children.Add(item);
                }
            }

            if (children.Count < 1)
            {
                return new RequirementResult("DECORATOR-BASE-HAS-CHILDREN", false);
            }

            _children = children;
            return new RequirementResult("DECORATOR-BASE-HAS-CHILDREN", true);
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

            return new RequirementResult( "DECORATOR-BASE-CHILDREN-TYPES", true);
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
                            var node = (ConstructorDeclarationSyntax)method.getNode();
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