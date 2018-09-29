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

namespace Steeltoe.CloudFoundry.Connector.AzureSqlDatabase.EF6.Test
{
    public class AzureSqlDatabaseTestHelpers
    {
        public static string SingleServiceVCAP = @"
        {
            'azure-sqldb': [
                {
                    'credentials': {
                        'username': 'testuser',
                        'uri': 'mssql://testuser:testpass@azuredbtest.database.windows.net:1433/test_db_name?encrypt=true&TrustServerCertificate=true&Connection%20Timeout=30',
                        'sqlDbName': 'test_db_name',
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

        public static string TwoServiceVCAP = @"
        {
            'azure-sqldb': [
                {
                    'credentials': {
                        'username': 'testuser',
                        'uri': 'mssql://localhost:1433/test_db_name_1',
                        'sqlDbName': 'test_db_name_1',
                        'password': 'testpass'
                    },
                    'syslog_drain_url': null,
                    'label': 'azure-sqldb',
                    'provider': null,
                    'plan': 'basic',
                    'name': 'myAzureSqlDatabaseService1',
                    'tags': [
                        'azure-sqldb'
                    ]
                },
                {
                    'credentials': {
                        'username': 'testuser',
                        'uri': 'mssql://localhost:1433/test_db_name_2',
                        'sqlDbName': 'test_db_name_2',
                        'password': 'testpass'
                    },
                    'syslog_drain_url': null,
                    'label': 'azure-sqldb',
                    'provider': null,
                    'plan': 'basic',
                    'name': 'myAzureSqlDatabaseService2',
                    'tags': [
                        'azure-sqldb'
                    ]
                },
            ]
        }";
    }
}
