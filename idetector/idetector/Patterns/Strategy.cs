using System;
using System.Collections.Generic;
using System.Text;
using idetector.Collections;
using idetector.Models;

namespace idetector.Patterns
{
    public class Strategy : IPattern
    {
        private int _score;
        private ClassCollection cc;

        public Strategy(ClassCollection _cc)
        {
            cc = _cc;
            PriorityCollection.ClearPriorities();
            PriorityCollection.AddPriority("strategy", "IsTrue", Priority.Low);
        }

        public void Scan()
        {
            if (IsTrue())
            {
                _score += PriorityCollection.GetPercentage("strategy", "IsTrue");
            }
        }

        public int Score()
        {
            return _score;
        }

        public bool IsTrue()
        {
            return true;
        }
    }
}
