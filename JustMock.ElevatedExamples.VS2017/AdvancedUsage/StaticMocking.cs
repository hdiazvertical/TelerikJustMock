using System;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.Core;

namespace JustMock.ElevatedExamples.AdvancedUsage.StaticMocking
{
    /// <summary>
    /// Static mocking is one of the advanced features supported in Telerik JustMock. It allows you to fake static constructors, 
    ///  methods and properties calls, set expectations and verify results using the AAA principle. Whether you mock static or 
    ///  instance calls there isn't much difference in the way your organize and write your tests.
    /// We can divide static mocking into the following major parts:
    ///     - Static constructor mocking
    ///     - Static method mocking
    ///     - Extension methods mocking
    /// See http://www.telerik.com/help/justmock/advanced-usage-static-mocking.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class StaticMocking_Tests
    {
        [TestMethod]
        public void ShouldMockStaticClass()
        {
            // ARRANGE
            // Getting the static instance(FooStatic) ready for mocking.
            Mock.SetupStatic(typeof(FooStatic));

            // ACT 
            // This should not throw NotImplementedException as the default mock behavior is loose. 
            //  This means there are only stubs generated for the methods inside the mock.
            FooStatic.Do();
        }

        [TestMethod]
        public void ShouldFakeInternalStaticCall()
        {
            // ARRANGE
            // Getting the static instance(FooInternal) ready for mocking.
            Mock.SetupStatic<FooInternal>();

            // ACT 
            // This should not throw NotImplementedException as the default mock behavior is loose. 
            //  This means there are only stubs generated for the methods inside the mock.
            FooInternal.DoIt();
        }

        [TestMethod]
        public void ShouldArrangeStaticFunction()
        {
            var expected = 0;

            // ARRANGE
            // Getting the static instance(Foo) ready for mocking disregarding the constructor.
            // If we don't mock the constructor, a NotImplementedException will be thrown.
            Mock.SetupStatic(typeof(Foo), StaticConstructor.Mocked);

            // Arranging: When the static(Foo.FooProp_GET) property is called, it should return expected.
            Mock.Arrange(() => Foo.FooProp).Returns(expected);

            // Assert
            Assert.AreEqual(expected, Foo.FooProp);
        }

        [TestMethod]
        [ExpectedException(typeof(StrictMockException))]
        public void ShouldMockStaticClassWithStrictBehavior()
        {
            var expected = 10;

            // ARRANGE
            // Getting the static instance(Foo) ready for mocking disregarding the constructor and applying strict behavior.
            // If we don't mock the constructor, a NotImplementedException will be thrown.
            Mock.SetupStatic(typeof(Foo), Behavior.Strict, StaticConstructor.Mocked);

            // Arranging: When the Foo.Execute() method is called with 10, it should return expected.
            Mock.Arrange(() => Foo.Execute(10)).Returns(expected);

            // ASSERT
            Assert.AreEqual(expected, Foo.Execute(10));

            // ACT - This throws MockException as there is no arrange associated with the Submit method.
            Foo.Submit();
        }

        [TestMethod]
        public void ShouldFakeStaticPropertyGet()
        {
            bool isCalled = false;
            var expected = 1;

            // ARRANGE
            // Getting the static instance(Foo) ready for mocking disregarding the constructor and applying strict behavior.
            // If we don't mock the constructor, a NotImplementedException will be thrown.
            Mock.SetupStatic(typeof(Foo), Behavior.Strict, StaticConstructor.Mocked);

            // Arranging: When the static(Foo.FooProp_GET) property is called, it should assign true to isCalled and return expected instead.
            Mock.Arrange(() => Foo.FooProp).DoInstead(() => { isCalled = true; }).Returns(expected);

            // ACT
            var actual = Foo.FooProp;

            // ASSERT
            Assert.AreEqual(expected, actual);
            Assert.IsTrue(isCalled);
        }

        [TestMethod]
        [ExpectedException(typeof(StrictMockException))]
        public void ShouldFakeStaticPropertySet()
        {
            bool isCalled = false;

            // ARRANGE
            // Getting the static instance(Foo) ready for mocking disregarding the constructor and applying strict behavior.
            // If we don't mock the constructor, a NotImplementedException will be thrown.
            Mock.SetupStatic(typeof(Foo), Behavior.Strict, StaticConstructor.Mocked);

            // Arranging: When the static(Foo.FooProp_SET) property is set to 10, it should assign true to isCalled instead.
            Mock.ArrangeSet(() => { Foo.FooProp = 10; }).DoInstead(() => { isCalled = true; });

            // ACT
            Foo.FooProp = 10;

            // ASSERT
            Assert.IsTrue(isCalled);

            // ACT - This should throw MockException.
            Foo.FooProp = 11;
        }

        [TestMethod]
        public void ShouldFakeExtensionMethod()
        {
            var expected = 11;
            // ARRANGE
            var foo = new Bar();

            // Arranging: When foo.Echo() is called with 10, it should return expected.
            Mock.Arrange(() => foo.Echo(10)).Returns(expected);

            // ACT
            var actual = foo.Echo(10);

            // ASSERT
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ShouldAssertMockingHttpContext()
        {
            bool isCalled = false;

            // ARRANGE
            // Arranging: When the HttpContext.Current_GET property is called, it should assign true to isCalled instead.
            Mock.Arrange(() => HttpContext.Current).DoInstead(() => isCalled = true);

            // ACT
            var ret = HttpContext.Current;

            // ASSERT
            Assert.IsTrue(isCalled);
        }
    }

    #region SUT
    public class Foo
    {
        static Foo()
        {
            throw new NotImplementedException();
        }

        public static void Submit()
        {
        }

        public static int Execute(int arg)
        {
            throw new NotImplementedException();
        }

        public static int FooProp
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }

    internal class FooInternal
    {
        internal static void DoIt()
        {
            throw new NotImplementedException();
        }
    }

    public static class FooStatic
    {
        public static void Do()
        {
            throw new NotImplementedException();
        }
    }

    public class Bar
    {
        public void Execute()
        {
            throw new NotImplementedException();
        }
    }

    public static class BarExtensions
    {
        public static int Echo(this Bar foo, int arg)
        {
            return default(int);
        }
    }
    #endregion
}
