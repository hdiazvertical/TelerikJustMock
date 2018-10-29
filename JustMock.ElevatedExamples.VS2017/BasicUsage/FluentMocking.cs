using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace JustMock.ElevatedExamples.BasicUsage.FluentMocking
{
    /// <summary>
    /// Fluent Assertions allow you to easily follow the Arrange Act Assert pattern in a straightforward way.
    /// Note that JustMock dynamically checks for any assertion mechanism provided by the underlying test framework 
    /// if such one is available (MSTest, XUnit, NUnit, MbUnit, Silverlight) and uses it, rather than using its own 
    /// MockAssertionException when a mock assertion fails. This functionality extends the JustMock tooling support 
    /// for different test runners. 
    /// See http://www.telerik.com/help/justmock/basic-usage-fluent-mocking.html for full documentation of the feature.
    /// 
    /// Note: To write in a fluent way, you will need to have the Telerik.JustMock.Helpers namespace included. 
    /// </summary>
    [TestClass]
    public class FluentMocking_Tests
    {
        [TestMethod]
        public void ShouldBeAbleToAssertSpecificFuntionForASetup()
        {
            // ARRANGE
            // Creating a mocked instance of the "Foo" class.
            var foo = Mock.Create<Foo>();

            // Arranging: When foo.Bar() is called, it should set foo.IsCalled to true instead.
            foo.Arrange(x => x.Bar()).DoInstead(() => foo.IsCalled = true);

            // ACT
            foo.Bar();

            // ASSERT
            Assert.IsTrue(foo.IsCalled);
        }
    }

    #region SUT
    public class Foo
    {
        public bool IsCalled { get; set; }

        public string Bar()
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
