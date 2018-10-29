using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace JustMock.ElevatedExamples.BasicUsage.RecursiveMocking
{
    /// <summary>
    /// Recursive mocks enable you to mock members that are obtained as a result of "chained" calls on a mock. 
    /// For example, recursive mocking is useful in the cases when you test code like this: foo.Bar.Baz.Do("x"). 
    /// See http://www.telerik.com/help/justmock/basic-usage-recursive-mocking.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class RecursiveMocking_Tests
    {
        [TestMethod]
        public void ShouldAssertStaticNestedMockWithRecursiveArrangement()
        {
            // ARRANGE
            var person = new Person();

            // Arranging: When PersonFactory.CreatePerson().Old() is called, it should return the expected person. 
            //              This will automatically create mock of PersonFactory.CreatePerson() 
            //              and a NullReferenceException will be avoided.
            Mock.Arrange(() => PersonFactory.CreatePerson().Old()).Returns(person);

            // ACT
            var actual = PersonFactory.CreatePerson().Old();

            // ASSERT
            Assert.IsTrue(object.ReferenceEquals(person, actual));
        }

        [TestMethod]
        public void ShouldAssertStaticNestedMockWithMixedPropertyUsingRecursiveArrangement()
        {
            // ARRANGE
            var person = new Person();

            // Arranging: When PersonFactory.CurrentPerson.Old() is called, it should return the expected person. 
            //              This will automatically create mock of PersonFactory.CurrentPerson
            //              and a NullReferenceException will be avoided.
            Mock.Arrange(() => PersonFactory.CurrentPerson.Old()).Returns(person);

            // ACT
            var actual = PersonFactory.CurrentPerson.Old();

            // ASSERT
            Assert.IsTrue(object.ReferenceEquals(person, actual));
        }
    }

    #region SUT
    public static class PersonFactory
    {
        public static Person CreatePerson()
        {
            return new Person();
        }

        public static Person CurrentPerson
        {
            get
            {
                return new Person();
            }
        }
    }

    public class Person
    {
        public Person Old()
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
