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
            if (IObserverClass != null)
            {
                HasSubjectInterface();
                if (ISubjectClass != null) HasObserversAndSubjects();
                if (observers != null && subjects != null) HasObserverRelations();
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
                if (cls.GetMethods().Count == 1 && cls.GetMethods()[0].ReturnType == "void" &&
                    !cls.GetMethods()[0].Modifiers.Contains("private"))
                {
                    IObserverClass = cls;
                    _results[IObserverClass.Identifier]
                        .Add(new RequirementResult("OBSERVER-HAS-OBSERVER-INTERFACE", true, cls));
                }
            }

            if (IObserverClass == null)
            {
                _results[IObserverClass.Identifier]
                    .Add(new RequirementResult("OBSERVER-HAS-OBSERVER-INTERFACE", false, IObserverClass));
            }
        }

        /// <summary>
        ///     Checking if there is a class which suffices as a 'Subject' interface
        /// </summary>
        public void HasSubjectInterface()
        {
            foreach (var cls in interfaces)
            {
                foreach (var method in cls.GetMethods())
                {
                    if (method.ReturnType == "void")
                    {
                        probability++;
                        if (probability >= 2)
                        {
                            ISubjectClass = cls;
                            _results[IObserverClass.Identifier]
                                .Add(new RequirementResult("OBSERVER-HAS-SUBJECT-INTERFACE", true, cls));
                            break;
                        }
                    }
                }
            }

            if (ISubjectClass == null)
            {
                _results[IObserverClass.Identifier]
                        .Add(new RequirementResult("OBSERVER-HAS-SUBJECT-INTERFACE", false, IObserverClass));
            }
        }

        /// <summary>
        ///     Checking to see if there is a relation between a subject and its observer(s)
        /// </summary>
        public void HasObserverRelations()
        {
            foreach (var subject in subjects)
            {
                foreach (var property in subject.GetProperties())
                {
                    //Checkt alleen voor collections die parameters hebben zoals List<Interface> etc.
                    var vT = property.ValueType;
                    if (vT.Contains(IObserverClass.Identifier.Replace("I", "")) &&
                        observers.Count > 0)
                    {
                        hasRelations = true;
                        _results[IObserverClass.Identifier]
                            .Add(new RequirementResult("OBSERVER-HAS-OBSERVER-RELATIONS", true, subject));
                    }
                }
            }

            if (!hasRelations)
            {
                _results[IObserverClass.Identifier]
                    .Add(new RequirementResult("OBSERVER-HAS-OBSERVER-RELATIONS", false, IObserverClass));
            }
        }

        /// <summary>
        ///     Checking to see if there are 'concrete' observers & subjects that implement their corresponding interface
        /// </summary>
        public void HasObserversAndSubjects()
        {
            foreach (var cls in _cc.GetClasses())
            {
                var parents = cls.Value.GetParents();
                if (parents != null && parents.Count > 0 && IObserverClass != null && ISubjectClass != null)
                {
                    if (parents.Contains(IObserverClass.Identifier)) observers.Add(cls.Value);
                    else if (parents.Contains(ISubjectClass.Identifier)) subjects.Add(cls.Value);
                }
            }

            if (observers.Count > 0 && subjects.Count > 0)
            {
                _results[IObserverClass.Identifier]
                    .Add(new RequirementResult("OBSERVER-HAS-OBSERVERS-AND-SUBJECTS", true, IObserverClass));
            }
            else
            {
                _results[IObserverClass.Identifier]
                    .Add(new RequirementResult("OBSERVER-HAS-OBSERVERS-AND-SUBJECTS", false, IObserverClass));
            }
        }
    }
}