using System;
using System.Collections.Generic;
using System.Linq;
using idetector.Collections;

namespace idetector.Patterns.Helper
{
    public class FacadeRelationTable: RelationTable
    {
        public FacadeRelationTable ListClassesWithSingleParent()
        {
            var r = new FacadeRelationTable();
            foreach (var result in this.relations)
            {
                if (result.Value.ListParents().Count != 1)
                {
                    continue;
                }
                
                r.AddRelation(result.Key.ToString(), result.Value.ListParents()[0]);
            }

            return r;
        }

        public bool MatchWithParents(FacadeRelationTable table)
        {
            throw new System.NotImplementedException();
        }
    }
}