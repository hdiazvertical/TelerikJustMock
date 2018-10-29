using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.ElevatedExamples.BasicUsage.Mock_DoInstead
{
    /// <summary>
    /// The DoInstead method is used to replace the actual implementation of a method with a mocked one.
    /// See http://www.telerik.com/help/justmock/basic-usage-mock-do-instead.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class Mock_DoInstead_Tests
    {
        [TestMethod]
        public void Echo_OnExecuteWithAnyIntArg_ShouldAssignArgToVariable()
        {
            // ARRANGE
            // Creating a mock instance of the "Foo" class.
            var foo = Mock.Create<Foo>();

            int expected = 4;
            int actual = 0;

            // Arranging: When foo.Echo() is called with any integer as an argument it should assign the argument to the "actual" variable.
            Mock.Arrange(() => foo.Echo(Arg.AnyInt)).DoInstead(
                (int a) =>
                {
                    actual = a;
                });

            // ACT
            foo.Echo(expected);

            // ASSERT
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Bar_OnExecuteWithAnyActionArg_ShouldPerformTheAction()
        {
            // ARRANGE
            // Creating a mock instance of the "Foo" class.
            var foo = Mock.Create<Foo>();

            int expected = 4;
            int actual = 0;

            // Arranging: When foo.Bar() is called with any Action as an argument it should perform that Action instead.
            Mock.Arrange(() => foo.Bar(Arg.IsAny<Action>())).DoInstead((Action action) => action());

            // ACT - Calling foo.Bar() with Action that will assign the expected value to the actual variable.
            foo.Bar(new Action(() =>
            {
                actual = expected;
            }));

            // ASSERT
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ShouldSetFieldOnMethodCallNoMatterTheInstance()
        {
            // ARRANGE
            var foo = new Foo();

            // Arranging: When foo.Submit() is called it should set foo.intField to 1 instead, no matter the Foo instance.
            Mock.Arrange(() => foo.Submit()).IgnoreInstance().DoInstead<Foo>(x => x.intField = 1);

            // ACT
            var newFoo = new Foo();
            newFoo.Submit();

            // ASSERT
            Assert.AreEqual(1, newFoo.intField);
        }


        [TestMethod]
        public void ShouldSetFieldOnClassInitialization()
        {
            // ARRANGE
            // Arranging: When new instance of the Foo class is created, instead of executing the Foo constructor, 
            //  it should set the intField to 1.
            Mock.Arrange(() => new Foo()).DoInstead<Foo>(x => x.intField = 1);

            // ACT
            var newFoo = new Foo();

            // ASSERT
            Assert.AreEqual(1, newFoo.intField);
        }
    }

    #region SUT
    
    public class Foo
    {
        public int intField;

        public void Submit() { }

        public int Echo(int num)
        {
            return num;
        }
        
        public void Bar(Action action)
        {
            action();
        }
    }

    #endregion
}