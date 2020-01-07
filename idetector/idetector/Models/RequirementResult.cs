using System.Collections.Generic;

namespace idetector.Models
{
    public class RequirementResult
    {
        public string Id;
        public bool Passed;

        public RequirementResult(string id, bool passed)
        {
            Id = id;
            Passed = passed;
        }
    }
}