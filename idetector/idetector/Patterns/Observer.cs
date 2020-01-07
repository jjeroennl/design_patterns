﻿using idetector.Collections;
using idetector.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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


        public Observer(ClassCollection _cc)
        {
            cc = _cc;
            
            PriorityCollection.ClearPriorities();
            PriorityCollection.AddPriority("observer", "HasUpdateFunction", Priority.Low);
            PriorityCollection.AddPriority("observer", "HasObserverInterfaceWithUpdateFunction", Priority.Medium);
            PriorityCollection.AddPriority("observer", "HasObserverInterfaceWithNamedUpdateFunction", Priority.High);
            PriorityCollection.AddPriority("observer", "HasSubjectFunctions", Priority.High);
            PriorityCollection.AddPriority("observer", "HasSubjectWithObserverList", Priority.Medium);
            PriorityCollection.AddPriority("observer", "ConcreteObserverExtendsIObserver", Priority.Medium);
        }

        public void Scan()
        {
            SetInterfaces();

            _score = 0;

            if (HasUpdateFunction())
            {
                _score += PriorityCollection.GetPercentage("observer", "HasUpdateFunction");
            }

            if (HasObserverInterfaceWithNamedUpdateFunction())
            {
                _score += PriorityCollection.GetPercentage("observer", "HasObserverInterfaceWithNamedUpdateFunction");
            } 
            else if (HasObserverInterfaceWithUpdateFunction())
            { 
                _score += PriorityCollection.GetPercentage("observer", "HasObserverInterfaceWithUpdateFunction");
            }

            if (HasSubjectFunctions())
            {
                _score += PriorityCollection.GetPercentage("observer", "HasSubjectFunctions");
            }

            if (HasSubjectWithObserverList())
            {
                _score += PriorityCollection.GetPercentage("observer", "HasSubjectWithObserverList");
            }

            if (ConcreteObserverExtendsIObserver())
            {
                _score += PriorityCollection.GetPercentage("observer", "ConcreteObserverExtendsIObserver");
            }
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
        public bool HasObserverInterfaceWithNamedUpdateFunction()
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
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        // Checks for existing interface with a void function that has an interface as parameter
        public bool HasObserverInterfaceWithUpdateFunction()
        {
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                if (cls.Value.Modifiers != null)
                {
                    if (cls.Value.IsInterface)
                    {
                        if (cls.Value.getMethods().Count > 1)
                        {
                            return false;
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
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        // Checks if there is an interface that has a void function called 'Update'
        public bool HasUpdateFunction()
        {
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                if (cls.Value.Modifiers != null)
                {
                    if (cls.Value.IsInterface)
                    {
                        foreach (var method in cls.Value.getMethods())
                        {
                            if (method.ReturnType.ToLower().Equals("void") &&
                                method.Identifier.ToLower().Equals("update"))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        // Checks for a subject class that has at least 2 void functions
        // that could indicate an observer pattern (subscribe & unsubscribe)
        public bool HasSubjectFunctions()
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
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        // Checks if there is a class that has a list of interfaces. 
        // If true, probably a subject class with a list filled with observers
        public bool HasSubjectWithObserverList()
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

                            ClassModel targetClass = cc.GetClass(target);

                            if (targetClass.IsInterface)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        // Checks if there is a class that extends 'IObserver'
        // names can be different though so idk if makes sense to check at all
        public bool ConcreteObserverExtendsIObserver()
        { 
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                List<string> parents = cls.Value.GetParents();
                if (parents != null)
                {
                    // ervan uitgaande dat observer interface altijd IObserver heet
                    if (parents.Contains("IObserver"))
                    {
                        ClassModel target = cc.GetClass(parents[0]);
                        if (target.IsInterface)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
