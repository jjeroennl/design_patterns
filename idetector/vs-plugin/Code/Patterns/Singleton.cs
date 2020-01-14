using System.Collections.Generic;
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
        private readonly ClassModel _cls;

        private readonly Dictionary<string, List<RequirementResult>> _results =
            new Dictionary<string, List<RequirementResult>>();

        private float _score;

        public Singleton(ClassModel cls)
        {
            _cls = cls;
        }

        public void Scan()
        {
            var list = new List<RequirementResult>();
            list.Add(IsStaticSelf());
            list.Add(IsCreateSelf());

            _results.Add(_cls.Identifier, list);

            IsPrivateConstructor();
            IsGetInstance();
        }

        public Dictionary<string, List<RequirementResult>> GetResults()
        {
            return _results;
        }


        public void IsPrivateConstructor()
        {
            foreach (var constructor in _cls.getConstructors())
                _results[_cls.Identifier].Add(
                    new RequirementResult("SINGLETON-PRIVATE-CONSTRUCTOR", constructor.HasModifier("private"), _cls,
                        constructor));
        }

        public RequirementResult IsStaticSelf()
        {
            var checkStatic = API.ClassHasPropertyOfType(_cls, _cls.Identifier, new[] {"private", "static"});

            return new RequirementResult("SINGLETON-STATIC-SELF", checkStatic, _cls);
        }

        public void IsGetInstance()
        {
            var instances = API.ClassGetMethodOfType(_cls, _cls.Identifier, new[] {"static"});
            foreach (var instance in instances)
            {
                _results[_cls.Identifier].Add(new RequirementResult("SINGLETON-GET-INSTANCE", true, _cls, instance));
                return;
            }
        }

        public RequirementResult IsCreateSelf()
        {
            var createSelf = API.ClassHasObjectCreationOfType(_cls, _cls.Identifier);

            return new RequirementResult("SINGLETON-CREATE-SELF", createSelf, _cls);
        }
    }
}