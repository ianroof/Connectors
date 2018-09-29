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
using Xunit;

namespace Steeltoe.CloudFoundry.Connector.AzureSqlDatabase.Test
{
    public class AzureSqlDatabaseProviderConfigurerTest
    {
        // shared variable to hold config (like from a source such as appsettings)
        private AzureSqlDatabaseProviderConnectorOptions config = new AzureSqlDatabaseProviderConnectorOptions()
        {
            HostName = "localhost",
            Port = 1433,
            Username = "username",
            Password = "password",
            SqlDbName = "database",
            ConnectionTimeout = 30,
            Encrypt = true,
            TrustServerCertificate = true
        };

        [Fact]
        public void UpdateConfiguration_WithNullSqlServerServiceInfo_ReturnsExpected()
        {
            AzureSqlDatabaseProviderConfigurer configurer = new AzureSqlDatabaseProviderConfigurer();
            configurer.UpdateConfiguration(null, config);

            Assert.Equal("localhost", config.HostName);
            Assert.Equal("username", config.Username);
            Assert.Equal("password", config.Password);
            Assert.Equal("database", config.SqlDbName);
            Assert.True(config.Encrypt);
            Assert.True(config.TrustServerCertificate);
            Assert.Equal(30, config.ConnectionTimeout);
        }

        [Fact]
        public void Update_With_ServiceInfo_Updates_Config()
        {
            AzureSqlDatabaseProviderConfigurer configurer = new AzureSqlDatabaseProviderConfigurer();
            AzureSqlDatabaseServiceInfo si = new AzureSqlDatabaseServiceInfo("MyId", "mssql://updatedserver:1433/updateddb", "updateduser", "updatedpassword");

            configurer.UpdateConfiguration(si, config);

            Assert.Equal("updatedserver", config.HostName);
            Assert.Equal("updateddb", config.SqlDbName);
            Assert.Equal("updateduser", config.Username);
            Assert.Equal("updatedpassword", config.Password);
            Assert.True(config.Encrypt);
            Assert.True(config.TrustServerCertificate);
            Assert.Equal(30, config.ConnectionTimeout);
        }

        [Fact]
        public void Update_With_ServiceInfo_CredsInUrl_Updates_Config()
        {
            AzureSqlDatabaseProviderConfigurer configurer = new AzureSqlDatabaseProviderConfigurer();
            AzureSqlDatabaseServiceInfo si = new AzureSqlDatabaseServiceInfo("MyId", "sqlserver://updateduser:updatedpassword@updatedserver:1433;databaseName=updateddb");

            configurer.UpdateConfiguration(si, config);

            Assert.Equal("updatedserver", config.HostName);
            Assert.Equal("updateddb", config.SqlDbName);
            Assert.Equal("updateduser", config.Username);
            Assert.Equal("updatedpassword", config.Password);
            Assert.True(config.Encrypt);
            Assert.True(config.TrustServerCertificate);
            Assert.Equal(30, config.ConnectionTimeout);
        }

        [Fact]
        public void Update_With_ServiceInfo_Extended_Properties_In_Query_Update_Config()
        {
            AzureSqlDatabaseProviderConfigurer configurer = new AzureSqlDatabaseProviderConfigurer();
            AzureSqlDatabaseServiceInfo si = new AzureSqlDatabaseServiceInfo("MyId", "sqlserver://updateduser:updatedpassword@updatedserver:1433/updateddb?encrypt=false&trustServerCertificate=false&connection%20timeout=15");

            configurer.UpdateConfiguration(si, config);

            Assert.Equal("updatedserver", config.HostName);
            Assert.Equal("updateddb", config.SqlDbName);
            Assert.Equal("updateduser", config.Username);
            Assert.Equal("updatedpassword", config.Password);
            Assert.False(config.Encrypt);
            Assert.False(config.TrustServerCertificate);
            Assert.Equal(15, config.ConnectionTimeout);
        }


        [Fact]
        public void Configure_Without_ServiceInfo_Returns_Config()
        {
            AzureSqlDatabaseProviderConfigurer configurer = new AzureSqlDatabaseProviderConfigurer();
            var opts = configurer.Configure(null, config);
            Assert.Contains("Data Source=localhost", opts);
            Assert.Contains("User Id=username;", opts);
            Assert.Contains("Password=password;", opts);
            Assert.Contains("Database=database;", opts);
            Assert.Contains("Encrypt=True;", opts);
            Assert.Contains("TrustServerCertificate=True;", opts);
            Assert.Contains("Connection Timeout=30;", opts);
        }

        [Fact]
        public void Configure_With_ServiceInfo_Overrides_Config()
        {
            AzureSqlDatabaseProviderConfigurer configurer = new AzureSqlDatabaseProviderConfigurer();

            // override provided by environment
            AzureSqlDatabaseServiceInfo si = new AzureSqlDatabaseServiceInfo("MyId", "mssql://servername:1433/test_db_name", "testuser", "testpass");

            // apply override
            var opts = configurer.Configure(si, config);

            // resulting options should contain values parsed from environment
            Assert.Contains("Data Source=servername", opts);
            Assert.Contains("Database=test_db_name;", opts);
            Assert.Contains("User Id=testuser;", opts);
            Assert.Contains("Password=testpass;", opts);
            Assert.Contains("Encrypt=True;", opts);
            Assert.Contains("TrustServerCertificate=True;", opts);
            Assert.Contains("Connection Timeout=30;", opts);
        }

        [Fact]
        public void Configure_With_ServiceInfo_CredsInUrl_Overrides_Config()
        {
            AzureSqlDatabaseProviderConfigurer configurer = new AzureSqlDatabaseProviderConfigurer();

            // override provided by environment
            AzureSqlDatabaseServiceInfo si = new AzureSqlDatabaseServiceInfo("MyId", "mssql://testuser:testpass@servername:1433/test_db_name");

            // apply override
            var opts = configurer.Configure(si, config);

            // resulting options should contain values parsed from environment
            Assert.Contains("Data Source=servername", opts);
            Assert.Contains("Database=test_db_name;", opts);
            Assert.Contains("User Id=testuser;", opts);
            Assert.Contains("Password=testpass;", opts);
            Assert.Contains("Encrypt=True;", opts);
            Assert.Contains("TrustServerCertificate=True;", opts);
            Assert.Contains("Connection Timeout=30;", opts);
        }

        [Fact]
        public void Configure_With_ServiceInfo_Extended_Properties_In_Query_Override_Config()
        {
            AzureSqlDatabaseProviderConfigurer configurer = new AzureSqlDatabaseProviderConfigurer();

            // override provided by environment
            AzureSqlDatabaseServiceInfo si = new AzureSqlDatabaseServiceInfo("MyId", "mssql://testuser:testpass@servername:1433/test_db_name?encrypt=false&trustServerCertificate=false&connection%20timeout=300");

            // apply override
            var opts = configurer.Configure(si, config);

            // resulting options should contain values parsed from environment
            Assert.Contains("Data Source=servername", opts);
            Assert.Contains("Database=test_db_name;", opts);
            Assert.Contains("User Id=testuser;", opts);
            Assert.Contains("Password=testpass;", opts);
            Assert.Contains("Encrypt=False;", opts);
            Assert.Contains("TrustServerCertificate=False;", opts);
            Assert.Contains("Connection Timeout=300;", opts);
        }
    }
}
