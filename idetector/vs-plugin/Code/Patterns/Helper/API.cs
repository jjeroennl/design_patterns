using System.Collections.Generic;
using System.Linq;
using idetector.Collections;
using idetector.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace idetector.Patterns.Helper
{
    public class API
    {
        /// <summary>
        ///     Check if the given class has a property of the given type and (optionally) the given modifiers
        /// </summary>
        /// <param name="cls">ClassModel Object</param>
        /// <param name="type">(optional) Returntype of property</param>
        /// <param name="modifiers">(optional) A list of modifiers of property</param>
        /// <returns>True or False</returns>
        public static bool ClassHasPropertyOfType(ClassModel cls, string type = null, string[] modifiers = null)
        {
            foreach (var property in cls.GetProperties())
                if (type == null || property.ValueType.Equals(type))
                {
                    if (modifiers == null) return true;

                    return modifiers.All(m => property.HasModifier(m));
                }

            return false;
        }

        /// <summary>
        ///     Check if the given class has a property of the given type and if it has a setter.
        /// </summary>
        /// <param name="cls">ClassModel Object</param>
        /// <param name="hasSetter">Wheter is should have an setter</param>
        /// <param name="type">(optional) Returntype of property</param>
        /// <returns></returns>
        public static bool ClassHasPropertySyntaxSetter(ClassModel cls, string type = null)
        {
            foreach (var property in cls.GetProperties())
            {
                if (type == null || property.ValueType.Equals(type))
                {
                    var node = (PropertyDeclarationSyntax)property.GetNode();

                    if (node.AccessorList.ToString().Contains("set"))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Check if the given class has a consturctor of the given type and (optionally) the given modifiers
        /// </summary>
        /// <param name="cls">ClassModel Object</param>
        /// <param name="type">(optional) Returntype of property</param>
        /// <param name="modifiers">(optional) A list of modifiers of property</param>
        /// <returns>True or False</returns>
        public static bool ClassHasConstructorOfType(ClassModel cls, string type = null, string[] modifiers = null)
        {
            if (modifiers != null && type != null)
                return cls.GetMethods().Any(
                    e =>
                        e.Modifiers.All(
                            i => modifiers.Any(m => m == i)
                        )
                        && e.Parameters.Contains(type)
                        && e.isConstructor);
            else if (modifiers == null && type != null)
                return cls.GetMethods().Any(
                    e =>
                        e.Parameters.Contains(type)
                        && e.isConstructor);
            else if (modifiers != null && type == null)
                return cls.GetMethods().Any(
                    e =>
                        e.Modifiers.All(
                            i => modifiers.Any(m => m == i)
                        )
                        && e.isConstructor);
            else return cls.GetMethods().Any(
                e =>
                    e.Parameters.Contains(type)
                    && e.isConstructor);

        }
        internal static MethodModel ClassGetMethodWithName(ClassModel cls, string name)
        {
            return cls.GetMethods().Where(e => e.Identifier.Equals(name))?.First();
        }

        /// <summary>
        ///     Check if the given class has a property of the given type and if it has a getter.
        /// </summary>
        /// <param name="cls">ClassModel Object</param>
        /// <param name="hasSetter">Wheter is should have an getter</param>
        /// <param name="type">(optional) Returntype of property</param>
        /// <returns></returns>
        public static bool ClassHasPropertySyntaxGetter(ClassModel cls, string type = null)
        {
            foreach (var property in cls.GetProperties())
            {
                if (type == null || property.ValueType.Equals(type))
                {
                    var node = (PropertyDeclarationSyntax)property.GetNode();

                    if (node.AccessorList.ToString().Contains("get"))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Get the given class has a property of the given type and (optionally) the given modifiers
        /// </summary>
        /// <param name="cls">ClassModel Object</param>
        /// <param name="type">Returntype of property</param>
        /// <param name="modifiers">(optional) A list of modifiers of property</param>
        /// <returns>True or False</returns>
        public static List<PropertyModel> ClassGetPropertiesOfType(ClassModel cls, string type,
            string[] modifiers = null)
        {
            List<PropertyModel> properties = new List<PropertyModel>();
            foreach (var property in cls.GetProperties())
                if (property.ValueType.Equals(type))
                {
                    if (modifiers == null) continue;
                    if (modifiers.All(m => property.HasModifier(m)))
                    {
                        properties.Add(property);
                    }
                }

            return properties;
        }

        /// <summary>
        ///     Check if the given class has a method of the given type and (optionally) the given modifiers
        /// </summary>
        /// <param name="cls">ClassModel Object</param>
        /// <param name="type">Returntype of property</param>
        /// <param name="modifiers">(optional) A list of modifiers of property</param>
        /// <param name="allowConstructor">Whether or not a constructor may be considered as a method</param>
        /// <returns>True or False</returns>
        public static bool ClassHasMethodOfType(ClassModel cls, string type, string[] modifiers = null,
            bool allowConstructor = false)
        {
            if (modifiers != null)
                return cls.GetMethods().Any(
                    e =>
                        modifiers.All(e.HasModifier)
                        && e.ReturnType == type
                        && (e.isConstructor && allowConstructor || e.isConstructor == false && !allowConstructor));
            return cls.GetMethods().Any(
                e =>
                    e.ReturnType == type
                    && (e.isConstructor && allowConstructor || e.isConstructor == false && !allowConstructor));
        }

        /// <summary>
        ///     Get the given class has a method of the given type and (optionally) the given modifiers
        /// </summary>
        /// <param name="cls">ClassModel Object</param>
        /// <param name="type">Returntype of property</param>
        /// <param name="modifiers">(optional) A list of modifiers of property</param>
        /// <param name="allowConstructor">Whether or not a constructor may be considered as a method</param>
        /// <returns>True or False</returns>
        public static List<MethodModel> ClassGetMethodOfType(ClassModel cls, string type, string[] modifiers = null)
        {
            List<MethodModel> methodList = new List<MethodModel>();
            if (modifiers != null)
            {
                foreach (var method in cls.GetMethods().Where(
                    e =>
                        modifiers.All(e.HasModifier)
                        && e.ReturnType == type
                        && e.isConstructor == false))
                {
                    methodList.Add(method);
                }
            }

            return methodList;
        }


        /// <summary>
        ///     Check if a given class has a method with a certain parameter type
        /// </summary>
        /// <param name="cls">ClassModel Object</param>
        /// <param name="type">parameter type to search for</param>
        /// <returns>True or False</returns>
        public static bool ClassHasMethodWithParam(ClassModel cls, string type)
        {
            return cls.GetMethods().Any(e => e.Parameters.Contains(type));
        }
      
        /// <summary>
        ///  Returns all methods from a certain class that have a specified parameter
        /// </summary>
        /// <param name="cls">ClassModel Object</param>
        /// <param name="type">parameter type to search for</param>
        /// <returns>List<MethodModel></returns>
        public static List<MethodModel> ClassGetMethodsWithParam(ClassModel cls, string type)
        {
            return cls.GetMethods().Where(e => e.Parameters.Contains(type)).ToList();
        }


        /// <summary>
        ///     Check if an object of a given type gets created in the given class
        /// </summary>
        /// <param name="cls">ClassModel Object</param>
        /// <param name="type">Returntype of property</param>
        /// <returns>True/False</returns>
        public static bool ClassHasObjectCreationOfType(ClassModel cls, string type)
        {
            foreach (var obj in cls.ObjectCreations)
                if (obj.Identifier.Equals(type))
                    return true;
            return false;
        }

        public static bool ClassIsAbstractOrInterface(ClassModel cls)
        {
            if (cls.IsAbstract || cls.IsInterface)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<ClassModel> ListChildren(ClassCollection collection, string type)
        {
            List<ClassModel> result = collection.GetClasses().Values.Where(e => e.HasParent(type)).Distinct().ToList();
            return result;
        }
        /// <summary>
        /// Check if a child has a call to the base constructor
        /// </summary>
        /// <param name="cls">Child class</param>
        /// <returns>True/False</returns>
        public static bool ChildCallsBaseConstructor(ClassModel cls)
        {
            foreach (var cons in cls.GetConstructors())
            {
                var node = (ConstructorDeclarationSyntax)cons.getNode();
                if (node.Initializer.ThisOrBaseKeyword.ToString().ToLower().Equals("base"))
                {
                    return true;
                }
            }

            return false;
        }

        public static List<ClassModel> ListAbstract(ClassCollection collection)
        {
            List<ClassModel> classes = new List<ClassModel>();
            foreach (var cls in collection.GetClasses())
            {
                if (cls.Value.IsAbstract)
                {
                    classes.Add(cls.Value);
                }
            }

            return classes;
        }


        public static List<ClassModel> ListInterfaces(ClassCollection collection)
        {
            List<ClassModel> classes = new List<ClassModel>();
            foreach (var cls in collection.GetClasses())
            {
                if (cls.Value.IsInterface)
                {
                    classes.Add(cls.Value);
                }
            }

            return classes;
        }

        public static List<ClassModel> ListInterfacesAndAbstracts(ClassCollection collection)
        {
            List<ClassModel> classes = new List<ClassModel>();
            foreach (var cls in collection.GetClasses())
            {
                if (cls.Value.IsInterface || cls.Value.IsAbstract)
                {
                    classes.Add(cls.Value);
                }
            }

            return classes;
        }

    }
}
