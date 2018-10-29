
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.JustMock;
using Telerik.OpenAccess;

namespace JustMock.ElevatedExamples.UsefulScenarios.OpenAccessDomainModelMocking
{
    [TestClass]
    public class OpenAccessDomainModelMocking_Tests
    {
        // Constructing fake DB as required. It will be used in our tests later on.
        private static IQueryable<OrmAccount> GetTestAccounts()
        {
            return new List<OrmAccount>
            {
                new OrmAccount { AccountId = 1, AccountName = "Darcy"}
            }.AsQueryable();
        }

        [TestMethod]
        public void GetAccountById_ShouldReturnCorrectAccount()
        {
            // ARRANGE
            // Creating mock of the data context.
            var ctx = Mock.Create<IOrmDataContextUnitOfWork>();

            // Arranging: When DataContext.OrmAccounts is called, it should return our fake accounts.
            Mock.Arrange(() => ctx.OrmAccounts).Returns(GetTestAccounts());
            
            // ACT
            var repo = new AccountRepository(ctx);
            var act = repo.GetAccountById(1);

            // ASSERT
            Assert.IsNotNull(act);
            Assert.AreEqual("Darcy", act.AccountName);
        }

        [TestMethod]
        public void GetAccountsForClient_ShouldReturnCorrectAccounts()
        {
            // ARRANGE
            // Creating mock of the data context.
            var ctx = Mock.Create<IOrmDataContextUnitOfWork>();
            
            // Arranging: When the stored procedure DataContext.SelectAccountsForClient is called with 10 as an argument,
            //  it should return our fake accounts.
            Mock.Arrange(() => ctx.SelectAccountsForClient(10))
                .Returns(GetTestAccounts());

            // ACT
            var repo = new AccountRepository(ctx);
            var acts = repo.GetAccountsForClient(10);

            // ARRANGE
            Assert.IsNotNull(acts);
            Assert.AreEqual(1, acts.Count());
        }
    }

    #region SUT
    public class OrmAccount
    {
        public int AccountId { get; set; }

        public string AccountName { get; set; }
    }

    public interface IOrmDataContextUnitOfWork : IUnitOfWork
    {
        IQueryable<OrmAccount> OrmAccounts
        {
            get;
        }
        IEnumerable<OrmAccount> SelectAccountsForClient(int? customerId);
        IEnumerable<OrmAccount> SelectAccountsForClient(int? customerId, out int returnValue);
    }

    public interface IAccountRepository
    {
        IEnumerable<OrmAccount> GetAccountsForClient(int idCrmCustomer);
        OrmAccount GetAccountById(int idAccount);
    }

    public class AccountRepository : IAccountRepository
    {
        private readonly IOrmDataContextUnitOfWork _dataContext;

        public AccountRepository(IOrmDataContextUnitOfWork dataContext)
        {
            this._dataContext = dataContext;
        }

        public IEnumerable<OrmAccount> GetAccountsForClient(int idClient)
        {
            return this._dataContext.SelectAccountsForClient(idClient).ToList();
        }

        public OrmAccount GetAccountById(int idAccount)
        {
            return _dataContext.OrmAccounts.Where(act => act.AccountId == idAccount).SingleOrDefault();
        }
    }
#endregion
}

