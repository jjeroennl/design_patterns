using System;
using idetector.Collections;
using idetector.Models;

namespace idetector.Patterns
{
    public class Singleton : IPattern
    {
        private int _score;
        private ClassModel cls;

        public Singleton(ClassModel _cls)
        {
            cls = _cls;


            PriorityCollection.AddPriority("singleton", "IsPrivateConstructor", Priority.Low);
            PriorityCollection.AddPriority("singleton", "IsStaticSelf", Priority.Low);
            PriorityCollection.AddPriority("singleton", "IsGetInstance", Priority.Low);
            PriorityCollection.AddPriority("singleton", "IsCreateSelf", Priority.Low);

        }

        public void Scan()
        {
            if (IsPrivateConstructor())
            {
                _score += PriorityCollection.GetPercentage("singleton", "IsPrivateConstructor");
            }

            if (IsStaticSelf())
            {
                _score += PriorityCollection.GetPercentage("singleton", "IsStaticSelf");
            }

            if (IsGetInstance())
            {
                _score += PriorityCollection.GetPercentage("singleton", "IsGetInstance");
            }

            if (IsCreateSelf())
            {
                _score += PriorityCollection.GetPercentage("singleton", "IsCreateSelf");
            }
        }

        public bool IsSingleton()
        {
            return _score > 59;
        }

        public int Score()
        {
            return _score;
        }

        public bool IsPrivateConstructor()
        {
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

            return false;
        }

        public bool IsStaticSelf()
        {
            foreach (var property in cls.getProperties())
            {
                if (property.ValueType.Equals(cls.Identifier))
                {
                    if (property.Modifiers[0].ToLower().Equals("private"))
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
            foreach (var method in cls.getMethods())
            {
                foreach (var modifier in method.Modifiers)
                {
                    if (modifier.ToLower().Equals("static") && !method.isConstructor)
                    {
                        if (method.ReturnType.Equals(cls.Identifier))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool IsCreateSelf()
        {
            foreach (var obj in cls.ObjectCreations)
            {
                if (obj.Identifier.Equals(cls.Identifier))
                {
                    return true;
                }
            }

            return false;
        }
    }
}