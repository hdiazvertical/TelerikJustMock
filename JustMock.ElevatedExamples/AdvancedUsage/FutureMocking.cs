using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.ElevatedExamples.AdvancedUsage.FutureMocking
{
    /// <summary>
    /// Future Mocking allows you to mock members without passing the dependency through a constructor or calling 
    /// a method. You can apply future mocking automatically based on an expectation rather than applying it to every 
    /// instance explicitly. You will find this handy especially in case you have to mock third party controls and 
    /// tools where you have little control over the way in which they are created. 
    /// We can apply the IgnoreInstance() method to our arrangements in order to arrange expectations for the future 
    /// instances of a given type.
    /// See http://www.telerik.com/help/justmock/advanced-usage-future-mocking.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class FutureMocking_Tests
    {
        [TestMethod]
        public void ShouldApplyIgnoreInstanceToVirtual()
        {
            var expected = 10;

            // ARRANGE
            // Creating a mocked instance of the "Calculus" class.
            // This mock will also be used as a fake of a given type for applying a future instance expectation to that type.
            var calculus = Mock.Create<Calculus>();

            // Arranging: When calculus.Sum() is called, it should return expected.
            //              Also, this expectations will apply for all newly created instances of the Calculus class 
            //              during the test.
            Mock.Arrange(() => calculus.Sum()).IgnoreInstance().Returns(expected);

            // ASSERT
            Assert.AreEqual(expected, calculus.Sum());
            Assert.AreEqual(expected, new Calculus().Sum()); // Calling the Sum method from a fresh instance.
        }

        public IList<object> FakeCollection()
        {
            List<object> resultCollection = new List<object>();

            resultCollection.Add("asd");
            resultCollection.Add(123);
            resultCollection.Add(true);

            return resultCollection;
        }

        [TestMethod]
        public void ShouldReturnFakeCollectionForFutureCall()
        {
            var expectedCollection = FakeCollection();

            // ARRANGE
            // Creating a mocked instance of the "Foo" class.
            // This mock will also be used as a fake of a given type for applying a future instance expectation to that type.
            var fooMocked = Mock.Create<Foo>();

            // Arranging: When fooMocked.RealCollection_GET is called, it should return expected collection.
            //              Also, this expectations will apply for all newly created instances of the Calculus class 
            //              during the test.
            Mock.Arrange(() => fooMocked.RealCollection).IgnoreInstance().ReturnsCollection(expectedCollection);

            // ACT
            var actualArrangedCollection = fooMocked.RealCollection;
            var actualUnArrangedCollection = new Foo().RealCollection; // Getting the RealCollection from a fresh instance.

            // ASSERT
            // Asserting for the arranged instance.
            Assert.AreEqual(expectedCollection.Count, actualArrangedCollection.Count);
            Assert.AreEqual(expectedCollection.FirstOrDefault(), actualArrangedCollection.FirstOrDefault());

            // Asserting for the new instance.
            Assert.AreEqual(expectedCollection.Count, actualUnArrangedCollection.Count);
            Assert.AreEqual(expectedCollection.FirstOrDefault(), actualUnArrangedCollection.FirstOrDefault());
        }

        [TestMethod]
        public void ShouldMockConstructorForFutureInstances()
        {
            // ARRANGE - Every new instantiation of the FooWithNotImplementedConstructor class should do nothing.
            //              Here we are future mocking the constructor of the FooWithNotImplementedConstructor class.
            Mock.Arrange(() => new FooWithNotImplementedConstructor()).DoNothing();

            // ACT
            var myNewInstance = new FooWithNotImplementedConstructor(); // This will not throw an exception

            // ASSERT
            Assert.IsNotNull(myNewInstance);
            Assert.IsInstanceOfType(myNewInstance, typeof(FooWithNotImplementedConstructor));
        }

        [TestMethod]
        public void ShouldMockNewObjectCreation()
        {
            // ARRANGE - Every new instantiation of the Foo class should return a predefined instance.
            var testObj = new Foo();
            testObj.MyProp = "Test";

            // Directly arranging the expression to return our predefined object.
            Mock.Arrange(() => new Foo()).Returns(testObj);

            // ACT
            var myNewInstance = GetNewFooInstance();

            // ASSERT
            Assert.IsNotNull(myNewInstance);
            Assert.IsInstanceOfType(myNewInstance, typeof(Foo));
            // Assert that the returned instance is equal to the predefined.
            Assert.AreEqual("Test", myNewInstance.MyProp);
        }

        public Foo GetNewFooInstance()
        {
            return new Foo();
        }
    }

    #region SUT
    public class Calculus
    {
        public virtual int Sum()
        {
            return 0;
        }
    }

    public class Foo
    {
        public string MyProp { get; set; }

        public Foo()
        {
        }

        public List<object> RealCollection
        {
            get { return collection; }
        }
        private List<object> collection = null;
    }

    public class FooWithNotImplementedConstructor
    {
        public FooWithNotImplementedConstructor()
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
