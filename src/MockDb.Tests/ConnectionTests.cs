using NUnit.Framework;
using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MockDb.Tests
{

    public class ConnectionTests
    {

        [Test]
        public void Connection_Open_Succeeds()
        {
            // Arrange
            // =================
            var target = new MockDatabase();
            var connection = target.CreateConnection();

            // Act
            // =================

            connection.Open();

            // Assert
            // =================
            Assert.That(connection.State, Is.EqualTo(ConnectionState.Open));

        }

        [Test]
        public void Connection_OpenThenClose_Succeeds()
        {
            // Arrange
            // =================
            var target = new MockDatabase();
            var connection = target.CreateConnection();

            // Act
            // =================

            connection.Open();
            connection.Close();

            // Assert
            // =================
            Assert.That(connection.State, Is.EqualTo(ConnectionState.Closed));

        }

        [Test]
        public async Task Connection_OpenAsync_Succeeds()
        {
            // Arrange
            // =================
            var target = new MockDatabase();
            var connection = target.CreateConnection();

            // Act
            // =================

            await connection.OpenAsync();

            // Assert
            // =================
            Assert.That(connection.State, Is.EqualTo(ConnectionState.Open));
        }


        [Test]
        public async Task Connection_OpenAsyncThenCloseAsync_Succeeds()
        {
            // Arrange
            // =================
            var target = new MockDatabase();
            var connection = target.CreateConnection();

            // Act
            // =================

            await connection.OpenAsync();
            await connection.CloseAsync();

            // Assert
            // =================
            Assert.That(connection.State, Is.EqualTo(ConnectionState.Closed));
        }

        [Test]
        public void Connection_OpenAsync_CancellationRequested()
        {
            // Arrange
            // =================
            var target = new MockDatabase();
            var connection = target.CreateConnection();

            // Act
            // =================
            AsyncTestDelegate action = async () =>
            {
                using (var source = new CancellationTokenSource())
                {
                    source.Cancel();
                    await connection.OpenAsync(source.Token);
                }
            };

            // Assert
            // =================
            Assert.ThrowsAsync<OperationCanceledException>(action);
        }

    }
}