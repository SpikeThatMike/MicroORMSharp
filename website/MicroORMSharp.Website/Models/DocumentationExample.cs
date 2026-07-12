namespace MicroORMSharp.Website.Models;

public sealed record DocumentationExample(
    string Id,
    string Title,
    string Summary,
    string Category,
    string CodeTitle,
    string Code,
    string Language = "csharp",
    string[]? Highlights = null)
{
    public string[] Highlights { get; init; } = Highlights ?? [];
}

public static class DocumentationExamples
{
    public const string AllCategories = "All";

    public static IReadOnlyList<DocumentationExample> All { get; } =
    [
        new(
            "registration",
            "Register a connection",
            "Add the first configured connection and optionally enable table helper APIs for schema setup tasks.",
            "Setup",
            "Program startup",
            """
            using MicroORMSharp;
            using MicroORMSharp.SqlGenerator;

            Database.AddConnectionString(
                DatabaseType.MySql,
                reference: "MainMySql",
                sqlConnection: "{connection}",
                allowTableExtensions: true
            );

            Database.Initialise();
            """,
            Highlights:
            [
                "The first connection becomes the default connection.",
                "Set allowTableExtensions only for trusted setup paths.",
                "By default when adding a new connection, a new connection will be opened & closed to check if it is working and will throw an exception if not. Set connectionTest if you do not want this feature"
            ]
        ),
        new(
            "multiple-registration",
            "Register multiple connections",
            "Add multiple connections and set the default.",
            "Setup",
            "Program startup",
            """
            using MicroORMSharp;
            using MicroORMSharp.SqlGenerator;

            Database.AddConnectionString(
                DatabaseType.MySql,
                reference: "MainMySql",
                sqlConnection: "{connection}",
                allowTableExtensions: true
            );

            Database.AddConnectionString(
                DatabaseType.MySql,
                reference: "ReportingMySql",
                sqlConnection: "{connection}",
                allowTableExtensions: false
            );

            Database.SetConnectionString("ReportingMySql"); //change the database
            Database.Initialise();
            """
        ),
        new(
            "modeling",
            "Map a model",
            "Use attributes to describe table names, identity columns, mapped names, defaults, precision, and ignored properties.",
            "Modeling",
            "Customer.cs",
            """
            using MicroORMSharp.SqlGenerator.Attributes;
            using MicroORMSharp.SqlGenerator.Interfaces;

            [DbTable("Customers")]
            public class Customer : IMicroORMSharp
            {
                [DbIdentity]
                public long Id { get; set; }

                [DbMaxLength(20)]
                [DbDefault("guest")]
                public string Forename { get; set; }

                public string Surname { get; set; }

                [DbColumn("Postalcode")]
                public string Postcode { get; set; }

                [DbDefault(true)]
                public bool Active { get; set; }

                [DbIgnore]
                public string FullName => $"{Forename} {Surname}"; 
            }
            """,
            Highlights:
            [
                "Every mapped entity must implement IMicroORMSharp.",
                "DbMaxLength is also validated before insert and update and will throw an error before ever calling the database.",
                "If you add a property to the class which doesnt exist in the database, you must ignore it otherwise when querying the database it will throw an exception"
            ]),
        new(
            "querying",
            "Query data",
            "Create LINQ-style where clauses, ordering, and pagination while MicroORMSharp generates SQL.",
            "Querying",
            "Common query shapes",
            """
            var customers = await Database.Query<Customer>()
                .Where(x => x.Active)
                .OrderBy(x => x.Id)
                .SetPagination(pageNumber: 2, pageSize: 10)
                .ExecuteAsync();

            var customer = await Database.Query<Customer>()
                .Where(x => x.Id == 1)
                .ExecuteSingleAsync();

            var activeCount = await Database.Query<Customer>()
                .Where(x => x.Active)
                .CountAsync();
            """,
            Highlights:
            [
                "Sync methods are available for all Async methods shown.",
                "SQL Server pagination requires an order and will default to the DbIdentity if not provided."
            ]),
        new(
            "projection",
            "Project only the columns you need",
            "Use Select for partial database table reads or SelectTo when the result should be mapped to another type.",
            "Querying",
            "Select and SelectTo",
            """
            var customers = await Database.Query<Customer>()
                .Select(x => x.Id, x => x.Forename, x => x.Surname)
                .ExecuteAsync();

            var namesOnly = await Database.Query<Customer>()
                .Where(x => x.Active)
                .SelectTo(x => new CustomerName
                {
                    Name = x.Forename + " " + x.Surname
                })
                .ExecuteAsync();

            var namesOnlyWithSpecificColumns = await Database.Query<Customer>()
                .Select(x => x.Forename, x => x.Surname)
                .Where(x => x.Active)
                .SelectTo(x => new CustomerName
                {
                    Name = x.Forename + " " + x.Surname
                })
                .ExecuteAsync();
            """,
            Highlights:
            [
                "Select keeps the entity type but only puts the selected columns in the select query.",
                "SelectTo projects to a non database class after the database call and original mapping is complete.",
                "SelectTo should be the last query step before execution."
            ]),
        new(
            "insert-update-delete",
            "Insert, Update, Delete rows",
            "Insert, update, and delete mapped entities using extension methods on the entity itself.",
            "CRUD",
            "CRUD extensions",
            """
            var customer = await new Customer
            {

                Forename = "John",
                Surname = "Doe",
                Postcode = "TE1 1ST",
                Active = true
            }.InsertAsync();

            customer.Forename = "Jane";
            customer = await customer.UpdateAsync(); //Updates all columns except identities

            customer.Forename = "Jane2";
            customer = await customer.UpdateAsync(x => new //updates specific columns only
            {
                x.Forename,
                x.Postcode
            });

            await customer.DeleteAsync();
            """,
            Highlights:
            [
                "Insert returns the entity with generated identity values where supported.",
                "Update selectors limit the columns included in the SQL statement."
            ]),
        new(
            "bulk-insert",
            "Bulk insert",
            "Insert a batch of entities with provider-specific bulk copy support.",
            "CRUD",
            "Batch writes",
            """
            var customers = new List<Customer>
            {
                new() { Forename = "John", Surname = "Doe", Postcode = "AA1", Active = true },
                new() { Forename = "Jane", Surname = "Doe", Postcode = "AA2", Active = true }
            };

            await customers.InsertAsync();
            """,
            Highlights:
            [
                "SQL Server uses SqlBulkCopy. MySql uses MySqlBulkCopy",
                "MySQL requires Allow Load Local Infile=True and local_infile enabled."
            ]),
        new(
            "get-connection",
            "Get a connection",
            "Create the default connection or specific connection to use with standard methods with SQL strings",
            "Connections",
            "Get connection",
            """
            using Dapper;

            using var defaultConnection = Database.GetConnection();
            using var specificConnection = Database.GetConnection("ReportingMySql");

            await defaultConnection.ExecuteAsync("EXECUTE USP_Test @p1", new { p1 = 1 });
            """,
            Highlights:
            [
                "Useful for complex queries for SP calls"
            ]),
        new(
            "contexts",
            "Use database context",
            "Create a database context when several operations should use the same connection or without changing the global default.",
            "Connections",
            "Using Database.CreateContext",
            """
            using var db = Database.CreateContext("ReportingMySql");

            var customers = await db.Query<Customer>()
                .Where(x => x.Active)
                .ExecuteAsync();

            var customer = await db.InsertAsync(new Customer
            {
                Forename = "Jane",
                Surname = "Doe",
                Active = true
            });

            var count = await db.Dapper.QuerySingleAsync<int>(
                "SELECT COUNT(*) FROM Customers;"
            );
            """,
            Highlights:
            [
                "Specify the connection or leave blank to create a context for the default connection",
                "Use this if you only want to open one connection for multiple queries"
            ]),
        new(
            "transactions",
            "Wrap database operations in a transaction",
            "Use WithTransactionAsync for commit-on-success and rollback-on-error behavior.",
            "Transactions",
            "Transactional work",
            """
            using var db = Database.CreateContext("MainMySql");

            var committed = await db.WithTransactionAsync(async trans =>
            {
                var customer = await trans.InsertAsync(new Customer
                {
                    Forename = "John",
                    Surname = "Doe",
                    Active = true
                });

                await trans.Dapper.ExecuteAsync(
                    "UPDATE Customers SET Active = @Active WHERE Id = @Id;",
                    new { Active = false, customer.Id }
                );
            });
            """,
            Highlights:
            [
                "Commit and Rollback are intentionally not exposed publicly.",
                "The transaction Dapper wrapper omits connection and transaction parameters.",
                "Returns true/false for the result of the transaction"
            ]),
        new(
            "joins",
            "Map joins",
            "Declare joins with DBJoin and query the aggregate model normally.",
            "Joins",
            "Customer class with joined orders",
            """
            [DbTable("Customer")]
            public class CustomerWithOrders : IMicroORMSharp
            {
                [DbIdentity]
                public long Id { get; set; }

                public string Name { get; set; }

                [DBJoin(typeof(Order), "Id", "CustomerId", DBJoinType.Left)]
                public List<Order> Orders { get; set; }
            }

            var customers = await Database.Query<CustomerWithOrders>()
                .Where(x => x.Id == customerId)
                .ExecuteAsync();
            """,
            Highlights:
            [
                "Supported join types are Left, Inner, and Right.",
                "Nested joins are supported up to 3 levels deep.",
                "Join on DbIdentity columns specifing the first Id as the table youre currently on and the secondary Id as the joined table"
            ])
    ];

    public static IReadOnlyList<string> Categories { get; } =
        All.Select(example => example.Category)
            .Distinct()
            .OrderBy(category => category)
            .Prepend(AllCategories)
            .ToList();
}
