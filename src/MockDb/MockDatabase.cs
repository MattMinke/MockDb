using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

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
            Mock<DbProviderFactory> mockFactory = new Mock<DbProviderFactory>(MockBehavior.Strict);

            mockFactory.SetupGet(o => o.CanCreateCommandBuilder)
                .Returns(false);
            mockFactory.SetupGet(o => o.CanCreateDataAdapter)
                .Returns(false);
            mockFactory.SetupGet(o => o.CanCreateDataSourceEnumerator)
                .Returns(false);

            mockFactory.Setup(o => o.CreateCommandBuilder())
                .Returns(() => null);
            mockFactory.Setup(o => o.CreateDataAdapter())
                .Returns(() => null);
            mockFactory.Setup(o => o.CreateDataSourceEnumerator())
                .Returns(() => null);

            mockFactory.Setup(o => o.CreateConnection())
                 .Returns(() => CreateConnection());
            mockFactory.Setup(o => o.CreateCommand())
                .Returns(() => CreateCommand());
            mockFactory.Setup(o => o.CreateParameter())
                .Returns(() => CreateParameter());

            return mockFactory.Object;
        }


        public DbConnection CreateConnection()
        {
            Mock<DbConnection> mockConnection = new Mock<DbConnection>(MockBehavior.Strict);
            ConnectionState state = ConnectionState.Closed;
            string connectionString = null;
            bool isDisposed = false;

            void ThrowIfDisposed()
            {
                if (isDisposed)
                {
                    throw new ObjectDisposedException(nameof(DbConnection));
                }
            }

            mockConnection.SetupGet(o => o.State)
                .Returns(() =>
                {
                    ThrowIfDisposed();
                    return state;
                });

            mockConnection.Setup(o => o.Open())
                .Callback(() =>
                {
                    ThrowIfDisposed();
                    state = ConnectionState.Open;
                });
            mockConnection.Setup(o => o.OpenAsync(It.IsAny<CancellationToken>()))
                .Returns<CancellationToken>((token) =>
                {
                    ThrowIfDisposed();
                    token.ThrowIfCancellationRequested();

                    state = ConnectionState.Open;
                    return Task.CompletedTask;
                });
            mockConnection.Setup(o => o.Close())
                .Callback(() =>
                {
                    ThrowIfDisposed();
                    state = ConnectionState.Closed;
                });
            mockConnection.Setup(o => o.CloseAsync())
                .Returns(() =>
                {
                    ThrowIfDisposed();
                    state = ConnectionState.Closed;
                    return Task.CompletedTask;
                });

            mockConnection.Protected().Setup("Dispose", ItExpr.IsAny<bool>())
                .Callback(() =>
                {
                    if (!isDisposed)
                    {
                        mockConnection.Object.Close();
                        isDisposed = true;
                    }
                });

            mockConnection.Setup(o => o.DisposeAsync())
                .Returns(async () =>
                {
                    if (!isDisposed)
                    {
                        await mockConnection.Object.CloseAsync();
                        isDisposed = true;
                    }
                });

            mockConnection.SetupSet(o => o.ConnectionString = It.IsAny<string>())
                .Callback<string>(value =>
                {
                    ThrowIfDisposed();
                    connectionString = value;
                });
            mockConnection.SetupGet(o => o.ConnectionString)
                .Returns(() =>
                {
                    ThrowIfDisposed();
                    return connectionString;
                });

            mockConnection.Protected().Setup<DbCommand>("CreateDbCommand")
                .Returns(() =>
                {
                    ThrowIfDisposed();
                    if (state != ConnectionState.Open)
                    {
                        throw new DataException($"Connection must be Open. Ensure Connection.Open() or Connection.OpenAsync() has been called");
                    }
                    return CreateCommand();
                });

            mockConnection.Protected().Setup<DbTransaction>("BeginDbTransaction", ItExpr.IsAny<IsolationLevel>())
                .Returns(() =>
                {
                    ThrowIfDisposed();
                    if (state != ConnectionState.Open)
                    {
                        throw new DataException($"Connection must be Open. Ensure Connection.Open() or Connection.OpenAsync() has been called");
                    }
                    return CreateTransaction();
                });
            mockConnection.Protected().Setup<ValueTask<DbTransaction>>("BeginDbTransactionAsync", ItExpr.IsAny<IsolationLevel>(), ItExpr.IsAny<CancellationToken>())
                .Returns(async () =>
                {
                    ThrowIfDisposed();
                    if (state != ConnectionState.Open)
                    {
                        throw new DataException($"Connection must be Open. Ensure Connection.Open() or Connection.OpenAsync() has been called");
                    }
                    return CreateTransaction();
                });

            return mockConnection.Object;
        }


        private DbCommand CreateCommand()
        {
            var mockCommand = new Mock<DbCommand>(MockBehavior.Strict);


            return mockCommand.Object;
        }

        private DbTransaction CreateTransaction()
        {
            Mock<DbTransaction> mockTransaction = new Mock<DbTransaction>(MockBehavior.Strict);

            return mockTransaction.Object;
        }

        private DbParameter CreateParameter()
        {
            Mock<DbParameter> mockParameter = new Mock<DbParameter>(MockBehavior.Strict);
   
            return mockParameter.Object;
        }
    }
}
