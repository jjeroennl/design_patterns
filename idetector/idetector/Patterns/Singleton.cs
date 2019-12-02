using System;
using idetector.Collections;
using idetector.Models;

namespace idetector.Patterns
{
    public class Singleton : IPattern
    {
        private int _score;

        public void Scan()
        {
            if(IsPrivateConstructor())
            {
                _score += 25;
            }
            if (IsStaticSelf())
            {
                _score += 25;
            }
            if (IsGetInstance())
            {
                _score += 25;
            }
            if (IsCreateSelf())
            {
                _score += 25;
            }
        }

        public int Score()
        {
            return _score;
        }

        public bool IsPrivateConstructor()
        {
            foreach (var item in ClassCollection.GetClasses())
            {
                ClassModel cls = item.Value;
                Console.WriteLine(cls.getMethods());
                foreach (var method in cls.getMethods())
                {

                    if (method.isConstructor)
                    {
                        foreach (var modifier in method.Modifiers)
                        {
                            if (modifier.ToLower().Equals("private"))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public bool IsStaticSelf()
        {
            foreach (var item in ClassCollection.GetClasses())
            {
                ClassModel cls = item.Value;
                foreach (var property in cls.getProperties())
                {
                    if (property.ValueType.Equals(cls.Identifier))
                    {
                        foreach (var modifier in property.Modifiers)
                        {
                            if (modifier.ToLower().Equals("static"))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public bool IsGetInstance()
        {
            foreach(var item in ClassCollection.GetClasses())
            {
                ClassModel cls = item.Value;
                foreach (var method in cls.getMethods())
                {
                    if (method.ReturnType.Equals(cls.Identifier))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsCreateSelf()
        {
            foreach (var item in ClassCollection.GetClasses())
            {
                ClassModel cls = item.Value;

                foreach (var obj in cls.ObjectCreations)
                {

                    if (obj.Identifier.Equals(cls.Identifier))
                   {
                        return true;
                   }
                }
            }
            
            return false;
        }
    }
}