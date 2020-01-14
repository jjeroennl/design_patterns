using System;
using System.Collections;
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
    public class Iterator : IPattern
    {
        private float _score;
        private ClassModel _cls;
        private  Dictionary<string, List<RequirementResult>>  _results = new Dictionary<string, List<RequirementResult>>() ;

        public Iterator(ClassModel cls)
        {
            _cls = cls;
        }

        public void Scan()
        {
            HasNext();
            HasReset();

        }

        private void HasNext()
        {
            var next = API.ClassGetMethodWithName(this._cls, "MoveNext");
            if (next != null)
            {
                _results[_cls.Identifier].Add(new RequirementResult("ITERATOR-HAS-NEXT", true, _cls, next ));
            }
            else
            {
                _results[_cls.Identifier].Add(new RequirementResult("ITERATOR-HAS-NEXT", false, _cls));
            }
        }


        private void HasReset()
        {
            var next = API.ClassGetMethodWithName(this._cls, "Reset");
            if (next != null)
            {
                _results[_cls.Identifier].Add(new RequirementResult("ITERATOR-HAS-RESET", true, _cls, next));
            }
            else
            {
                _results[_cls.Identifier].Add(new RequirementResult("ITERATOR-HAS-RESET", false, _cls));
            }
        }

        public Dictionary<string, List<RequirementResult>> GetResults()
        {
            return _results;
        }


       
    }

}
