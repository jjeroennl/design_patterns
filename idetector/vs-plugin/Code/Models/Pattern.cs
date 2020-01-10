using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace idetector.Models
{
    public class Pattern
    {
        public ClassModel @Class;
        public int Score;
        public List<RequirementResult> RequirementResults;

        public Pattern(ClassModel cls, int score, List<RequirementResult> requirements)
        {
            this.Class = cls;
            this.Score = score;
            this.RequirementResults = requirements; 
        }
    }
}
