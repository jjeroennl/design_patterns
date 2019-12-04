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
        public void Test_GetPercentage()
        {
            PriorityCollection.ClearPriorities();
            PriorityCollection.AddPriority("singleton", "IsPrivateConstructor", Priority.Low);
            PriorityCollection.AddPriority("singleton", "IsStaticSelf", Priority.Low);
            PriorityCollection.AddPriority("singleton", "IsGetInstance", Priority.Low);

            int percentage = PriorityCollection.GetPercentage("singleton", "IsPrivateConstructor");
            Assert.Equal(33, percentage);


            PriorityCollection.AddPriority("facade", "IsFacadeTest1", Priority.Medium);
            PriorityCollection.AddPriority("facade", "IsFacadeTest2", Priority.High);

            int percentage2 = PriorityCollection.GetPercentage("facade", "IsFacadeTest1");
            Assert.Equal(40, percentage2);
        }
    }
}
