using MicroORMSharp.SqlGenerator;
using MicroORMSharp.Tests.MySql.Models;
using System;
using System.Linq.Expressions;

namespace MicroORMSharp.Tests.MySql
{
    [TestClass]
    public sealed class DbQueryGuards
    {
        [TestMethod]
        public void DbQuery_Where_Throws_When_FilterIsNull()
        {
            var query = new DbQuery<Customers>();
            Expression<Func<Customers, bool>> filter = null!;

            Assert.ThrowsException<ArgumentNullException>(() => query.Where(filter));
        }

        [TestMethod]
        public void DbQuery_Take_Throws_When_CountIsNegative()
        {
            var query = new DbQuery<Customers>();

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => query.Take(-1));
        }

        [TestMethod]
        public void DbQuery_OrderBy_Throws_When_ExpressionIsNull()
        {
            var query = new DbQuery<Customers>();
            Expression<Func<Customers, object>> orderBy = null!;

            Assert.ThrowsException<ArgumentNullException>(() => query.OrderBy(orderBy));
        }

        [TestMethod]
        public void DbQuery_OrderByDescending_Throws_When_ExpressionIsNull()
        {
            var query = new DbQuery<Customers>();
            Expression<Func<Customers, object>> orderBy = null!;

            Assert.ThrowsException<ArgumentNullException>(() => query.OrderByDescending(orderBy));
        }

        [TestMethod]
        public void DbQuery_SelectTo_SelectorIsNull()
        {
            var query = new DbQuery<Customers>();
            Expression<Func<Customers, CustomerName>> selector = null!;

            Assert.ThrowsException<ArgumentNullException>(() => query.SelectTo(selector));
        }

        [TestMethod]
        public void DbQuery_SelectTo_SelectAlreadyUsed()
        {
            var query = new DbQuery<Customers>()
                .Select(x => x.Forename);

            Assert.ThrowsException<InvalidOperationException>(() => query.SelectTo(x => new CustomerName { Name = x.Forename }));
        }

        [TestMethod]
        public void DbQuery_Select_SelectToAlreadyUsed()
        {
            var query = new DbQuery<Customers>();
            query.SelectTo(x => new CustomerName { Name = x.Forename });

            Assert.ThrowsException<InvalidOperationException>(() => query.Select(x => x.Surname));
        }

        [TestMethod]
        public void DbQuery_SetPagination_PageNumberIsLessThanOne()
        {
            var query = new DbQuery<Customers>();

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => query.SetPagination(0, 10));
        }

        [TestMethod]
        public void DbQuery_SetPagination_PageSizeIsLessThanOne()
        {
            var query = new DbQuery<Customers>();

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => query.SetPagination(1, 0));
        }

    }
}
