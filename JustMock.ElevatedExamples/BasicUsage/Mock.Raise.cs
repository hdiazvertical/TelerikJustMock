using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.ElevatedExamples.BasicUsage.Mock_Raise
{
    /// <summary>
    /// The Raise method is used for raising mocked events. You can use custom or standard events.
    /// See http://www.telerik.com/help/justmock/basic-usage-mock-raise.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class Mock_Raise_Tests
    {
        [TestMethod]
        public void ShouldRaiseNonVirtualEvent()
        {
            // ARRANGE
            // Creating a mocked instance of the "FooEvent" class.
            var foo = Mock.Create<FooEvent>();

            bool isCalled = false;

            foo.StandardEvent += (sender, args) => { isCalled = true; };

            // ACT - Raising foo.StandardEvent
            Mock.Raise(() => foo.StandardEvent += null, EventArgs.Empty);

            // ASSERT
            Assert.IsTrue(isCalled);
        }

        [TestMethod]
        public void TestEventMethod_OnTestEventRaised_IsInvokedShouldBeTrue()
        {
            // ARRANGE
            // Creating a mocked instance of the "FooTestHandler" class.
            var foo = Mock.Create<FooTestHandler>(Behavior.CallOriginal);

            // ACT - Raising foo.TestEvent
            Mock.Raise(() => foo.TestEvent += null, EventArgs.Empty);
            
            // ASSERT
            Assert.IsTrue(foo.IsInvoked);
        }

        [TestMethod]
        public void ShouldRaiseStaticEvent()
        {
            FooArgs args = null;

            // ARRANGE
            // Setting up the StaticEventFoo class for mocking.
            Mock.SetupStatic(typeof(StaticEventFoo));

            StaticEventFoo.Done += (o, e) => args = e;

            // ACT - Raising the static event.
            Mock.Raise(() => StaticEventFoo.Done += null, new FooArgs(null));

            // ASSERT
            Assert.IsNotNull(args);
        }
    }

    #region SUT
    public class FooEvent
    {
        public event EventHandler<EventArgs> StandardEvent;
    }

    public delegate void TestHandler(object sender, EventArgs args);

    public class FooTestHandler
    {
        public event TestHandler TestEvent;

        public FooTestHandler()
        {
            TestEvent += TestEventMethod;
        }

        public void TestEventMethod(object sender, EventArgs args)
        {
            IsInvoked = true;
        }

        public bool IsInvoked { get; set; }
    }

    public class StaticEventFoo
    {
        public static event EventHandler<FooArgs> Done;
    }

    public class FooArgs : EventArgs
    {
        public FooArgs()
        {
        }

        public FooArgs(string value)
        {
            this.Value = value;
        }

        public string Value { get; set; }
    }
    #endregion
}
