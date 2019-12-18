using idetector.Collections;
using idetector.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace idetector.Patterns
{
    public class Observer : IPattern
    {

        private int _score;
        private ClassCollection cc;


        public Observer(ClassCollection _cc)
        {
            cc = _cc;
        }

        public void Scan()
        {
            throw new NotImplementedException();
        }

        public int Score()
        {
            return _score;
        }

        // Checks if there is an existing interface class, that contains a void function with an interface as parameter,
        // if true it probably is an update function for the observer pattern
        public bool HasInterfaceWithVoidFunction()
        {
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                if (cls.Value.Modifiers != null)
                {
                    if (cls.Value.IsInterface)
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
            return false;
        }

        // Observer pattern usually has an subscriber interface with sub/unsub methods
        // which are both void methods with an interface as parameter.
        public bool Has_Abstract_Class_Or_Interface_With_Subscriber_Functions()
        {
            int voidsThatAreAbstractOrInterfaces = 0;
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                if (cls.Value.Modifiers != null)
                {
                    if (cls.Value.IsInterface)
                    {
                        foreach (var method in cls.Value.getMethods())
                        {
                            if (method.ReturnType.ToLower().Equals("void"))
                            {
                                // checks if parameter is an interface
                                string parameters = method.Parameters.Replace("(", string.Empty).Replace(")", string.Empty);
                                string[] paramList = parameters.Split(" ");
                                ClassModel targetClass = cc.GetClass(paramList[0]);
                                if (targetClass.IsInterface || (targetClass.Modifiers[0] == "abstract") || (targetClass.Modifiers[1] == "abstract"))
                                {
                                    voidsThatAreAbstractOrInterfaces++;
                                    if (voidsThatAreAbstractOrInterfaces == 2)
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

        // Checks if there is a class that has a list of interfaces. 
        //If true, probably a subject class with a list filled with observers
        public bool HasSubjectWithList()
        {
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                if (cls.Value.Modifiers != null)
                {
                    foreach (var property in cls.Value.getProperties())
                    {
                        string identifier = property.Identifier.ToString();

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
    }
}
