﻿using idetector.Models;
using System;
using System.Collections.Generic;

namespace idetector.Collections
{
    public class PriorityCollection
    {
        /// <summary>
        /// Dictionary with pattern name and list of tuples with method names and priorities.
        /// </summary>
        private static Dictionary<string, List<Tuple<string, Priority>>> patterns = new Dictionary<string, List<Tuple<string, Priority>>>();

        /// <summary>
        /// Clears patterns dictionary.
        /// </summary>
        public static void ClearPriorities()
        {
            patterns.Clear();
        }

        /// <summary>
        /// Method to add priority to patterns dictionary.
        /// </summary>
        /// <param name="pattern">Pattern name in string format.</param>
        /// <param name="method">Method name in string format.</param>
        /// <param name="priority">Priority level in Priority enum format.</param>
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

        /// <summary>
        /// Method to retrieve a list with all priorities based on given pattern.
        /// </summary>
        /// <param name="pattern">Pattern name in string format.</param>
        /// <returns>List of priorities combined with method names.</returns>
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

        /// <summary>
        /// Method to retrieve a list with all percentages based on given pattern.
        /// </summary>
        /// <param name="pattern">Pattern name in string format.</param>
        /// <returns>List of percentages combines with method names.</returns>
        public static List<Tuple<string, float>> GetPercentages(string pattern)
        {
            try
            {
                List<Tuple<string, Priority>> methods = GetPriorities(pattern);
                List<Tuple<string, float>> percentages = new List<Tuple<string, float>>();

                float counter = 0;
                foreach (var method in methods)
                {
                    counter += (float)method.Item2;
                }
                foreach (var method in methods)
                {
                    percentages.Add(new Tuple<string, float>(method.Item1, 100 / counter * (float)method.Item2));
                }

                return percentages;
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }

        /// <summary>
        /// Method to retrieve percentage based on given pattern and method.
        /// </summary>
        /// <param name="pattern">Pattern name in string format.</param>
        /// <param name="method">Method name in string format.</param>
        /// <returns>Percentage.</returns>
        public static float GetPercentage(string pattern, string method)
        {
            try
            {
                List<Tuple<string, float>> percentages = GetPercentages(pattern);

                foreach (var item in percentages)
                {
                    if (item.Item1 == method)
                    {
                        return item.Item2;
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static Dictionary<string, List<Tuple<string, Priority>>> GetPriorities()
        {
            return patterns;
        }
    }
}
