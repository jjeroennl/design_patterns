using idetector.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace idetector.Collections
{
    class PriorityCollection
    {
        /// <summary>
        /// Tkey as Namespace/Classname
        /// </summary>
        private static Dictionary<string, List<Priority>> priorities = new Dictionary<string, List<Priority>>();

        public static void AddPriority(string pattern, Priority priority)
        {
            if (priorities.ContainsKey(pattern))
            {
                priorities[pattern].Add(priority);
            }
            else
            {
                priorities.Add(pattern, new List<Priority>());
                priorities[pattern].Add(priority);
            }
        }

        public static List<Priority> GetPriorities(string pattern)
        {
            try
            {
                return priorities[pattern];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Dictionary<string, List<Priority>> GetPriorities()
        {
            return priorities;
        }
    }
}
