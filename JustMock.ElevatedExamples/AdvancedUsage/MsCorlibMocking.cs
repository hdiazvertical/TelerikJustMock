using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using System.IO;

namespace JustMock.ElevatedExamples.AdvancedUsage.MsCorlibMocking
{
    /// <summary>
    /// Telerik JustMock enables you to mock methods from the .NET Framework, i.e. from MsCorlib.
    /// Even more. it allows you to mock MsCorlib functions/members by directly arranging their behavior, without any prior configurations.
    /// See http://www.telerik.com/help/justmock/advanced-usage-mscorlib-mocking.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class MsCorlibMocking_Tests
    {
        [TestMethod]
        public void ShouldAssertCustomValueForDateTimeNow()
        {
            var expected = new DateTime(1900, 4, 12);

            // ARRANGE - Here we arrange, when DateTime.Now is called it should return expected DateTime.
            Mock.Arrange(() => DateTime.Now).Returns(expected);

            // ACT
            var now = DateTime.Now;

            // ASSERT
            Assert.AreEqual(expected.Year, now.Year);
            Assert.AreEqual(expected.Month, now.Month);
            Assert.AreEqual(expected.Day, now.Day);
        }

        [TestMethod]
        public void ShouldArrangeCustomValueForDateTimeNowInCustomClass()
        {
            var expected = new DateTime(1900, 4, 12);

            // ARRANGE - Here we arrange, when DateTime.Now is called it should return expected DateTime.
            Mock.Arrange(() => DateTime.Now).Returns(expected);

            // ACT - This time we will get the DateTime.Now from another instance.
            var now = new NestedDateTime().GetDateTime();

            // Assert
            Assert.AreEqual(expected.Year, now.Year);
            Assert.AreEqual(expected.Month, now.Month);
            Assert.AreEqual(expected.Day, now.Day);
        }

        [TestMethod]
        public void ShouldMockFileInfoDelete()
        {
            var filename = this.GetType().Assembly.ManifestModule.FullyQualifiedName;
            var MyFileInfo = new FileInfo(filename);

            var isCalled = false;

            // ARRANGE - When MyFileInfo.Delete() is called it should set isCalled to true instead of executing its original logic.
            Mock.Arrange(() => MyFileInfo.Delete()).DoInstead(() => isCalled = true);

            // ACT
            MyFileInfo.Delete();

            // ASSERT
            Assert.IsTrue(isCalled);
        }

        [TestMethod]
        public void ShouldMockDriveInfoGetDrives()
        {
            bool isCalled = false;

            // ARRANGE - When the static DriveInfo.GetDrives() method is called it should set isCalled to true instead of executing its original logic.
            Mock.Arrange(() => DriveInfo.GetDrives()).DoInstead(() => isCalled = true);

            // ACT
            DriveInfo.GetDrives();

            // ASSERT
            Assert.IsTrue(isCalled);
        }

        [TestMethod]
        public void ShouldMockStaticFileForReadOperaton()
        {
            byte[] fakeBytes = "byte content".ToCharArray().Select(c => (byte)c).ToArray();

            // ARRANGE - When File.ReadAllBytes() is called with any string argument, it should return fakeBytes array.
            Mock.Arrange(() => File.ReadAllBytes(Arg.IsAny<string>())).Returns(fakeBytes);

            // ACT
            var actual = File.ReadAllBytes("ping");

            // ASSERT - If both have the same length.
            Assert.AreEqual(fakeBytes.Length, actual.Length);

            // ASSERT - If both have the same content
            var notificator = true;

            for (var i = 0; i < fakeBytes.Length; i++)
            {
                if (fakeBytes[i] != actual[i])
                {
                    notificator = false;
                    break;
                }
            }

            Assert.AreEqual(true, notificator);
        }

        [TestMethod]
        public void ShouldMockFileOpenWithCustomFileStream()
        {
            byte[] actual = new byte[255];

            // Writing locally, can be done from resource manifest as well.

            using (StreamWriter writer = new StreamWriter(new MemoryStream(actual)))
            {
                writer.WriteLine("Hello world");
                writer.Flush();
            }

            // ARRANGE
            // Arranging the file stream.
            FileStream fileStreamMock = Mock.Create<FileStream>(Constructor.Mocked);

            // Mocking the specific call and setting up expectations. 
            // We replace the actual implementation.
            // Calling the Write method will result in coping the content of the 'actual' byte array to the passed byte array.
            Mock.Arrange(() => fileStreamMock.Write(null, 0, 0)).IgnoreArguments()
                .DoInstead((byte[] content, int offset, int len) => actual.CopyTo(content, offset));

            // Arranging that File.Open() with any string and FileMode arguments, should return fileStreamMock.
            Mock.Arrange(() => File.Open(Arg.AnyString, Arg.IsAny<FileMode>())).Returns(fileStreamMock);


            // ACT - 'fileStream' is assigned with the custom stream returned from File.Open.
            var fileStream = File.Open("hello.txt", FileMode.Open);
            byte[] fakeContent = new byte[actual.Length];

            // Original task
            // After this, as arranged, the content of 'actual' and 'fakeContent' will be the same.
            fileStream.Write(fakeContent, 0, actual.Length);

            // ASSERT
            Assert.AreEqual(fakeContent.Length, actual.Length);

            for (var i = 0; i < fakeContent.Length; i++)
            {
                Assert.AreEqual(fakeContent[i], actual[i]);
            }
        }

    }

    #region SUT
    public class NestedDateTime
    {
        public DateTime GetDateTime()
        {
            return DateTime.Now;
        }
    }
    #endregion
}
