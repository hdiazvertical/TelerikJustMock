using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.ElevatedExamples.AdvancedUsage.ExtensionMethodsMocking
{
    /// <summary>
    /// Extension Methods mocking is one of the advanced features supported in Telerik JustMock. In this topic we will go through 
    ///  some examples that show how easy and straightforward it is to assert expectations related to extension methods in your tests. 
    ///  When you have to assert some expectations related to extension methods, you can use everything you already know in JustMock.
    /// See http://www.telerik.com/help/justmock/advanced-usage-extension-methods-mocking.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class ExtensionMethodsMocking_Tests
    {
        [TestMethod]
        public void ShouldAssertExtensionMethodMockingWithArguments()
        {
            string expected = "bar";

            // ARRANGE
            var foo = new Foo();

            // Arranging: When the extension method foo.Echo() is called with any string argument, it should return expected.
            Mock.Arrange(() => foo.Echo(Arg.IsAny<string>())).Returns(expected);

            // ACT
            string result = foo.Echo("hello");

            // ASSERT
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ShouldAssertExtensionMethodWithMultipleArguments()
        {
            // ARRANGE
            var foo = new Foo();

            // Arranging: When the extension method foo.Echo() is called with any integer argument and second itneger argument equals to 10, 
            //  it should return the arguments sum.
            Mock.Arrange(() => foo.Echo(Arg.IsAny<int>(), Arg.Matches<int>(x => x == 10))).Returns((int arg1, int arg2) => arg1 + arg2);

            // ACT
            int actual = foo.Echo(1, 10);

            // ASSERT
            Assert.AreEqual(11, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public void ShouldAssertOccurrencesForExtenstionMethod()
        {
            string expected = "test";

            // ARRANGE
            // Creating a mock instance of the "Foo" class.
            var foo = Mock.Create<Foo>();

            // Arranging: When the extension method foo.Echo() is called with any string argument, it should return expected. 
            //  Also this method should never occur with the specific arguments during the test.
            Mock.Arrange(() => foo.Echo(Arg.IsAny<string>())).Returns(expected).OccursNever();

            // ACT
            var actual = foo.Echo(expected);

            // ASSERT
            Assert.AreEqual(expected, actual); // This will pass.
            Mock.Assert(foo); // This will throw AssertFailedException as the method is arranged to not occur.
        }

        [TestMethod]
        public void ShouldAssertInterfaceExtensionMethodCall()
        {
            //ARRANGE
            // Creating a mocked instances of the "DataProvider" class and the "IObjectScope" interface.
            var libararies = Mock.Create<DataProvider>();
            var objectScope = Mock.Create<IObjectScope>();

            // Arranging: When the extension method GetScope() is called, it should return objectScope.
            Mock.Arrange(() => libararies.GetScope()).Returns(objectScope);

            // ACT
            var actual = libararies.GetScope();

            // ASSERT
            Assert.IsTrue(actual.Equals(objectScope));
        }
    }

    #region SUT
    public class Foo
    {
        public void Execute()
        {
            throw new NotImplementedException();
        }

        public string Title { get; set; }
    }

    public static class ExtendFoo
    {
        public static int Echo(this Foo foo)
        {
            return default(int);
        }

        public static string Echo(this Foo foo, string value)
        {
            return value;
        }

        public static int Echo(this Foo foo, int arg1, int arg2)
        {
            return default(int);
        }
    }

    public interface IDataProvider
    {
    }

    public interface IObjectScope
    {
    }

    public class DataProvider : IDataProvider
    {
    }

    public static class DataExtensions
    {
        public static IObjectScope GetScope(this IDataProvider provider)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
