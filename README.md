# MicroORMSharp
**MicroORMSharp** is a lightweight micro ORM for .NET built on top of Dapper. It focuses on the common operations
- CRUD operations (inserts, updates, deletes)
- Querying with LINQ-style API
- Querying data with LEFT,INNER,RIGHT joins with nested join support
- Optional table operations (create, drop, truncate, exists)
- Bulk insert support
- The ability use native Dapper methods without handling connections

Designed to be reduce repetitive SQL and object mapping.

| Databases | Supported |
| --- | --- |
| MySQL | ✅ |
| SQL Server | ⚠ |
| Others | ❌ |


### ⚠SQL Server support⚠
> SQL Server integration has not been fully tested and may have unwanted side effects, unexpected behavior, or provider-specific issues. If you use SQL Server, test carefully in a non-production environment before relying on it in live systems.
> The main reason for this I use a locally hosted MySQL for my projects and do not currently have access to a SQL server database

### Supported versions
| Version | Supported .NET versions |
| --- | --- |
| `1.x` | `.NET Core 3.0`, `.NET 5`, `.NET 6`, `.NET 7`, `.NET 8`, `.NET 9`, `.NET 10` |

`MicroORMSharp` currently targets `.NET Standard 2.1`, so `.NET Framework` is not supported by the `1.x` package line.

## Installation
```bash
dotnet add package MicroORMSharp
```

## How MicroORMSharp works
1. Register your connection string.
2. Create a model that implements `IMicroORMSharp`.
3. Query data with `Database.Query<T>()` or a context created with `Database.CreateContext(...)`.
4. Call extension methods like `InsertAsync()`.

### Basic registration

```csharp
using MicroORMSharp;
using MicroORMSharp.SqlGenerator;

Database.AddConnectionString(
    DatabaseType.MySql,
    reference: "MainMySql",
    sqlConnection: "Server=localhost;Database=test;User ID=root;Password=admin;Port=3306;",
    allowTableExtensions: true
);
```

The first connection you add becomes the current/default connection automatically.

### Working with multiple connections
```csharp
//Default as its the first added
Database.AddConnectionString(
    DatabaseType.SqlServer,
    reference: "PrimarySqlServer",
    sqlConnection: "Server=.;Database=AppDb;Trusted_Connection=True;TrustServerCertificate=True;",
    allowTableExtensions: true
);

Database.AddConnectionString(
    DatabaseType.MySql,
    reference: "ReportingMySql",
    sqlConnection: "Server=localhost;Database=ReportingDb;User ID=app;Password=secret;Port=3306;",
    allowTableExtensions: false,
    connectionTest: false //By default when adding an connection, MicroORMSharp will open a connection and close it to ensure the connection works, adding this stops that behaviour
);

//Set the default
Database.SetConnectionString("ReportingMySql");

//Get a specific connection
var namedConnection = Database.GetConnection("PrimarySqlServer");

//Get all connections
var allConnections = Database.GetAllConnections();

//Remove a connection
Database.RemoveConnectionString("ReportingMySql");
```

#### When to use `allowTableExtensions`
Set `allowTableExtensions: true` if you want to use table extension methods:

- `CreateTable()`
- `CreateTableAsync()`
- `DropTable()`
- `DropTableAsync()`
- `TruncateTable()`
- `TruncateTableAsync()`

If this flag is not enabled for the active connection reference, those methods will throw an exception.

## Handling connection strings properly
In most use cases, avoid hardcoding connection strings in source:
- Store in `appsettings.json`, user secrets, environment variables, or your secret store
- Read at startup
- Register them once with `Database.AddConnectionString(...)`

`appsettings.json`
```json
{
  "ConnectionStrings": {
    "MainDb": "Server=.;Database=AppDb;Trusted_Connection=True;TrustServerCertificate=True;",
    "ReportingDb": "Server=localhost;Database=ReportingDb;User ID=app;Password=secret;Port=3306;"
  }
}
```

`Program.cs`

```csharp
using MicroORMSharp;
using MicroORMSharp.SqlGenerator;

var builder = WebApplication.CreateBuilder(args);

Database.AddConnectionString(
    DatabaseType.SqlServer,
    reference: "MainDb",
    sqlConnection: builder.Configuration.GetConnectionString("MainDb")!,
    allowTableExtensions: true
);
```
```csharp
using Microsoft.Extensions.Configuration;
using MicroORMSharp;
using MicroORMSharp.SqlGenerator;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var sqlServerConnection = configuration.GetConnectionString("MainDb")
    ?? throw new InvalidOperationException("Missing connection string: MainDb");

Database.AddConnectionString(
    DatabaseType.SqlServer,
    "MainDb",
    sqlServerConnection,
    allowTableExtensions: true
);
```

