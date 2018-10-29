using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.ElevatedExamples.AdvancedUsage.MockingDLLImports
{
    /// <summary>
    /// In elevated mode, you can use Telerik JustMock to mock imported functions (decorated with the [DLLImport()] attribute). 
    /// See http://www.telerik.com/help/justmock/advanced-usage-mocking-dll-imports.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class MockingDLLImports_Tests
    {
        [TestMethod]
        public void FormatCurrentProcessId_OnExecute_ShouldReturnExpected()
        {
            var expected = 3500;

            // ARRANGE - When the static Foo.GetCurrentProcessId() function is called, it should return expected.
            //  Note, GetCurrentProcessId() is imported function from the Kernel32 dll.
            Mock.Arrange(() => Foo.GetCurrentProcessId()).Returns(expected);

            // ACT
            var myFoo = new Foo();
            var actual = myFoo.FormatCurrentProcessId();

            // ASSERT
            Assert.AreEqual(string.Format("The current process ID is {0}", expected), actual);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void Start_OnExecute_ShouldThrowNotImplementedException()
        {
            // ARRANGE - When the static Foo.MessageBox() function is called with any type matching arguments, it should do nothing.
            //  Note, MessageBox() is imported function from the user32 dll.
            Mock.Arrange(() => Foo.MessageBox(Arg.IsAny<IntPtr>(), Arg.AnyString, Arg.AnyString, Arg.IsAny<uint>())).DoNothing();

            // ACT
            var myFoo = new Foo();
            myFoo.Start();
        } 

        [TestMethod]
        public void Is64BitProcessMessage_On64BitExecute_ShouldReturnExpected()
        {
            // ARRANGE - When the static, non-public IsWow64Process() function is called from an instance of type "Foo", it should return true.
            //  Note, IsWow64Process() is imported function from the Kernel32 dll.
            Mock.NonPublic.Arrange<bool>(typeof(Foo), "IsWow64Process").Returns(true);

            // ACT
            var myFoo = new Foo();
            var actual = myFoo.Is64BitProcessMessage();

            // ASSERT
            Assert.AreEqual("This is a 64 bit process!", actual);
        } 

        [TestMethod]
        public void Is64BitProcessMessage_On32BitExecute_ShouldReturnExpected()
        {
            // ARRANGE - When the static, non-public IsWow64Process() function is called from an instance of type "Foo", it should return false.
            //  Note, IsWow64Process() is imported function from the Kernel32 dll.
            Mock.NonPublic.Arrange<bool>(typeof(Foo), "IsWow64Process").Returns(false);

            // ACT
            var myFoo = new Foo();
            var actual = myFoo.Is64BitProcessMessage();

            // ASSERT
            Assert.AreEqual("This is a 32 bit process!", actual);
        } 
    }

    #region SUT
    public class Foo
    {
        [DllImport("Kernel32.dll")]
        public static extern int GetCurrentProcessId();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MessageBox
           (IntPtr hWnd, String text, String caption, uint type);

        [DllImport("Kernel32.dll")]
        private static extern bool IsWow64Process();

        public string FormatCurrentProcessId()
        {
            var myPId = GetCurrentProcessId();
            string returnValue = string.Format("The current process ID is {0}", myPId);
            return returnValue;
        }

        public void Start()
        {
            var msgBoxRet = MessageBox(new IntPtr(0), "Process Starts Now!", "Message Dialog", 0);

            throw new NotImplementedException();
        }

        public string Is64BitProcessMessage()
        {
            bool is64Bit = IsWow64Process();

            if (is64Bit)
            {
                return string.Format("This is a 64 bit process!");
            }
            return string.Format("This is a 32 bit process!");
        }
    } 
    #endregion
}
