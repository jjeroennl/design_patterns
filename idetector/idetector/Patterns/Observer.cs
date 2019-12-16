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

        public bool HasInterfaceWithObserverFunctions()
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
                                if (targetClass.IsInterface && targetClass.Identifier.Contains("observer")) ;
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

        public bool HasSubjectWithList()
        {
            foreach (KeyValuePair<string, ClassModel> cls in cc.GetClasses())
            {
                if (cls.Value.Modifiers != null)
                {
                    if (cls.Value.Identifier == "subject")
                    {
                        foreach (var method in cls.Value.getMethods())
                        {
                            if (method.ReturnType.ToLower().Equals("void"))
                            {
                                // checks if parameter is an interface
                                string parameters = method.Parameters.Replace("(", string.Empty).Replace(")", string.Empty);
                                string[] paramList = parameters.Split(" ");
                                ClassModel targetClass = cc.GetClass(paramList[0]);

                                if (targetClass.IsInterface && targetClass.Identifier.Contains("observer"))
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
    }
}
