using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using System.Text;

namespace JustMock.ElevatedExamples.BasicUsage.SequentialMocking
{
    /// <summary>
    /// Sequential mocking allows you to return different values on the same or different consecutive calls to 
    /// one and the same type. In other words, you can set up expectations for successive calls of the same type. 
    /// See http://www.telerik.com/help/justmock/basic-usage-sequential-mocking.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class SequentialMocking_Tests
    {
        [TestMethod]
        public void LogData_OnSequentialCalls_ShouldLogAsExpected()
        {
            var expected = "first data return\r\nsecond data return\r\nthird data return\r\n";

            // ARRANGE
            // Creating a mocked instance of the "Foo" class with Behavior.CallOriginal.
            var foo = Mock.Create<Foo>(Behavior.CallOriginal);

            // Arranging: foo.GetData() should return expected strings in sequences.
            Mock.Arrange(() => foo.GetData()).Returns("first data return").InSequence();
            Mock.Arrange(() => foo.GetData()).Returns("second data return").InSequence();
            Mock.Arrange(() => foo.GetData()).Returns("third data return").InSequence();

            // ACT
            foo.LogData();
            foo.LogData();
            foo.LogData();

            // ASSERT
            Assert.AreEqual(expected, foo.logger.ToString());
        }
    }

    #region SUT
    public class Foo
    {
        public StringBuilder logger = new StringBuilder();
        
        public string GetData()
        {
            throw new NotImplementedException();
        }

        public void LogData()
        {
            var data = this.GetData();

            this.logger.AppendLine(data);
        }
    }
    #endregion
}