## Initialising Database
Initialise the database classes, this creates a cache of all models & properties instead of doing reflection at runtime to retrieve these. This is recommended to run at the start of your application.

```csharp
Database.Initialise();
//or
SqlGeneratorCache.Initialise();
```

## Creating models
Every mapped entity should implement `IMicroORMSharp`.
```csharp
using MicroORMSharp.SqlGenerator.Attributes;
using MicroORMSharp.SqlGenerator.Interfaces;

[DbTable("Customers")]
public class Customer : IMicroORMSharp
{
    [DbIdentity]
    public long Id { get; set; }

    public string Forename { get; set; }
    public string Surname { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string AddressLine3 { get; set; }
    public string AddressLine4 { get; set; }

    [DbColumn("Postalcode")]
    public string Postcode { get; set; }

    public bool Active { get; set; }

    [DbIgnore]
    public string FullName => $"{Forename} {Surname}";
}
```

### Attribute reference
- `[DbTable("Customers")]` map to the table
- `[DbTable("MyDatabase", "dbo", "Customers")]` map to the table
- `[DbColumn("Postalcode")]` map a property when the C# property doesn't match the table schema
- `[DbIdentity]` marks the identity/primary key column used by insert/update/delete behavior
- `[DbIgnore]` the property will be ignored completely. This is good for combining properties together or for properties that are not mapped to the database at all, such as calculated properties
- `[DbMaxLength(20)]` limits a string column length and validates values before insert/update
- `[DbPrecision(10, 3)]` configures decimal precision and scale for create table generation
- `[DbDefault("guest")]`, `[DbDefault(12.345)]`, `[DbDefault(7)]`, `[DbDefault(true)]` define column defaults used in table creation and when null values are inserted or updated

If you use the table extension methods to create tables, these attributes are used to generate the correct SQL schema.
When using `[DbMaxLength(20)]` on a string property, if you try to insert or update a value longer than 20 characters, an exception is thrown to prevent data truncation before it hits the database.

Attributes used for table creation:
```csharp
[DbTable("ConfiguredEntities")]
public class AttributeExample : IMicroORMSharp
{
    [DbIdentity]
    public long Id { get; set; }

    [DbMaxLength(20)]
    [DbDefault("guest")]
    public string? Name { get; set; }

    [DbPrecision(10, 3)]
    [DbDefault(12.345)]
    public decimal? Amount { get; set; }

    [DbDefault(7)]
    public int? Quantity { get; set; }

    [DbDefault(true)]
    public bool? IsEnabled { get; set; }
}
```

## Querying data
### Basic query examples
- `Execute()`
- `ExecuteAsync()`
- `ExecuteSingle()`
- `ExecuteSingleAsync()`
- `Any()`
- `AnyAsync()`
- `Count()`
- `CountAsync()`
```csharp
var customers = await Database.Query<Customer>()
    .ExecuteAsync();

var customer = await Database.Query<Customer>()
    .Where(x => x.Id == 1)
    .ExecuteSingleAsync();

var hasActiveCustomers = await Database.Query<Customer>()
    .Where(x => x.Active)
    .AnyAsync();

var activeCustomerCount = await Database.Query<Customer>()
    .Where(x => x.Active)
    .CountAsync();
```

### Using a context
Use `Database.CreateContext(...)` when you want a scoped connection and database type for several operations without changing the global current connection.
Context methods use the context connection automatically. They do not expose connection or transaction parameters.

```csharp
using var db = Database.CreateContext("ReportingMySql");

var customers = await db.Query<Customer>()
    .Where(x => x.Active)
    .ExecuteAsync();

var customer = await db.InsertAsync(new Customer
{
    Forename = "Jane",
    Surname = "Doe"
});

var count = await db.Dapper.QuerySingleAsync<int>(
    "SELECT COUNT(*) FROM Customers;"
);
```

### Selecting columns
`Select` allows you to specify which columns to query while still returning the entity type. This is useful when you only need a subset of the columns for read-only operations, and want to avoid querying unnecessary data.
`SelectTo` allows you to project the result into a different class, which is useful when you want to return a custom shape of data that doesn't match the entity type, such as a DTO or an anonymous type. This can help reduce over-fetching and improve performance by only querying the columns that are needed for the projection.
You can use either `Select` or `SelectTo` depending on your needs, if you use both, an exception is thrown.

`Select` can be used anywhere in the query chain
`SelectTo` can only be used last in the query chain before `Execute`, `ExecuteAsync`, `ExecuteSingle`, or `ExecuteSingleAsync`. This is because `SelectTo` switches from `DbQuery<T>` into a projection wrapper that is responsible for the final mapping step.

Similar to `Select`, `SelectTo` will only query the columns needed for the projection.

