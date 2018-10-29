using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.ElevatedExamples.AdvancedUsage.PrivateAccessorNamespace
{
    /// <summary>
    /// The Telerik JustMock PrivateAccessor allows you to call non-public members of the tested code right in your unit tests. 
    /// See http://www.telerik.com/help/justmock/advanced-usage-private-accessor.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class PrivateAccessor_Tests
    {
        [TestMethod]
        public void ShouldCallArrangedPrivateMethod()
        {
            // ARRANGE
            // Create a mocked instance of your class under test with original behavior. 
            //  You can also use original instance object and perform partial mocking.
            var mockedClass = Mock.Create<ClassWithNonPublicMembers>(Behavior.CallOriginal);

            // Arranging: When the private MePrivate() is called from the mockedClass instance, it should return 5.
            Mock.NonPublic.Arrange<int>(mockedClass, "MePrivate").Returns(5);

            // ACT
            // Wrapping the mocked instance with a private accessor.
            var inst = new PrivateAccessor(mockedClass);
            // Calling the non-public method by giving its exact name.
            var actual = inst.CallMethod("MePrivate");

            // ASSERT - No matter the mock is wrapped, it should keep its arrangements.
            Assert.AreEqual(5, actual);
        } 

        [TestMethod]
        public void ShouldGetArrangedPrivateProperty()
        {
            // ARRANGE
            // Create a mocked instance of your class under test with original behavior. 
            //  You can also use original instance object and perform partial mocking.
            var mockedClass = Mock.Create<ClassWithNonPublicMembers>(Behavior.CallOriginal);

            // Arranging: When the private Prop is called from the mockedClass instance, it should return 5.
            Mock.NonPublic.Arrange<int>(mockedClass, "Prop").Returns(5);

            // ACT
            // Wrapping the mocked instance with a private accessor.
            var inst = new PrivateAccessor(mockedClass);
            // Calling the non-public method by giving its exact name.
            var actual = inst.GetProperty("Prop");

            // ASSERT - No matter the mock is wrapped, it should keep its arrangements.
            Assert.AreEqual(5, actual);
        }

        [TestMethod]
        public void ShouldCallArrangedStaticPrivateMethod()
        {
            // ARRANGE
            // Setup your class under test for static mocking. 
            //  You can also use original instance object and perform partial mocking.
            Mock.SetupStatic(typeof(ClassWithNonPublicMembers));

            // Arranging: When the private MeStaticPrivate is called from an instance of type ClassWithNonPublicMembers, it should return 5.
            Mock.NonPublic.Arrange<int>(typeof(ClassWithNonPublicMembers), "MeStaticPrivate").Returns(5);

            // ACT
            // Wrapping the mocked instance by type.
            var inst = PrivateAccessor.ForType(typeof(ClassWithNonPublicMembers));
            // Calling the non-public static method by giving its exact name.
            var actual = inst.CallMethod("MeStaticPrivate");

            // ASSERT - No matter the mock is wrapped, it should keep its arrangements.
            Assert.AreEqual(5, actual);
        } 
    }

    #region SUT
    public class ClassWithNonPublicMembers
    {
        private int Prop { get; set; }

        private int MePrivate()
        {
            return 1000;
        }

        private static int MeStaticPrivate()
        {
            return 2000;
        }
    } 
    #endregion
}
