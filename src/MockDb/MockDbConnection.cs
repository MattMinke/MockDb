using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MockDb
{
    // https://github.com/dotnet/runtime/blob/master/src/libraries/System.Data.Common/src/System/Data/Common/DbConnection.cs
    public class MockDbConnection : DbConnection
    {
        private ConnectionState _state;
        private MockDatabase _database;
        private DbProviderFactory _dbProviderFactory;
        

        public MockDbConnection(MockDatabase database)
            : this(database, database.CreateProviderFactory())
        {
        }

        public MockDbConnection(MockDatabase database, DbProviderFactory dbProviderFactory)
            : base()
        {
            _database = database;
            _dbProviderFactory = dbProviderFactory;
        }


        /// <summary>
        /// True if the current instance has been disposed; otherwise false.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the current instance has been disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(DbConnection));
            }
        }
        private void ThrowIfClosed()
        {
            if (_state == ConnectionState.Closed)
            {
                throw new DataException($"Connection must be Open. Ensure Connection.Open() or Connection.OpenAsync() has been called");
            }
        }

        /// <summary>
        /// The connection string used to establish the initial connection. 
        /// </summary>
        public override string ConnectionString { get; set; }

        /// <summary>
        /// Gets the name of the current database after a connection is opened, or the database
        /// name specified in the connection string before the connection is opened.
        /// </summary>
        public override string Database { get { ThrowIfDisposed(); return "MockDb"; } }

        /// <summary>
        /// Gets the name of the database server to which to connect.
        /// </summary>
        public override string DataSource { get { ThrowIfDisposed(); return "localhost"; } }

        /// <summary>
        /// Gets a string that represents the version of the server to which the object is connected.
        /// </summary>
        public override string ServerVersion { get { ThrowIfDisposed(); return typeof(MockDbConnection).Assembly.GetName().Version.ToString(); } }

        /// <summary>
        /// The time (in seconds) to wait for a connection to open. 
        /// </summary>
        public override int ConnectionTimeout { get { ThrowIfDisposed(); return int.MaxValue; } }

        /// <summary>
        /// The state of the connection.
        /// </summary>
        public override ConnectionState State { get { ThrowIfDisposed(); return _state; } }


        public override void ChangeDatabase(string databaseName)
        {
            throw new NotSupportedException($"Method {nameof(ChangeDatabase)} is not supported");
        }

        // No need to override this method. The behavior in the base class is sufficient. 
        // https://github.com/dotnet/runtime/blob/master/src/libraries/System.Data.Common/src/System/Data/Common/DbConnection.cs#L170-L190
        //public override Task OpenAsync(CancellationToken cancellationToken)

        public override void Open()
        {
            ThrowIfDisposed();
            _state = ConnectionState.Open;
        }

        public override void Close()
        {
            ThrowIfDisposed();
            _state = ConnectionState.Closed;
        }

        // No need to override this method. The behavior in the base class is sufficient.
        // https://github.com/dotnet/runtime/blob/master/src/libraries/System.Data.Common/src/System/Data/Common/DbConnection.cs#L91-L102
        //public override Task CloseAsync()


        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            ThrowIfDisposed();
            ThrowIfClosed();
            return new MockDbTransaction(isolationLevel, this, _database);
        }

        // No need to override this method. The behavior in the base class is sufficient.
        // https://github.com/dotnet/runtime/blob/master/src/libraries/System.Data.Common/src/System/Data/Common/DbConnection.cs#L66-L81
        //protected override ValueTask<DbTransaction> BeginDbTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken)



        protected override DbCommand CreateDbCommand()
        {
            ThrowIfDisposed();
            ThrowIfClosed();
            var command = DbProviderFactory.CreateCommand();
            command.Connection = this;
            return command;
        }

        protected override DbProviderFactory DbProviderFactory
        {
            get
            {
                ThrowIfDisposed();
                return _dbProviderFactory;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
               
                Close();
                _database = null;
                _dbProviderFactory = null;
                IsDisposed = true;

            }
        }

        // No need to override this method. The behavior in the base class is sufficient.
        // https://github.com/dotnet/runtime/blob/master/src/libraries/System.Data.Common/src/System/Data/Common/DbConnection.cs#L104-L108
        //public override ValueTask DisposeAsync()
    }
}
