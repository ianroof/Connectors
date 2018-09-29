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

using Steeltoe.Extensions.Configuration.CloudFoundry;
using Xunit;

namespace Steeltoe.CloudFoundry.Connector.Services.Test
{
    public class AzureSqlDatabaseServiceInfoFactoryTest
    {
        [Fact]
        public void Accept_AcceptsValidServiceBinding()
        {
            Service s = new Service()
            {
                Label = "azure-sqldb",
                Tags = new string[] { "azure-sqldb" },
                Name = "azureSqlDbServiceInstance",
                Plan = "free",
                Credentials = new Credential()
                {
                    { "hostname", new Credential("azuredbtest.database.windows.net") },
                    { "port", new Credential("1433") },
                    { "sqldbName", new Credential("test_db_name") },
                    { "username", new Credential("testuser") },
                    { "password", new Credential("testpass") },
                    { "uri", new Credential("mssql://testuser:testpass@azuredbtest.database.windows.net:1433/test_db_name?encrypt=true&TrustServerCertificate=true&Connection%20Timeout=30") },
                }
            };
            AzureSqlDatabaseServiceInfoFactory factory = new AzureSqlDatabaseServiceInfoFactory();
            Assert.True(factory.Accept(s));
        }

        [Fact]
        public void Accept_AcceptsValidCUPServiceBinding()
        {
            Service s = new Service()
            {
                Label = "user-provided",
                Tags = new string[] { },
                Name = "azureSqlDbServiceInstance",
                Plan = "free",
                Credentials = new Credential()
                {
                    { "username", new Credential("testuser") },
                    { "password", new Credential("testpass") },
                    { "uri", new Credential("mssql://testuser:testpass@azuredbtest.database.windows.net:1433/test_db_name?encrypt=true&TrustServerCertificate=true&Connection%20Timeout=30") },
                 }
            };
            AzureSqlDatabaseServiceInfoFactory factory = new AzureSqlDatabaseServiceInfoFactory();
            Assert.True(factory.Accept(s));
        }

        [Fact]
        public void Accept_AcceptsNoLabelNoTagsServiceBinding()
        {
            Service s = new Service()
            {
                Name = "azureSqlDbServiceInstance",
                Credentials = new Credential()
                {
                    { "hostname", new Credential("azuredbtest.database.windows.net") },
                    { "port", new Credential("1433") },
                    { "sqldbName", new Credential("test_db_name") },
                    { "username", new Credential("testuser") },
                    { "password", new Credential("testpass") },
                    { "uri", new Credential("mssql://testuser:testpass@azuredbtest.database.windows.net:1433/test_db_name?encrypt=true&TrustServerCertificate=true&Connection%20Timeout=30") },
                }
            };
            AzureSqlDatabaseServiceInfoFactory factory = new AzureSqlDatabaseServiceInfoFactory();
            Assert.True(factory.Accept(s));
        }

        [Fact]
        public void Accept_AcceptsLabelNoTagsServiceBinding()
        {
            Service s = new Service()
            {
                Label = "azure-sqldb",
                Name = "azureSqlDbServiceInstance",
                Plan = "free",
                Credentials = new Credential()
                {
                    { "hostname", new Credential("azuredbtest.database.windows.net") },
                    { "port", new Credential("1433") },
                    { "sqldbName", new Credential("test_db_name") },
                    { "username", new Credential("testuser") },
                    { "password", new Credential("testpass") },
                    { "uri", new Credential("mssql://testuser:testpass@azuredbtest.database.windows.net:1433/test_db_name?encrypt=true&TrustServerCertificate=true&Connection%20Timeout=30") },
                }
            };
            AzureSqlDatabaseServiceInfoFactory factory = new AzureSqlDatabaseServiceInfoFactory();
            Assert.True(factory.Accept(s));
        }

        [Fact]
        public void Accept_RejectsInvalidServiceBinding()
        {
            Service s = new Service()
            {
                Label = "p-foobar",
                Tags = new string[] { "foobar" },
                Name = "azureSqlDbServiceInstance",
                Plan = "100mb-dev",
                Credentials = new Credential()
                {
                    { "hostname", new Credential("azuredbtest.database.windows.net") },
                    { "port", new Credential("1433") },
                    { "sqldbName", new Credential("test_db_name") },
                    { "username", new Credential("testuser") },
                    { "password", new Credential("testpass") },
                    { "uri", new Credential("foobar://testuser:testpass@azuredbtest.database.windows.net:1433/test_db_name?encrypt=true&TrustServerCertificate=true&Connection%20Timeout=30") },
                }
            };
            AzureSqlDatabaseServiceInfoFactory factory = new AzureSqlDatabaseServiceInfoFactory();
            Assert.False(factory.Accept(s));
        }

        [Fact]
        public void Create_CreatesValidServiceBinding()
        {
            Service s = new Service()
            {
                Label = "azure-sqldb",
                Tags = new string[] { "azure-sqldb" },
                Name = "azureSqlDbServiceInstance",
                Plan = "free",
                Credentials = new Credential()
                {
                    { "hostname", new Credential("azuredbtest.database.windows.net") },
                    { "port", new Credential("1433") },
                    { "sqldbName", new Credential("test_db_name") },
                    { "username", new Credential("testuser") },
                    { "password", new Credential("testpass") },
                    { "uri", new Credential("mssql://testuser:testpass@azuredbtest.database.windows.net:1433/test_db_name?encrypt=true&TrustServerCertificate=true&Connection%20Timeout=30") },
                }
            };
            AzureSqlDatabaseServiceInfoFactory factory = new AzureSqlDatabaseServiceInfoFactory();
            var info = factory.Create(s) as AzureSqlDatabaseServiceInfo;
            Assert.NotNull(info);
            Assert.Equal("azureSqlDbServiceInstance", info.Id);
            Assert.Equal("testpass", info.Password);
            Assert.Equal("testuser", info.UserName);
            Assert.Equal("azuredbtest.database.windows.net", info.Host);
            Assert.Equal(1433, info.Port);
            Assert.Equal("test_db_name", info.Path);
            Assert.Equal("encrypt=true&TrustServerCertificate=true&Connection%20Timeout=30", info.Query);
        }

        [Fact]
        public void Create_CreatesValidServiceBinding_NoUri()
        {
            Service s = new Service()
            {
                Label = "azure-sqldb",
                Tags = new string[] { "sqlserver", "relational" },
                Name = "azureSqlDbServiceInstance",
                Plan = "free",
                Credentials = new Credential()
                {
                    { "hostname", new Credential("azuredbtest.database.windows.net") },
                    { "port", new Credential("1433") },
                    { "sqldbName", new Credential("test_db_name") },
                    { "username", new Credential("testuser") },
                    { "password", new Credential("testpass") },
                }
            };
            AzureSqlDatabaseServiceInfoFactory factory = new AzureSqlDatabaseServiceInfoFactory();
            var info = factory.Create(s) as AzureSqlDatabaseServiceInfo;
            Assert.NotNull(info);
            Assert.Equal("azureSqlDbServiceInstance", info.Id);
            Assert.Equal("testpass", info.Password);
            Assert.Equal("testuser", info.UserName);
            Assert.Equal("azuredbtest.database.windows.net", info.Host);
            Assert.Equal(1433, info.Port);
            Assert.Equal("test_db_name", info.Path);
        }
    }
}
