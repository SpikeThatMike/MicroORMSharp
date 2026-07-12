using MudBlazor;

namespace MicroORMSharp.Website.Models
{
    public sealed record ShowcaseItem(string Title, string Body, string Icon, Color Color);

    public static class ShowcaseItems
    {
        public static readonly IReadOnlyList<ShowcaseItem> Items =
        [
            new(
                "Multiple Connections",
                "Manage and query multiple connections.",
                Icons.Material.Filled.Cable,
                Color.Primary
            ),
            new(
                "Model mapping",
                "Create models for database tables with names, identity, ignored properties, defaults, column names, and validation.",
                Icons.Material.Filled.Schema,
                Color.Secondary
            ),
            new(
                "Querying",
                "Use LINQ style methods to query the database: Where, Select, SelectTo, ordering, and pagination",
                Icons.Material.Filled.FilterAlt,
                Color.Tertiary
            ),
            new(
                "Entity CRUD operations",
                "Insert, updates, partial update, bulk insert and delete",
                Icons.Material.Filled.EditNote,
                Color.Info
            ),
            new(
                "Transactions",
                "Create transaction context to easily deal with transactions.",
                Icons.Material.Filled.AccountTree,
                Color.Success
            ),
            new(
                "Join mapping",
                "Model table relationships with multiple joins supported.",
                Icons.Material.Filled.Hub,
                Color.Warning
            )
        ];
    }
}
