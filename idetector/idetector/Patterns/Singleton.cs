using System;
using System.Collections.Generic;
using idetector.Collections;
using idetector.Models;
using idetector.Patterns.Helper;

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
        private Dictionary<string, RequirementResult> _results = new Dictionary<string, RequirementResult>();

        public Singleton(ClassModel _cls)
        {
            cls = _cls;
        }

        public void Scan()
        {
            _results.Add(cls.Identifier, IsPrivateConstructor());
            _results.Add(cls.Identifier, IsStaticSelf());
            _results.Add(cls.Identifier, IsGetInstance());
            _results.Add(cls.Identifier, IsCreateSelf());
        }

        public Dictionary<string, RequirementResult> GetResults()
        {
            return _results;
        }


        public RequirementResult IsPrivateConstructor()
        {
            foreach (var constructor in cls.getConstructors())
            {
                if (!constructor.HasModifier("private"))
                {
                    return new RequirementResult("SINGLETON-PRIVATE-CONSTRUCTOR", false);
                }
            }

            return new RequirementResult("SINGLETON-PRIVATE-CONSTRUCTOR", true);
        }

        public RequirementResult IsStaticSelf()
        {
            var checkStatic = API.ClassHasPropertyOfType(cls, cls.Identifier, new [] {"private", "static"});

            return new RequirementResult("SINGLETON-STATIC-SELF", checkStatic);
        }

        public RequirementResult IsGetInstance()
        {
            var checkInstance = API.ClassHasMethodOfType(cls, cls.Identifier, new [] {"static"});
            return new RequirementResult("SINGLETON-GET-INSTANCE", checkInstance);
        }

        public RequirementResult IsCreateSelf()
        {
            var createSelf = API.ClassHasObjectCreationOfType(cls, cls.Identifier);

            return new RequirementResult("SINGLETON-CREATE-SELF", createSelf);
        }
    }
}