```csharp
// Select keeps the result as Customer
var customers = await Database.Query<Customer>()
    .Select(x => x.Id, x => x.Forename, x => x.Surname)
    .ExecuteAsync();

// SelectTo maps the result into a different class
var customerNames = await Database.Query<Customer>()
    .Where(x => x.Active)
    .SelectTo(x => new CustomerName
    {
        Name = x.Forename + " " + x.Surname
    })
    .ExecuteAsync();
```

### Filtering, ordering, limiting, and pagination
You can add where clauses, order by columns, take top results, and paginate query results.
```csharp
var customers = await Database.Query<Customer>()
    .Where(x => x.Id > 10 && x.Active)
    .ExecuteAsync();

var customers = await Database.Query<Customer>()
    .OrderByDescending(x => x.Id)
    .ThenBy(x => x.Forename)
    .ExecuteAsync();

var customers = await Database.Query<Customer>()
    .Take(10)
    .ExecuteAsync();

var customers = await Database.Query<Customer>()
    .OrderBy(x => x.Id)
    .SetPagination(pageNumber: 2, pageSize: 10)
    .ExecuteAsync();
```

`SetPagination(pageNumber, pageSize)` calculates the correct offset for you.
- MySQL uses `LIMIT ... OFFSET ...`
- SQL Server uses `ORDER BY ... OFFSET ... ROWS FETCH NEXT ... ROWS ONLY`

For SQL Server pagination a ORDER BY clause is required, if you do not specify one, it will fall back to the identity column or the first column when no identity is found.

### Timeout and cancellation token
You can set timeout and cancellation token per query or default them
```csharp
var customers = await Database.Query<Customer>()
    .SetTimeout(30)
    .SetCancellationToken(token)
    .ExecuteAsync();

Database.SetDefaultTimeout(60);
Database.SetDefaultCancellationToken(cancellationToken);
```

## Insert, update, and delete
The entity extension methods are the main write API.
### Insert
`Insert` / `InsertAsync` returns the inserted entity, including the generated identity value where supported.

```csharp
var customer = new Customer
{
    Forename = "John",
    Surname = "Doe",
    AddressLine1 = "1 Test Street",
    AddressLine2 = "Test Town",
    AddressLine3 = "Test City",
    AddressLine4 = "Test County",
    Postcode = "TE1 1ST",
    Active = true
};

customer = customer.Insert();
customer = await customer.InsertAsync();

//If you only want to run the insert:
customer.InsertOnly();
await customer.InsertOnlyAsync();
```

### Bulk insert
Bulk insert is available on `IEnumerable<T>`
Provider behavior:
- SQL Server uses `SqlBulkCopy`
- MySQL uses `MySqlBulkCopy`

For MySQL, make sure:
- the connection string includes `Allow Load Local Infile=True;`
- the database has `local_infile` enabled
```csharp
var customers = new List<Customer>
{
    new() { Forename = "John", Surname = "Doe", AddressLine1 = "A", AddressLine2 = "B", AddressLine3 = "C", AddressLine4 = "D", Postcode = "AA1", Active = true },
    new() { Forename = "Jane", Surname = "Doe", AddressLine1 = "A", AddressLine2 = "B", AddressLine3 = "C", AddressLine4 = "D", Postcode = "AA2", Active = true }
};
await customers.InsertAsync();
```

### Update
`Update` / `UpdateAsync` returns the updated entity.
```csharp
customer.Forename = "Jane";

customer = customer.Update();
customer = await customer.UpdateAsync();
customer = customer.Update(x => new { x.Forename, x.Postcode });
customer = await customer.UpdateAsync(x => new { x.Forename, x.Postcode });

//If you only want to execute the update:
customer.UpdateOnly();
await customer.UpdateOnlyAsync();
customer.UpdateOnly(x => new { x.Forename, x.Postcode });
await customer.UpdateOnlyAsync(x => new { x.Forename, x.Postcode });
```
When a selector is supplied, only the chosen mapped, non-identity columns are included in the `UPDATE` statement.
By default when an update is executed, all mapped, non-identity columns are included in the `UPDATE` statement.

### Delete
```csharp
await customer.DeleteAsync();
```

## Table helper methods
These methods require `allowTableExtensions: true` on the connection registration.
```csharp
//On a T which implements IMicroORMSharp
var customer = new Customer();
var exists = await customer.TableExistsAsync();
await customer.CreateTableAsync();
await customer.TruncateTableAsync();
await customer.DropTableAsync();

//On a List<T> which implements IMicroORMSharp
var customers = new List<Customer>();
var exists = await customers.TableExistsAsync();
await customers.CreateTableAsync();
await customers.TruncateTableAsync();
await customers.DropTableAsync();
```

## Scoped connections
The high-level write, query, and table APIs no longer take a public `IDbConnection`. Use a context when several operations should use the same configured connection.

