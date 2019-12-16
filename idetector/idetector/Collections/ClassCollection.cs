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
        private  Dictionary<string, ClassModel> cache = new Dictionary<string, ClassModel>();

        public void AddClass(ClassModel classModel)
        {
            if (cache.ContainsKey(classModel.Identifier.ToString()))
            {
                cache[classModel.Identifier] = classModel;
            }
            else
            {
                cache.Add(classModel.Identifier, classModel);
            }
        }

        public ClassModel GetClass(string identifier)
        {
            try
            {
                return cache[identifier];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void Clear()
        {
            cache = new Dictionary<string, ClassModel>();
        }
        
        public Dictionary<string, ClassModel> GetClasses()
        {
            return cache;
        }



    }
}
