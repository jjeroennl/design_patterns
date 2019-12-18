using System;
using System.Collections.Generic;
using idetector.Collections;
using idetector.Models;
using idetector.Patterns.Helper;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace idetector.Patterns.Facade
{
    public class Facade : IPattern
    {
        private ClassCollection collection;
        private int _score = 0;
        private List<ClassModel> results = new List<ClassModel>();
        private Dictionary<string, int> _scores;

        public Facade(ClassCollection collection)
        {
            this.collection = collection;
            this._scores = new Dictionary<string, int>();
        }

        public void Scan()
        {
            FacadeRelationTable table = new FacadeRelationTable();
            foreach (var model in collection.GetClasses())
            {
                if (model.Value.Identifier.Equals("Program"))
                {
                    continue;
                }

                foreach (var objects in model.Value.ObjectCreations)
                {
                    table.AddRelation(model.Value.Identifier, objects.Identifier);
                }
            }

            this.FacadeCheck(collection, table, table.ListClassesWithSingleParent());
        }

        public void FacadeCheck(ClassCollection collection, RelationTable parents, RelationTable relations)
        {
            foreach (var relation in relations.GetRelations())
            {
                var group = relation.Value.ListParents();
                group.Add(relation.Key.ToString());
                var facade = new List<string>();
                foreach (var item in group)
                {
                    facade.Add(item);
                }

                if (group.Count > 2)
                {
                    bool isFacade = true;

                    foreach (var modelName in group)
                    {
                        isFacade = RecursiveCheckGroup(parents, facade, collection.GetClass(modelName), isFacade);
                    }

                    var scoreInt = 0;
                    if (isFacade)
                    {
                        scoreInt = 100;
                    }

                    this.SaveScores(facade, scoreInt);
                }
            }
        }

        private void SaveScores(List<String> facade, int scoreInt)
        {
            foreach (var className in facade)
            {
                if (this._scores.ContainsKey(className))
                {
                    if (this._scores[className] < scoreInt)
                    {
                        this._scores[className] = scoreInt;
                    }
                }
                else
                {
                    this._scores[className] = scoreInt;
                }
            }
        }

        private bool RecursiveCheckGroup(RelationTable parents, List<string> group, ClassModel model, bool found)
        {
            foreach (var objectCreation in model.ObjectCreations)
            {
                var result = parents.GetRelationTo(objectCreation.Identifier);
                if (result != null)
                {
                    foreach (var p in result.ListParents())
                    {
                        if (!group.Contains(p))
                        {
                            return false;
                        }
                        else
                        {
                            if (!group.Contains(objectCreation.Identifier))
                            {
                                group.Add(objectCreation.Identifier);
                            }
                        }
                    }

                    found = RecursiveCheckGroup(parents, group, objectCreation, found);
                }
                else
                {
                    return false;
                }
            }


            return found;
        }

        public int Score(ClassModel item)
        {
            if (!this._scores.ContainsKey(item.Identifier.ToString()))
            {
                return 0;
            }
            else
            {
                return this._scores[item.Identifier.ToString()];
            }
        }

        public int Score(string item)
        {
            if (!this._scores.ContainsKey(item.ToString()))
            {
                return 0;
            }
            else
            {
                return this._scores[item.ToString()];
            }
        }

        public List<ClassModel> Results()
        {
            return results;
        }
    }
}