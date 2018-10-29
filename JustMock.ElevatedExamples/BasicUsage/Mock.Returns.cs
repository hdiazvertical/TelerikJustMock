using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.ElevatedExamples.BasicUsage.Mock_Returns
{
    /// <summary>
    /// The Returns method is used with non void calls to ignore the actual call and return a custom value.
    /// See http://www.telerik.com/help/justmock/basic-usage-mock-returns.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class Mock_Returns_Tests
    {
        [TestMethod]
        public void ShouldArrangeReturnValueForStaticFunction()
        {
            var expected = "Test";

            // ARRANGE
            // Setting up the static Bar class for mocking.
            Mock.SetupStatic(typeof(Bar));

            // Arranging: When Bar.Execute() is called, it should return the expected string.
            Mock.Arrange(() => Bar.Execute()).Returns(expected);

            // ACT
            var actual = Bar.Execute();

            // ASSERT
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ShouldExecuteMockForSameInstanceInSameContext()
        {
            // ARRANGE
            // Creating a mocked instance of the "Foo" class.
            var foo = Mock.Create<Foo>();

            // Arranging: When foo.Echo() is called with any integer as an argument, 
            //  it should return that argument for every future instance of the Foo class.
            Mock.Arrange(() => foo.Echo(Arg.AnyInt)).IgnoreInstance().Returns((int arg1) => arg1);

            // ACT
            var expected = 10;
            var actual = new Foo().Echo(expected);

            // ASSERT
            Assert.AreEqual(expected, actual);
        }
    }

    #region SUT
    public static class Bar
    {
        public static string Execute()
        {
            return string.Empty;
        }
    }

    public class Foo
    {
        public int Echo(int myInt)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
