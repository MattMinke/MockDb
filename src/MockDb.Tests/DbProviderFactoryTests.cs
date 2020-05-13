using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MockDb.Tests
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class DbProviderFactoryTests
    {
        [Test]
        public void ProviderFactory_CreateConnection_Succeeds()
        {
            // Arrange
            // =================
            var target = new MockDatabase();
            var factory = target.CreateProviderFactory();

            // Act
            // =================

            var connection = factory.CreateConnection();

            // Assert
            // =================
            Assert.That(connection, Is.Not.Null);

        }
    }
}
