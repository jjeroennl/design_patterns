using idetector.Models;
using idetector.Parser;
using idetector.Patterns.Helper;
using Xunit;

namespace xUnitTest
{
    public class ApiTests
    {
        public ClassModel prepare()
        {
            var tree = (@" 
            namespace Test{
               class User{
                    private static User me;

                    private User(){

                    }
                    
                    public static User getUser(){
                        if(this.me == null){
                            this.me = new User();
                        }
                        
                        return this.me;
                    }
                }
            }");

            return CodeParser.Parse(tree).GetClass("User");
        }

        [Fact]
        public void Test_ClassHas()
        {
            var cls = this.prepare();
            Assert.True(API.ClassHasMethodOfType(cls, "User"));
            Assert.True(API.ClassHasMethodOfType(cls, "User", new []{"public", "static"}));
            Assert.False(API.ClassHasMethodOfType(cls, "User", new []{"private", "static"}));
            Assert.True(API.ClassHasPropertyOfType(cls, "User"));
            Assert.True(API.ClassHasPropertyOfType(cls, "User", new[]{"private", "static"}));
            Assert.False(API.ClassHasPropertyOfType(cls, "User", new[]{"public", "static"}));

            Assert.False(API.ClassHasMethodOfType(cls, "NoExist"));
            Assert.False(API.ClassHasPropertyOfType(cls, "NoExist"));

            Assert.True(API.ClassHasObjectCreationOfType(cls, "User"));
            Assert.False(API.ClassHasObjectCreationOfType(cls, "SomethingElse"));

        }
    }
}