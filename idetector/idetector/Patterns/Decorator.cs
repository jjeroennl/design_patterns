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
        private ClassModel _decorator;
        private ClassModel _base;
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
                if (checkAmountChildren())
                {
                    _score += 25;
                    if (checkChildrenType())
                    {
                        _score += 25;
                        if (checkDecorators())
                        {
                            _score += 25;
                        }
                    }
                }

            }
        }


        /// <summary>
        /// Checks if interface has children
        /// </summary>
        /// <returns></returns>
        private bool checkAmountChildren()
        {
            List<ClassModel> list = new List<ClassModel>();
            foreach (var item in _collection.Values)
            {
                if (item.HasParent(_cls.Identifier))
                {
                    list.Add(item);
                }
            }

            if (list.Count == 2)
            {
                _children = list;
                return true;
            }
            return false;
        }

        private bool checkChildrenType()
        {
            var list = _children;
            var a = list[0].IsInterface || list[0].IsAbstract;
            var b = list[1].IsInterface || list[1].IsAbstract;
            if ((a && b == false) || (b && a == false))
            {
                if (a)
                {
                    _decorator = list[0];
                    _base = list[1];
                }
                else
                {
                    _decorator = list[1];
                    _base = list[0];
                }
                return true;
            }

            return false;
        }

        private bool checkDecorators()
        {
            bool isBase = true;
            bool isDecorator = false;
            foreach (var item in _collection.Values)
            {
                if (item.HasParent(_base.Identifier))
                {
                    isBase = false;
                }

                if (item.HasParent(_decorator.Identifier))
                {
                    isDecorator = true;
                }
            }

            if (isBase && isDecorator)
            {
                return true;
            }

            return false;
        }

    }
}