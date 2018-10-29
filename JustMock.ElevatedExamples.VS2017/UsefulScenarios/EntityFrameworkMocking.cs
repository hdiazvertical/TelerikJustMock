using System;
using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JustMock.ElevatedExamples.UsefulScenarios.EntityFrameworkMocking
{
    /// <summary>
    /// With Microsoft Entity Framework you develop data access application by using a conceptual application model 
    ///  instead of relational storage schema. Telerik JustMock can be used in conjunction with Microsoft Entity 
    ///  Framework. In this topic we will cover some scenarios in unit testing Microsoft Entity Framework.
    /// See http://www.telerik.com/help/justmock/basic-usage-<FEATURE>.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class EntityFrameworkMocking_Tests
    {
        // Constructing fake DB as required. It will be used in our tests later on.
        public IList<Dinner> FakeDinners()
        {
            return new List<Dinner>()
            {
                new Dinner()
                {
                    DinnerID = 1,
                    Title = "FakeDinner 1",
                    EventDate = new DateTime(1222, 12, 12),
                    Address = "Fake Address 1",
                    HostedBy = "Fake Host 1"
                },
                new Dinner()
                {
                    DinnerID = 2,
                    Title = "FakeDinner 2",
                    EventDate = new DateTime(1222, 12, 12),
                    Address = "Fake Address 2",
                    HostedBy = "Fake Host 2"
                },
                new Dinner()
                {
                    DinnerID = 3,
                    Title = "FakeDinner 3",
                    EventDate = new DateTime(1222, 12, 12),
                    Address = "Fake Address 3",
                    HostedBy = "Fake Host 3"
                }
            };
        }

        [TestMethod]
        public void ShouldReturnFakeCollectionWhenExpected()
        {
            // ARRANGE
            NerdDinners nerdDinners = new NerdDinners();

            // Arranging: When nerdDinners.Dinners_GET is called, it should return fake collection (fakeDinners).
            Mock.Arrange(() => nerdDinners.Dinners).ReturnsCollection(FakeDinners());

            // ACT - We call the nerdDinners.Dinners and search for a dinner with a certain DinnerID.
            var actualQuery = from d in nerdDinners.Dinners
                              where d.DinnerID == 1
                              select d;

            // ASSERT
            // We assert that the nerdDinners.Dinners collection will actually return a collection with 2 items.
            Assert.AreEqual(3, nerdDinners.Dinners.Count());
            // We assert that there is only one item in our collection and this item has DinnerID equal to one.
            Assert.AreEqual(1, actualQuery.Count());
            Assert.AreEqual(1, actualQuery.First().DinnerID);
        }

        [TestMethod]
        public void ShouldReturnFakeCollectionForFutureInstance()
        {
            // ARRANGE
            NerdDinners nerdDinners = new NerdDinners();

            // Arranging: When nerdDinners.Dinners_GET is called, it should return fake collection (fakeDinners) no matter the instance.
            Mock.Arrange(() => nerdDinners.Dinners).IgnoreInstance().ReturnsCollection(FakeDinners());

            // ACT
            // We will create another instance of the NerdDinners class and will act on it.
            NerdDinners actualNerdDinners = new NerdDinners();
            
            var actualQuery = from d in actualNerdDinners.Dinners
                              where d.DinnerID == 1
                              select d;

            // ASSERT
            // We assert that the actualNerdDinners.Dinners collection will actually return a collection with 2 items.
            Assert.AreEqual(3, actualNerdDinners.Dinners.Count());
            // We assert that there is only one item in our collection and this item has DinnerID equal to one.
            Assert.AreEqual(1, actualQuery.Count());
            Assert.AreEqual(1, actualQuery.First().DinnerID);
        }

        [TestMethod]
        public void ShouldFakeAddingNewEntityToContext()
        {
            var dinner = new Dinner { DinnerID = 1 };

            IList<Dinner> dinners = new List<Dinner>();

            // ARRANGE
            NerdDinners nerdDinners = new NerdDinners();

            // Arranging: When nerdDinners.Dinners.Add() is called with a Dinner argument, that equals dinner, it should add that argument 
            //             to the dinners collection instead.
            Mock.Arrange(() => nerdDinners.Dinners.Add(dinner)).DoInstead((Dinner d) => dinners.Add(d));
            //            Then we arrange that SaveChanges() should do nothing for the nerdDinners instance.
            Mock.Arrange(() => nerdDinners.SaveChanges()).DoNothing();

            // ACT
            nerdDinners.Dinners.Add(dinner);
            nerdDinners.SaveChanges();

            // ASSERT
            Assert.AreEqual(1, dinners.Count);
            Assert.AreEqual(1, dinners[0].DinnerID);
        }

        [TestMethod]
        public void GetDinnerInfo_OnExecute_ShouldReturnExpected()
        {
            // We compare culture-dependent strings. Fix the culture for this testing scenario
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            // ARRANGE
            var dbMock = Mock.Create<NerdDinners>();

            // Arranging: When Dinners is called from the database, it should return our FakeDinners collection 
            //  no matter the instance.
            Mock.Arrange(() => dbMock.Dinners).IgnoreInstance().ReturnsCollection(FakeDinners());

            // ACT
            // Creating a new instance of the system under test (the SUT class).
            var realSUT = new SUT();
            // We act on the method under test.
            string realInfo = realSUT.GetAndFormatDinnerInfo(2);

            // ASSERT - Asserting against the expectations
            Assert.AreEqual(
                "Title: FakeDinner 2 \n" +
                "DinnerID: 2 \n" +
                "EventDate: 12/12/1222 00:00:00 \n" +
                "Address: Fake Address 2 \n" +
                "HostedBy: Fake Host 2",
                realInfo);
        }
    }

    #region SUT
    // This will be our database for the examples.
    //------------------------------------------//
    public class NerdDinners : DbContext
    {
        public DbSet<Dinner> Dinners { get; set; }
    }
    
    public class Dinner
    {
        public int DinnerID { get; set; }
        
        public string Title { get; set; }
        
        public DateTime EventDate { get; set; }
        
        public string Address { get; set; }
        
        public string HostedBy { get; set; }
    }
    //------------------------------------------//
    
    public class SUT
    {
        public string GetAndFormatDinnerInfo(int dinnerID)
        {
            StringBuilder info = new StringBuilder();
            
            using(var dbContext = new NerdDinners())
            {
                Dinner checkedDinner = (from d in dbContext.Dinners
                                        where d.DinnerID == dinnerID
                                        select d).FirstOrDefault();

                info.AppendFormat("Title: {0} \n", checkedDinner.Title);
                info.AppendFormat("DinnerID: {0} \n", checkedDinner.DinnerID);
                info.AppendFormat("EventDate: {0} \n", checkedDinner.EventDate.Date);
                info.AppendFormat("Address: {0} \n", checkedDinner.Address);
                info.AppendFormat("HostedBy: {0}", checkedDinner.HostedBy);
            }
            return info.ToString();
        }
    }
    #endregion
}