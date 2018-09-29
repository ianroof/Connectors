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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Steeltoe.CloudFoundry.Connector.EFCore.Test;
using Steeltoe.CloudFoundry.Connector.Test;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using System;
using System.Data.SqlClient;
using Xunit;

namespace Steeltoe.CloudFoundry.Connector.AzureSqlDatabase.EFCore.Test
{
    public class AzureSqlDatabaseDbContextOptionsExtensionsTest
    {
        public AzureSqlDatabaseDbContextOptionsExtensionsTest()
        {
            Environment.SetEnvironmentVariable("VCAP_APPLICATION", null);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", null);
        }

        [Fact]
        public void UseAzureSqlDatabase_ThrowsIfDbContextOptionsBuilderNull()
        {
            // Arrange
            DbContextOptionsBuilder optionsBuilder = null;
            DbContextOptionsBuilder<GoodDbContext> goodBuilder = null;
            IConfigurationRoot config = null;

            // Act and Assert
            var ex = Assert.Throws<ArgumentNullException>(() => AzureSqlDatabaseDbContextOptionsExtensions.UseAzureSqlDatabase(optionsBuilder, config));
            Assert.Contains(nameof(optionsBuilder), ex.Message);

            var ex2 = Assert.Throws<ArgumentNullException>(() => AzureSqlDatabaseDbContextOptionsExtensions.UseAzureSqlDatabase(optionsBuilder, config, "foobar"));
            Assert.Contains(nameof(optionsBuilder), ex2.Message);

            var ex3 = Assert.Throws<ArgumentNullException>(() => AzureSqlDatabaseDbContextOptionsExtensions.UseAzureSqlDatabase<GoodDbContext>(goodBuilder, config));
            Assert.Contains(nameof(optionsBuilder), ex3.Message);

            var ex4 = Assert.Throws<ArgumentNullException>(() => AzureSqlDatabaseDbContextOptionsExtensions.UseAzureSqlDatabase<GoodDbContext>(goodBuilder, config, "foobar"));
            Assert.Contains(nameof(optionsBuilder), ex4.Message);
        }

        [Fact]
        public void UseAzureSqlDatabase_ThrowsIfConfigurationNull()
        {
            // Arrange
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
            DbContextOptionsBuilder<GoodDbContext> goodBuilder = new DbContextOptionsBuilder<GoodDbContext>();
            IConfigurationRoot config = null;

            // Act and Assert
            var ex = Assert.Throws<ArgumentNullException>(() => AzureSqlDatabaseDbContextOptionsExtensions.UseAzureSqlDatabase(optionsBuilder, config));
            Assert.Contains(nameof(config), ex.Message);

            var ex2 = Assert.Throws<ArgumentNullException>(() => AzureSqlDatabaseDbContextOptionsExtensions.UseAzureSqlDatabase(optionsBuilder, config, "foobar"));
            Assert.Contains(nameof(config), ex2.Message);

            var ex3 = Assert.Throws<ArgumentNullException>(() => AzureSqlDatabaseDbContextOptionsExtensions.UseAzureSqlDatabase<GoodDbContext>(goodBuilder, config));
            Assert.Contains(nameof(config), ex3.Message);

            var ex4 = Assert.Throws<ArgumentNullException>(() => AzureSqlDatabaseDbContextOptionsExtensions.UseAzureSqlDatabase<GoodDbContext>(goodBuilder, config, "foobar"));
            Assert.Contains(nameof(config), ex4.Message);
        }

        [Fact]
        public void UseAzureSqlDatabase_ThrowsIfServiceNameNull()
        {
            // Arrange
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
            DbContextOptionsBuilder<GoodDbContext> goodBuilder = new DbContextOptionsBuilder<GoodDbContext>();
            IConfigurationRoot config = new ConfigurationBuilder().Build();
            string serviceName = null;

            // Act and Assert
            var ex2 = Assert.Throws<ArgumentException>(() => AzureSqlDatabaseDbContextOptionsExtensions.UseAzureSqlDatabase(optionsBuilder, config, serviceName));
            Assert.Contains(nameof(serviceName), ex2.Message);

            var ex4 = Assert.Throws<ArgumentException>(() => AzureSqlDatabaseDbContextOptionsExtensions.UseAzureSqlDatabase<GoodDbContext>(goodBuilder, config, serviceName));
            Assert.Contains(nameof(serviceName), ex4.Message);
        }

        [Fact]
        public void AddDbContext_NoVCAPs_AddsDbContext_WithSqlServerConnection()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            IConfigurationRoot config = new ConfigurationBuilder().Build();

            // Act and Assert
            services.AddDbContext<GoodDbContext>(options => options.UseAzureSqlDatabase(config));

