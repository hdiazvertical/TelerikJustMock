using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;



namespace JustMock.ElevatedExamples.BasicUsage.Mock_CallOriginal
{
    /// <summary>
    /// The CallOriginal method marks a mocked method/property call that should execute the original method/property implementation.
    /// See http://www.telerik.com/help/justmock/basic-usage-mock-call-original.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class Mock_CallOriginal_Tests
    {
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ShouldAssertCallOriginal()
        {
            // ARRANGE
            // Creating a mock instance of the "FooBase" class.
            var foo = Mock.Create<FooBase>();

            // Arranging: When foo.Submit() is called with any integer as an argument it should call the original implementation.
            Mock.Arrange(() => foo.Submit(Arg.AnyInt)).CallOriginal();

            // ACT
            foo.Submit(1);

            // ASSERT - We are asserting with the [ExpectedException(typeof(NotImplementedException))] test attribute.
        }

        [TestMethod]
        public void GetString_CallsWithDifferentArgs_ReturnAsExpected()
        {
            // ARRANGE
            // Creating a mock instance of the "FooBase" class.
            var foo = Mock.Create<FooBase>();

            // Arranging: When foo.GetString() is called with "x" as an argument it should call the original implementation.
            Mock.Arrange(() => foo.GetString("x")).CallOriginal();
            // Arranging: When foo.GetString() is called with "y" as an argument it should return "z".
            Mock.Arrange(() => foo.GetString("y")).Returns("z");

            // ACT
            var actualForArgX = foo.GetString("x");
            var actualForArgY = foo.GetString("y");

            // Expected return values.
            var expectedX = "x";
            var expectedY = "z";

            // ASSERT - Asserting that the expected return values are equal to the actual.
            Assert.AreEqual(expectedX, actualForArgX);
            Assert.AreEqual(expectedY, actualForArgY);
        }
    }

    #region SUT
    public class FooBase
    {
        public void Submit(int arg)
        {
            throw new NotImplementedException();
        }

        public string GetString(string str)
        {
            return str;
        }
    } 
    #endregion
}
