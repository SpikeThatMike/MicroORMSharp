using MicroORMSharp.SqlGenerator;
using MicroORMSharp.SqlGenerator.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MicroORMSharp
{
    public static partial class Database
    {
        //This needs to be refactored to use COUNT(*) instead
        public static int Count<T>(this DbQuery<T> dbQuery) where T : IMicroORMSharp
        {
            if (dbQuery == null)
            {
                throw new ArgumentNullException(nameof(dbQuery));
            }

            return dbQuery.Execute().Count();
        }

        //This needs to be refactored to use COUNT(*) instead
        public static async Task<int> CountAsync<T>(this DbQuery<T> dbQuery) where T : IMicroORMSharp
        {
            if (dbQuery == null)
            {
                throw new ArgumentNullException(nameof(dbQuery));
            }

            return (await dbQuery.ExecuteAsync()).Count();
        }
    }
}
