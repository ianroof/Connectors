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

using Steeltoe.CloudFoundry.Connector.Services;
using System.Collections.Generic;
using Xunit;

namespace Steeltoe.CloudFoundry.Connector.Test.Services
{
    public class HystrixRabbitMQServiceInfoTest
    {
        [Fact]
        public void Constructor_CreatesExpected()
        {
            string uri = "amqp://03c7a684-6ff1-4bd0-ad45-d10374ffb2af:l5oq2q0unl35s6urfsuib0jvpo@192.168.0.81:5672/fb03d693-91fe-4dc5-8203-ff7a6390df66";
            List<string> uris = new List<string>() { "amqp://03c7a684-6ff1-4bd0-ad45-d10374ffb2af:l5oq2q0unl35s6urfsuib0jvpo@192.168.0.81:5672/fb03d693-91fe-4dc5-8203-ff7a6390df66" };

            // string managementUri = "https://03c7a684-6ff1-4bd0-ad45-d10374ffb2af:l5oq2q0unl35s6urfsuib0jvpo@pivotal-rabbitmq.system.testcloud.com/api/";
            // List<string> managementUris = new List<string>() { "https://03c7a684-6ff1-4bd0-ad45-d10374ffb2af:l5oq2q0unl35s6urfsuib0jvpo@pivotal-rabbitmq.system.testcloud.com/api/" };
            bool isSSLEnabled = false;

            HystrixRabbitMQServiceInfo r1 = new HystrixRabbitMQServiceInfo("myId", uri, isSSLEnabled);
            HystrixRabbitMQServiceInfo r2 = new HystrixRabbitMQServiceInfo("myId", uri, uris, isSSLEnabled);

            Assert.Equal("myId", r1.Id);
            Assert.Equal("amqp", r1.Scheme);
            Assert.Equal("192.168.0.81", r1.Host);
            Assert.Equal(5672, r1.Port);
            Assert.Equal("l5oq2q0unl35s6urfsuib0jvpo", r1.Password);
            Assert.Equal("03c7a684-6ff1-4bd0-ad45-d10374ffb2af", r1.UserName);
            Assert.Equal("fb03d693-91fe-4dc5-8203-ff7a6390df66", r1.Path);
            Assert.Null(r1.Query);
            Assert.Null(r1.Uris);
            Assert.Equal(uri, r1.Uri);

            Assert.Equal("myId", r2.Id);
            Assert.Equal("amqp", r2.Scheme);
            Assert.Equal("192.168.0.81", r2.Host);
            Assert.Equal(5672, r2.Port);
            Assert.Equal("l5oq2q0unl35s6urfsuib0jvpo", r2.Password);
            Assert.Equal("03c7a684-6ff1-4bd0-ad45-d10374ffb2af", r2.UserName);
            Assert.Equal("fb03d693-91fe-4dc5-8203-ff7a6390df66", r2.Path);
            Assert.Null(r2.Query);
            Assert.Equal(uris[0], r2.Uris[0]);
            Assert.Equal(uri, r2.Uri);
        }
    }
}
