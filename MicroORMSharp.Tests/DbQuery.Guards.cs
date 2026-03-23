using MicroORMSharp.SqlGenerator;
using System;
using System.Linq.Expressions;

namespace MicroORMSharp.Tests
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
    }
}
