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
using System.Net;

namespace Steeltoe.CloudFoundry.Connector.AzureSqlDatabase
{
    public class AzureSqlDatabaseProviderConfigurer
    {
        public string Configure(AzureSqlDatabaseServiceInfo si, AzureSqlDatabaseProviderConnectorOptions configuration)
        {
            UpdateConfiguration(si, configuration);
            return configuration.ToString();
        }

        public void UpdateConfiguration(AzureSqlDatabaseServiceInfo si, AzureSqlDatabaseProviderConnectorOptions configuration)
        {
            if (si == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(si.Uri))
            {
                configuration.Port = si.Port;
                configuration.HostName = si.Host;
                configuration.SqlDbName = si.Path.Replace("databaseName=", string.Empty);
                configuration.Username = si.UserName;
                configuration.Password = si.Password;
                configuration.ExtractValuesFromQueryString(si.Query);
            }
        }
    }
}
