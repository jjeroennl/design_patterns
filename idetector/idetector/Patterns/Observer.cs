using idetector.Collections;
using idetector.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Type = System.Type;

namespace idetector.Patterns
{
    public class Observer : IPattern
    {

        private float _score;
        private readonly ClassCollection cc;
        private List<string> interfaces = new List<string>();
        private List<RequirementResult> _results = new List<RequirementResult>();


        public Observer(ClassCollection _cc)
        {
            cc = _cc;
        }

        public void Scan()
        {
            _score = 0;
            _results.Clear();
            _results.Add(HasObserverInterface());
            _results.Add(HasObserverRelations());


        }

        public List<RequirementResult> GetResult()
        {
            return _results;
        }

        public float Score()
        {
            return _score;
        }

        // Checks if there is an observer interface
        public RequirementResult HasObserverInterface()
        {
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                if (cls.Value.Modifiers != null)
                {
                    if (cls.Value.IsInterface)
                    {
                        // check of naam IObserver is?
                        // check of de interface een void functie heeft die een interface als parameter heeft?
                        // check of de naam van bovenstaande functie update is? @justin

                        return new RequirementResult("OBSERVER-HAS-OBSERVER-INTERFACE", true);
                    }
                }
            }
            return new RequirementResult("OBSERVER-HAS-OBSERVER-INTERFACE", false);
        }

        // Checks if there are relations between the subject and the observer(s)
        public RequirementResult HasObserverRelations()
        {
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                List<string> parents = cls.Value.GetParents();


                    // get all classes that extend the observer interface
                    // API: get all interfaces from tree
                    if (parents != null && parents.Contains("IObserver"))
                    {
                        interfaces.Add(cls.Value.Identifier);
                    }
            }

            // Op zoek naar collection
            // Op zoek of er een object is die een IEnumerable is
            // Checken of die IEnumerable alle interfaces die we hebben gevonden bevat
            // API: check if type of checked attribute is IEnumerable

            if (interfaces.Count > 0)
            {
                return new RequirementResult("OBSERVER-HAS-OBSERVER-RELATIONS", true);
            }
            return new RequirementResult("OBSERVER-HAS-OBSERVER-RELATIONS", false);
        }

    }
}
