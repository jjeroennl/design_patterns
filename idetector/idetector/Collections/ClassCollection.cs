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

        public void Clear()
        {
            classList = new Dictionary<string, ClassModel>();
        }
        
        public Dictionary<string, ClassModel> GetClasses()
        {
            return classList;
        }


        public List<ClassModel> GetClassListExcept(List<ClassModel> classes)
        {
            List<ClassModel> allClasses = new List<ClassModel>();
            foreach (var cls in GetClasses())
            {
                allClasses.Add(cls.Value);
            }

            var result = allClasses.Except(classes).ToList();
            return result;
        }



    }
}
