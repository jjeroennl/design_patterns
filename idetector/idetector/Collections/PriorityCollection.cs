using idetector.Models;
using System;
using System.Collections.Generic;

namespace idetector.Collections
{
    class PriorityCollection
    {
        /// <summary>
        /// Tkey as Namespace/Classname
        /// </summary>
        private static Dictionary<string, List<Tuple<string, Priority>>> priorities = new Dictionary<string, List<Tuple<string, Priority>>>();

        public static void AddPriority(string pattern, string methodName, Priority priority)
        {
            pattern = pattern.ToLower();
            if (priorities.ContainsKey(pattern))
            {
                priorities[pattern].Add(new Tuple<string, Priority>(methodName, priority));
            }
            else
            {
                priorities.Add(pattern, new List<Tuple<string, Priority>>());
                priorities[pattern].Add(new Tuple<string, Priority>(methodName, priority));
            }
        }

        public static List<Tuple<string, Priority>> GetPriorities(string pattern)
        {
            pattern = pattern.ToLower();
            try
            {
                return priorities[pattern];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<Tuple<string, int>> GetPercentage(string pattern)
        {


            return null;

        }

        public static Dictionary<string, List<Tuple<string, Priority>>> GetPriorities()
        {
            return priorities;
        }
    }
}
