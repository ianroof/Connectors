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

using Microsoft.Extensions.Configuration;
using Steeltoe.CloudFoundry.Connector.Test;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using System;
using System.Collections.Generic;
using Xunit;

namespace Steeltoe.CloudFoundry.Connector.AzureSqlDatabase.Test
{
    public class AzureSqlDatabaseProviderConnectorOptionsTest
    {
        [Fact]
        public void Constructor_ThrowsIfConfigNull()
        {
            // Arrange
            IConfiguration config = null;

            // Act and Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new AzureSqlDatabaseProviderConnectorOptions(config));
            Assert.Contains(nameof(config), ex.Message);
        }

        [Fact]
        public void Constructor_BindsValues()
        {
            var appsettings = new Dictionary<string, string>()
                {
                    ["azure-sqldb:credentials:hostName"] = "servername",
                    ["azure-sqldb:credentials:port"] = "1433",
                    ["azure-sqldb:credentials:uri"] = "mssql://servername:1433/databaseName=test_db_name",
                    ["azure-sqldb:credentials:sqldbName"] = "test_db_name",
                    ["azure-sqldb:credentials:username"] = "username",
                    ["azure-sqldb:credentials:password"] = "password"
            };

            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(appsettings);
            var config = configurationBuilder.Build();

            var sconfig = new AzureSqlDatabaseProviderConnectorOptions(config);
            Assert.Equal("servername", sconfig.HostName);
            Assert.Equal(1433, sconfig.Port);
            Assert.Equal("password", sconfig.Password);
            Assert.Equal("username", sconfig.Username);
        }

        [Fact]
        public void Constructor_BindsValues_And_Sets_Extended_Properties_From_QueryString()
        {
            var appsettings = new Dictionary<string, string>()
            {
                ["azure-sqldb:credentials:hostName"] = "servername",
                ["azure-sqldb:credentials:port"] = "1433",
                ["azure-sqldb:credentials:uri"] = "mssql://servername:1433/databaseName=test_db_name?encrypt=true&trustServerCertificate=true&connection%20Timeout=45",
                ["azure-sqldb:credentials:sqldbName"] = "test_db_name",
                ["azure-sqldb:credentials:username"] = "username",
                ["azure-sqldb:credentials:password"] = "password"
            };

            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(appsettings);
            var config = configurationBuilder.Build();

            var sconfig = new AzureSqlDatabaseProviderConnectorOptions(config);
            Assert.Equal("servername", sconfig.HostName);
            Assert.Equal(1433, sconfig.Port);
            Assert.Equal("password", sconfig.Password);
            Assert.Equal("username", sconfig.Username);
            Assert.True(sconfig.Encrypt);
            Assert.True(sconfig.TrustServerCertificate);
            Assert.Equal(45, sconfig.ConnectionTimeout);
        }

        [Fact]
        public void CloudFoundryConfig_Found_By_Name()
        {
            // arrange
            Environment.SetEnvironmentVariable("VCAP_APPLICATION", TestHelpers.VCAP_APPLICATION);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", AzureSqlDatabaseTestHelpers.SingleServerVCAPNoTag);

            // add settings to config
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddEnvironmentVariables();
            configurationBuilder.AddCloudFoundry();
            var config = configurationBuilder.Build();

            // act
            var sconfig = new AzureSqlDatabaseProviderConnectorOptions(config);

            // assert
            Assert.NotEqual("localhost", sconfig.HostName);
            Assert.NotEqual("test_db_name", sconfig.SqlDbName);
            Assert.NotEqual("testuser", sconfig.Username);
            Assert.NotEqual("testpass", sconfig.Password);
        }

        [Fact]
        public void CloudFoundryConfig_Found_By_Tag()
        {
            // arrange
            Environment.SetEnvironmentVariable("VCAP_APPLICATION", TestHelpers.VCAP_APPLICATION);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", AzureSqlDatabaseTestHelpers.SingleServerVCAPIgnoreName);

            // add settings to config
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddEnvironmentVariables();
            configurationBuilder.AddCloudFoundry();
            var config = configurationBuilder.Build();

            // act
            var sconfig = new AzureSqlDatabaseProviderConnectorOptions(config);

            // assert
            Assert.NotEqual("localhost", sconfig.HostName);
            Assert.NotEqual("test_db_name", sconfig.HostName);
            Assert.NotEqual("testuser", sconfig.Username);
            Assert.NotEqual("testpass", sconfig.Password);
        }
    }
}
