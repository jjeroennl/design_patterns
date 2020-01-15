using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using idetector.Models;

namespace idetector.Collections
{
    public class ClassCollection
    { 
        private Dictionary<string, ClassModel> classList = new Dictionary<string, ClassModel>();

        public void AddClass(ClassModel classModel)
        {
            if (classList.ContainsKey(classModel.Identifier.ToString()))
            {
                classList[classModel.Identifier] = classModel;
            }
            else
            {
                classList.Add(classModel.Identifier, classModel);
            }
        }

        public ClassModel GetClass(string identifier)
        {
            try
            {
                return classList[identifier];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Dictionary<string, ClassModel> GetClasses()
        {
            return classList;
        }


        /// <summary>
        /// This function returns a list of classes, except for the classes that are given in the argument
        /// </summary>
        /// <param name="classesToExclude">Classes to skip</param>
        /// <returns></returns>
        public List<ClassModel> GetClassListExcept(List<ClassModel> classesToExclude)
        {
            List<ClassModel> allClasses = new List<ClassModel>();
            foreach (var cls in GetClasses())
            {
                allClasses.Add(cls.Value);
            }

            var result = allClasses.Except(classesToExclude).ToList();
            return result;
        }

        public ClassCollection GetNamespace(string @namespace)
        {
            ClassCollection n = new ClassCollection();
            var col = this.GetClasses().Where(e => e.Value.Namespace == @namespace);
            foreach (var model in col)
            {
                n.AddClass(model.Value);
            }

            return n;
        }

    }
}
