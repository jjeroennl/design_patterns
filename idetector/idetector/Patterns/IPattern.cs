using System.Collections.Generic;
using idetector.Models;

namespace idetector.Patterns
{
    public interface IPattern
    {
        void Scan();
        Dictionary<string, RequirementResult> GetResults();
    }
}