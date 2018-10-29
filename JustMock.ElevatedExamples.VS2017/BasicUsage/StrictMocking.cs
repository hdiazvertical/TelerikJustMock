using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Core;

namespace JustMock.ElevatedExamples.BasicUsage.StrictMocking
{

    /// <summary>
    /// You may have a case where you want to enable only arranged calls and to reject others. 
    /// In such cases you need to set the mock Behavior to Strict.
    /// See http://www.telerik.com/help/justmock/basic-usage-strict-mocking.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class StrictMocking_Tests
    {
        [TestMethod]
        public void Execute_OnCall_ShouldNotCallOtherMethods()
        {
            // ARRANGE
            // Creating a mocked instance of the "Foo" class with Behavior.Strict. 
            // This means, every non-arranged call from this instance will throw MockException.
            var foo = Mock.Create<Foo>(Behavior.Strict);

            // Arranging: That foo.Execute() must be called with its original implementation.
            Mock.Arrange(() => foo.Execute()).CallOriginal().MustBeCalled();

            // ACT
            foo.Execute();

            // ASSERT
            Mock.Assert(foo);
        }

        [TestMethod]
        [ExpectedException(typeof(StrictMockException))]
        public void VoidCall_OnCall_ShouldCallOtherMethod()
        {
            // ARRANGE
            // Creating a mocked instance of the "Foo" class with Behavior.Strict. 
            // This means, every non-arranged call from this instance will throw MockException.
            var foo = Mock.Create<Foo>(Behavior.Strict);

            // Arranging: When foo.VoidCall() is called, it should be with its original implementation.
            Mock.Arrange(() => foo.VoidCall()).CallOriginal();

            // ACT
            foo.VoidCall();
        }
    }

    #region SUT

    public class Foo
    {
        public void Execute()
        {
            
        }

        public void VoidCall()
        {
            this.Execute();
        }
    }

    #endregion
}