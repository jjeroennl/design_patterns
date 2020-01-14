using idetector.Data;
using idetector.Models;
using System;
using System.Collections.Generic;

namespace idetector
{
    public class ScoreCalculator
    {
        //pattern name, <requirement name, requirement>
        private Dictionary<string, Dictionary<string, PatternRequirement>> _priorities = new Dictionary<string, Dictionary<string, PatternRequirement>>();

        private Dictionary<string, float> _scores = new Dictionary<string, float>();


        public ScoreCalculator(Dictionary<string, List<PatternRequirement>> requirements)
        {
            foreach (var req in requirements)
            {
                AddPattern(req.Key, req.Value);
            }
        }

        public void AddPattern(string pattern, List<PatternRequirement> requirements)
        {
            float total = 0;
            Dictionary<string, PatternRequirement> reqs = new Dictionary<string, PatternRequirement>();
            foreach (var req in requirements)
            {
                total += req.Weight;
                reqs.Add(req.Id, req);

            }
            
            _priorities.Add(pattern, reqs);
            _scores.Add(pattern, total);

        }

        public int GetScore(string pattern, List<RequirementResult> results)
        {
            float score = 0;
            int val = 0;
            foreach (var result in results)
            {
                if (result.Passed)
                {
                    score += _priorities[pattern][result.Id].Weight;
                }
            }


            var x = _scores[pattern];
            score = (score / _scores[pattern]) * 100;
            val = (int)score;
            return val;
        }
        
        public Dictionary<string, int> GetScore(string pattern, Dictionary<string, List<RequirementResult>> results)
        {
            var scores = new Dictionary<string, int>();
            foreach (var result in results)
            {
                float score = 0;
                int val = 0;
                HashSet<string> classes = new HashSet<string>();
                foreach (var r in result.Value)
                {
                    if (r.Passed)
                    {
                        score += _priorities[pattern][r.Id].Weight;
                    }
                }
                score = (((int)((score / _scores[pattern]) * 100) / classes.Count));

                scores.Add(result.Key, (int)score);
            }
            return scores;
        }
    }
}