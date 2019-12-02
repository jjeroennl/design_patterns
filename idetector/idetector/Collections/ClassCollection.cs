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

        /// <summary>
        /// Tkey as Namespace/Classname
        /// </summary>
        private static Dictionary<string, ClassModel> cache = new Dictionary<string, ClassModel>();

        public static void AddClass(ClassModel classModel)
        {
            if (cache.ContainsKey(classModel.Identifier))
            {
                cache[classModel.Identifier] = classModel;
            }
            else
            {
                cache.Add(classModel.Identifier, classModel);
            }
        }

        public static ClassModel GetClass(string identifier)
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



    }
}
