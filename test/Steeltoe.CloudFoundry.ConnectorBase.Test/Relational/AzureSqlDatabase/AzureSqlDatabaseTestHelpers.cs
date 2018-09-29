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

namespace Steeltoe.CloudFoundry.Connector.AzureSqlDatabase.Test
{
    public class AzureSqlDatabaseTestHelpers
    {
        public static string SingleServerVCAP = @"
        {
            'azure-sqldb': [
                {
                    'credentials': {
                        'username': 'testuser',
                        'uri': 'mssql://testuser:testpass@azuredbtest.database.windows.net:1433/test_db_name?encrypt=true&TrustServerCertificate=true&Connection%20Timeout=30',
                        'sqldbName': 'test_db_name',
                        'password': 'testpass'
                    },
                    'syslog_drain_url': null,
                    'label': 'azure-sqldb',
                    'provider': null,
                    'plan': 'Basic',
                    'name': 'myAzureSqlDatabaseService',
                    'tags': [
                        'azure-sqldb'
                    ]
                },
            ]
        }";

        public static string SingleServerVCAPNoTag = @"
        {
            'azure-sqldb': [
                {
                    'credentials': {
                        'username': 'uf33b2b30783a4087948c30f6c3b0c90f',
                        'uri': 'mssql://localhost:1433/test_db_name',
                        'sqldbName': 'test_db_name',
                        'password': 'Pefbb929c1e0945b5bab5b8f0d110c503'
                    },
                    'syslog_drain_url': null,
                    'label': 'azure-sqldb',
                    'provider': null,
                    'plan': 'Basic',
                    'name': 'myAzureSqlDatabaseService',
                    'tags': [
                    ]
                },
            ]
        }";

        public static string SingleServerVCAPIgnoreName = @"
        {
            'user-provided': [
                {
                    'name': 'azure-sql-database-config-user-provided-service',
                    'instance_name': 'azure-sql-database-config-user-provided-service',
                    'binding_name': null,
                    'credentials': {
                        'uri': 'mssql://localhost:1433;databaseName=test_db_name',
                        'sqldbName': 'test_db_name',
                        'username': '',
                        'password': ''
                    },
                    'syslog_drain_url': '',
                    'volume_mounts': [],
                    'label': 'user-provided',
                    'tags': []
            }
            ]
        }";

        public static string SingleServerVCAP_CredsInUrl = @"
        {
            'azure-sqldb': [
                {
                    'credentials': {
                        'uri': 'mssql://testuser:testpass@azuredbtest.database.windows.net:1433/test_db_name?encrypt=true&TrustServerCertificate=true&Connection%20Timeout=30',
                        'sqldbName': 'test_db_name',
                    },
                    'syslog_drain_url': null,
                    'label': 'azure-sqldb',
                    'provider': null,
                    'plan': 'Basic',
                    'name': 'myAzureSqlDatabaseService',
                    'tags': [
                        'azure-sqldb'
                    ]
                },
            ]
        }";

        public static string TwoServerVCAP = @"
        {
            'azure-sqldb': [
                {
                    'credentials': {
                        'username': 'testuser',
                        'uri': 'mssql://localhost:1433/test_db_1',
                        'sqldbName': 'test_db_1',
                        'password': 'testpass'
                    },
                    'syslog_drain_url': null,
                    'label': 'azure-sqldb',
                    'provider': null,
                    'plan': 'sharedVM',
                    'name': 'myAzureSqlDatabaseService',
                    'tags': [
                        'azure-sqldb'
                    ]
                },
                {
                    'credentials': {
                        'username': 'testuser',
                        'uri': 'mssql://localhost:1433/test_db_2',
                        'sqldbName': 'test_db_2',
                        'password': 'testpass'
                    },
                    'syslog_drain_url': null,
                    'label': 'azure-sqldb',
                    'provider': null,
                    'plan': 'basic',
                    'name': 'myAzureSqlDatabaseService',
                    'tags': [
                        'azure-sqldb'
                    ]
                },
            ]
        }";
    }
}
