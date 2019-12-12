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

        public List<ClassModel> GetAbstractClasses()
        {
            List<ClassModel> abstractClasses = new List<ClassModel>();
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                if (cls.Value.Modifiers != null)
                {
                    foreach (var modifier in cls.Value.Modifiers)
                    {
                        if (modifier.ToLower().Equals("abstract"))
                        {
                            abstractClasses.Add(cls.Value);
                        }
                    }
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

        public bool IsAbstractCreatorClass()
        {
           if (GetAbstractClasses().Count != 0)
           {
               return true;
           }

            return false;
        }

        public bool IsAbstractProductInterface()
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
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public bool IsInheritingAbstractCreatorClass()
        {
            if (GetAbstractClasses() != null)
            {
                foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
                {
                    foreach (var abstractClass in GetAbstractClasses())
                    {
                        if (cls.Value.HasParent(abstractClass.Identifier))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool IsProductInterface()
        {
            if (GetInterfaces().Count != 0)
            {
                return true;
            }

            return false;
        }

        public bool IsInheritingProductInterface()
        {
            foreach (var interf in GetInterfaces())
            {
                if (interf != null)
                {
                    foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
                    {
                        if (cls.Value.HasParent(interf.Identifier))
                        {
                            return true;
                        }
                    }
                }
            }
            
            return false;
        }

        public bool IsFactoryClass()
        {
            return true;
        }
    }
}
