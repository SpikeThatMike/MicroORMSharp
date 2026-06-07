using MicroORMSharp.SqlGenerator;
using MicroORMSharp.Tests.Models;

namespace MicroORMSharp.Tests.MySql
{
    public partial class DatabaseTests
    {
        [TestMethod]
        [DoNotParallelize]
        public async Task JoinQueries()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customer = new TestJoinCustomer
            {
                Name = "Join Test Customer",
                Email = "join-test@example.com",
                CreatedDate = new DateTime(2026, 3, 24, 10, 0, 0, DateTimeKind.Utc)
            };

            var orderTemplate = new TestJoinOrder();

            await TestDatabaseFixture.EnsureTableCreatedAsync(customer);
            await TestDatabaseFixture.EnsureTableCreatedAsync(orderTemplate);

            try
            {
                customer = await customer.InsertAsync();

                await new TestJoinOrder
                {
                    CustomerId = customer.Id,
                    OrderDate = customer.CreatedDate,
                    TotalAmount = 10.50m,
                    Status = "Open"
                }.InsertAsync();

                await new TestJoinOrder
                {
                    CustomerId = customer.Id,
                    OrderDate = customer.CreatedDate.AddHours(1),
                    TotalAmount = 25.00m,
                    Status = "Paid"
                }.InsertAsync();

                var query = Database.Query<TestJoinCustomer>()
                    .Where(x => x.Id == customer.Id);

                var executeResult = (await query.ExecuteAsync()).FirstOrDefault();
                Assert.AreEqual(2, executeResult.Orders.Count, "ExecuteAsync returned incorrect order count");

                var executeSingleResult = await query.ExecuteSingleAsync();
                Assert.IsNotNull(executeSingleResult, "ExecuteSingleAsync returned null for a joined query");
                Assert.AreEqual(2, executeSingleResult.Orders.Count, "ExecuteSingleAsync returned incorrect order count");

                Assert.IsTrue(await query.AnyAsync(), "AnyAsync returned no customer");
                Assert.AreEqual(1, await query.CountAsync(), "CountAsync returned no customer");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(orderTemplate);
                await TestDatabaseFixture.AssertTableDroppedAsync(customer);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task NestedJoinQueries()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customer = new NestedJoinCustomer
            {
                Name = "Nested Join Customer"
            };

            var orderTemplate = new NestedJoinOrder();
            var statusTemplate = new NestedJoinOrderStatus();

            await TestDatabaseFixture.EnsureTableCreatedAsync(customer);
            await TestDatabaseFixture.EnsureTableCreatedAsync(statusTemplate);
            await TestDatabaseFixture.EnsureTableCreatedAsync(orderTemplate);

            try
            {
                customer = await customer.InsertAsync();

                var openStatus = await new NestedJoinOrderStatus
                {
                    Name = "Open"
                }.InsertAsync();

                var paidStatus = await new NestedJoinOrderStatus
                {
                    Name = "Paid"
                }.InsertAsync();

                await new NestedJoinOrder
                {
                    CustomerId = customer.Id,
                    StatusId = openStatus.Id,
                    OrderDate = new DateTime(2026, 3, 24, 10, 0, 0),
                    TotalAmount = 10.50m
                }.InsertAsync();

                await new NestedJoinOrder
                {
                    CustomerId = customer.Id,
                    StatusId = paidStatus.Id,
                    OrderDate = new DateTime(2026, 3, 24, 11, 0, 0),
                    TotalAmount = 25.00m
                }.InsertAsync();

                var query = Database.Query<NestedJoinCustomer>()
                    .Where(x => x.Id == customer.Id);

                var executeResult = (await query.ExecuteAsync()).FirstOrDefault();
                Assert.IsNotNull(executeResult, "ExecuteAsync return no customer");
                Assert.AreEqual(2, executeResult!.Orders.Count, "ExecuteAsync returned no orders");
                Assert.AreEqual(2, executeResult.Orders.Count(x => x.OrderStatus != null), "ExecuteAsync returned no order statuses");
                CollectionAssert.AreEquivalent(new[] { "Open", "Paid" }, executeResult.Orders.Select(x => x.OrderStatus.Name).ToArray(), "ExecuteAsync incorrect statuses returned");

                var executeSingleResult = await query.ExecuteSingleAsync();
                Assert.IsNotNull(executeSingleResult, "ExecuteSingleAsync returned no customer");
                Assert.AreEqual(2, executeSingleResult.Orders.Count, "ExecuteSingleAsync returned incorrect order count");
                Assert.IsTrue(executeSingleResult.Orders.All(x => x.OrderStatus != null), "ExecuteSingleAsync should map nested joined one-to-one rows");

                Assert.IsTrue(await query.AnyAsync(), "AnyAsync returned no customer");
                Assert.AreEqual(1, await query.CountAsync(), "CountAsync returned no customer");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(orderTemplate);
                await TestDatabaseFixture.AssertTableDroppedAsync(statusTemplate);
                await TestDatabaseFixture.AssertTableDroppedAsync(customer);
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task LeftJoinQueries()
        {
            TestDatabaseFixture.UseMySqlConnection();

            var customer = new LeftJoinCustomer
            {
                Name = "Left Join Customer"
            };

            var orderTemplate = new LeftJoinOrder();

            await TestDatabaseFixture.EnsureTableCreatedAsync(customer);
            await TestDatabaseFixture.EnsureTableCreatedAsync(orderTemplate);

            try
            {
                customer = await customer.InsertAsync();

                var query = Database.Query<LeftJoinCustomer>()
                    .Where(x => x.Id == customer.Id);

                var result = await query.ExecuteSingleAsync();

                Assert.IsNotNull(result, "ExecuteSingleAsync no customer returned");
                Assert.AreEqual(customer.Id, result.Id, "ExecuteSingleAsync customer id doesnt match");
                Assert.IsNotNull(result.Orders, "ExecuteSingleAsync order is null");
                Assert.AreEqual(0, result.Orders.Count, "ExecuteSingleAsync returned orders when none were expected");
                Assert.IsTrue(await query.AnyAsync(), "AnyAsync customers not found");
                Assert.AreEqual(1, await query.CountAsync(), "CountAsync returned incorrect customer count");
            }
            finally
            {
                await TestDatabaseFixture.AssertTableDroppedAsync(orderTemplate);
                await TestDatabaseFixture.AssertTableDroppedAsync(customer);
            }
        }
    }
}
