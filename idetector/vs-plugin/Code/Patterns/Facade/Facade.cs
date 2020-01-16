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
        private Dictionary<string, List<RequirementResult>>_results = new Dictionary<string, List<RequirementResult>>();


        public Facade(ClassCollection collection)
        {
            this.collection = collection;
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

        public Dictionary<string, List<RequirementResult>> GetResults()

        {
            return _results;
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
                    List<string> classlist = new List<string>();

                    foreach (var modelName in group)
                    {
                        try
                        {
                            isFacade = RecursiveCheckGroup(parents, facade, collection.GetClass(modelName), isFacade, classlist);
                        }
                        catch (StackOverflowException exception)
                        {
                            //If a class from outside triggers the facade, and the facade triggers the class
                            //it creates a infinite loop
                            isFacade = false;
                        }
                    }

                    SaveScores(relation.Key.ToString(), facade, isFacade);
                }
            }
        }

        private void SaveScores(string identifier, List<string> facade, bool isFacade)
        {
            foreach (var className in facade)
            {
                if (!_results.ContainsKey(identifier))
                {
                    _results.Add(identifier, new List<RequirementResult>());
                }
                if (isFacade)
                {
                    _results[identifier].Add(new RequirementResult("FACADE-IS-FACADE", true, collection.GetClass(className)));
                }
                else
                {
                    _results[identifier].Add(new RequirementResult("FACADE-IS-FACADE", false, collection.GetClass(className)));
                }
            }
        }

        private bool RecursiveCheckGroup(RelationTable parents, List<string> group, ClassModel model, bool found, List<string> classlist)
        {

                foreach (var objectCreation in model.ObjectCreations)
                {
                    if (classlist.Contains(objectCreation.Identifier))
                    {
                        return found;
                    }
                    classlist.Add(objectCreation.Identifier);

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

                        found = RecursiveCheckGroup(parents, group, objectCreation, found, classlist);
                    }
                    else
                    {
                        return false;
                    }
                }

            return found;
        }
    }
}