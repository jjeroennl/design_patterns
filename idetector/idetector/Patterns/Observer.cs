using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using idetector.Collections;
using idetector.Models;
using idetector.Patterns.Helper;

namespace idetector.Patterns
{
    public class Observer : IPattern
    {
        private readonly ClassCollection _cc;

        private readonly Dictionary<string, List<RequirementResult>>
            _results = new Dictionary<string, List<RequirementResult>>();

        private bool hasRelations;
        private ClassModel IObserverClass;
        private ClassModel ISubjectClass;
        private readonly List<ClassModel> observers = new List<ClassModel>();
        private int probability;
        private readonly List<ClassModel> subjects = new List<ClassModel>();
        private List<ClassModel> interfaces;

        public Observer(ClassCollection cc)
        {
            _cc = cc;
            interfaces = API.ListInterfacesAndAbstracts(_cc);
        }

        public void Scan()
        {
            foreach (var c in _cc.GetClasses())
            {
                _results.Add(c.Value.Identifier, new List<RequirementResult>());
            }

            HasObserverInterface();
            HasSubjectInterface();
            if (IObserverClass != null)
            {
                if (ISubjectClass != null) HasObserversAndSubjects();
                if (observers != null && subjects != null) HasObserverRelations();
            }

            foreach (var c in _cc.GetClasses())
            {
                foreach (var item in _results[c.Value.Identifier].ToArray())
                {
                    Debug.Write("ID: " + item.Id);
                    Debug.Write(", Passed: " + item.Passed);
                    Debug.Write(", Class: " + item.Class.Identifier);
                    Debug.WriteLine("");
                }
            }
        }

        public Dictionary<string, List<RequirementResult>> GetResults()
        {
            return _results;
        }

        /// <summary>
        ///     Checking if there is a class which suffices as a 'Object' interface
        /// </summary>
        public void HasObserverInterface()
        {
            foreach (var cls in interfaces)
            {
                if (cls.getMethods().Count == 1 && cls.getMethods()[0].ReturnType == "void" &&
                    !cls.getMethods()[0].Modifiers.Contains("private"))
                {
                    IObserverClass = cls;
                    Debug.WriteLine("HasObserverInterface Added: " + cls.Identifier + " true");
                    _results[cls.Identifier].Add(new RequirementResult("OBSERVER-HAS-OBSERVER-INTERFACE", true, cls));
                }


                bool add = true;
                foreach (var result in _results[cls.Identifier].ToArray())
                {
                    if (result.Id.Equals("OBSERVER-HAS-OBSERVER-INTERFACE")) add = false;
                }

                if (add)
                {
                    Debug.WriteLine("HasObserverInterface Added: " + cls.Identifier + " false");
                    _results[cls.Identifier]
                        .Add(new RequirementResult("OBSERVER-HAS-OBSERVER-INTERFACE", false, cls));
                }

            }
        }

        /// <summary>
        ///     Checking if there is a class which suffices as a 'Subject' interface
        /// </summary>
        public void HasSubjectInterface()
        {
            foreach (var cls in interfaces)
            {
                foreach (var method in cls.getMethods())
                {
                    if (method.ReturnType == "void")
                    {
                        probability++;
                        // als er 3 voids zijn of meer dan wss een subject interface
                        if (probability >= 2)
                        {
                            ISubjectClass = cls;
                            Debug.WriteLine("HasSubjectInterface Added: " + cls.Identifier + " true");
                            _results[cls.Identifier]
                                .Add(new RequirementResult("OBSERVER-HAS-SUBJECT-INTERFACE", true, cls));
                        }
                    }

                    break;
                }
                bool add = true;
                    foreach (var result in _results[cls.Identifier].ToArray())
                    {
                        if (result.Id.Equals("OBSERVER-HAS-SUBJECT-INTERFACE")) add = false;
                    }

                    if (add)
                    {
                        Debug.WriteLine("HasSubjectInterface Added: " + cls.Identifier + " false");
                        _results[cls.Identifier]
                            .Add(new RequirementResult("OBSERVER-HAS-SUBJECT-INTERFACE", false, cls));

                    }
                
            }
        }

        // Checks if there are relations between the subject and the observer(s)
        public void HasObserverRelations()
        {
            foreach (var subject in subjects)
            {
                foreach (var property in subject.getProperties())
                {
                    //checkt alleen voor dingen die parameters hebben zoals List<Interface> etc.
                    var collectionType = property.ValueType;

                    if (collectionType.Contains(IObserverClass.Identifier) && collectionType != null &&
                        observers.Count > 0)
                    {
                        hasRelations = true;
                        Debug.WriteLine("HasObserverRelations Added: " + subject.Identifier + " true, " +
                                        subject.Identifier);
                        _results[subject.Identifier]
                            .Add(new RequirementResult("OBSERVER-HAS-OBSERVER-RELATIONS", true, subject));
                    }
                }

                if (!hasRelations)
                {
                    bool add = true;
                    foreach (var result in _results[subject.Identifier].ToArray())
                    {
                        if (result.Id.Equals("OBSERVER-HAS-OBSERVER-RELATIONS")) add = false;
                    }

                    if (add)
                    {
                        Debug.WriteLine("HasObserverRelations Added: " + subject.Identifier + " false, " +
                                        subject.Identifier);
                        _results[subject.Identifier]
                            .Add(new RequirementResult("OBSERVER-HAS-OBSERVER-RELATIONS", false, subject));
                    }
                }

            }
        }

        public void HasObserversAndSubjects()
        {
            foreach (var cls in _cc.GetClasses())
            {
                var parents = cls.Value.GetParents();

                if (parents != null && parents.Count > 0 && IObserverClass != null && ISubjectClass != null)
                {
                    if (parents.Contains(IObserverClass.Identifier))
                    {
                        observers.Add(cls.Value);
                        Debug.WriteLine("HasObserversAndSubjects Added: " + cls.Value.Identifier + " true, " +
                                        IObserverClass.Identifier);
                        _results[cls.Value.Identifier]
                            .Add(new RequirementResult("OBSERVER-HAS-OBSERVERS-AND-SUBJECTS", true, cls.Value));
                    }

                    else if (parents.Contains(ISubjectClass.Identifier))
                    {
                        subjects.Add(cls.Value);
                        Debug.WriteLine("HasObserversAndSubjects Added: " + cls.Value.Identifier + " true, " +
                                        ISubjectClass.Identifier);
                        _results[cls.Value.Identifier]
                            .Add(new RequirementResult("OBSERVER-HAS-OBSERVERS-AND-SUBJECTS", true, cls.Value));
                    }
                }
                else
                {
                    Debug.WriteLine("HasObserversAndSubjects Added: " + cls.Value.Identifier + " false");
                    _results[cls.Value.Identifier]
                        .Add(new RequirementResult("OBSERVER-HAS-OBSERVERS-AND-SUBJECTS", false, cls.Value));
                }
            }
        }
    }
}