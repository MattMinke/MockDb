using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Moq;

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

            return mockConnection.Object;
        }

    }
}
