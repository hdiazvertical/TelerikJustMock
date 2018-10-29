using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.ElevatedExamples.BasicUsage.MockingProperties
{
    /// <summary>
    /// Mocking properties is similar to mocking methods, but there are a few cases that need special attention 
    /// like mocking indexers and particular set operations. 
    /// See http://www.telerik.com/help/justmock/basic-usage-mock-properties.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class MockingProperties_Tests
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void FormatString_OnExecuteWithPing_ShouldThrowAnException()
        {
            // ARRANGE
            // Arranging: Foo.MyStringProp should return the expected string.
            Mock.Arrange(() => Foo.MyStringProp).Returns("Ping");

            // ACT
            var foo = new Foo();
            var actual = foo.FormatString();
        }

        [TestMethod]
        public void FormatString_OnExecuteWithOUTPing_ShouldReturnExpected()
        {
            var expected = "All is good";

            // ARRANGE
            // Arranging: Foo.MyStringProp should return the expected string.
            Mock.Arrange(() => Foo.MyStringProp).Returns("Pong");

            // ACT
            var foo = new Foo();
            var actual = foo.FormatString();

            // ASSERT
            Assert.AreEqual(expected, actual);
        }
    }

    #region SUT
    public class Foo
    {
        public static string MyStringProp { get; set; }

        public string FormatString()
        {
            if (MyStringProp == "Ping")
            {
                throw new Exception("Ping is not a valid string!!!");
            }
            else
            {
                string result = "All is good";
                return result;
            }
        }
    }
    #endregion
}
