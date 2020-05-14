using System;
using System.Data;
using System.Data.Common;

namespace MockDb
{
    public class MockDbTransaction : DbTransaction
    {
        private MockDatabase _database;
        private readonly IsolationLevel _isolationLevel;
        private MockDbConnection _dbConnection;

        public MockDbTransaction(
            IsolationLevel isolationLevel,
            MockDbConnection mockDbConnection, 
            MockDatabase database)
        {
            _isolationLevel = isolationLevel;
            _dbConnection = mockDbConnection;
            _database = database;
        }

        /// <summary>
        /// True if the current instance has been disposed; otherwise false.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the current instance has been disposed.
        /// </summary>
        void ThrowIfDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(DbConnection));
            }
        }


        public override IsolationLevel IsolationLevel
        {
            get
            {
                ThrowIfDisposed();
                return _isolationLevel;
            }
        }

        protected override DbConnection DbConnection
        {
            get
            {
                ThrowIfDisposed();
                return _dbConnection;
            }
        }

        public override void Commit()
        {
            ThrowIfDisposed();
        }

        public override void Rollback()
        {
            ThrowIfDisposed();
        }

        protected override void Dispose(bool disposing)
        {
            _database = null;
            _dbConnection = null;
        }

    }
}
