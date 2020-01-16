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
        //DECORATOR-METHOD-SETS-COMPONENT
        //DECORATOR-CONCRETE-METHOD-SETS-PROPERTY

        private ClassModel _cls;
        private ClassCollection _collection;
        private List<ClassModel> _children = new List<ClassModel>();
        private ClassModel _decoratorInterface;
        private List<ClassModel> _decorators = new List<ClassModel>();
        private List<ClassModel> _bases = new List<ClassModel>();
        private bool isDecorator = false;

        private Dictionary<string, List<RequirementResult>>
            _results = new Dictionary<string, List<RequirementResult>>();


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
                var parent = cls.Parents.First(e => cls.HasParent(e.Identifier));
                _cls = _collection.GetClass(parent.Identifier);

                _decoratorInterface = cls;
                results.Add(CheckChildren());
                results.Add(FindDecorators());
                results.Add(DecoratorHasBaseProperty());
                results.Add(DecoratorHasBasePropertySetter());
                results.Add(DecoratorBasePropertySetterSets());
                _results.Add(_cls.Identifier, results);
            }
        }

        public Dictionary<string, List<RequirementResult>> GetResults()
        {
            return _results;
        }

        private List<ClassModel> findDecoratorInterface(ClassCollection cc)
        {
            var interfaces = API.ListInterfaces(cc);
            var abstracts = API.ListAbstract(cc);
            var classes = interfaces.Concat(abstracts);
            List<ClassModel> abstractDecorators = new List<ClassModel>();
            foreach (var inf in classes)
            {
                var parent = abstracts.FirstOrDefault(e => e.HasParent(inf.Identifier));
                if (parent != null)
                {
                    abstractDecorators.Add(parent);
                }
            }

            return abstractDecorators;
        }


        public RequirementResult CheckChildren()
        {
            var classes =
                _collection.GetClassListExcept(API.ListAbstract(_collection).Concat(API.ListInterfaces(_collection))
                    .ToList());

            foreach (var cls in classes)
            {
                if (cls.HasParent(_cls.Identifier))
                {
                    return new RequirementResult("DECORATOR-BASE-HAS-CHILDREN", true, cls);
                }
            }

            return new RequirementResult("DECORATOR-BASE-HAS-CHILDREN", false, _cls);
        }

        public RequirementResult DecoratorHasBaseProperty()
        {
            bool result = API.ClassHasPropertyOfType(_decoratorInterface, _cls.Identifier);


            return new RequirementResult("DECORATOR-HAS-BASE-PROPERTY", result, _decoratorInterface);
        }

        public RequirementResult DecoratorHasBasePropertySetter()
        {
            var result = API.ClassHasMethodWithParam(_decoratorInterface, _cls.Identifier);


            return new RequirementResult("DECORATOR-METHOD-SETS-COMPONENT", result, _decoratorInterface);
        }

        public RequirementResult FindDecorators()
        {
            _decorators = API.ListChildren(_collection, _decoratorInterface.Identifier);

            if (_decorators.Count < 0)
            {
                return new RequirementResult("DECORATOR-HAS-CHILDREN", false, _decoratorInterface);
            }

            return new RequirementResult("DECORATOR-HAS-CHILDREN", true, _decoratorInterface);
        }
        public RequirementResult DecoratorBasePropertySetterSets()
        {
            bool result = false;
            var methods = API.ClassGetMethodsWithParam(_decoratorInterface, _cls.Identifier);
            var propertyNames = _decoratorInterface.GetProperties().Where(e => e.ValueType.Equals(_cls.Identifier)).ToList();
            foreach (var method in methods)
            {
                foreach (var property in propertyNames)
                {
                    if (method.Body.Contains(property.Identifier))
                    {
                        result = true;
                    }
                }
            }
            return new RequirementResult("DECORATOR-CONCRETE-METHOD-SETS-PROPERTY", result, _decoratorInterface);
        }

    }
}