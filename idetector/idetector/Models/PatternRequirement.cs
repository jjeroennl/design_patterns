using System;
using System.Collections.Generic;
using System.Text;

namespace idetector.Models
{
    public class PatternRequirement
    {
        public float Weight;
        public string Id;
        public string Title;
        public string Description;
        public string ErrorMessage;

        public PatternRequirement(string id, string title, string description, string errormessage, float weight = 1)
        {
            Weight = weight;
            ErrorMessage = errormessage;
            Id = id;
            Title = title;
            Description = description;
        }
    }
}