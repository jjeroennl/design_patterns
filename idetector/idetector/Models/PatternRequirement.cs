using System;
using System.Collections.Generic;
using System.Text;

namespace idetector.Models
{
    public class PatternRequirement
    {
        public float Weight;
        public string Id;
        public string ErrorMessage;

        public PatternRequirement(string id,  float weight, string errormessage)
        {
            Weight = weight;
            ErrorMessage = errormessage;
            Id = id;
        }
    }
}
