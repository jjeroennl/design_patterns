using idetector.Collections;
using idetector.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;


namespace xUnitTest
{
    public class PriorityCollectionTests
    {
        [Fact]
        public void Test_AddPriorityToCollection()
        {
            PriorityCollection.ClearPriorities();
            PriorityCollection.AddPriority("singleton", "IsPrivateConstructor", Priority.High);

            Assert.Single(PriorityCollection.GetPriorities());
        }

        [Fact]
        public void Test_GetPriorityWithKey()
        {
            PriorityCollection.ClearPriorities();
            PriorityCollection.AddPriority("singleton", "IsPrivateConstructor", Priority.High);
            PriorityCollection.AddPriority("singleton", "IsCreateSelf", Priority.High);

            PriorityCollection.AddPriority("facade", "IsPublicNamespace", Priority.High);

            Assert.Single(PriorityCollection.GetPriorities("facade"));
            Assert.Equal(2, PriorityCollection.GetPriorities("singleton").Count);
        }

        [Fact]
        public void Test_ClearPriorities()
        {
            PriorityCollection.AddPriority("singleton", "IsPrivateConstructor", Priority.High);
            PriorityCollection.ClearPriorities();

            Assert.Null(PriorityCollection.GetPriorities("singleton"));
        }

        [Fact]
        public void Test_GetPercentage_MultiplePatterns()
        {
            PriorityCollection.ClearPriorities();

            // Singleton
            PriorityCollection.AddPriority("singleton", "IsPrivateConstructor", Priority.Low);
            PriorityCollection.AddPriority("singleton", "IsStaticSelf", Priority.Low);
            PriorityCollection.AddPriority("singleton", "IsGetInstance", Priority.Low);

            Assert.InRange(PriorityCollection.GetPercentage("singleton", "IsPrivateConstructor"), 33, 34);

            // Facade
            PriorityCollection.AddPriority("facade", "IsFacadeTest1", Priority.Low);
            PriorityCollection.AddPriority("facade", "IsFacadeTest2", Priority.Medium);

            Assert.InRange(PriorityCollection.GetPercentage("facade", "IsFacadeTest1"), 33, 34);
            Assert.InRange(PriorityCollection.GetPercentage("facade", "IsFacadeTest2"), 66, 67);

            // Observer
            PriorityCollection.AddPriority("observer", "IsObserverTest1", Priority.Low);
            PriorityCollection.AddPriority("observer", "IsObserverTest2", Priority.Medium);
            PriorityCollection.AddPriority("observer", "IsObserverTest3", Priority.High);

            Assert.InRange(PriorityCollection.GetPercentage("observer", "IsObserverTest1"), 16, 17);
            Assert.InRange(PriorityCollection.GetPercentage("observer", "IsObserverTest2"), 33, 34);
            Assert.Equal(50, PriorityCollection.GetPercentage("observer", "IsObserverTest3"));
        }

        [Fact]
        public void Test_GetPercentage_Empty()
        {
            PriorityCollection.ClearPriorities();

            Assert.Equal(0, PriorityCollection.GetPercentage("singleton", "IsPrivateConstructor"));
        }
    }
}
