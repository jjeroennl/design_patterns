using idetector.Collections;
using idetector.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private bool hasRelations = false;

        private Dictionary<string, List<RequirementResult>>
            _results = new Dictionary<string, List<RequirementResult>>();

        public Observer(ClassCollection cc)
        {
            _cc = cc;
        }
        public void AddResult(ClassModel correspondingClass, string type, bool passed)
        {
            foreach (var record in _results)
            {
                // if correspondingcls identifier is already in class only add a new line with type and bool and class,
                // otherwise make a new record with said identifier
                Debug.WriteLine("HERE: " + record.Key);
                if (record.Key == correspondingClass.Identifier)
                {
                    Debug.WriteLine("Added new record: " + correspondingClass.Identifier);
                    _results[correspondingClass.Identifier]
                        .Add(new RequirementResult(type, passed, correspondingClass));
                }
                else
                {
                    Debug.WriteLine("Added new sub-record: " + correspondingClass.Identifier);
                    _results.Add(correspondingClass.Identifier, new List<RequirementResult>());
                    _results[correspondingClass.Identifier]
                        .Add(new RequirementResult(type, passed, correspondingClass));

                }
            }
        }

        public void Scan()
        {
            // als die eerste niet true is de rest ook niet doen
            // dingen worden niet geadd door addresult?
            HasObserverInterface();
            if (IObserverClass != null)
            {
                HasSubjectInterface();
                HasObserversAndSubjects();
                if (observers != null && subjects != null) HasObserverRelations();
            }
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

            foreach (var cls in classes)
            {
                if (cls.getMethods().Count == 1 && cls.getMethods()[0].ReturnType == "void" && cls.getMethods()[0].Modifiers.Contains("private"))
                {
                    IObserverClass = cls;
                    AddResult(cls, "OBSERVER-HAS-OBSERVER-INTERFACE", true);
                }
                else AddResult(cls, "OBSERVER-HAS-OBSERVER-INTERFACE", false);
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
                            AddResult(cls, "OBSERVER-HAS-SUBJECT-INTERFACE", true);
                        }
                    }
                    break;
                }
                if (ISubjectClass == null) AddResult(cls, "OBSERVER-HAS-SUBJECT-INTERFACE", false);
            }
        }

        // Checks if there are relations between the subject and the observer(s)
        public void HasObserverRelations()
        {
            foreach (KeyValuePair<string, ClassModel> cls in _cc.GetClasses())
            {
                foreach (var subject in subjects)
                {
                    foreach (var property in subject.getProperties())
                    {
                        // checkt alleen voor dingen die parameters hebben zoals List<Interface> etc.
                        string collectionType = property.ValueType;
                        if (observers.Count >= 1 && collectionType.Contains(IObserverClass.Identifier))
                        {
                            hasRelations = true;
                            AddResult(cls.Value, "OBSERVER-HAS-OBSERVER-RELATIONS", true);
                        }
                    }
                    if (!hasRelations) AddResult(cls.Value, "OBSERVER-HAS-OBSERVER-RELATIONS", false);
                }
            }
        }

        public void HasObserversAndSubjects()
        {
            foreach (KeyValuePair<string, ClassModel> cls in _cc.GetClasses())
            {
                List<string> parents = cls.Value.GetParents();

                if (parents != null && IObserverClass != null && ISubjectClass != null)
                {
                    if (parents.Contains(IObserverClass.Identifier))
                    {
                        observers.Add(cls.Value);
                        AddResult(cls.Value, "OBSERVER-HAS-OBSERVERS-AND-SUBJECTS", true);
                    }

                    else if (parents.Contains(ISubjectClass.Identifier))
                    {
                        subjects.Add(cls.Value);
                        AddResult(cls.Value, "OBSERVER-HAS-OBSERVERS-AND-SUBJECTS", true);
                    }
                }
                AddResult(cls.Value, "OBSERVER-HAS-OBSERVERS-AND-SUBJECTS", false);
            }
        }
    }
}
