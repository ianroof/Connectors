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

namespace Steeltoe.CloudFoundry.Connector.Services
{
    public class AzureSqlDatabaseServiceInfoFactory : ServiceInfoFactory
    {
        public AzureSqlDatabaseServiceInfoFactory()
            : base(new Tags("azure-sqldb"), AzureSqlDatabaseServiceInfo.SQLSERVER_SCHEME)
        {
        }

        public AzureSqlDatabaseServiceInfoFactory(Tags tags, string scheme)
            : base(tags, scheme)
        {
        }

        public AzureSqlDatabaseServiceInfoFactory(Tags tags, string[] schemes)
            : base(tags, schemes)
        {
        }

        public override bool Accept(Service binding)
        {
            return TagsMatch(binding) || LabelStartsWithTag(binding);
        }

        public override IServiceInfo Create(Service binding)
        {
            string uri = GetUriFromCredentials(binding.Credentials);
            string username = GetUsernameFromCredentials(binding.Credentials);
            string password = GetPasswordFromCredentials(binding.Credentials);

            if (uri == null)
            {
                string host = GetHostFromCredentials(binding.Credentials);
                int port = GetPortFromCredentials(binding.Credentials);

                string database = GetStringFromCredentials(binding.Credentials, "name");

                if (host != null)
                {
                    uri = new UriInfo(DefaultUriScheme, host, port, username, password, database).ToString();
                }
            }

            return Create(binding.Name, uri, username, password);
        }

        public IServiceInfo Create(string id, string url, string username, string password)
        {
            if (username == null && password == null)
            {
                return new AzureSqlDatabaseServiceInfo(id, url);
            }
            else
            {
                return new AzureSqlDatabaseServiceInfo(id, url, username, password);
            }
        }
    }
}
