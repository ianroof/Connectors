﻿// Copyright 2017 the original author or authors.
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
using Microsoft.Extensions.DependencyInjection;
using Steeltoe.CloudFoundry.Connector.Relational;
using Steeltoe.CloudFoundry.Connector.Test;
using Steeltoe.Common.HealthChecks;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using System;
using System.Data;
using Xunit;

namespace Steeltoe.CloudFoundry.Connector.AzureSqlDatabase.Test
{
    public class AzureSqlDatabaseProviderServiceCollectionExtensionsTest
    {
        public AzureSqlDatabaseProviderServiceCollectionExtensionsTest()
        {
            Environment.SetEnvironmentVariable("VCAP_APPLICATION", null);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", null);
        }

        [Fact]
        public void AddSqlServerConnection_ThrowsIfServiceCollectionNull()
        {
            // Arrange
            IServiceCollection services = null;
            IConfigurationRoot config = null;

            // Act and Assert
            var ex = Assert.Throws<ArgumentNullException>(() => AzureSqlDatabaseProviderServiceCollectionExtensions.AddAzureSqlDatabaseConnection(services, config));
            Assert.Contains(nameof(services), ex.Message);

            var ex2 = Assert.Throws<ArgumentNullException>(() => AzureSqlDatabaseProviderServiceCollectionExtensions.AddAzureSqlDatabaseConnection(services, config, "foobar"));
            Assert.Contains(nameof(services), ex2.Message);
        }

        [Fact]
        public void AddSqlServerConnection_ThrowsIfConfigurationNull()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            IConfigurationRoot config = null;

            // Act and Assert
            var ex = Assert.Throws<ArgumentNullException>(() => AzureSqlDatabaseProviderServiceCollectionExtensions.AddAzureSqlDatabaseConnection(services, config));
            Assert.Contains(nameof(config), ex.Message);

            var ex2 = Assert.Throws<ArgumentNullException>(() => AzureSqlDatabaseProviderServiceCollectionExtensions.AddAzureSqlDatabaseConnection(services, config, "foobar"));
            Assert.Contains(nameof(config), ex2.Message);
        }

        [Fact]
        public void AddSqlServerConnection_ThrowsIfServiceNameNull()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            IConfigurationRoot config = null;
            string serviceName = null;

            // Act and Assert
            var ex = Assert.Throws<ArgumentNullException>(() => AzureSqlDatabaseProviderServiceCollectionExtensions.AddAzureSqlDatabaseConnection(services, config, serviceName));
            Assert.Contains(nameof(serviceName), ex.Message);
        }

        [Fact]
        public void AddSqlServerConnection_NoVCAPs_AddsSqlServerConnection()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            IConfigurationRoot config = new ConfigurationBuilder().Build();

            // Act and Assert
            AzureSqlDatabaseProviderServiceCollectionExtensions.AddAzureSqlDatabaseConnection(services, config);

           var service = services.BuildServiceProvider().GetService<IDbConnection>();
           Assert.NotNull(service);
        }

        [Fact]
        public void AddSqlServerConnection_WithServiceName_NoVCAPs_ThrowsConnectorException()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            IConfigurationRoot config = new ConfigurationBuilder().Build();

            // Act and Assert
            var ex = Assert.Throws<ConnectorException>(() => AzureSqlDatabaseProviderServiceCollectionExtensions.AddAzureSqlDatabaseConnection(services, config, "foobar"));
            Assert.Contains("foobar", ex.Message);
        }

        [Fact]
        public void AddSqlServerConnection_MultipleSqlServerServices_ThrowsConnectorException()
        {
            // Arrange an environment where multiple sql server services have been provisioned
            IServiceCollection services = new ServiceCollection();
            Environment.SetEnvironmentVariable("VCAP_APPLICATION", TestHelpers.VCAP_APPLICATION);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", AzureSqlDatabaseTestHelpers.TwoServerVCAP);

            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddCloudFoundry();
            var config = builder.Build();

            // Act and Assert
            var ex = Assert.Throws<ConnectorException>(() => AzureSqlDatabaseProviderServiceCollectionExtensions.AddAzureSqlDatabaseConnection(services, config));
            Assert.Contains("Multiple", ex.Message);
        }

        [Fact]
        public void AddSqlServerConnection_WithVCAPs_AddsSqlServerConnection()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            Environment.SetEnvironmentVariable("VCAP_APPLICATION", TestHelpers.VCAP_APPLICATION);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", AzureSqlDatabaseTestHelpers.SingleServerVCAP);

            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddCloudFoundry();
            var config = builder.Build();

            // Act and Assert
            AzureSqlDatabaseProviderServiceCollectionExtensions.AddAzureSqlDatabaseConnection(services, config);

            var service = services.BuildServiceProvider().GetService<IDbConnection>();
            Assert.NotNull(service);
            var connString = service.ConnectionString;
            Assert.Contains("test_db_name", connString);
            Assert.Contains("azuredbtest.database.windows.net", connString);
            Assert.Contains("1433", connString);
            Assert.Contains("testuser", connString);
            Assert.Contains("testpass", connString);
            Assert.Contains("Encrypt=True", connString);
            Assert.Contains("TrustServerCertificate=True", connString);
            Assert.Contains("Connection Timeout=30", connString);
        }

        [Fact]
        public void AddSqlServerConnection_AddsRelationalHealthContributor()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddCloudFoundry();
            var config = builder.Build();

            // Act
            AzureSqlDatabaseProviderServiceCollectionExtensions.AddAzureSqlDatabaseConnection(services, config);
            var healthContributor = services.BuildServiceProvider().GetService<IHealthContributor>() as RelationalHealthContributor;

            // Assert
            Assert.NotNull(healthContributor);
        }
    }
}
