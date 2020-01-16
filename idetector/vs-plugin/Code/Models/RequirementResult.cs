using System.Collections.Generic;

namespace idetector.Models
{
    public class RequirementResult
    {
        public string Id;
        public bool Passed;

        public ClassModel @Class;
        public MethodModel @Method;

        public RequirementResult(string id, bool passed, ClassModel cls, MethodModel mthd = null)
        {
            Id = id;
            Passed = passed;
            @Class = cls;

            if (mthd == null)
            {
                @Method = mthd;
            }
        }
    }
}