using idetector.Models;
using System;
using System.Collections.Generic;

namespace idetector.Collections
{
    public class PriorityCollection
    {
        /// <summary>
        /// Tkey as Namespace/Classname
        /// </summary>
        private static Dictionary<string, List<Tuple<string, Priority>>> patterns = new Dictionary<string, List<Tuple<string, Priority>>>();

        public static void AddPriority(string pattern, string method, Priority priority)
        {
            pattern = pattern.ToLower();
            if (patterns.ContainsKey(pattern))
            {
                patterns[pattern].Add(new Tuple<string, Priority>(method, priority));
            }
            else
            {
                patterns.Add(pattern, new List<Tuple<string, Priority>>());
                patterns[pattern].Add(new Tuple<string, Priority>(method, priority));
            }
        }

        public static List<Tuple<string, Priority>> GetPriorities(string pattern)
        {
            pattern = pattern.ToLower();
            try
            {
                return patterns[pattern];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<Tuple<string, int>> GetPercentages(string pattern)
        {
            List<Tuple<string, Priority>> methods = GetPriorities(pattern);
            List<Tuple<string, int>> percentages = new List<Tuple<string, int>>();

            int counter = 0;
            foreach (var method in methods)
            {
                counter += (int)method.Item2;
            }

            foreach (var method in methods)
            {
                percentages.Add(new Tuple<string, int>(method.Item1, 100 / counter * (int)method.Item2));
            }

            return percentages;
        }

        public static int GetPercentage(string pattern, string method)
        {
            List<Tuple<string, int>> percentages = GetPercentages(pattern);

            foreach (var item in percentages)
            {
                if (item.Item1 == method)
                {
                    return item.Item2;
                }
            }

            return 0;
        }

        public static Dictionary<string, List<Tuple<string, Priority>>> GetPriorities()
        {
            return patterns;
        }
    }
}
