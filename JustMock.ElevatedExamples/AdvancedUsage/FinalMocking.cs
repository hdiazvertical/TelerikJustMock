using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Core;

namespace JustMock.ElevatedExamples.AdvancedUsage.FinalMocking
{
    /// <summary>
    /// Final mocking is one of the advanced features supported in Telerik JustMock. It allows you to fake final 
    /// method/property calls, set expectations and verify results using the AAA principle. Faking final or virtual 
    /// method/property calls doesn't affect the way you write your tests, i.e. the same syntax is used for mocking 
    /// both final and non-final calls. 
    /// See http://www.telerik.com/help/justmock/advanced-usage-final-mocking.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class FinalMocking_Tests
    {
        [TestMethod]
        public void ShouldSetupACallToAFinalMethod()
        {
            var expected = 10;

            // ARRANGE
            // Creating a mock instance of the "Foo" class.
            var foo = Mock.Create<Foo>();

            // Arranging: When foo.Echo() is called with any integer argument, it should return expected.
            Mock.Arrange(() => foo.Echo(Arg.IsAny<int>())).Returns(expected);

            // ACT
            var actual = foo.Echo(1);

            // ASSERT
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ShouldSetupACallToAFinalProperty()
        {
            var expected = "bar";

            // ARRANGE
            // Creating a mock instance of the "Foo" class.
            var foo = Mock.Create<Foo>();

            // Arranging: When foo.FooProp_GET is called, it should return expected.
            Mock.Arrange(() => foo.FooProp).Returns(expected);

            // ACT
            var actual = foo.FooProp;

            // ASSERT
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(StrictMockException))]
        public void ShouldAssertPropertySet()
        {
            // ARRANGE
            // Creating a mock instance of the "Foo" class with Behavior.Strict.
            var foo = Mock.Create<Foo>(Behavior.Strict);

            // Arranging: That foo.FooProp must be set to "ping" during the test method.
            Mock.ArrangeSet(() => foo.FooProp = "ping").MustBeCalled();

            // Act
            foo.FooProp = "ping";

            // ASSERT
            Mock.Assert(foo);

            // ACT - This will throw MockException due to the strict behavior of the mock.
            foo.FooProp = "foo";
        }

        [TestMethod]
        public void ShouldAssertOnMethodOverload()
        {
            // ARRANGE
            // Creating a mock instance of the "Foo" class.
            var foo = Mock.Create<Foo>();

            // Arranging: When foo.Execute() is called with any integer as an argument, it should return that integer.
            Mock.Arrange(() => foo.Execute(Arg.IsAny<int>())).Returns((int result) => result);
            // Arranging: When foo.Execute() is called with any integers as an arguments, it should return the sum of these integers.
            Mock.Arrange(() => foo.Execute(Arg.IsAny<int>(), Arg.IsAny<int>())).Returns((int arg1, int arg2) => arg1 + arg2);

            // ASSERT
            Assert.AreEqual(foo.Execute(1), 1);
            Assert.AreEqual(foo.Execute(1, 1), 2);
        }

        [TestMethod]
        public void ShouldAssertOnMethodCallbacks()
        {
            bool isCalled = false;

            // ARRANGE
            // Creating a mock instance of the "Foo" class.
            var foo = Mock.Create<Foo>();

            // Arranging: When foo.Echo() is called with any integer as an argument, it should raise foo.OnEchoCallback with true as an argument.
            Mock.Arrange(() => foo.Echo(Arg.IsAny<int>())).Raises(() => foo.OnEchoCallback += null, true);

            foo.OnEchoCallback += delegate(bool echoed)
            {
                isCalled = echoed;
            };

            // ACT
            foo.Echo(10);

            // ASSERT
            Assert.IsTrue(isCalled);
        }

        [TestMethod]
        public void ShouldAssertOnGenericTypesAndMethod()
        {
            string expected = "ping";

            // ARRANGE
            var foo = Mock.Create<FooGeneric>();

            // Arranging: When foo.Echo<string, string>() is called with an expected argument, it should return that argument.
            Mock.Arrange(() => foo.Echo<string, string>(expected)).Returns((string s) => s);

            // ACT
            string actual = foo.Echo<string, string>(expected);

            // ASSERT
            Assert.AreEqual(expected, actual);
        }
    }

    #region SUT
    public class Foo
    {
        public int Execute(int arg1, int arg2)
        {
            throw new NotImplementedException();
        }

        public int Execute(int arg1)
        {
            throw new NotImplementedException();
        }

        public int Echo(int arg1)
        {
            return arg1;
        }

        public string FooProp { get; set; }

        public delegate void EchoEventHandler(bool echoed);
        public event EchoEventHandler OnEchoCallback;
    }

    public class FooGeneric
    {
        public TRet Echo<T, TRet>(T arg1)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
