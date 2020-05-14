using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace MockDb
{
    public class MockDatabase
    {
        /// <summary>
        /// Create a <see cref="DbProviderFactory"/> using the provided configuration that can be used to 
        /// create <see cref="DbParameter"/>, <see cref="DbCommand"/>, and <see cref="DbConnection"/> objects
        /// </summary>
        /// <returns>A new instance that inherits from <see cref="DbProviderFactory"/> using the provided configuration.</returns>
        public DbProviderFactory CreateProviderFactory()
        {
            return new MockDbProviderFactory(this);
        }

        public DbConnection CreateConnection()
        {
            return new MockDbConnection(this);
        }
    }
}
