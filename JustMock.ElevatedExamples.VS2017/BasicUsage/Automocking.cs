using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Telerik.JustMock.AutoMock;

namespace JustMock.ElevatedExamples.BasicUsage.Automocking
{
    /// <summary>
    /// Automocking allows the developer to create an instance of a class (the system under test) without having 
    /// to explicitly create each individual dependency as a unique mock. The mocked dependencies are still available 
    /// to the developer if methods or properties need to be arranged as part of the test. 
    /// See http://www.telerik.com/help/justmock/basic-usage-automocking.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class Automocking_Tests
    {
        [TestMethod]
        public void ShouldAutoMockConcretedDependecies()
        {
            // ARRANGE
            // Creating a MockingContainer of PersonRepository. 
            // To instantiate the system under test (the container) you should use the Instance property 
            // For example: container.Instance. 
            var container = new MockingContainer<PersonRepository>();

            // Arranging: That the Walk() method from the Person instance 
            //              must be called from the container during the test method. 
            container.Arrange<Person>(anml => anml.Walk()).MustBeCalled();

            // ACT - Calling Walk() from the mocked instance of PersonRepository
            container.Instance.Walk();

            // ASSERT - Asserting all expectations against this method.
            container.Assert<Person>(anml => anml.Walk());
        }

        [TestMethod]
        public void ShouldTransferFundsBetweenTwoAccounts()
        {
            // ARRANGE
            // Creating a MockingContainer of AccountService. 
            // To instantiate the system under test (the container) you should use the Instance property 
            // For example: container.Instance. 
            var container = new MockingContainer<AccountService>();

            decimal expectedBalance = 100;

            container.Bind<Account>().ToMock().InjectedIntoParameter("fromAccount")
                .AndArrange(x =>
                {
                    // Arranging: That the Balance property from the Account instance 
                    //              must be called from the container during the test method 
                    //              and it should return expectedBalance.
                    Mock.Arrange(() => x.Balance).Returns(expectedBalance).MustBeCalled();

                    // Arranging: That the Withdraw() method from the Account instance 
                    //              must be called from the container during the test method. 
                    Mock.Arrange(() => x.Withdraw(expectedBalance)).MustBeCalled();
                });

            container.Bind<Account>().ToMock().InjectedIntoParameter("toAccount")
                .AndArrange(x =>
                {
                    // Arranging: That the Deposit() method from the Account instance 
                    //              must be called from the container during the test method. 
                    Mock.Arrange(() => x.Deposit(expectedBalance)).MustBeCalled();
                });

            // ACT - Calling TransferFunds() from the mocked instance of AccountService
            container.Instance.TransferFunds(expectedBalance);

            // ASSERT - Asserting all expectations for the container.
            container.Assert();
        }
    }

    #region SUT
    public interface IAnimal
    {
        void Walk();
    }

    public class Person : IAnimal
    {
        public void Walk()
        {

        }
    }

    public class PersonRepository
    {
        public PersonRepository(Person person)
        {
            this.person = person;
        }

        public void Walk()
        {
            (this.person as IAnimal).Walk();
        }

        private readonly Person person;
    }

    public class Account
    {
        public decimal Balance { get; set; }

        public void Deposit(decimal amount)
        {
            throw new NotImplementedException();
        }

        public void Withdraw(decimal amount)
        {
            throw new NotImplementedException();
        }
    }

    public class AccountService
    {
        public AccountService(Account fromAccount, Account toAccount)
        {
            this.fromAccount = fromAccount;
            this.toAccount = toAccount;
        }

        public void TransferFunds(decimal amount)
        {
            if (fromAccount.Balance <= amount)
            {
                fromAccount.Withdraw(amount);
                toAccount.Deposit(amount);
            }
        }

        private readonly Account fromAccount;
        private readonly Account toAccount;
    }
    #endregion
}
