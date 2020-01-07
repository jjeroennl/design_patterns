using System;
using System.Collections.Generic;
using idetector.Collections;
using idetector.Models;

namespace idetector.Patterns
{
    /*ID's:
     * SINGLETON-PRIVATE-CONSTRUCTOR
     * SINGLETON-STATIC-SELF
     * SINGLETON-GET-INSTANCE
     * SINGLETON-CREATE-SELF
     */
    public class Singleton : IPattern
    {
        private float _score;
        private ClassModel cls;
        private List<RequirementResult> _results = new List<RequirementResult>();

        public Singleton(ClassModel _cls)
        {
            cls = _cls;

        }

        public void Scan()
        {
            _results.Add(IsPrivateConstructor());
            _results.Add(IsStaticSelf());
            _results.Add(IsGetInstance());
            _results.Add(IsCreateSelf());

        }

        public List<RequirementResult> GetResult()
        {
            return _results;
        }


        public RequirementResult IsPrivateConstructor()
        {
            foreach (var method in cls.getMethods())
            {
                if (method.isConstructor)
                {
                    foreach (var modifier in method.Modifiers)
                    {
                        if (modifier.ToLower().Equals("private"))
                        {
                            return new RequirementResult("SINGLETON-PRIVATE-CONSTRUCTOR", true);
                        }
                    }
                }
            }

            return new RequirementResult("SINGLETON-PRIVATE-CONSTRUCTOR", false); 
        }

        public RequirementResult IsStaticSelf()
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
                                return new RequirementResult("SINGLETON-STATIC-SELF", true);
                            }
                        }
                    }
                }
            }

            return new RequirementResult("SINGLETON-STATIC-SELF", false);
        }

        public RequirementResult IsGetInstance()
        {
            foreach (var method in cls.getMethods())
            {
                foreach (var modifier in method.Modifiers)
                {
                    if (modifier.ToLower().Equals("static") && !method.isConstructor)
                    {
                        if (method.ReturnType.Equals(cls.Identifier))
                        {
                            return new RequirementResult("SINGLETON-GET-INSTANCE", true);
                        }
                    }
                }
            }

            return new RequirementResult("SINGLETON-GET-INSTANCE", false);
        }

        public RequirementResult IsCreateSelf()
        {
            foreach (var obj in cls.ObjectCreations)
            {
                if (obj.Identifier.Equals(cls.Identifier))
                {
                    return new RequirementResult("SINGLETON-CREATE-SELF", true);
                }
            }

            return new RequirementResult("SINGLETON-CREATE-SELF", false);
        }
    }
}