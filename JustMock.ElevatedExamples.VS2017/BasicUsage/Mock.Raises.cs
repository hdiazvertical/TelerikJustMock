using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.ElevatedExamples.BasicUsage.Mock_Raises
{
    /// <summary>
    /// The Raises method is used to fire an event once a method is called.
    /// See http://www.telerik.com/help/justmock/basic-usage-mock-raises.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class Mock_Raises_Tests
    {
        [TestMethod]
        public void ShouldAssertCustomEventCall()
        {
            bool isCalled = false;

            // ARRANGE
            // Creating a mocked instance of the "Foo" class.
            var foo = Mock.Create<Foo>();

            // Arranging: When foo.Echo() is called with any integer as an argument, 
            //  it should raise foo.OnEchoCallback with arg: true.
            Mock.Arrange(() => foo.Echo(Arg.IsAny<int>())).Raises(() => foo.OnEchoCallback += null, true);

            foo.OnEchoCallback += delegate(bool arg)
            {
                isCalled = arg;
            };

            // ACT
            foo.Echo(10);

            // ASSERT
            Assert.IsTrue(isCalled);
        }
    }

    #region SUT
    public class Foo
    {
        public int Echo(int arg)
        {
            return arg * 3;
        }

        public string FooProp { get; set; }


        public delegate void EchoEventHandler(bool echoed);
        public event EchoEventHandler OnEchoCallback;

    }
    #endregion
}
