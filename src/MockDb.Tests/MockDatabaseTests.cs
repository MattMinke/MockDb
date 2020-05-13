using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace MockDb.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class MockDatabaseTests
    {
        [Test]
        public void MockDatabase_CreateConnection_Succeeds()
        {
            // Arrange
            // =================
            var target = new MockDatabase();


            // Act
            // =================
            var connection = target.CreateConnection();

            // Assert
            // =================
            Assert.That(connection, Is.Not.Null);
        }

        [Test]
        public void MockDatabase_CreateProviderFactory_Succeeds()
        {
            // Arrange
            // =================
            var target = new MockDatabase();


            // Act
            // =================
            var providerFactory = target.CreateProviderFactory();

            // Assert
            // =================
            Assert.That(providerFactory, Is.Not.Null);
        }

    }
}