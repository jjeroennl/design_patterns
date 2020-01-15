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
        public string WikipediaURL;

        public PatternRequirement(string id, string title, string description, string errorMessage, string wikiLink = "", float weight = 1)
        {
            Weight = weight;
            ErrorMessage = errorMessage;
            Id = id;
            Title = title;
            Description = description;
            WikipediaURL = wikiLink;
        }
    }
}