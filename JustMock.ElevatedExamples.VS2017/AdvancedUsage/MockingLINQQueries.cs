using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using System.Data.Linq;

namespace JustMock.ElevatedExamples.AdvancedUsage.MockingLINQQueries
{
    /// <summary>
    /// With JustMock you can mock LINQ queries with custom select.
    /// See http://www.telerik.com/help/justmock/advanced-usage-mocking-linq-queries.html for full documentation of the feature.
    /// </summary>
    [TestClass]
    public class MockingLINQQueries_Tests
    {
        // This method will generate our expected products collection. 
        // We will use it in some of the following test methods.
        private List<Product> ReturnExpextedCollectionOfProducts()
        {
            var productList = new List<Product>()
            {
                new Product()
                {
                    ProductID = 1,
                    CategoryID = 1,
                    ProductName = "test product",
                    UnitsInStock = 3,
                    QuantityPerUnit = "1",
                    Discontinued = false,
                    voa_class = 1
                },
                new Product()
                {
                    ProductID = 2,
                    CategoryID = 1,
                    ProductName = "Foo stuff",
                    UnitsInStock = 50,
                    QuantityPerUnit = "1",
                    Discontinued = true,
                    voa_class = 1
                },
                new Product()
                {
                    ProductID = 3,
                    CategoryID = 2,
                    ProductName = "More Stuff",
                    UnitsInStock = 0,
                    QuantityPerUnit = "1",
                    Discontinued = true,
                    voa_class = 1
                }
            };
            return productList;
        }

        // This method will generate our expected categories collection. 
        // We will use it in some of the following test methods.
        private List<Category> ReturnExpectedCollectionOfCategories()
        {
            var categoriesList = new List<Category>()
            {
                new Category() { CategoryID = 1, CategoryName = "First" },
                new Category() { CategoryID = 2, CategoryName = "Second" }
            };
            return categoriesList;
        }

        [TestMethod]
        public void ShouldAssertWithCustomSelect()
        {
            var simpleDataInstance = new SimpleData();

            // ARRANGE - When simpleDataInstance.Products_GET is called, it should return expected collection.
            Mock.Arrange(() => simpleDataInstance.Products).ReturnsCollection(ReturnExpextedCollectionOfProducts());

            // ACT - Applying a LINQ query for simpleDataMock.Products.
            var actual = (from p in simpleDataInstance.Products
                          where p.UnitsInStock == 50
                          select p.ProductID).SingleOrDefault();

            // ASSERT
            Assert.AreEqual(2, actual);
        }

        [TestMethod]
        public void ShouldAssertProjectionWhenCombinedWithWhere()
        {
            var simpleDataInstance = new SimpleData();

            // ARRANGE - When simpleDataInstance.Products_GET is called, it should return expected collection.
            Mock.Arrange(() => simpleDataInstance.Products).ReturnsCollection(ReturnExpextedCollectionOfProducts());

            // ACT - Applying a LINQ query for simpleDataMock.Products.
            var actual = (from p in simpleDataInstance.Products
                          where p.UnitsInStock == 50
                          select new { p.ProductID, p.ProductName }).SingleOrDefault();

            // ASSERT
            Assert.AreEqual(2, actual.ProductID);
        }

        [TestMethod]
        public void ShouldAssertWithJoinClause()
        {
            var simpleDataInstance = new SimpleData();

            // ARRANGE
            // Arranging: When simpleDataInstance.Products_GET is called, it should return expected products collection.
            Mock.Arrange(() => simpleDataInstance.Products).ReturnsCollection(ReturnExpextedCollectionOfProducts());
            // Arranging: When simpleDataInstance.Categories_GET is called, it should return expected categories collection.
            Mock.Arrange(() => simpleDataInstance.Categories).ReturnsCollection(ReturnExpectedCollectionOfCategories());

            // ACT - Applying a LINQ query for simpleDataMock.Products.
            var actual = from product in simpleDataInstance.Products
                         join category in simpleDataInstance.Categories on product.CategoryID equals category.CategoryID
                         select category.CategoryName;

            // ASSERT
            Assert.AreEqual(3, actual.Count());
        }

        private IQueryable<Product> GetFakeProducts()
        {
            List<Product> productList = ReturnExpextedCollectionOfProducts();

            return productList.AsQueryable();
        }

        [TestMethod]
        public void ShouldAssertEnumerableSource()
        {
            IList<Product> products;

            var dataContextInstance = new MyDataContext();

            // ARRANGE - When dataContextInstance.Products_GET is called, it should return expected fake products collection.
            Mock.Arrange(() => dataContextInstance.Products).ReturnsCollection(GetFakeProducts());

            // ACT
            products = dataContextInstance.Products.ToList<Product>();

            // ASSERT
            Assert.AreEqual(3, products.Count());
        }

        [TestMethod]
        public void ShouldAssertWithExpressionAsAnArgument()
        {
            int expectedId = 10;

            // ARRANGE
            var simpleDataInstance = new SimpleData();

            // Arranging: When simpleDataInstance.Products_GET is called, it should return expected products collection.
            Mock.Arrange(() => simpleDataInstance.Products).ReturnsCollection(ReturnExpextedCollectionOfProducts());
            // Directly arranging a chain call with LINQ query in it. If called with the exact parameters, it should return expectedId.
            var datum = simpleDataInstance.Products.Where(x => x.ProductID == 2).FirstOrDefault();
            Mock.Arrange(() => datum.GetId()).Returns(expectedId);

            // ACT
            Product actual = simpleDataInstance.GetProduct(2);

            // ASSERT
            Assert.AreEqual(expectedId, actual.GetId());
        }
    }

    #region SUT

    public class SimpleData
    {
        public ExtendedQuery<Product> Products
        {
            get
            {
                return null;
            }
        }

        public ExtendedQuery<Category> Categories
        {
            get
            {
                return null;
            }
        }

        public Product GetProduct(int id)
        {
            return this.Products.Where(x => x.ProductID == id).FirstOrDefault();
        }
    }

    public abstract class ExtendedQuery<T> : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }

    public class Product
    {
        public int ProductID { get; set; }

        public int CategoryID { get; set; }

        public string ProductName { get; set; }

        public int UnitsInStock { get; set; }

        public string QuantityPerUnit { get; set; }

        public bool Discontinued { get; set; }

        public int voa_class { get; set; }

        public int GetId()
        {
            return this.ProductID;
        }
    }

    public class Category
    {
        public int CategoryID { get; set; }

        public string CategoryName { get; set; }
    }

    public class MyDataContext : DataContext
    {
        public MyDataContext()
            : base(string.Empty)
        {
        }

        public Table<Product> Products
        {
            get
            {
                return this.GetTable<Product>();
            }
        }
    }

    #endregion
}