            var service = services.BuildServiceProvider().GetService<GoodDbContext>();
            Assert.NotNull(service);
            var con = service.Database.GetDbConnection();
            Assert.NotNull(con);
            Assert.IsType<SqlConnection>(con);
        }

        [Fact]
        public void AddDbContext_WithServiceName_NoVCAPs_ThrowsConnectorException()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            IConfigurationRoot config = new ConfigurationBuilder().Build();

            // Act and Assert
            services.AddDbContext<GoodDbContext>(options => options.UseAzureSqlDatabase(config, "foobar"));

            var ex = Assert.Throws<ConnectorException>(() => services.BuildServiceProvider().GetService<GoodDbContext>());
            Assert.Contains("foobar", ex.Message);
        }

        [Theory]
        [InlineData("myAzureSqlDatabaseService1", "test_db_name_1")]
        [InlineData("myAzureSqlDatabaseService2", "test_db_name_2")]
        public void AddDbContext_WithServiceName_WithVCAPs_Returns_Specified(string serviceName, string expectedDatabaseName)
        {
            IServiceCollection services = new ServiceCollection();

            Environment.SetEnvironmentVariable("VCAP_APPLICATION", TestHelpers.VCAP_APPLICATION);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", AzureSqlDatabaseTestHelpers.TwoServiceVCAP);

            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddCloudFoundry();
            var config = builder.Build();

            // Act and Assert
            services.AddDbContext<GoodDbContext>(options => options.UseAzureSqlDatabase(config, serviceName));

            var built = services.BuildServiceProvider();
            var service = built.GetService<GoodDbContext>();
            Assert.NotNull(service);

            var con = service.Database.GetDbConnection();
            Assert.NotNull(con);
            Assert.IsType<SqlConnection>(con);

            var connString = con.ConnectionString;
            Assert.NotNull(connString);
            Assert.Contains("Database=" + expectedDatabaseName, connString);
        }

        [Fact]
        public void AddDbContext_MultipleAzureSqlDatabaseServices_ThrowsConnectorException()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            Environment.SetEnvironmentVariable("VCAP_APPLICATION", TestHelpers.VCAP_APPLICATION);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", AzureSqlDatabaseTestHelpers.TwoServiceVCAP);

            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddCloudFoundry();
            var config = builder.Build();

            // Act and Assert
            services.AddDbContext<GoodDbContext>(options =>
                  options.UseAzureSqlDatabase(config));

            var ex = Assert.Throws<ConnectorException>(() => services.BuildServiceProvider().GetService<GoodDbContext>());
            Assert.Contains("Multiple", ex.Message);
        }

        [Fact]
        public void AddDbContexts_WithVCAPs_AddsDbContexts()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            Environment.SetEnvironmentVariable("VCAP_APPLICATION", TestHelpers.VCAP_APPLICATION);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", AzureSqlDatabaseTestHelpers.SingleServiceVCAP);

            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddCloudFoundry();
            var config = builder.Build();

            // Act and Assert
            services.AddDbContext<GoodDbContext>(options => options.UseAzureSqlDatabase(config));

            var built = services.BuildServiceProvider();
            var service = built.GetService<GoodDbContext>();
            Assert.NotNull(service);

            var con = service.Database.GetDbConnection();
            Assert.NotNull(con);
            Assert.IsType<SqlConnection>(con);

            var connString = con.ConnectionString;
            Assert.NotNull(connString);
            Assert.Contains("Database=test_db_name", connString);
            Assert.Contains("Data Source=azuredbtest.database.windows.net", connString);
            Assert.Contains("User Id=testuser", connString);
            Assert.Contains("Password=testpass", connString);
            Assert.Contains("Encrypt=True", connString);
            Assert.Contains("TrustServerCertificate=True", connString);
            Assert.Contains("Connection Timeout=30", connString);
        }

        [Fact]
        public void AddDbContexts_Calls_Provided_OptionsAction()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            Environment.SetEnvironmentVariable("VCAP_APPLICATION", TestHelpers.VCAP_APPLICATION);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", AzureSqlDatabaseTestHelpers.SingleServiceVCAP);

            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddCloudFoundry();
            var config = builder.Build();

            bool actionCalled = false;

            // Act and Assert
            services.AddDbContext<GoodDbContext>(options =>
            {
                options.UseAzureSqlDatabase(config, (Action<SqlServerDbContextOptionsBuilder>)(o => actionCalled = true));
            });

            var built = services.BuildServiceProvider();

            var service = built.GetService<GoodDbContext>();
            Assert.NotNull(service);

            var con = service.Database.GetDbConnection();
            Assert.NotNull(con);
            Assert.True(actionCalled);
        }
    }
}
