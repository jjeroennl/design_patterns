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
        private ClassModel _cls;
        private Dictionary<string, List<RequirementResult>> _results = new Dictionary<string, List<RequirementResult>>();

        public Singleton(ClassModel cls)
        {
            _cls = cls;
        }

        public void Scan()
        {
            _results.Add(_cls.Identifier, IsPrivateConstructor());
            _results.Add(_cls.Identifier, IsStaticSelf());
            _results.Add(_cls.Identifier, IsGetInstance());
            _results.Add(_cls.Identifier, IsCreateSelf());
        }

        public Dictionary<string, List<RequirementResult>> GetResults()
        {
            return _results;
        }


        public List<RequirementResult> IsPrivateConstructor()
        {
            foreach (var constructor in _cls.getConstructors())
            {
                if (!constructor.HasModifier("private"))
                {
                    return new RequirementResult("SINGLETON-PRIVATE-CONSTRUCTOR", false, _cls);
                }
            }

            return new RequirementResult("SINGLETON-PRIVATE-CONSTRUCTOR", true, _cls);
        }

        public List<RequirementResult> IsStaticSelf()
        {
            var checkStatic = API.ClassHasPropertyOfType(_cls, _cls.Identifier, new [] {"private", "static"});

            return new RequirementResult("SINGLETON-STATIC-SELF", checkStatic);
        }

        public List<RequirementResult> IsGetInstance()
        {
            var checkInstance = API.ClassHasMethodOfType(_cls, _cls.Identifier, new [] {"static"});
            return new RequirementResult("SINGLETON-GET-INSTANCE", checkInstance);
        }

        public List<RequirementResult> IsCreateSelf()
        {
            var createSelf = API.ClassHasObjectCreationOfType(_cls, _cls.Identifier);

            return new RequirementResult("SINGLETON-CREATE-SELF", createSelf);
        }
    }
}