```csharp
using var db = Database.CreateContext("MainMySql");

var customer = await db.InsertAsync(new Customer
{
    Forename = "John",
    Surname = "Doe",
    AddressLine1 = "1 Test Street",
    AddressLine2 = "Test Town",
    AddressLine3 = "Test City",
    AddressLine4 = "Test County",
    Postcode = "TE1 1ST",
    Active = true
});

var customers = await db.Query<Customer>()
    .Where(x => x.Active)
    .ExecuteAsync();

customer.Forename = "Updated";
customer = await db.UpdateAsync(customer);

await db.DeleteAsync(customer);
```

You can still get a raw connection with `Database.GetConnection(...)` when you need one for your own code, or use `Database.WithConnection(...)` / `DBContext.WithConnection(...)`.

## Transactions
No public transaction methods are exposed.
`WithTransaction` / `WithTransactionAsync` methods will pass a `TransactionContext` object to the callback and execute the commit or rollback if there is an error.
This includes query, extension& dapper methods, so you do not need to pass a connection or transaction into each call.
If the callback completes, the transaction is committed and the method returns `true`.
If the callback throws, the transaction is rolled back and the method returns `false`.

```csharp
using var db = Database.CreateContext("MainMySql");

var committed = await db.WithTransactionAsync(async trans =>
{
    var customer = await trans.InsertAsync(new Customer
    {
        Forename = "John",
        Surname = "Doe",
        AddressLine1 = "1 Test Street",
        AddressLine2 = "Test Town",
        AddressLine3 = "Test City",
        AddressLine4 = "Test County",
        Postcode = "TE1 1ST",
        Active = true
    });

    await trans.Dapper.ExecuteAsync(
        "UPDATE Customers SET Active = @Active WHERE Id = @Id;",
        new { Active = false, customer.Id }
    );

    var count = await trans.Dapper.QuerySingleAsync<int>(
        "SELECT COUNT(*) FROM Customers;"
    );
});
```

`Commit()` and `Rollback()` are not exposed publicly.

For a global/default connection transaction, use `Database.WithTransactionAsync(...)`:

```csharp
var committed = await Database.WithTransactionAsync(async trans =>
{
    await trans.Dapper.ExecuteAsync(
        "UPDATE Customers SET Active = @Active WHERE Id = @Id;",
        new { Active = false, Id = 1 }
    );
});
```

## Using raw Dapper through `Database.Dapper`
MicroORMSharp includes a Dapper wrapper so you can mix higher-level ORM helpers with raw SQL in the same codebase. Available wrappers include:
- `Execute` / `ExecuteAsync`
- `Query` / `QueryAsync`
- `QueryFirst`
- `QueryFirstOrDefault`
- `QuerySingle`
- `QuerySingleOrDefault`

These methods can accept an explicit `connection` or `transaction`. `DBContext.Dapper` is bound to the context connection.
Inside `WithTransaction`, `trans.Dapper` is bound to the transaction and intentionally omits connection and transaction parameters.

```csharp
var rows = await Database.Dapper.QueryAsync<Customer>(
    "SELECT * FROM Customers WHERE Active = @Active;",
    new { Active = true }
);
```

For transaction-scoped raw SQL, use `WithTransaction` and call `trans.Dapper` as shown in the transaction examples.

## Join mapping
You can define joined relationships with `DBJoin`.
```csharp
[DbTable("Customer")]
public class CustomerWithOrders : IMicroORMSharp
{
    [DbIdentity]
    public long Id { get; set; }

    public string Name { get; set; }
    public string Email { get; set; }

    [DBJoin(typeof(Order), "Id", "CustomerId", DBJoinType.Left)]
    public List<Order> Orders { get; set; }
}

[DbTable("Order")]
public class Order : IMicroORMSharp
{
    [DbIdentity]
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; }
}
```
Then query as normal:
```csharp
var customers = await Database.Query<CustomerWithOrders>().ExecuteAsync();
```

You can specify `DBJoinType.Inner`, `DBJoinType.Left`, `DBJoinType.Right` for joins.
Nested joins are supported up to 3 levels deep. Queries that exceed that limit throw an `InvalidOperationException`.

## Additional helpers
```csharp
var query = Database.Query<Customer>()
    .Where(x => x.Active)
    .OrderBy(x => x.Id);

var sqlQuery = query.GetSqlQuery(DatabaseType.MySql);
var sqlParameters = query.GetSqlParameters();
```

## Issues
If you find a bug or want to suggest an improvement, please open an issue or pull request.

> This package is provided as-is, without guarantees of any kind, and you are responsible for validating its behavior in your environment before production use. The authors and contributors are not liable for data loss, downtime, corruption, security issues, financial loss, or other damages resulting from use of the package.

## Author
- [@SpikeThatMike](https://github.com/SpikeThatMike)
