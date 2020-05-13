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
    [ExcludeFromCodeCoverage]
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
            mockFactory.Setup(o => o.CreateConnection())
                .Returns(() => CreateConnection());

            return mockFactory.Object;
        }

        public DbConnection CreateConnection()
        {
            Mock<DbConnection> mockConnection = new Mock<DbConnection>(MockBehavior.Strict);
            ConnectionState state = ConnectionState.Closed;


            mockConnection.SetupGet(o => o.State)
               .Returns(() =>
               {
                   return state;
               });

            mockConnection.Setup(o => o.Open())
                .Callback(() =>
                {
                    state = ConnectionState.Open;
                });
            mockConnection.Setup(o => o.OpenAsync(It.IsAny<CancellationToken>()))
                .Returns<CancellationToken>((token) =>
               {
                   token.ThrowIfCancellationRequested();

                   state = ConnectionState.Open;
                   return Task.CompletedTask;
               });
            mockConnection.Setup(o => o.Close())
               .Callback(() =>
               {
                   state = ConnectionState.Closed;
               });
            mockConnection.Setup(o => o.CloseAsync())
                .Returns(() =>
                {
                    state = ConnectionState.Closed;
                    return Task.CompletedTask;
                });

            mockConnection.Protected().Setup("Dispose", ItExpr.IsAny<bool>())
                .Callback(() =>
                {
                    mockConnection.Object.Close();
                });

            mockConnection.SetupProperty(o => o.ConnectionString, null);

            return mockConnection.Object;
        }

      
    }
}
