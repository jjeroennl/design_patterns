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

            Assert.Equal(33, PriorityCollection.GetPercentage("singleton", "IsPrivateConstructor"));

            // Facade
            PriorityCollection.AddPriority("facade", "IsFacadeTest1", Priority.Low);
            PriorityCollection.AddPriority("facade", "IsFacadeTest2", Priority.Medium);

            Assert.Equal(33, PriorityCollection.GetPercentage("facade", "IsFacadeTest1"));
            Assert.Equal(66, PriorityCollection.GetPercentage("facade", "IsFacadeTest2"));

            // Observer
            PriorityCollection.AddPriority("observer", "IsObserverTest1", Priority.Low);
            PriorityCollection.AddPriority("observer", "IsObserverTest2", Priority.Medium);
            PriorityCollection.AddPriority("observer", "IsObserverTest3", Priority.High);

            Assert.Equal(16, PriorityCollection.GetPercentage("observer", "IsObserverTest1"));
            Assert.Equal(32, PriorityCollection.GetPercentage("observer", "IsObserverTest2"));
            Assert.Equal(48, PriorityCollection.GetPercentage("observer", "IsObserverTest3"));
        }

        [Fact]
        public void Test_GetPercentage_Empty()
        {
            PriorityCollection.ClearPriorities();

            Assert.Equal(0, PriorityCollection.GetPercentage("singleton", "IsPrivateConstructor"));
        }
    }
}
