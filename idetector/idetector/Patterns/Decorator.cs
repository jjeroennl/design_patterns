using System.Collections.Generic;
using idetector.Collections;
using idetector.Models;

namespace idetector.Patterns
{
    public class Decorator : IPattern
    {
        private ClassModel _cls;
        private Dictionary<string, ClassModel> _collection;
        private List<ClassModel> _children = new List<ClassModel>();
        private ClassModel _decoratorInterface;
        private List<ClassModel> _decorators;
        private List<ClassModel> _bases;
        private int _score = 0;

        public Decorator(ClassModel cls, Dictionary<string, ClassModel> collection)
        {
            _collection = collection;
            _cls = cls;
        }

        public void Scan()
        {
            if (_cls.IsInterface || _cls.IsAbstract)
            {
                if (checkChildren().isTrue)
                {
                    _score += 25;

                    if (findDecorators().isTrue)
                    {
                        _score += 25;
                    }
                }
            }
        }


        /// <summary>
        /// Checks if interface has children
        /// </summary>
        /// <returns></returns>
        private CheckedMessage checkChildren()
        {
            List<ClassModel> list = new List<ClassModel>();
            foreach (var item in _collection.Values)
            {
                if (item.HasParent(_cls.Identifier))
                {
                    list.Add(item);
                }
            }

            if (list.Count > 1)
            {
                _children = list;
                return checkChildrenType();
            }

            return new CheckedMessage("The class did not have enough children to qualify", false);
        }

        private CheckedMessage checkChildrenType()
        {
            var i = 0;
            foreach (var child in _children)
            {
                if (child.IsAbstract)
                {
                    i++;
                    _decoratorInterface = child;
                }
                else {_bases.Add(child);}
                
            }
            if (i > 1)
            {
                return false;
            }

            return true;
        }

        private CheckedMessage decoratorBaseHasComponent()
        {
            foreach (var property in _decoratorInterface.getProperties())
            {
                if (property.Identifier.Equals(_cls.Identifier))
                {
                    return true;
                }
            }

            return false;
        }

        private CheckedMessage constructorSetsComponent()
        {

        }

        private CheckedMessage findDecorators()
        {
            CheckedMessage isDecorator = false;
            foreach (var item in _collection.Values)
            {
                if (item.HasParent(_decoratorInterface.Identifier))
                {
                    _decorators.Add(item);
                }
            }

            return _decorators.Count > 0;
        }

        private CheckedMessage checkDecoratorCallsBase()
        {

        }
    }
}