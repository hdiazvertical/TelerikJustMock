using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.ElevatedExamples.AdvancedUsage.PartialMocking
{
    /// <summary>
    /// Partial mocks allow you to mock some of the methods of a class while keeping the rest intact. Thus, you keep your 
    /// original object, not a mock object, and you are still able to write your test methods in isolation. Partial mocking 
    /// can be performed on both static and instance calls. 
    /// See http://www.telerik.com/help/justmock/advanced-usage-partial-mocking.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class PartialMocking_Tests
    {
        [TestMethod]
        public void ShouldMockInstanceCallPartially()
        {
            // ARRANGE
            // Creating original instance of the Foo class.
            Foo foo = new Foo();

            // Arranging against the original Foo instance: 
            //  When foo.Echo is called with any integer as an argument, it should return that integer.
            Mock.Arrange(() => foo.Echo(Arg.IsAny<int>())).Returns((int arg) => arg);

            // ACT
            int actual = foo.Echo(10);

            // ASSERT
            Assert.AreEqual(10, actual);
        }

        [TestMethod]
        public void ShouldAssertCallsPartially()
        {
            // ARRANGE
            // Creating original instance of the Foo class.
            Foo foo = new Foo();

            // Arranging against the original Foo instance: 
            //  When foo.Echo is called with any integer as an argument, it should return that integer.
            Mock.Arrange(() => foo.Echo(Arg.IsAny<int>())).Returns((int arg) => arg);

            // ACT
            foo.Echo(10);
            foo.Echo(10);

            // ASSERT - Asserting occurrences (non-arranged) against the original Foo instance.
            Mock.Assert(() => foo.Echo(10), Occurs.Exactly(2));
        }

        [TestMethod]
        public void ShouldArrangeStaticCallPartially()
        {
            var expected = 10;
            // ARRANGE
            // Arranging without setting up the Foo class for static mocking:
            //  When Foo.FooStaticProp_GET is called, it should return expected.
            Mock.Arrange(() => Foo.FooStaticProp).Returns(expected);

            // ACT
            int actual = Foo.FooStaticProp;

            // ASSERT
            Assert.AreEqual(expected, actual);
        }
    }

    #region SUT
    public class Foo
    {
        public static int FooStaticProp { get; set; }

        public int Echo(int arg1)
        {
            return default(int);
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    } 
    #endregion
}
