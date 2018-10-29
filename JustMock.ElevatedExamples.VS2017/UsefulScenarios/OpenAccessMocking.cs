using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.OpenAccess;
using System.Linq;
using Telerik.OpenAccess.Metadata;
using Telerik.OpenAccess.Metadata.Fluent;

namespace JustMock.ElevatedExamples.UsefulScenarios.OpenAccessMocking
{
    /// <summary>
    /// Telerik OpenAccess is an Enterprise-grade .Net ORM that does the data access plumbing in desktop and web applications. 
    ///  Supporting forward (model-first) and reverse (schema-first) mapping, the tool offers transparent persistence for your 
    ///  data access layer and business objects. Telerik OpenAccess provides tight Visual Studio integration and allows you to 
    ///  seamlessly create database independent code.
    /// Telerik JustMock can be used in conjunction with Telerik OpenAccess to fake the data access layer to make it easier 
    ///  for you to test your target logic without worrying about its dependencies.
    /// See http://www.telerik.com/help/justmock/advanced-usage-openaccess-mocking.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class OpenAccessMocking_Tests
    {
        // HELPER FUNCTIONS.
        //------------------------------------------//

        // Constructing fake DB as required. It will be used in our tests later on.
        public IList<Dinner> FakeDinners()
        {
            return new List<Dinner>()
            {
                new Dinner()
                {
                    DinnerID = 1,
                    Title = "FakeDinner 1",
                },
                new Dinner()
                {
                    DinnerID = 2,
                    Title = "FakeDinner 2",
                },
                new Dinner()
                {
                    DinnerID = 3,
                    Title = "FakeDinner 3",
                }
            };
        }

        // GetDinnerByTitle method retrieves a specific element of the collection that matches the passed name argument.
        public Dinner GetDinnerByTitle(IObjectScope scope, string name)
        {
            return (from cat in scope.Extent<Dinner>()
                    where cat.Title == name
                    select cat).SingleOrDefault();
        }
        //------------------------------------------//

        [TestMethod]
        public void ShouldReturnFakeCollectionWhenExpected()
        {
            // ARRANGE
            NerdDinners nerdDinners = new NerdDinners();

            // Arranging: When nerdDinners.Dinners_GET is called, it should return fake collection (FakeDinners).
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

            // Arranging: When nerdDinners.Dinners_GET is called, it should return fake collection (FakeDinners) no matter the instance.
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
            var nerdDinners = Mock.Create<NerdDinners>();

            // Arranging: When nerdDinners.Dinners.Add() is called with any object as an argument, it should add that argument 
            //             to the dinners collection instead.
            Mock.Arrange(() => nerdDinners.Add(Arg.IsAny<object>())).DoInstead((Dinner d) => dinners.Add(d));
            //            Then we arrange that SaveChanges() should do nothing for the nerdDinners instance.
            Mock.Arrange(() => nerdDinners.SaveChanges()).DoNothing();

            // ACT
            nerdDinners.Add(dinner);
            nerdDinners.SaveChanges();

            // ASSERT
            Assert.AreEqual(1, dinners.Count);
            Assert.AreEqual(1, dinners[0].DinnerID);
        }
        
        [TestMethod]
        public void ShouldMockLINQQuery()
        {
            // ARRANGE
            var scope = Mock.Create<IObjectScope>();

            // Arranging: When scope.Extent<Dinner>() is called, it should return FakeDinners.
            Mock.Arrange(() => scope.Extent<Dinner>()).ReturnsCollection(FakeDinners());

            // ACT
            var actualDinner = GetDinnerByTitle(scope, "FakeDinner 3");

            // ASSERT
            Assert.AreEqual(3, actualDinner.DinnerID);
        }

        [TestMethod]
        public void ShouldMockTransaction()
        {
            Dinner newDinner = new Dinner();
            newDinner.DinnerID = 3;
            newDinner.Title = "TestDinner";

            IList<Dinner> dinners = new List<Dinner>();
            bool isCalled = false;

            // ARRANGE
            // Creating mock instances of the IObjectScope and ITransaction interfaces.
            var scope = Mock.Create<IObjectScope>();
            var transaction = Mock.Create<ITransaction>();

            // Arranging: When transaction.Begin() is called, it should assign true to IsCalled instead.
            Mock.Arrange(() => transaction.Begin()).DoInstead(() => { isCalled = true; });
            // Arranging: When transaction.Commit() is called, it should add newDinner to the dinners list instead.
            Mock.Arrange(() => transaction.Commit()).DoInstead(() => dinners.Add(newDinner));

            // Arranging: When scope.Transaction_GET is called, it should return the mocked instance of ITransaction (transaction).
            Mock.Arrange(() => scope.Transaction).Returns(transaction);
            // Arranging: When scope.Extent<Dinner>() is called, it should return the dinners collection.
            Mock.Arrange(() => scope.Extent<Dinner>()).ReturnsCollection(dinners);

            // ACT
            scope.Transaction.Begin();
            scope.Transaction.Commit();

            IQueryable<Dinner> productQuery = scope.Extent<Dinner>().Where(p => p.DinnerID == 3);

            // ASSERT
            Assert.IsTrue(isCalled);
            Assert.AreEqual(1, productQuery.Count());
        }

        [TestMethod]
        public void GetDinnerInfo_OnExecute_ShouldReturnExpected()
        {
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
                "DinnerID: 2 \n",
                realInfo);
        }
    }

    #region SUT
    // This will be our database for the examples.
    //------------------------------------------//
    public class OpenAccessTestMetadataSource : FluentMetadataSource
    {
        protected override IList<MappingConfiguration> PrepareMapping()
        {
            // Getting Started with the Fluent Mapping API
            // http://www.telerik.com/help/openaccess-orm/fluent-mapping-overview.html

            List<MappingConfiguration> configurations = new List<MappingConfiguration>();

            MappingConfiguration<Dinner> dinnerConfiguration = new MappingConfiguration<Dinner>();
            dinnerConfiguration.MapType(x => new
            {
                DinnerID = x.DinnerID,
                Title = x.Title,
            }).ToTable("Dinners");
            dinnerConfiguration.HasProperty(x => x.DinnerID).IsIdentity(KeyGenerator.Autoinc);

            configurations.Add(dinnerConfiguration);

            return configurations;
        }
    }

    public partial class NerdDinners : OpenAccessContext
    {
        static MetadataContainer metadataContainer = new OpenAccessTestMetadataSource().GetModel();
        static BackendConfiguration backendConfiguration = new BackendConfiguration()
        {
            Backend = "mssql"
        };

        private const string DbConnection = @"data source=.\sqlexpress;initial catalog=FluentMappingDatabase;integrated security=True";

        public NerdDinners()
            : base(DbConnection, backendConfiguration, metadataContainer)
        {

        }

        public IQueryable<Dinner> Dinners
        {
            get
            {
                return this.GetAll<Dinner>();
            }
        }
    }

    public class Dinner
    {
        public int DinnerID { get; set; }

        public string Title { get; set; }
    }
    //------------------------------------------//

    public class SUT
    {
        public string GetAndFormatDinnerInfo(int dinnerID)
        {
            StringBuilder info = new StringBuilder();

            using (var dbContext = new NerdDinners())
            {
                Dinner checkedDinner = (from d in dbContext.Dinners
                                        where d.DinnerID == dinnerID
                                        select d).FirstOrDefault();

                info.AppendFormat("Title: {0} \n", checkedDinner.Title);
                info.AppendFormat("DinnerID: {0} \n", checkedDinner.DinnerID);
            }
            return info.ToString();
        }
    }
    #endregion
}
