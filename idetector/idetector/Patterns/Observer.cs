using idetector.Collections;
using idetector.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace idetector.Patterns
{
    public class Observer : IPattern
    {

        private float _score;
        private Dictionary<ClassModel, int> _scores = new Dictionary<ClassModel, int>();
        private ClassCollection cc;
        private List<ClassModel> interfaces = new List<ClassModel>();
        private List<RequirementResult> _results = new List<RequirementResult>();


        public Observer(ClassCollection _cc)
        {
            cc = _cc;
        }

        public void Scan()
        {
            SetInterfaces();

            _score = 0;

            // doet bijna hetzelfde
            // 1 checkt of er een interface is met een void function genaamd update die een interface als parameter heeft
            // 2 checkt of er een interface is met een void function die een interface als parameter heeft
            _results.Add(HasObserverInterfaceWithUpdateFunction());
            _results.Add(HasObserverInterfaceWithNamedUpdateFunction());

            _results.Add(HasSubjectWithObserverList());
            _results.Add(HasSubjectFunctions());
            _results.Add(ConcreteObserverExtendsIObserver());

        }

        public List<RequirementResult> GetResult()
        {
            return _results;
        }

        private void SetInterfaces()
        {
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                if (cls.Value.IsInterface)
                {
                    interfaces.Add(cls.Value);
                }
            }
        }

        public int Score(ClassModel clsModel)
        {
            if (this._scores.ContainsKey(clsModel)) return this._scores[clsModel];
            return 0;
        }

        // Checks for existing interface with a void function that's named 'update' and has an interface as parameter
        public RequirementResult HasObserverInterfaceWithNamedUpdateFunction()
        {
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                if (cls.Value.Modifiers != null)
                {
                    if (cls.Value.IsInterface)
                    {
                        foreach (var method in cls.Value.getMethods())
                        {
                            if (method.ReturnType.ToLower().Equals("void") && method.Identifier.ToLower().Equals("update"))
                            {
                                // checks if parameter is an interface
                                string parameters = method.Parameters.Replace("(", string.Empty).Replace(")", string.Empty);
                                string[] paramList = parameters.Split(" ");
                                ClassModel targetClass = cc.GetClass(paramList[0]);
                                if (targetClass.IsInterface)
                                {
                                    return new RequirementResult("OBSERVER-HAS-INTERFACE-WITH-NAMED-UPDATE-FUNCTION", true);
                                }
                            }
                        }
                    }
                }
            }
            return new RequirementResult("OBSERVER-HAS-INTERFACE-WITH-NAMED-UPDATE-FUNCTION", false);
        }

        // Checks for existing interface with a void function that has an interface as parameter
        public RequirementResult HasObserverInterfaceWithUpdateFunction()
        {
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                if (cls.Value.Modifiers != null)
                {
                    if (cls.Value.IsInterface)
                    {
                        if (cls.Value.getMethods().Count > 1)
                        {
                            return new RequirementResult("OBSERVER-HAS-INTERFACE-WITH-UPDATE-FUNCTION", false);
                        }
                        else
                        {
                            foreach (var method in cls.Value.getMethods())
                            {
                                if (method.ReturnType.ToLower().Equals("void"))
                                {
                                    // checks if parameter is an interface
                                    string parameters = method.Parameters.Replace("(", string.Empty).Replace(")", string.Empty);
                                    string[] paramList = parameters.Split(" ");
                                    ClassModel targetClass = cc.GetClass(paramList[0]);
                                    if (targetClass.IsInterface)
                                    {
                                        return new RequirementResult("OBSERVER-HAS-INTERFACE-WITH-UPDATE-FUNCTION", true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return new RequirementResult("OBSERVER-HAS-INTERFACE-WITH-UPDATE-FUNCTION", false);
        }
        
        // Checks for a subject class that has at least 2 void functions
        // that could indicate an observer pattern (subscribe & unsubscribe)
        public RequirementResult HasSubjectFunctions()
        {
            int probability = 0;
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                if (cls.Value.Modifiers != null)
                {
                    foreach (var method in cls.Value.getMethods())
                    {
                        // does it have to be public idk
                        if (method.Modifiers.Contains("public") && method.ReturnType.ToLower().Equals("void"))
                        {
                            // checks if parameter is an interface
                            string parameters = method.Parameters.Replace("(", string.Empty).Replace(")", string.Empty);
                            string[] paramList = parameters.Split(" ");
                            ClassModel targetClass = cc.GetClass(paramList[0]);
                            // Checks if parameter is an observer interface or abstract class
                            if (targetClass.IsInterface || (targetClass.Modifiers.Contains("abstract")) )
                            {
                                probability++;
                                if (probability == 2)
                                {
                                    return new RequirementResult("OBSERVER-HAS-SUBJECT-FUNCTIONS", true);
                                }
                            }
                        }
                    }
                }
            }
            return new RequirementResult("OBSERVER-HAS-SUBJECT-FUNCTIONS", false);
        }

        // Checks if there is a class that has a list of interfaces. 
        // If true, probably a subject class with a list filled with observers
        public RequirementResult HasSubjectWithObserverList()
        {
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                if (cls.Value.Modifiers != null)
                {
                    foreach (var property in cls.Value.getProperties())
                    {
                        string identifier = property.ValueType;
                        // Usually observers are stored in some kind of collection, most often in a list (no checks for other collection types for now)
                        if (identifier.ToLower().Contains("list"))
                        {
                            int pFrom = identifier.IndexOf("<") + "<".Length;
                            int pTo = identifier.LastIndexOf(">");

                            string target = identifier.Substring(pFrom, pTo - pFrom);

                            ClassModel targetClass = cc.GetClass("IObserver");

                            // not sure if it has to be a list of interfaces or a list of classes that extend an interface but
                            // example code had a list of interfaces 
                            return new RequirementResult("OBSERVER-HAS-SUBJECT-WITH-OBSERVER-LIST", true);
                            }
                    }
                }
            }

            return new RequirementResult("OBSERVER-HAS-SUBJECT-WITH-OBSERVER-LIST", false);
        }

        // Checks if there is a class that extends 'IObserver'
        // names can be different though so idk if makes sense to check at all
        public RequirementResult ConcreteObserverExtendsIObserver()
        { 
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                List<string> parents = cls.Value.GetParents();
                if (parents != null)
                {
                    // ervan uitgaande dat observer interface altijd IObserver heet
                    // misschien aanpassen naar het checken van interface implemention en dan kijken of die interface weer voldoet aan
                    // de observer interface requirements ofzo
                    if (parents.Contains("IObserver"))
                    {
                        ClassModel target = cc.GetClass(parents[0]);
                        if (target.IsInterface)
                        {
                            return new RequirementResult("OBSERVER-CONCRETE-OBSERVER-EXTENDS-IOBSERVERS", true);
                        }
                    }
                }
            }

            return new RequirementResult("OBSERVER-CONCRETE-OBSERVER-EXTENDS-IOBSERVERS", false);
        }
    }
}
