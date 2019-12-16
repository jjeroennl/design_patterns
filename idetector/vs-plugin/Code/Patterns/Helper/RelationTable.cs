using System.Collections;
using System.Collections.Generic;

namespace idetector.Patterns.Helper
{
    public class RelationTable
    {
        protected Dictionary<string, ParentList> relations;

        public RelationTable()
        {
            this.relations = new Dictionary<string, ParentList>();
        }
        
        /// <summary>
        /// Add a relation between two classes
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void AddRelation(string from, string to)
        {
            if (!this.relations.ContainsKey(to))
            {
                this.relations[to] = new ParentList();
            }
            
            this.relations[to].Add(from);
        }

        /// <summary>
        /// Get specific relations to class
        /// Returns NULL when relation is not found
        /// </summary>
        /// <param name="to"></param>
        /// <returns>ParentList | NULL</returns>
        public ParentList GetRelationTo(string to)
        {
            return !this.relations.ContainsKey(to) ? null : this.relations[to];
        }
        
        /// <summary>
        /// Get specific relations to class
        /// Returns NULL when relation is not found
        /// </summary>
        /// <param name="to"></param>
        /// <returns>ParentList | NULL</returns>
        public List<string> GetRelationsFrom(string from)
        {
            var plist = new List<string>();
            foreach (var result in this.relations)
            {
                if (result.Value.ListParents().Contains(from))
                {
                    plist.Add(result.Key);
                }
            }

            return plist;
        }
        
        public override string ToString()
        {
            string returnVal = "";

            foreach(var relation in relations)
            {
                returnVal += relation.Key.ToString() + " -> " + string.Join( ",", relation.Value.ListParents().ToArray() ) + "\n";
            }
            
            return returnVal;
        }

        public Dictionary<string, ParentList> GetRelations()
        {
            return relations;
        }
    }
}