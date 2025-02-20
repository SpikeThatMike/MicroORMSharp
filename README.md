# MicroORMSharp

**MicroORMSharp** is a lightweight, fast, and flexible micro ORM for .NET, designed to simplify database interactions using Dapper. It provides essential functionalities such as Insert, Delete, Update, and Query for straightforward data manipulation without the overhead of a full-fledged ORM like Entity Framework.

## Installation

```cs
dotnet add package MicroORMSharp;
```
## How to use

### Add connection string
```cs
Database.AddConnectionString(
    DatabaseType.MySql, // Database type
    "MySql", // Reference
    "Server=localhost;Database=test;User ID=root;Password=admin;Port=3306;", // Connection string
    allowTableExtensions: true, // allow using the table extensions
);
```

### Create class
```cs
    [DbTable("Customers")]
    public class Customers : IMicroORMSharp
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
        public string FullName
        {
            get
            {
                return Forename + " " + Surname;
            }
        }
    }
```

### Query database
```cs
// All methods have an synchronous version
var customers = await Database.Query<Customers>().ExecuteAsync();
var customer = await Database.Query<Customers>().ExecuteSingleAsync();
var hasCustomers = await Database.Query<Customers>().AnyAsync();
var customerCount = await Database.Query<Customers>().CountAsync();

var customers = await Database.Query<Customers>().Select(x => x.Id).ExecuteAsync();
var customers = await Database.Query<Customers>().Where(x => x.Id > 10).ExecuteAsync();
var customers = await Database.Query<Customers>().Take(10).ExecuteAsync();
var customers = await Database.Query<Customers>().OrderBy(x => x.Id).ExecuteAsync();
var customers = await Database.Query<Customers>().OrderByDescending(x => x.Id).ExecuteAsync();
var customers = await Database.Query<Customers>().SetCancellationToken(token).ExecuteAsync();
var customers = await Database.Query<Customers>().SetTimeout(30).ExecuteAsync();
```

### Class extensions
```cs
// All methods have an synchronous version
var customer = new Customers()
{
    Forename = "John",
    Surname = "Doe",
    AddressLine1 = "Test Street",
    AddressLine2 = "Test Town",
    AddressLine3 = "Test City",
    AddressLine4 = "Test County",
    Postcode = "Postcode",
    Active = true,
};

// Table extensions, allowTableExtensions must be set to true
var exists = await customer.TableExistsAsync();
await customer.CreateTableAsync();
await customer.DropTableAsync();
await customer.TruncateTableAsync();

customer = await customer.InsertAsync(); // await customer.InsertOnlyAsync(); - doesnt return a class with the inserted id
customer.Forename = "John1";
await customer.UpdateAsync();
await customer.DeleteAsync();


var customers = new List<Customers>();

// Table extensions, allowTableExtensions must be set to true
var exists = await customers.TableExistsAsync();
await customers.CreateTableAsync();
await customers.DropTableAsync();
await customers.TruncateTableAsync();

// For MySql, AllowLoadLocalInfile=true" must be in the connection string and SET GLOBAL local_infile=1; set on the server
await customers.InsertAsync(); // Uses SqlBulkCopy / MySqlBulkCopy
```


## Issues

Any issues, please create an issue or pull request.


## Authors

- [@SpikeThatMike](https://github.com/SpikeThatMike)
