using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace MockDb
{



    /// <summary>
    /// Provides factory methods for creating common objects used to interact with a data source. 
    /// </summary>
    /// <remarks>
    /// https://github.com/dotnet/runtime/blob/master/src/libraries/System.Data.Common/src/System/Data/Common/DbProviderFactory.cs
    /// </remarks>
    public class MockDbProviderFactory : DbProviderFactory
    {
        private readonly MockDatabase _database;

        public MockDbProviderFactory(MockDatabase database)
        {
            _database = database;
        }


        public override DbCommand CreateCommand()
        {
            return new MockDbCommand();
        }

        public override DbConnection CreateConnection()
        {
            return new MockDbConnection(_database, this);
        }

        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            throw new NotImplementedException();
        }

        public override DbParameter CreateParameter()
        {
            return new MockDbParameter();
        }

    }
}
