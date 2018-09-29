// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Steeltoe.CloudFoundry.Connector.Services;
using System;
using Xunit;

namespace Steeltoe.CloudFoundry.Connector.AzureSqlDatabase.EF6.Test
{
    public class AzureSqlDatabaseDbContextConnectorFactoryTest
    {
        [Fact]
        public void Constructor_ThrowsIfTypeNull()
        {
            // Arrange
            AzureSqlDatabaseProviderConnectorOptions config = new AzureSqlDatabaseProviderConnectorOptions();
            AzureSqlDatabaseServiceInfo si = null;
            Type dbContextType = null;

            // Act and Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new AzureSqlDatabaseDbContextConnectorFactory(si, config, dbContextType));
            Assert.Contains(nameof(dbContextType), ex.Message);
        }

        [Fact]
        public void Create_ThrowsIfNoValidConstructorFound()
        {
            // Arrange
            AzureSqlDatabaseProviderConnectorOptions config = new AzureSqlDatabaseProviderConnectorOptions();
            AzureSqlDatabaseServiceInfo si = null;
            Type dbContextType = typeof(BadAzureSqlDatabaseDbContext);

            // Act and Assert
            var ex = Assert.Throws<ConnectorException>(() => new AzureSqlDatabaseDbContextConnectorFactory(si, config, dbContextType).Create(null));
            Assert.Contains("BadAzureSqlDatabaseDbContext", ex.Message);
        }

        [Fact]
        public void Create_ReturnsDbContext()
        {
            AzureSqlDatabaseProviderConnectorOptions config = new AzureSqlDatabaseProviderConnectorOptions()
            {
                HostName = "localhost",
                Port = 1433,
                Password = "password",
                Username = "username",
                SqlDbName = "database"
            };
            AzureSqlDatabaseServiceInfo si = new AzureSqlDatabaseServiceInfo("MyId", "mssql://azuredbtest.database.windows.net:1433/test_db_name?encrypt=true&trusServerCertificate=false&connection%20timeout=30", "testuser", "testpass");
            var factory = new AzureSqlDatabaseDbContextConnectorFactory(si, config, typeof(GoodAzureSqlDatabaseDbContext));
            var context = factory.Create(null);
            Assert.NotNull(context);
            var gcontext = context as GoodAzureSqlDatabaseDbContext;
            Assert.NotNull(gcontext);

            var con = gcontext.Database.Connection;
            Assert.NotNull(con);

            var connString = con.ConnectionString;
            Assert.NotNull(connString);
            Assert.Contains("Database=test_db_name", connString);
            Assert.Contains("Data Source=azuredbtest.database.windows.net", connString);
            Assert.Contains("User Id=testuser", connString);
            Assert.Contains("Password=testpass", connString);
            Assert.Contains("Encrypt=True", connString);
            Assert.Contains("TrustServerCertificate=False", connString);
            Assert.Contains("Connection Timeout=30", connString);
        }
    }
}
