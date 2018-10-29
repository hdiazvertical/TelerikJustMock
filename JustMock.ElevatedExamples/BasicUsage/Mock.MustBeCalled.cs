using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.ElevatedExamples.BasicUsage.Mock_MustBeCalled
{
    /// <summary>
    /// The MustBeCalled method is used to assert that a call to a given method or property is made during the execution of a test.
    /// See http://www.telerik.com/help/justmock/basic-usage-mock-must-be-called.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class Mock_MustBeCalled_Tests
    {
        [TestMethod]
        public void Execute_OnExecute_ShouldBeCalled()
        {
            // ARRANGE
            var foo = new Foo();

            // Arranging: foo.Execute() should be called during the test method.
            Mock.Arrange(() => foo.Execute()).MustBeCalled();

            // ACT
            foo.Execute();

            // ASSERT - Asserting all arrangements on "foo".
            Mock.Assert(foo);
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public void Execute_NotExecuted_ShouldNotBeCalled()
        {
            // ARRANGE
            var foo = new Foo();

            // Arranging: foo.Execute() should be called during the test method.
            Mock.Arrange(() => foo.Execute()).MustBeCalled();

            // ACT - Not calling foo.Execute() should generate AssertFailedException.

            // ASSERT - Asserting all arrangements on "foo".
            Mock.Assert(foo);
        }

        [TestMethod]
        public void Echo_OnExecuteWithTheCorrectArgs_ShouldBeCalled()
        {
            // ARRANGE
            // Creating a mocked instance of the "Foo" class.
            var foo = Mock.Create<Foo>();

            // Arranging: foo.Echo() with argument 1 should be called during the test method.
            Mock.Arrange(() => foo.Echo(1)).MustBeCalled();

            // ACT
            var actual = foo.Echo(1);

            // ASSERT - Asserting all arrangements on "foo".
            Mock.Assert(foo);
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException))]
        public void Execute_OnExecuteWithWronArgs_ShouldNotBeCalled()
        {
            // ARRANGE
            // Creating a mocked instance of the "Foo" class.
            var foo = Mock.Create<Foo>();

            // Arranging: foo.Echo() with argument 1 should be called during the test method.
            Mock.Arrange(() => foo.Echo(1)).MustBeCalled();

            // ACT - Calling foo.Echo() with different argument than the expected.
            foo.Echo(10);

            // ASSERT - Asserting all arrangements on "foo". 
            Mock.Assert(foo);
        }
    }

    #region SUT
    public class Foo
    {
        public void Execute()
        {
        }

        public int Execute(int str)
        {
            return str;
        }

        public int Echo(int a)
        {
            return 5;
        }
    }
    #endregion
}
