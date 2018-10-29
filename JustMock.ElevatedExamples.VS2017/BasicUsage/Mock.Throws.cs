using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using System.Text;

namespace JustMock.ElevatedExamples.BasicUsage.Mock_Throws
{
    /// <summary>
    /// The Throws method is used to throw an exception when a given call is made.
    /// See http://www.telerik.com/help/justmock/basic-usage-mock-throws.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class Mock_Throws_Tests
    {
        [TestMethod]
        public void Execute_OnUnauthorizedAccessExceptionThrown_ShouldGiveProperMessage()
        {
            var expected = "Unauthorized Access !!!";

            // ARRANGE
            // Arranging: When FooStatic.ExecuteStatic() is called, it should throw UnauthorizedAccessException.
            Mock.Arrange(() => FooStatic.ExecuteStatic()).Throws<UnauthorizedAccessException>();

            // ACT
            var foo = new Foo();
            foo.Execute();

            var actual = foo.logger.ToString();

            // ASSERT
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Execute_OnStackOverflowExceptionThrown_ShouldGiveProperMessage()
        {
            // We compare culture-dependant strings
            var expected = "Stack Overflow Detected !!!\r\n" + new StackOverflowException().Message + "\r\n";

            // ARRANGE
            // Arranging: When FooStatic.ExecuteStatic() is called, it should throw StackOverflowException.
            Mock.Arrange(() => FooStatic.ExecuteStatic()).Throws<StackOverflowException>();

            // ACT
            var foo = new Foo();
            foo.Execute();

            var actual = foo.logger.ToString();

            // ASSERT
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void Execute_OnNonExpectedExceptionThrown_ShouldThrowThatException()
        {
            // ARRANGE
            // Arranging: When FooStatic.ExecuteStatic() is called, it should throw NotImplementedException.
            Mock.Arrange(() => FooStatic.ExecuteStatic()).Throws<NotImplementedException>();

            // ACT
            var foo = new Foo();
            foo.Execute();
        }
    }

    #region SUT
    static class FooStatic
    {
        public static void ExecuteStatic()
        {

        }
    }
    public class Foo
    {
        public StringBuilder logger = new StringBuilder();

        public void Execute()
        {
            try
            {
                FooStatic.ExecuteStatic();
            }
            catch (UnauthorizedAccessException)
            {
                logger.Append("Unauthorized Access !!!");
            }
            catch (StackOverflowException ex)
            {
                logger.AppendLine("Stack Overflow Detected !!!").AppendLine(ex.Message);
            }
        }
    }
    #endregion
}
