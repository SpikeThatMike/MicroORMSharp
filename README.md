# MicroORMSharp
**MicroORMSharp** is a lightweight micro ORM for .NET built on top of Dapper. It focuses on the common operations
- CRUD operations (inserts, updates, deletes)
- Querying with LINQ-style API
- Querying data with LEFT,INNER,RIGHT joins with nested join support
- Optional table operations (create, drop, truncate, exists)
- Bulk insert support
- The ability use native Dapper methods without handling connections

Designed to be reduce repetitive SQL and object mapping.

## Installation
```bash
dotnet add package MicroORMSharp
```

## Supported databases

- MySQL
- SQL Server (untested)

## How MicroORMSharp works
1. Register your connection string.
2. Create a model that implements `IMicroORMSharp`.
3. Query data with `Database.Query<T>()`
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
- `[DbIgnore]` excludes a property from persistence

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

### Selecting columns
You can select specific columns if you don't want to query the whole class
```csharp
var customers = await Database.Query<Customer>()
    .Select(x => x.Id, x => x.Forename, x => x.Surname)
    .ExecuteAsync();
```

### Filtering, ordering, and limiting
You can add where clauses, order by columns and take top results
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
```

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

//If you only want to execute the update:
customer.UpdateOnly();
await customer.UpdateOnlyAsync();
```

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

## Passing your own connection
The write and table extension methods accept an explicit `IDbConnection` through `dbConnection`.
This is useful when:
- You manage the connections elsewhere
- You want several operations to use the same connection

```csharp
using var connection = Database.GetConnection();
connection.Open();

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

customer = await customer.InsertAsync(dbConnection: connection);

var exists = await customer.TableExistsAsync(dbConnection: connection);

customer.Forename = "Updated";
customer = await customer.UpdateAsync(dbConnection: connection);

await customer.DeleteAsync(dbConnection: connection);
```

When `dbConnection` is provided, MicroORMSharp reuses it instead of creating a new one.

## Passing your own transaction

The same extension methods also accept `IDbTransaction` through `dbTransaction`.
If a transaction is supplied, MicroORMSharp uses the transaction's connection automatically if no connection is provided

```csharp
using var connection = Database.GetConnection();
connection.Open();

using var transaction = connection.BeginTransaction();

try
{
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

    customer = await customer.InsertAsync(dbTransaction: transaction);

    customer.Forename = "Jane";
    customer = await customer.UpdateAsync(dbTransaction: transaction);

    var exists = await customer.TableExistsAsync(dbTransaction: transaction);

    if (exists)
    {
        await customer.DeleteAsync(dbTransaction: transaction);
    }

    transaction.Commit();
}
catch
{
    transaction.Rollback();
    throw;
}
```
This is the best approach when several operations must succeed or fail together.

## Using raw Dapper through `Database.Dapper`
MicroORMSharp includes a Dapper wrapper so you can mix higher-level ORM helpers with raw SQL in the same codebase. Available wrappers include:
- `Execute` / `ExecuteAsync`
- `Query` / `QueryAsync`
- `QueryFirst`
- `QueryFirstOrDefault`
- `QuerySingle`
- `QuerySingleOrDefault`

These methods also accept an explicit `connection` or `transaction`.

```csharp
var rows = await Database.Dapper.QueryAsync<Customer>(
    "SELECT * FROM Customers WHERE Active = @Active;",
    new { Active = true }
);


//Using a transactiom
using var connection = Database.GetConnection();
connection.Open();

using var transaction = connection.BeginTransaction();

try
{
    await Database.Dapper.ExecuteAsync(
        "INSERT INTO Customers (Forename, Surname, AddressLine1, AddressLine2, AddressLine3, AddressLine4, Postalcode, Active) " +
        "VALUES (@Forename, @Surname, @AddressLine1, @AddressLine2, @AddressLine3, @AddressLine4, @Postcode, @Active);",
        new
        {
            Forename = "John",
            Surname = "Doe",
            AddressLine1 = "1 Test Street",
            AddressLine2 = "Test Town",
            AddressLine3 = "Test City",
            AddressLine4 = "Test County",
            Postcode = "TE1 1ST",
            Active = true
        },
        transaction: transaction
    );

    var count = await Database.Dapper.QuerySingleAsync<int>(
        "SELECT COUNT(*) FROM Customers;",
        transaction: transaction
    );

    transaction.Commit();
}
catch
{
    transaction.Rollback();
    throw;
}
```

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

## Issues
If you find a bug or want to suggest an improvement, please open an issue or pull request.

## Author
- [@SpikeThatMike](https://github.com/SpikeThatMike)
