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
        private int _score;
        private ClassCollection cc;

        public FactoryMethod(ClassCollection _cc)
        {
            cc = _cc;
            PriorityCollection.ClearPriorities();
            PriorityCollection.AddPriority("factorymethod", "IsAbstractCreatorClass", Priority.Low);
        }

        public void Scan()
        {
            if (IsAbstractCreatorClass())
            {
                _score = 2;
            }
        }

        public int Score()
        {
            return _score;
        }

        public ClassModel GetAbstractCreatorClass()
        {
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                foreach (var modifier in cls.Value.Modifiers)
                {
                    if (modifier.ToLower().Equals("abstract"))
                    {
                        return cls.Value;
                    }
                }
            }
            return null;
        }

        public bool IsAbstractCreatorClass()
        {
           if (GetAbstractCreatorClass() != null)
           {
               return true;
           }

            return false;
        }

        public ClassModel GetAbstractProductInterface()
        {
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                foreach (var method in cls.Value.getMethods())
                {
                    foreach (var modifier in method.Modifiers)
                    {
                        if (modifier.ToLower().Equals("abstract"))
                        {
                            if (cc.GetClass(method.ReturnType).IsInterface)
                            {
                                return cc.GetClass(method.ReturnType);
                            }
                        }
                    }
                }
            }

            return null;
        }

        public bool IsAbstractProductInterface()
        {
            if (GetAbstractProductInterface() != null)
            {
                return true;
            }

            return false;
        }

        public bool IsInheritingAbstractCreatorClass()
        {
            if (GetAbstractCreatorClass() != null)
            {
                foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
                {
                    if (cls.Value.HasParent(GetAbstractCreatorClass().Identifier))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsProductInterface()
        {
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                if (cls.Value.IsInterface)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsInheritingProductInterface()
        {
            return false;
        }
    }
}
