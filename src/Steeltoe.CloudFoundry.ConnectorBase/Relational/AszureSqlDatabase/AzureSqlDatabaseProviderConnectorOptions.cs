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
using System;
using System.Net;
using System.Text;

namespace Steeltoe.CloudFoundry.Connector.AzureSqlDatabase
{
    public class AzureSqlDatabaseProviderConnectorOptions : AbstractServiceConnectorOptions
    {
        public const string Default_Server = ".";
        public const int Default_Port = 1433;
        private const string AZURE_SQL_DATABASE_CREDENTIALS_SECTION_PREFIX = "azure-sqldb:credentials";

        public AzureSqlDatabaseProviderConnectorOptions()
        {
        }

        public AzureSqlDatabaseProviderConnectorOptions(IConfiguration config)
            : base()
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var section = config.GetSection(AZURE_SQL_DATABASE_CREDENTIALS_SECTION_PREFIX);
            section.Bind(this);

            if (!string.IsNullOrEmpty(Uri))
            {
                ExtractValuesFromQueryString(new UriBuilder(Uri).Query);
            }
        }

        public string SqlDbName { get; set; }

        public string HostName { get; set; } = Default_Server;

        public int Port { get; set; } = Default_Port;

        public string Username { get; set; }

        public string Password { get; set; }

        public string Uri { get; set; }

        public bool Encrypt { get; set; }

        public int? ConnectionTimeout { get; set; }

        public bool TrustServerCertificate { get; set; }

        public void ExtractValuesFromQueryString(string queryString)
        {
            if (string.IsNullOrEmpty(queryString))
            {
                return;
            }

            string[] queryParams = queryString.TrimStart('?').Split('&');
            foreach (var queryParam in queryParams)
            {
                var keyValue = queryParam.Split('=');
                if (keyValue.Length == 2)
                {
                    var key = WebUtility.UrlDecode(keyValue[0].ToUpperInvariant());
                    if (key == "ENCRYPT")
                    {
                        Encrypt = bool.Parse(keyValue[1]);
                    }
                    else if (key == "CONNECTION TIMEOUT")
                    {
                        ConnectionTimeout = int.Parse(keyValue[1]);
                    }
                    else if (key == "TRUSTSERVERCERTIFICATE")
                    {
                        TrustServerCertificate = bool.Parse(keyValue[1]);
                    }
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AddKeyValue(sb, "Data Source", $"{HostName},{Port}");
            AddKeyValue(sb, "Database", SqlDbName);
            AddKeyValue(sb, "User Id", Username);
            AddKeyValue(sb, "Password", Password);
            AddKeyValue(sb, "Encrypt", Encrypt.ToString());
            AddKeyValue(sb, "TrustServerCertificate", TrustServerCertificate.ToString());
            AddKeyValue(sb, "Connection Timeout", ConnectionTimeout);

            return sb.ToString();
        }
    }
}
