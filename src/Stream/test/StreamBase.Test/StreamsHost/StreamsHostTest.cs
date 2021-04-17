﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Steeltoe.Messaging.RabbitMQ.Config;
using Steeltoe.Stream.Attributes;
using Steeltoe.Stream.Config;
using Steeltoe.Stream.Messaging;
using System;
using System.ComponentModel;
using Xunit;

namespace Steeltoe.Stream.StreamsHost
{
    public class StreamsHostTest
    {
        [Fact]
        public void HostCanBeStarted()
        {
            FakeHostedService service;
            using (var host = StreamsHost.CreateDefaultBuilder<SampleSink>()
                .ConfigureServices(svc => svc.AddSingleton<IHostedService, FakeHostedService>())
                .Start())
            {
                Assert.NotNull(host);
                service = (FakeHostedService)host.Services.GetRequiredService<IHostedService>();
                Assert.NotNull(service);
                Assert.Equal(1, service.StartCount);
                Assert.Equal(0, service.StopCount);
                Assert.Equal(0, service.DisposeCount);
            }

            Assert.Equal(1, service.StartCount);
            Assert.Equal(0, service.StopCount);
            Assert.Equal(1, service.DisposeCount);
        }

        [Fact]
        public void HostSetsupSpringBootConfigSource()
        {
            Environment.SetEnvironmentVariable("SPRING_APPLICATION_JSON", "{\"spring.cloud.stream.bindings.input.destination\":\"foobar\",\"spring.cloud.stream.bindings.output.destination\":\"barfoo\"}");
            using (var host = StreamsHost.CreateDefaultBuilder<SampleSink>()
                .Start())
            {
                var rabbitBindingsOptions = host.Services.GetService<IOptionsMonitor<BindingServiceOptions>>();
                Assert.NotNull(rabbitBindingsOptions);
                Assert.Equal("foobar", rabbitBindingsOptions.CurrentValue.Bindings["input"].Destination);
                Assert.Equal("barfoo", rabbitBindingsOptions.CurrentValue.Bindings["output"].Destination);
            }
        }

        [Fact]
        [Trait("Category", "SkipOnMacOS")]
        public void HostConfiguresRabbitOptions()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            Environment.SetEnvironmentVariable("VCAP_APPLICATION", TestHelpers.VCAP_APPLICATION);
            Environment.SetEnvironmentVariable("VCAP_SERVICES", GetCloudFoundryRabbitMqConfiguration());
            using (var host = StreamsHost
                .CreateDefaultBuilder<SampleSink>()
                .ConfigureAppConfiguration(c => c.AddCloudFoundry())
                .Start())
            {
                var rabbitOptionsMonitor = host.Services.GetService<IOptionsMonitor<RabbitOptions>>();
                Assert.NotNull(rabbitOptionsMonitor);
                var rabbitOptions = rabbitOptionsMonitor.CurrentValue;

                Assert.Equal("Dd6O1BPXUHdrmzbP", rabbitOptions.Username);
                Assert.Equal("7E1LxXnlH2hhlPVt", rabbitOptions.Password);
                Assert.Equal("cf_b4f8d2fa_a3ea_4e3a_a0e8_2cd040790355", rabbitOptions.VirtualHost);
                Assert.Equal($"Dd6O1BPXUHdrmzbP:7E1LxXnlH2hhlPVt@192.168.0.90:3306", rabbitOptions.Addresses);
            }
        }

        private static string GetCloudFoundryRabbitMqConfiguration() => @"
        {
            ""p-rabbitmq"": [{
                ""credentials"": {
                    ""uri"": ""amqp://Dd6O1BPXUHdrmzbP:7E1LxXnlH2hhlPVt@192.168.0.90:3306/cf_b4f8d2fa_a3ea_4e3a_a0e8_2cd040790355""
                },
                ""syslog_drain_url"": null,
                ""label"": ""p-rabbitmq"",
                ""provider"": null,
                ""plan"": ""standard"",
                ""name"": ""myRabbitMQService1"",
                ""tags"": [
                    ""rabbitmq"",
                    ""amqp""
                ]
            }]
        }";
    }

    [EnableBinding(typeof(ISink))]
    public class SampleSink
    {
        [StreamListener("input")]
        public void HandleInputMessage(string foo)
        {
        }
    }
}