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

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Steeltoe.CloudFoundry.Connector.Services;
using System;
using System.Reflection;

namespace Steeltoe.CloudFoundry.Connector.AzureSqlDatabase.EFCore
{
    public static class AzureSqlDatabaseDbContextOptionsExtensions
    {
        private static string[] sqlServerEntityAssemblies = new string[] { "Microsoft.EntityFrameworkCore.SqlServer" };

        private static string[] sqlServerEntityTypeNames = new string[] { "Microsoft.EntityFrameworkCore.SqlServerDbContextOptionsExtensions" };

        public static DbContextOptionsBuilder UseAzureSqlDatabase(this DbContextOptionsBuilder optionsBuilder, IConfiguration config, object azureSqlDatabaseOptionsAction = null)
        {
            if (optionsBuilder == null)
            {
                throw new ArgumentNullException(nameof(optionsBuilder));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var connection = GetConnection(config);

            return DoUseAzureSqlDatabase(optionsBuilder, connection, azureSqlDatabaseOptionsAction);
        }

        public static DbContextOptionsBuilder UseAzureSqlDatabase(this DbContextOptionsBuilder optionsBuilder, IConfiguration config, string serviceName, object azureSqlDatabaseOptionsAction = null)
        {
            if (optionsBuilder == null)
            {
                throw new ArgumentNullException(nameof(optionsBuilder));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentException(nameof(serviceName));
            }

            var connection = GetConnection(config, serviceName);

            return DoUseAzureSqlDatabase(optionsBuilder, connection, azureSqlDatabaseOptionsAction);
        }

        public static DbContextOptionsBuilder<TContext> UseAzureSqlDatabase<TContext>(this DbContextOptionsBuilder<TContext> optionsBuilder, IConfiguration config, object azureSqlDatabaseOptionsAction = null)
            where TContext : DbContext
        {
            if (optionsBuilder == null)
            {
                throw new ArgumentNullException(nameof(optionsBuilder));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var connection = GetConnection(config);

            return DoUseAzureSqlDatabase<TContext>(optionsBuilder, connection, azureSqlDatabaseOptionsAction);
        }

        public static DbContextOptionsBuilder<TContext> UseAzureSqlDatabase<TContext>(this DbContextOptionsBuilder<TContext> optionsBuilder, IConfiguration config, string serviceName, object azureSqlDatabaseOptionsAction = null)
            where TContext : DbContext
        {
            if (optionsBuilder == null)
            {
                throw new ArgumentNullException(nameof(optionsBuilder));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentException(nameof(serviceName));
            }

            var connection = GetConnection(config, serviceName);

            return DoUseAzureSqlDatabase<TContext>(optionsBuilder, connection, azureSqlDatabaseOptionsAction);
        }

        private static MethodInfo FindUseSqlMethod(Type type, Type[] parameterTypes)
        {
            var typeInfo = type.GetTypeInfo();
            var declaredMethods = typeInfo.DeclaredMethods;

            foreach (MethodInfo ci in declaredMethods)
            {
                var parameters = ci.GetParameters();
                if (parameters.Length == 3 &&
                    ci.Name.Equals("UseSqlServer", StringComparison.InvariantCultureIgnoreCase) &&
                    parameters[0].ParameterType.Equals(parameterTypes[0]) &&
                    parameters[1].ParameterType.Equals(parameterTypes[1]) &&
                    ci.IsPublic && ci.IsStatic)
                {
                    return ci;
                }
            }

            return null;
        }

        private static string GetConnection(IConfiguration config, string serviceName = null)
        {
            AzureSqlDatabaseServiceInfo info = null;
            if (string.IsNullOrEmpty(serviceName))
            {
                info = config.GetSingletonServiceInfo<AzureSqlDatabaseServiceInfo>();
            }
            else
            {
                info = config.GetRequiredServiceInfo<AzureSqlDatabaseServiceInfo>(serviceName);
            }

            AzureSqlDatabaseProviderConnectorOptions sqlServerConfig = new AzureSqlDatabaseProviderConnectorOptions(config);

            AzureSqlDatabaseProviderConnectorFactory factory = new AzureSqlDatabaseProviderConnectorFactory(info, sqlServerConfig, null);

            return factory.CreateConnectionString();
        }

        private static DbContextOptionsBuilder DoUseAzureSqlDatabase(DbContextOptionsBuilder builder, string connection, object azureSqlDatabaseOptionsAction = null)
        {
            Type extensionType = ConnectorHelpers.FindType(sqlServerEntityAssemblies, sqlServerEntityTypeNames);
            if (extensionType == null)
            {
                throw new ConnectorException("Unable to find DbContextOptionsBuilder extension, are you missing SqlServer EntityFramework Core assembly");
            }

            MethodInfo useMethod = FindUseSqlMethod(extensionType, new Type[] { typeof(DbContextOptionsBuilder), typeof(string) });
            if (extensionType == null)
            {
                throw new ConnectorException("Unable to find UseSqlServer extension, are you missing SqlServer EntityFramework Core assembly");
            }

            object result = ConnectorHelpers.Invoke(useMethod, null, new object[] { builder, connection, azureSqlDatabaseOptionsAction });
            if (result == null)
            {
                throw new ConnectorException(string.Format("Failed to invoke UseSqlServer extension, connection: {0}", connection));
            }

            return (DbContextOptionsBuilder)result;
        }

        private static DbContextOptionsBuilder<TContext> DoUseAzureSqlDatabase<TContext>(DbContextOptionsBuilder<TContext> builder, string connection, object azureSqlDatabaseOptionsAction = null)
            where TContext : DbContext
        {
            return (DbContextOptionsBuilder<TContext>)DoUseAzureSqlDatabase((DbContextOptionsBuilder)builder, connection, azureSqlDatabaseOptionsAction);
        }
    }
}
