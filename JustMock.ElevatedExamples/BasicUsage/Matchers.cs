using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.ElevatedExamples.BasicUsage.Matchers
{
    /// <summary>
    /// See http://www.telerik.com/help/justmock/basic-usage-matchers.html for full documentation of the feature.
    /// Matchers let you ignore passing actual values as arguments used in mocks. 
    /// Instead, they give you the possibility to pass just an expression that satisfies the 
    /// argument type or the expected value range. There are several types of matchers supported in Telerik JustMock:
    ///     - Defined Matchers:
    ///         Arg.AnyBool
    ///         Arg.AnyDouble
    ///         Arg.AnyFloat
    ///         Arg.AnyGuid
    ///         Arg.AnyInt
    ///         Arg.AnyLong
    ///         Arg.AnyObject
    ///         Arg.AnyShort
    ///         Arg.AnyString
    ///         Arg.NullOrEmpty
    ///     - Arg.IsAny<[Type]>();
    ///     - Arg.IsInRange([FromValue : int], [ToValue : int], [RangeKind])
    ///     - Arg.Matches<T>(Expression<Predicate<T>> expression) 
    /// </summary>
    [TestClass]
    public class Matchers_Tests
    {
        [TestMethod]
        public void ShouldMockMethodUsingDefinedMathcers()
        {
            // ARRANGE
            // Creating a mock instance of the "Foo" class with Behavior.CallOriginal.
            var foo = Mock.Create<Foo>(Behavior.CallOriginal);

            // Arranging: foo.Execute() should do nothing and must be called with defined matchers for arguments.
            Mock.Arrange(() => foo.Execute(Arg.AnyInt, Arg.AnyString, Arg.AnyBool, Arg.AnyGuid, Arg.AnyObject))
                .DoNothing()
                .MustBeCalled();

            // ACT
            foo.Execute(1, "test", false, new Guid(), null);

            // ASSERT
            Mock.Assert(foo);
        }

        [TestMethod]
        public void ShouldMockMethodUsingArgIsAny()
        {
            // ARRANGE
            // Creating a mock instance of the "Foo" class with Behavior.CallOriginal.
            var foo = Mock.Create<Foo>(Behavior.CallOriginal);

            // Arranging: foo.Execute() should do nothing and must be called with Arg.IsAny<T>() matchers for arguments.
            Mock.Arrange(() => foo.Execute(
                Arg.IsAny<int>(), Arg.IsAny<string>(), Arg.IsAny<bool>(), Arg.IsAny<Guid>(), Arg.IsAny<object>()))
                    .DoNothing()
                    .MustBeCalled();

            // ACT
            foo.Execute(1, "test", false, new Guid(), null);

            // ASSERT
            Mock.Assert(foo);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ShouldMockMethodUsingArgIsInRange()
        {
            // ARRANGE
            // Creating a mock instance of the "Foo" class with Behavior.CallOriginal.
            var foo = Mock.Create<Foo>(Behavior.CallOriginal);

            // Arranging: foo.Execute() should do nothing and must be called with Arg.IsAny<T>() matchers for arguments.
            Mock.Arrange(() => foo.Execute(Arg.IsInRange<int>(5, 10, RangeKind.Inclusive)))
                    .DoNothing()
                    .MustBeCalled();

            // ACT - Calls in the expected range [5, 10].
            foo.Execute(5);
            foo.Execute(8);
            foo.Execute(10);

            // ASSERT
            Mock.Assert(foo);

            // ACT - This will call the original method implementation.
            foo.Execute(1);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ShouldMockMethodUsingArgMatches()
        {
            // ARRANGE
            // Creating a mock instance of the "Foo" class with Behavior.CallOriginal.
            var foo = Mock.Create<Foo>(Behavior.CallOriginal);

            // Arranging: foo.Execute() should do nothing and must be called when instantiated with integer matching 5 as an argument.
            Mock.Arrange(() => foo.Execute(Arg.Matches<int>(x => x == 5)))
                    .DoNothing()
                    .MustBeCalled();

            // ACT - Call with the expected argument (5).
            foo.Execute(5);

            // ASSERT
            Mock.Assert(foo);

            // ACT - This will call the original method implementation.
            foo.Execute(1);
        }
    }

    #region SUT
    public class Foo
    {
        public void Execute(int arg1)
        {
            throw new NotImplementedException();
        }

        public void Execute(int arg1, string arg2, bool arg3, Guid arg4, object arg5)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
