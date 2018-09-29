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

using Xunit;

namespace Steeltoe.CloudFoundry.Connector.Services.Test
{
    public class AzureSqlDatabaseServiceInfoTest
    {
        [Fact]
        public void Constructor_CreatesExpected()
        {
            string uri = "mssql://localhost:1433;databaseName=test_db_name";
            SqlServerServiceInfo r1 = new SqlServerServiceInfo("myId", uri, "testuser", "testpass");

            Assert.Equal("myId", r1.Id);
            Assert.Equal("mssql", r1.Scheme);
            Assert.Equal("localhost", r1.Host);
            Assert.Equal(1433, r1.Port);
            Assert.Equal("testpass", r1.Password);
            Assert.Equal("testuser", r1.UserName);
            Assert.Equal("databaseName=test_db_name", r1.Path);
        }
    }
}
