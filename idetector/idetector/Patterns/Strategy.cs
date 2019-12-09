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
        private ClassModel cls;

        public Strategy(ClassModel _cls)
        {
            cls = _cls;
            PriorityCollection.ClearPriorities();
            PriorityCollection.AddPriority("strategy", "IsPrivateConstructor", Priority.Low);
            PriorityCollection.AddPriority("strategy", "IsStaticSelf", Priority.Low);
            PriorityCollection.AddPriority("strategy", "IsGetInstance", Priority.Low);
            PriorityCollection.AddPriority("strategy", "IsCreateSelf", Priority.Low);
        }
        public void Scan()
        {
            throw new NotImplementedException();
        }
    }
}
