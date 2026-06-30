using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Threading.Tasks;

namespace MicroORMSharp
{
    public static partial class Database
    {
        public static bool Any<T>(this DbQuery<T> dbQuery) where T : IMicroORMSharp
        {
            if (dbQuery == null)
            {
                throw new ArgumentNullException(nameof(dbQuery));
            }

            T result = dbQuery.ExecuteSingle();
            return result != null;
        }

        public static async Task<bool> AnyAsync<T>(this DbQuery<T> dbQuery) where T : IMicroORMSharp
        {
            if (dbQuery == null)
            {
                throw new ArgumentNullException(nameof(dbQuery));
            }

            T result = await dbQuery.ExecuteSingleAsync();
            return result != null;
        }
    }
}
