using System.Collections.Generic;
using System.Linq;

namespace idetector.Patterns.Helper
{
    public class ParentList
    {
        private List<string> parents;

        public ParentList()
        {
            this.parents = new List<string>();
        }
        
        public void Add(string parent)
        {
            this.parents.Add(parent);
        }

        public bool HasParent(string parent)
        {
            return this.parents.Any(e => e == parent);
        }
        
        public List<string> ListParents()
        {
            return this.parents;
        }
    }
}