using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.ElevatedExamples.BasicUsage.AssertingOccurrence
{
    /// <summary>
    /// See http://www.telerik.com/help/justmock/basic-usage-asserting-occurrence.html for full documentation of the feature.
    /// Occurrence is used in conjunction with Mock.Assert and Mock.AssertSet to determine how many times a call has occurred.
    /// There are 6 types of occurrence that we can use:
    ///    Occurs.Never() - Specifies that a particular call is never made on a mock.
    ///    Occurs.Once() - Specifies that a call has occurred only once on a mock.
    ///    Occurs.AtLeastOnce() - Specifies that a call has occurred at least once on a mock.
    ///    Occurs.AtLeast(numberOfTimes) - Specifies the number of times at least a call should occur on a mock.
    ///    Occurs.AtMost(numberOfTimes) - Specifies the number of times at most a call should occur on a mock.
    ///    Occurs.Exactly(numberOfTimes) - Specifies exactly the number of times a call should occur on a mock. 
    /// Furthermore, you can set occurrence directly in the arrangement of a method.
    /// You can use one of 5 different constructs of Occur:
    ///    Occurs(numberOfTimes) - Specifies exactly the number of times a call should occur on a mock.
    ///    OccursOnce() - Specifies that a call should occur only once on a mock.
    ///    OccursNever() - Specifies that a particular call should never be made on a mock.
    ///    OccursAtLeast(numberOfTimes) - Specifies that a call should occur at least once on a mock.
    ///    OccursAtMost(numberOfTimes) - Specifies the number of times at most a call should occur on a mock. 
    /// </summary>
    [TestClass]
    public class AssertingOccurrence_Tests
    {
        [TestMethod]
        public void PopulateList_OnCallWithCertainArg_ShouldCallAddMethodExpectedNumberOfTimes()
        {
            var expectedOccurrences = 10;

            // ARRANGE
            // Creating a mock instance of the "Foo" class with Behavior.CallOriginal.
            var foo = Mock.Create<Foo>(Behavior.CallOriginal);

            // Arranging: foo.myList.Add() should be called expected number of times no matter the argument.
            Mock.Arrange(()=> foo.myList.Add(Arg.AnyInt)).Occurs(expectedOccurrences);

            // ACT
            foo.PopulateList(expectedOccurrences);

            // ASSERT
            Mock.Assert(foo);
        }
    }

    #region SUT
    public class Foo
    {
        public IList<int> myList = new List<int>();

        public void PopulateList(int count)
        {
            for (int i = 0; i < count; i++)
            {
                this.myList.Add(i);
            }
        }
    }
    #endregion
}
