using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.ElevatedExamples.AdvancedUsage.SealedMocking
{
    /// <summary>
    /// Sealed mocking is one of the advanced features supported in Telerik JustMock. It allows you to fake sealed classes 
    /// and calls to their methods/properties, set expectations and verify results using the AAA principle. Faking sealed 
    /// classes and calls to their methods/properties doesn't affect the way you write your tests, i.e. the same syntax is 
    /// used for mocking non sealed classes. 
    /// See http://www.telerik.com/help/justmock/advanced-usage-sealed-mocking.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class SealedMocking_Tests
    {
        [TestMethod]
        public void ShouldAssertFinalMethodCallOnASealedClass()
        {
            var expected = 10;

            // ARRANGE
            // Creating a mock instance of the "FooSealed" class (sealed class).
            var foo = Mock.Create<FooSealed>();
            
            // Arranging: When foo.Echo() is called with any integer argument, it should return expected.
            Mock.Arrange(() => foo.Echo(Arg.IsAny<int>())).Returns(expected);

            // ACT
            var actual = foo.Echo(1);

            // ASSERT
            Assert.AreEqual(expected, actual);    
        }

        [TestMethod]
        public void ShouldCreateMockForASealedClassWithInternalConstructor()
        {
            var expected = 10;

            // ARRANGE
            // Creating a mock instance of the "FooSealedInternal" class (sealed class with internal constructor).
            var foo = Mock.Create<FooSealedInternal>();
            
            // Arranging: When foo.Echo() is called with any integer argument, it should return expected.
            Mock.Arrange(() => foo.Echo(Arg.IsAny<int>())).Returns(expected);

            // ASSERT - Asserting that our mock in not null.
            Assert.IsNotNull(foo);

            // ACT
            var actual = foo.Echo(1);

            // ASSERT
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ShouldAssertCallOnVoid()
        {
            bool isCalled = false;

            // ARRANGE
            // Creating a mock instance of the "Foo" class (sealed class).
            var foo = Mock.Create<Foo>();

            // Arranging: When foo.Execute() is called, it should assign true to the isCalled boolean instead.
            Mock.Arrange(() => foo.Execute()).DoInstead(() => isCalled = true);

            // Act
            foo.Execute();

            // Assert
            Assert.IsTrue(isCalled);
        }

        [TestMethod]
        public void ShouldAssertCallOnVoidThroughAnInterface()
        {
            bool isCalled = false;

            // ARRANGE
            // Creating a mock instance of the "Foo" class (sealed class).
            var foo = Mock.Create<Foo>();

            // Arranging: When foo.Execute() is called, it should assign true to the isCalled boolean instead.
            Mock.Arrange(() => foo.Execute()).DoInstead(() => isCalled = true);

            // ACT - Acting through the IFoo interface.
            IFoo iFoo = foo;
            iFoo.Execute();

            // ASSERT
            Assert.IsTrue(isCalled);
        }
    }

    #region SUT
    public sealed class FooSealed
    {
        public int Echo(int arg1)
        {
            return arg1;
        }
    }

    public sealed class FooSealedInternal
    {
        internal FooSealedInternal()
        {

        }

        public int Echo(int arg1)
        {
            return arg1;
        }
    }

    public interface IFoo
    {
        void Execute();
        void Execute(int arg1);
    }

    public sealed class Foo : IFoo
    {
        public void Execute()
        {
            throw new NotImplementedException();
        }

        void IFoo.Execute(int arg1)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
