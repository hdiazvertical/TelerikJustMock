using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.ElevatedExamples.BasicUsage.Mock_DoNothing
{
    /// <summary>
    /// The DoNothing method is used to arrange that a call to a method or property should be ignored.
    /// See http://www.telerik.com/help/justmock/basic-usage-mock-do-nothing.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class Mock_DoNothing_Tests
    {
        [TestMethod]
        public void CallSubmit_OnExecute_ShouldCallSubmit()
        {
            // ARRANGE
            // Creating a mocked instance of the "Foo" class with the same behavior as the original.
            var foo = Mock.Create<Foo>(Behavior.CallOriginal);

            // Arranging: foo.Submit() should do nothing and it must be called during the test method.
            Mock.Arrange(() => foo.Submit()).DoNothing().MustBeCalled();

            // ACT
            foo.CallSubmit();

            // ASSERT - Asserting all arrangements on "foo".
            Mock.Assert(foo);
        }

        [TestMethod]
        public void Echo_Arranged_ShouldDoNothing()
        {
            // ARRANGE
            // Creating a mock instance of the "Foo" class.
            var foo = Mock.Create<Foo>();

            // Arranging: When foo.Echo() is called with any integer as an argument it should do nothing.
            Mock.Arrange(() => foo.Echo(Arg.AnyInt)).DoNothing();

            // ACT
            // Declaring a variable with 
            int actual = 0;
            actual = foo.Echo(10);

            // ASSERT
            Assert.AreEqual(0, actual);
        }
    }

    #region SUT
    public class Foo
    {
        public void Submit()
        {
            throw new NotImplementedException();
        }

        public void CallSubmit()
        {
            this.Submit();
        }

        public int Echo(int num)
        {
            return num;
        }
    } 
    #endregion
}