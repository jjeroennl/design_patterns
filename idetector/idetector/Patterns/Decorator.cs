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
        private ClassModel _cls;
        private Dictionary<string, ClassModel> _collection;
        private List<ClassModel> _children = new List<ClassModel>();
        private ClassModel _decoratorInterface;
        private List<ClassModel> _decorators = new List<ClassModel>();
        private List<ClassModel> _bases = new List<ClassModel>();
        private int _score = 0;

        public Decorator(ClassModel cls, Dictionary<string, ClassModel> collection)
        {
            _collection = collection;
            _cls = cls;
            PriorityCollection.ClearPriorities();
            PriorityCollection.AddPriority("decorator", "checkChildren", Priority.Low);
            PriorityCollection.AddPriority("decorator", "findDecorators", Priority.Low);
            PriorityCollection.AddPriority("decorator", "decoratorBaseHasComponent", Priority.Low);
            PriorityCollection.AddPriority("decorator", "constructorSetsComponent", Priority.Low);
            PriorityCollection.AddPriority("decorator", "DecoratorCallsBase", Priority.Low);
        }

        public void Scan()
        {
            if (_cls.IsInterface || _cls.IsAbstract)
            {
                if (checkChildren().isTrue)
                {
                    _score += PriorityCollection.GetPercentage("decorator", "checkChildren");

                    if (findDecorators().isTrue)
                    { 
                        _score += PriorityCollection.GetPercentage("decorator", "findDecorators");
                        if (decoratorBaseHasComponent().isTrue)
                        {
                            _score += PriorityCollection.GetPercentage("decorator", "decoratorBaseHasComponent");
                        }

                        if (constructorSetsComponent().isTrue)
                        {
                            _score += PriorityCollection.GetPercentage("decorator", "constructorSetsComponent");
                        }

                        if (DecoratorCallsBase().isTrue)
                        {
                            _score += PriorityCollection.GetPercentage("decorator", "DecoratorCallsBase");
                        }
                    }
                }
                
            }
        }

        public int Score()
        {
            return _score;
        }


        /// <summary>
        /// Checks if interface has children
        /// </summary>
        /// <returns>CheckedMessage</returns>
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

            return new CheckedMessage("The class did not have enough children to qualify", false, _cls.Identifier);
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
                else
                {
                    _bases.Add(child);
                }

            }

            if (i != 1)
            {
                return new CheckedMessage("Decorator pattern does not allow multiple base decorator classes", false,
                    _cls.Identifier);
            }

            return new CheckedMessage(true);

        }

        private CheckedMessage decoratorBaseHasComponent()
        {
            foreach (var property in _decoratorInterface.getProperties())
            {
                if (property.ValueType.Equals(_cls.Identifier))
                {
                    return new CheckedMessage(true);
                }
            }

            return new CheckedMessage("The base decorator did not have a property/field of type: " + _cls.Identifier,
                false, _decoratorInterface.Identifier);
        }

        private CheckedMessage constructorSetsComponent()
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
                            return new CheckedMessage(true);

                        }
                        return new CheckedMessage("One or more decorator does not take a " + _cls.Identifier + "as parameter", false, decorator.Identifier);
                    }

                }
            }
            return new CheckedMessage("The decorator base does not take a value of " + _cls.Identifier + "as parameter", false, _decoratorInterface.Identifier);
        }

        private CheckedMessage findDecorators()
        {
            foreach (var item in _collection.Values)
            {
                if (item.HasParent(_decoratorInterface.Identifier))
                {
                    _decorators.Add(item);
                }
            }

            if (_decorators.Count > 0)
            {
                return new CheckedMessage(true);
            }

            return new CheckedMessage("the base decorator does not have any children", false,
                _decoratorInterface.Identifier);
        }

        private CheckedMessage DecoratorCallsBase()
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
                            return new CheckedMessage("Not all decorators call the base constructor", false, decorator.Identifier);
                        }
                    }
                }
            }
            return new CheckedMessage(true);
        }
    }
}