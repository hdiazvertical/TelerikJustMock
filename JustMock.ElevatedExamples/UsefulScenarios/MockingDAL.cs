using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using System.Text;

namespace JustMock.ElevatedExamples.UsefulScenarios.MockingDAL
{
    /// <summary>
    /// <FEATURE HELP>
    /// </summary>
    [TestClass]
    public class MockingDAL_Tests
    {
        [TestMethod]
        public void GetAndFormatDinnerInfo_OnExecuteWithMockedDB_ShouldReturnExpected()
        {
            // We compare culture-dependent strings. Fix the culture for this testing scenario
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            // This will be our fake Dinner. 
            //  We will use it as a return value to the mocked Retrieve method.
            var fakeDinner = new Dinner()
            {
                DinnerID = 1,
                Title = "FakeDinner 1",
                EventDate = new DateTime(1222, 12, 12),
                Address = "Fake Address 1",
                HostedBy = "Fake Host 1"
            };

            // ARRANGE
            // Creating a mocked instance of the generic IMaintanable<Dinner> interface.
            var iMaintanable = Mock.Create<IMaintanable<Dinner>>();

            // Arranging: When the Retrieve method is called with any string as an argument, 
            //  it should return fakeDinner.
            Mock.Arrange(() => iMaintanable.Retrieve(1)).Returns(fakeDinner);

            // ACT
            // Creating an actual instance of our class under test.
            var actualSUT = new SUT(iMaintanable);
            var actualDinner = actualSUT.GetAndFormatDinnerInfo(1);

            // ASSERT - Asserting against the expected values.
            Assert.AreEqual(
                "Title: FakeDinner 1 \n" +
                "DinnerID: 1 \n" +
                "EventDate: 12/12/1222 00:00:00 \n" +
                "Address: Fake Address 1 \n" +
                "HostedBy: Fake Host 1",
                actualDinner);
        }
    }

    #region SUT
    // This will be our database for the examples.
    //------------------------------------------//
    public class Dinner
    {
        public int DinnerID { get; set; }

        public string Title { get; set; }

        public DateTime EventDate { get; set; }

        public string Address { get; set; }

        public string HostedBy { get; set; }
    }
    //------------------------------------------//

    public interface IMaintanable<T>
    {
        void Create(T obj);
        T Retrieve(int key);
        void Update(T obj);
        void Delete(int key);
    }

    public class DinnerManager : IMaintanable<Dinner>
    {
        public void Create(Dinner obj)
        {
            // Inserts record in the DB.
            throw new NotImplementedException();
        }

        public Dinner Retrieve(int dinnerId)
        {
            // Retrieves record from the DB.
            throw new NotImplementedException();
        }

        public void Update(Dinner obj)
        {
            // Updates the record in the DB.
            throw new NotImplementedException();
        }

        public void Delete(int dinnerId)
        {
            // Deletes record from the DB.
            throw new NotImplementedException();
        }
    }

    public class SUT
    {
        private IMaintanable<Dinner> dinnerManager;

        public SUT(IMaintanable<Dinner> manager)
        {
            this.dinnerManager = manager;
        }

        public string GetAndFormatDinnerInfo(int dinnerID)
        {
            StringBuilder info = new StringBuilder();

            Dinner checkedDinner = dinnerManager.Retrieve(dinnerID);

            info.AppendFormat("Title: {0} \n", checkedDinner.Title);
            info.AppendFormat("DinnerID: {0} \n", checkedDinner.DinnerID);
            info.AppendFormat("EventDate: {0} \n", checkedDinner.EventDate.Date);
            info.AppendFormat("Address: {0} \n", checkedDinner.Address);
            info.AppendFormat("HostedBy: {0}", checkedDinner.HostedBy);

            return info.ToString();
        }
    }
    #endregion
}
