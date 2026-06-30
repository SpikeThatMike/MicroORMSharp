[![NuGet Version](https://img.shields.io/nuget/v/MicroORMSharp?logo=nuget&label=Latest%20version&style=flat)](https://www.nuget.org/packages/MicroORMSharp)
[![NuGet Downloads](https://img.shields.io/nuget/dt/MicroORMSharp?style=flat&logo=nuget&label=Downloads)](https://www.nuget.org/packages/MicroORMSharp)
[![GitHub License](https://img.shields.io/github/license/SpikeThatMike/MicroORMSharp?style=flat&label=License&logo=github)](https://github.com/SpikeThatMike/MicroORMSharp/blob/main/LICENSE.txt)

# MicroORMSharp
**MicroORMSharp** is a lightweight ORM for .NET built on Dapper.
- Entity-based CRUD operations (inserts, updates, deletes)
- Bulk insert support
- LINQ-style query builder
- Automatic SQL generation
- LEFT, INNER, RIGHT joins with nested join support
- Transaction scopes
- Optional table operations (create, drop, truncate, exists)
- Multiple database connections
- Raw Dapper access when needed

Designed to reduce repetitive SQL and manual object mapping through all of your projects

| Databases | Supported |
| --- | --- |
| MySQL | ✅ |
| SQL Server | ✅ |
| Others | ❌ |

### Additional database providers
> Currently only MySQL and SQL Server are supported.
> Support for additional providers is welcome through issues and pull requests.

## Why MicroORMSharp?
MicroORMSharp is not intended to replace Entity Framework or anything like that

It is designed for developers who:
- Want Dapper-level performance
- Prefer working with POCO classes
- Need lightweight LINQ-style query generation
- Want simple transaction and connection management
- Do not require change tracking or migrations


***⚠ Still early in development, verify results in a test environment before using in production ⚠***

## Quick Example
```csharp
var customers = await Database.Query<Customer>()
    .Where(x => x.Active)
    .OrderBy(x => x.Id)
    .ExecuteAsync();

var customer = await new Customer
{
    Forename = "John",
    Surname = "Doe"
}.InsertAsync();
```

### Requirements
| Version | Supported .NET versions |
| --- | --- |
| `1.x` | `.NET Core 3.0`, `.NET 5`, `.NET 6`, `.NET 7`, `.NET 8`, `.NET 9`, `.NET 10` |

`MicroORMSharp` currently targets `.NET Standard 2.1`, so `.NET Framework` is not supported.

## Installation
```bash
dotnet add package MicroORMSharp
```

## How it works
1. Register your connection.
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
    connectionTest: false //By default when adding a connection, MicroORMSharp will open a connection and close it to ensure the connection works, adding this stops that behaviour
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
Set `allowTableExtensions: true` if you want to use table extension methods, async methods available:

- `CreateTable()`
- `DropTable()`
- `TruncateTable()`

If the flag is not enabled for the active connection reference, an exception will be thrown.


## Initialising Database
Initialise the database classes, this creates a cache of all models & properties instead of doing reflection at runtime. This is recommended to run at the start of your application but will self initialise if not.

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

    [DbMaxLength(20)]
    [DbDefault("guest")]
    public string Forename { get; set; }
    public string Surname { get; set; }
    public string AddressLine1 { get; set; }

    [DbPrecision(10, 3)]
    [DbDefault(12.345)]
    public decimal Amount { get; set; }

    [DbColumn("Postalcode")]
    public string Postcode { get; set; }

    [DbDefault(true)]
    public bool Active { get; set; }

    [DbIgnore]
    public string FullName => $"{Forename} {Surname}";
}
```

### Attribute reference
- `[DbTable("Customers")]` table name
- `[DbTable("MyDatabase", "dbo", "Customers")]` table name
- `[DbColumn("Postalcode")]` map a table column to a property when the name doesn't match
- `[DbIdentity]` marks the identity/primary key column used by insert/update/delete behavior
- `[DbIgnore]` the property will be ignored. Used for combining properties or properties not mapped to the database
- `[DbMaxLength(20)]` limits a string column length and validates values before insert/update
- `[DbPrecision(10, 3)]` configures decimal precision and scale for create table generation
- `[DbDefault("guest")]`, `[DbDefault(12.345)]`, `[DbDefault(7)]`, `[DbDefault(true)]` define defaults used in table creation and when null values are inserted or updated

Using the table extension methods to create tables, these attributes are used to generate the correct SQL schema.
When using `[DbMaxLength(20)]` on a property, if you try to insert or update a value longer than 20 characters, an exception is thrown to prevent data truncation before it hits the database.

## Querying data
### Basic query examples
- `Execute()`
- `ExecuteSingle()`
- `Any()`
- `Count()`

Async methods are available

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

Supported Methods inside of Where clause for SQL properties
- string.Contains - `%LIKE%`
- string.StartsWith - `LIKE%`
- string.EndsWith - `%LIKE`
- string.Equals - `=`
- string.Trim - `TRIM()`
- string.TrimStart - `LTRIM()`
- string.TrimEnd - `RTRIM()`
- IEnumerable.Contains - `IN`

### Using a context
Use `Database.CreateContext(...)` when you want a scoped connection and database type for several operations without changing the global connection.
Context methods use the context connection automatically. They do not expose connection or transaction parameters.
However you will notice the syntax slightly differs for insert,update,delete. You must pass the object in rather than using an extension method

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
`Select` allows you to specify columns to query while returning the entity type. Used for when you need a subset of the columns and want to avoid querying unnecessary data.
`SelectTo` allows you to map the result into a different class used when you want to return a custom class that doesn't match the entity type. This can help reduce over-fetching and improve performance by only querying the columns that are needed for the projection.
You can use either `Select` or `SelectTo` depending on your needs, you cannot use both in the same query.

`Select` can be used anywhere in the query chain
`SelectTo` can only be used last in the query chain before `Execute` or `ExecuteSingle`. This is because `SelectTo` switches from `DbQuery<T>` into a wrapper that is responsible for the final mapping step.

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
Where clauses, Order by columns, take top results, and paginate results.
```csharp
var customers = await Database.Query<Customer>()
    .Where(x => x.Id > 10 && x.Active)
    .ExecuteAsync();

var customers = await Database.Query<Customer>()
    .OrderByDescending(x => x.Id)
    .ThenBy(x => x.Forename)
    .ExecuteAsync();

var customers = await Database.Query<Customer>()
    .Take(10) //MySQL limit, SQL server TOP
    .ExecuteAsync();

var customers = await Database.Query<Customer>()
    .OrderBy(x => x.Id)
    .SetPagination(pageNumber: 2, pageSize: 10)
    .ExecuteAsync();
```

`SetPagination(pageNumber, pageSize)` calculates the correct offset for you.
- MySQL uses `LIMIT ... OFFSET ...`
- SQL Server uses `ORDER BY ... OFFSET ... ROWS FETCH NEXT ... ROWS ONLY`

For SQL Server pagination a `ORDER BY` clause is required, if none is specified, it will fall back to the identity column or the first column when no identity is found.

### Timeout and cancellation token
Set timeout and cancellation token per query or default for all operations
```csharp
var customers = await Database.Query<Customer>()
    .SetTimeout(30)
    .SetCancellationToken(token)
    .ExecuteAsync();

Database.SetDefaultTimeout(60);
Database.SetDefaultCancellationToken(cancellationToken);
```

## Insert, update, and delete

### Insert
`Insert` returns the inserted entity, including the generated identity value where supported.

```csharp
var customer = new Customer
{
    Forename = "John",
    Surname = "Doe",
    AddressLine1 = "1 Test Street",
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
    new() { Forename = "John", Surname = "Doe", AddressLine1 = "A", Postcode = "AA1", Active = true },
    new() { Forename = "Jane", Surname = "Doe", AddressLine1 = "A", Postcode = "AA2", Active = true }
};
await customers.InsertAsync();
```

### Update
`Update` updates a row from the table off the identity.
Returns the updated entity by automatically requerying the database. Use UpdateOnly if you don't want this functionality
```csharp
customer.Forename = "Jane";

customer = customer.Update();
customer = customer.Update(x => new { x.Forename, x.Postcode });

//If you only want to execute the update:
customer.UpdateOnly();
customer.UpdateOnly(x => new { x.Forename, x.Postcode });
```
When a selector is supplied, only the chosen mapped, non-identity columns are included in the `UPDATE` statement.
By default when an update is executed, all mapped, non-identity columns are included in the `UPDATE` statement.

### Delete
`Delete` deletes a row from the table off the identity
```csharp
customer.Delete();
```

## Transactions
No public transaction methods are exposed.
`WithTransaction` / `WithTransactionAsync` methods will pass a `TransactionContext` object to the callback and execute the commit or rollback if there is an error.
This includes query, extension & dapper methods, so you do not need to pass a connection or transaction into each call.
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

## Table helper methods
These methods require `allowTableExtensions: true` on the connection registration.
```csharp
var customer = new Customer();
var exists = await customer.TableExists();
customer.CreateTable();
customer.TruncateTable();
customer.DropTable();
 
var customers = new List<Customer>();
var exists = await customers.TableExists();
customers.CreateTable();
customers.TruncateTable();
customers.DropTable();
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

## Using raw Dapper through `Database.Dapper`
MicroORMSharp includes a Dapper wrapper so you can mix MicroORMSharp with SQL. Available wrappers include:
- `Execute`
- `Query`
- `QueryFirst`
- `QueryFirstOrDefault`
- `QuerySingle`
- `QuerySingleOrDefault`

These methods can accept an explicit `connection` or `transaction`. `DBContext.Dapper` is bound to the context connection.
Inside `WithTransaction`, `transaction.Dapper` is bound to the transaction and intentionally omits connection and transaction parameters. If you provide these inside of a command definition, an error will be thrown.

```csharp
var rows = await Database.Dapper.QueryAsync<Customer>(
    "SELECT * FROM Customers WHERE Active = @Active;",
    new { Active = true }
);
```

For transaction-scoped raw SQL, use `WithTransaction` and call `trans.Dapper` as shown in the transaction examples.

## Join mapping
You can define joined relationships with `DBJoin` passing in the join class type, the key to join on the main table, the key on the joined table and join type.
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