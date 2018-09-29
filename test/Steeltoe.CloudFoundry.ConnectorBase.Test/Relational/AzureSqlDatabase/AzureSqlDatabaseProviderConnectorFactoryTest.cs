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
using System.Data.SqlClient;
using Xunit;

namespace Steeltoe.CloudFoundry.Connector.AzureSqlDatabase.Test
{
    public class AzureSqlDatabaseProviderConnectorFactoryTest
    {
        [Fact]
        public void Constructor_ThrowsIfConfigNull()
        {
            // Arrange
            AzureSqlDatabaseProviderConnectorOptions config = null;

            // Act and Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new AzureSqlDatabaseProviderConnectorFactory(null, config, typeof(SqlConnection)));
            Assert.Contains(nameof(config), ex.Message);
        }

        [Fact]
        public void Create_ReturnsSqlConnection()
        {
            AzureSqlDatabaseProviderConnectorOptions config = new AzureSqlDatabaseProviderConnectorOptions()
            {
                HostName = "servername",
                Password = "password",
                Username = "username",
                SqlDbName = "database"
            };
            AzureSqlDatabaseServiceInfo si = new AzureSqlDatabaseServiceInfo("MyId", "mssql//servername:1433/databaseName=test_db_name", "testuser", "testpass");
            var factory = new AzureSqlDatabaseProviderConnectorFactory(si, config, typeof(SqlConnection));
            var connection = factory.Create(null);
            Assert.NotNull(connection);
        }
    }
}
