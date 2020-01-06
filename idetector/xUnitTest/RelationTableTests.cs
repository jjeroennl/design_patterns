using idetector.Patterns.Helper;
using Xunit;

namespace xUnitTest
{
    public class RelationTableTests
    {
        [Fact]
        void RelationTestTable()
        {
            var relation = new RelationTable();
            Assert.NotNull(relation);

            relation.AddRelation("Admin", "User");
            
            Assert.NotNull(relation.GetRelationTo("User"));
            Assert.Null(relation.GetRelationTo("Admin"));
            Assert.True(relation.GetRelationTo("User").HasParent("Admin"));
            Assert.Single(relation.GetRelationTo("User").ListParents());
            Assert.Equal("Admin", relation.GetRelationTo("User").ListParents()[0]);
            
            relation.AddRelation("Developer", "User");
            Assert.Null(relation.GetRelationTo("Admin"));
            Assert.NotNull(relation.GetRelationTo("User"));
            Assert.True(relation.GetRelationTo("User").HasParent("Admin"));
            Assert.True(relation.GetRelationTo("User").HasParent("Developer"));
            Assert.Equal(2, relation.GetRelationTo("User").ListParents().Count);
            Assert.Equal("Admin", relation.GetRelationTo("User").ListParents()[0]);
            Assert.Equal("Developer", relation.GetRelationTo("User").ListParents()[1]);

        }
        
    }
}