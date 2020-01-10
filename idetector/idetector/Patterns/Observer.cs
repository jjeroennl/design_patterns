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

        private float _score;
        private readonly ClassCollection cc;
        private List<ClassModel> observers = new List<ClassModel>();
        private List<RequirementResult> _results = new List<RequirementResult>();
        private ClassModel IObserverClass;
        private ClassModel ISubjectClass;
        private int probability = 0;

        public Observer(ClassCollection _cc)
        {
            cc = _cc;
        }

        public void Scan()
        {
            _score = 0;
            _results.Clear();
            _results.Add(HasObserverInterface());
            _results.Add(HasSubjectInterface());
        }

        public List<RequirementResult> GetResult()
        {
            return _results;
        }

        public float Score()
        {
            return _score;
        }

        // Checks if there is an observer interface/abstract class
        public RequirementResult HasObserverInterface()
        {
            List<ClassModel> interfacesAndAbstracts = API.ListInterfacesAndAbstracts(cc);
            foreach (var cls in interfacesAndAbstracts)
            {
                // observer interface heeft alleen een void update function
                if (cls.getMethods().Count <= 1)
                {
                    foreach (var method in cls.getMethods())
                    {
                        if (method.ReturnType == "void" && !method.Modifiers.Contains("private"))
                        {
                            IObserverClass = cls;
                            return new RequirementResult("OBSERVER-HAS-OBSERVER-INTERFACE", true);
                        }
                    }
                }
            }
            return new RequirementResult("OBSERVER-HAS-OBSERVER-INTERFACE", false);
        }

        public RequirementResult HasSubjectInterface()
        {
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                foreach (var method in cls.Value.getMethods())
                {
                    if (method.ReturnType == "void")
                    {
                        probability++;
                        if (probability >= 2)
                        {
                            ISubjectClass = cls.Value;
                            return new RequirementResult("OBSERVER-HAS-SUBJECT-INTERFACE", true);
                        }

                    }
                }
            }
            return new RequirementResult("OBSERVER-HAS-SUBJECT-INTERFACE", false);
        }

        // Checks if there are relations between the subject and the observer(s)
        public RequirementResult HasObserverRelations()
        {
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                List<string> parents = cls.Value.GetParents();

                // get all classes that extend the observer interface
                // API: get all interfaces from tree
                if (parents != null && parents.Contains(IObserverClass.Identifier))
                {
                    observers.Add(cls.Value);
                }
            }

            foreach (var property in ISubjectClass.getProperties())
            { 
                string t1 = property.ValueType;

                Debug.WriteLine("ValueType: " + t1);
                Debug.WriteLine("Class: " + IObserverClass.Identifier);
                if (observers.Count >= 1 && t1.Contains(IObserverClass.Identifier))
                {
                    return new RequirementResult("OBSERVER-HAS-OBSERVER-RELATIONS", true);
                }
            }

            // Op zoek naar collection
            // Op zoek of er een object is die een IEnumerable is
            // Checken of die IEnumerable alle interfaces die we hebben gevonden bevat
            // API: check if type of checked attribute is IEnumerable

            return new RequirementResult("OBSERVER-HAS-OBSERVER-RELATIONS", false);
        }
    }
}