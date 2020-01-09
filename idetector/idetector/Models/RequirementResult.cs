using System.Collections.Generic;

namespace idetector.Models
{
    public class RequirementResult
    {
        public string Id;
        public bool Passed;
        public ClassModel Cls;

        public RequirementResult(string id, bool passed, ClassModel cls)
        {
            Id = id;
            Passed = passed;
            Cls = cls;
        }
    }
}