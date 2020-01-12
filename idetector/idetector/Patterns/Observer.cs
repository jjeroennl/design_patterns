using idetector.Collections;
using idetector.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using idetector.Patterns.Helper;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Type = System.Type;

namespace idetector.Patterns
{
    public class Observer : IPattern
    {
        private readonly ClassCollection _cc;
        private List<ClassModel> observers = new List<ClassModel>();
        private List<ClassModel> subjects = new List<ClassModel>();
        private ClassModel IObserverClass;
        private ClassModel ISubjectClass;
        private int probability = 0;

        private Dictionary<string, List<RequirementResult>>
            _results = new Dictionary<string, List<RequirementResult>>();

        public Observer(ClassCollection cc)
        {
            _cc = cc;
        }

        public void Scan()
        {
            _results.Clear();
            HasObserverInterface();
            //HasSubjectInterface();
        }

        public Dictionary<string, List<RequirementResult>> GetResults()
        {
            return _results;
        }

        /// <summary>
        /// Checking if there is a class which suffices as a 'Object' interface
        /// </summary>
        public void HasObserverInterface()
        {
            var interfaces = API.ListInterfaces(_cc);
            var abstracts = API.ListAbstract(_cc);
            var classes = interfaces.Concat(abstracts);
            var classes2 = _cc.GetClasses();

            foreach (var cls in classes2)
            {
                foreach (var method in cls.Value.getMethods())
                {
                    if (method.ReturnType == "void" && !method.Modifiers.Contains("private")) // evt check of maar 1 method is
                    {
                        IObserverClass = cls.Value;
                        _results.Add(cls.Value.Identifier, new List<RequirementResult>());
                        _results[cls.Value.Identifier].Add(new RequirementResult("OBSERVER-HAS-OBSERVER-INTERFACE", true, IObserverClass));
                        // done??
                    }
                }
                // ja wat idk wat dit moet voorstellen
                //_results.Add(cls.Identifier, new List<RequirementResult>());
                //_results[cls.Identifier].Add(new RequirementResult("OBSERVER-HAS-OBSERVER-INTERFACE", false, cls));
            }
        }

        /// <summary>
        /// Checking if there is a class which suffices as a 'Subject' interface
        /// </summary>
        public void HasSubjectInterface()
        {
            var interfaces = API.ListInterfaces(_cc);
            var abstracts = API.ListAbstract(_cc);
            var classes = interfaces.Concat(abstracts);

            foreach (var cls in classes)
            {
                foreach (var method in cls.getMethods())
                {
                    if (method.ReturnType == "void")
                    {
                        probability++;
                        // als er 3 voids zijn of meer dan wss een subject interface
                        if (probability > 2)
                        {
                            ISubjectClass = cls;
                            _results.Add(cls.Identifier, new List<RequirementResult>());
                            _results[cls.Identifier].Add(new RequirementResult("OBSERVER-HAS-SUBJECT-INTERFACE", true, ISubjectClass));
                        }

                    }
                }
                _results.Add("null", new List<RequirementResult>());
                _results["null"].Add(new RequirementResult("OBSERVER-HAS-OBSERVER-INTERFACE", false, cls));
            }
        }

        // Checks if there are relations between the subject and the observer(s)
        public void HasObserverRelations()
        {
            foreach (KeyValuePair<string, ClassModel> cls in _cc.GetClasses())
            {
                List<string> parents = cls.Value.GetParents();

                if (parents != null && parents.Contains(IObserverClass.Identifier))
                {
                    observers.Add(cls.Value);
                }
                else if (parents != null && parents.Contains(ISubjectClass.Identifier))
                {
                    subjects.Add(cls.Value);
                }

                foreach (var subject in subjects)
                {
                    foreach (var property in subject.getProperties())
                    {
                        // list type
                        string collectionType = property.ValueType;
                        //Debug.WriteLine("ValueType: " + collectionType);
                        //Debug.WriteLine("Class: " + IObserverClass.Identifier);
                        if (observers.Count >= 1 && collectionType.Contains(IObserverClass.Identifier))
                        {
                            // er is een collection met als parameter de iobserver interface en er is minstens een observer klasse
                            _results.Add(cls.Value.Identifier, new List<RequirementResult>());
                            _results[cls.Value.Identifier].Add(new RequirementResult("OBSERVER-HAS-OBSERVER-RELATIONS", true, IObserverClass));
                        }
                    }
                    _results.Add(cls.Value.Identifier, new List<RequirementResult>());
                    _results[cls.Value.Identifier].Add(new RequirementResult("OBSERVER-HAS-OBSERVER-RELATIONS", true, IObserverClass));
                }
            }
        }
    }
}