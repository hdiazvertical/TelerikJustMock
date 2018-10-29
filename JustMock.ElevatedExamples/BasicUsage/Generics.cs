using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.ElevatedExamples.BasicUsage.Generics
{
    /// <summary>
    /// Telerik JustMock allows you to mock generic classes/interfaces/methods in the same way 
    /// as you do it for non-generic ones.
    /// See http://www.telerik.com/help/justmock/basic-usage-generics.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class Generics_Tests
    {
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ShouldMockGenericMethodDependingOnItsConstraint()
        {
            // ARRANGE
            // Creating a mock instance of the "FooGeneric" class.
            var foo = Mock.Create<FooGeneric>(Behavior.CallOriginal);

            // Arranging: When foo.Bar<string>() generic is called, it should do nothing.
            Mock.Arrange(() => foo.Bar<string>()).DoNothing();

            // ACT
            // This will do nothing.
            foo.Bar<string>();

            // This will throw NotImplementedException.
            foo.Bar<int>();
        }

        [TestMethod]
        public void ShouldMockAGenericMethodWithOutArgs()
        {
            string expected = "ping";

            // ARRANGE
            // Creating a mock instance of the "FooGenericByRef" class.
            var foo = Mock.Create<FooGenericByRef>();

            // Arranging: When foo.Submit<string>() generic is called, it should pass already initialized variable.
            Mock.Arrange(() => foo.Submit<string>(out expected));

            // ACT
            string actual = string.Empty;
            foo.Submit<string>(out actual);

            // ASSERT
            Assert.AreEqual(expected, actual);
        }
    }

    #region SUT

    public class FooGeneric
    {
        public void Bar<T>()
        {
            throw new NotImplementedException();
        }
    }

    public class FooGenericByRef
    {
        public void Submit<T>(out T arg1)
        {
            arg1 = default(T);
        }
    }
    #endregion
}
