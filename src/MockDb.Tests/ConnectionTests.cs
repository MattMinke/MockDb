using NUnit.Framework;
using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace MockDb.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
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
            Assert.ThrowsAsync<TaskCanceledException>(action);
            Assert.That(connection.State, Is.EqualTo(ConnectionState.Closed));
        }

        [Test]
        public void Connection_CreateCommand_FailsWhenConnectionIsNotOpen()
        {
            // Arrange
            // =================
            var target = new MockDatabase();
            var connection = target.CreateConnection();

            // Act
            // =================

            TestDelegate action = () =>
            {
                var command = connection.CreateCommand();
            };

            // Assert
            // =================
            Assert.Throws<DataException>(action);
            Assert.That(connection.State, Is.EqualTo(ConnectionState.Closed));
        }

        [Test]
        public void Connection_CreateCommand_SucceedsWhenConnectionIsOpen()
        {
            // Arrange
            // =================
            var target = new MockDatabase();
            var connection = target.CreateConnection();

            // Act
            // =================

            connection.Open();
            var command = connection.CreateCommand();


            // Assert
            // =================
            Assert.That(connection.State, Is.EqualTo(ConnectionState.Open));
            Assert.That(command, Is.Not.Null);
        }

        [Test]
        public void Connection_BeginTransaction_FailsWhenConnecitonIsNotOpen()
        {
            // Arrange
            // =================
            var target = new MockDatabase();
            var connection = target.CreateConnection();

            // Act
            // =================

            TestDelegate action = () =>
            {
                var transaction = connection.BeginTransaction();
            };

            // Assert
            // =================
            Assert.Throws<DataException>(action);
            Assert.That(connection.State, Is.EqualTo(ConnectionState.Closed));
        }

        [Test]
        public void Connection_BeginTransaction_SucceedsWhenConnectionIsOpen()
        {
            // Arrange
            // =================
            var target = new MockDatabase();
            var connection = target.CreateConnection();

            // Act
            // =================

            connection.Open();
            var transaction = connection.BeginTransaction();


            // Assert
            // =================
            Assert.That(connection.State, Is.EqualTo(ConnectionState.Open));
            Assert.That(transaction, Is.Not.Null);
        }

        [Test]
        public void Connection_BeginTransactionAsync_FailsWhenConnecitonIsNotOpen()
        {
            // Arrange
            // =================
            var target = new MockDatabase();
            var connection = target.CreateConnection();

            // Act
            // =================

            AsyncTestDelegate action = async () =>
            {
                var transaction = await connection.BeginTransactionAsync();
            };

            // Assert
            // =================
            Assert.ThrowsAsync<DataException>(action);
            Assert.That(connection.State, Is.EqualTo(ConnectionState.Closed));
        }

        [Test]
        public async Task Connection_BeginTransactionAsync_SucceedsWhenConnectionIsOpen()
        {
            // Arrange
            // =================
            var target = new MockDatabase();
            var connection = target.CreateConnection();

            // Act
            // =================

            connection.Open();
            var transaction = await connection.BeginTransactionAsync();


            // Assert
            // =================
            Assert.That(connection.State, Is.EqualTo(ConnectionState.Open));
            Assert.That(transaction, Is.Not.Null);
        }

        [Test]
        public void Connection_Dispose_Succeeds()
        {
            // Arrange
            // =================
            var target = new MockDatabase();
            var connection = target.CreateConnection();

            // Act
            // =================

            connection.Dispose();

            TestDelegate action = () =>
            {
                // Any Action should throw an object disposed exception. we will test with Open. 
                connection.Open();
            };

            // Assert
            // =================
            Assert.Throws<ObjectDisposedException>(action);
        }

        [Test]
        public async Task Connection_DisposeAsync_Succeeds()
        {
            // Arrange
            // =================
            var target = new MockDatabase();
            var connection = target.CreateConnection();

            // Act
            // =================

            await connection.DisposeAsync();

            TestDelegate action = () =>
            {
                // Any Action should throw an object disposed exception. we will test with Open. 
                connection.Open();
            };

            // Assert
            // =================
            Assert.Throws<ObjectDisposedException>(action);
        }

        [Test]
        public void Connection_ConnectionString_IsPersistent()
        {
            // Arrange
            // =================
            const string CONNECTION_STRING = "Test_Value";
            var target = new MockDatabase();
            var connection = target.CreateConnection();

            // Act
            // =================

            connection.ConnectionString = CONNECTION_STRING;

            // Assert
            // =================
            Assert.That(connection.ConnectionString, Is.EqualTo(CONNECTION_STRING));
        }
    }
}