﻿// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using k8s;
using Microsoft.Rest;
using Moq;
using RichardSzalay.MockHttp;
using System;
using System.Net;
using System.Net.Http;
using Xunit;

namespace Steeltoe.Extensions.Configuration.Kubernetes.Test
{
    public class KubernetesSecretProviderTest
    {
        [Fact]
        public void KubernetesSecretProvider_ThrowsOnNulls()
        {
            // arrange
            var client = new Mock<k8s.Kubernetes>();
            var settings = new KubernetesConfigSourceSettings("default", "test");

            // act
            var ex1 = Assert.Throws<ArgumentNullException>(() => new KubernetesSecretProvider(null, settings));
            var ex2 = Assert.Throws<ArgumentNullException>(() => new KubernetesSecretProvider(client.Object, null));

            // assert
            Assert.Equal("kubernetes", ex1.ParamName);
            Assert.Equal("settings", ex2.ParamName);
        }

        [Fact]
        public void KubernetesSecretProvider_ThrowsOn403()
        {
            // arrange
            var mockHttpMessageHandler = new MockHttpMessageHandler();
            mockHttpMessageHandler.Expect(HttpMethod.Get, "*").Respond(HttpStatusCode.Forbidden);

            var client = new k8s.Kubernetes(new KubernetesClientConfiguration { Host = "http://localhost" }, httpClient: mockHttpMessageHandler.ToHttpClient());
            var settings = new KubernetesConfigSourceSettings("default", "test");
            var provider = new KubernetesSecretProvider(client, settings);

            // act
            var ex = Assert.Throws<HttpOperationException>(() => provider.Load());

            // assert
            Assert.Equal(HttpStatusCode.Forbidden, ex.Response.StatusCode);
        }

        [Fact]
        public void KubernetesSecretProvider_AddsToDictionaryOnSuccess()
        {
            // arrange
            var mockHttpMessageHandler = new MockHttpMessageHandler();
            mockHttpMessageHandler
                .Expect(HttpMethod.Get, "*")
                .Respond(new StringContent("{\"kind\":\"SecretList\",\"apiVersion\":\"v1\",\"metadata\":{\"selfLink\":\"/api/v1/namespaces/default/secrets\",\"resourceVersion\":\"3007004\"},\"items\":[{\"metadata\":{\"name\":\"testsecret\",\"namespace\":\"default\",\"selfLink\":\"/api/v1/namespaces/default/secrets/testsecret\",\"uid\":\"04a256d5-5480-4e6a-ab1a-81b1df2b1f15\",\"resourceVersion\":\"724153\",\"creationTimestamp\":\"2020-04-17T14:32:42Z\",\"annotations\":{\"kubectl.kubernetes.io/last-applied-configuration\":\"{\\\"apiVersion\\\":\\\"v1\\\",\\\"data\\\":{\\\"testKey\\\":\\\"dGVzdFZhbHVl\\\"},\\\"kind\\\":\\\"Secret\\\",\\\"metadata\\\":{\\\"annotations\\\":{},\\\"name\\\":\\\"testsecret\\\",\\\"namespace\\\":\\\"default\\\"},\\\"type\\\":\\\"Opaque\\\"}\\n\"}},\"data\":{\"testKey\":\"dGVzdFZhbHVl\"},\"type\":\"Opaque\"}]}\n"));

            var client = new k8s.Kubernetes(new KubernetesClientConfiguration() { Host = "http://localhost" }, httpClient: mockHttpMessageHandler.ToHttpClient());
            var settings = new KubernetesConfigSourceSettings("default", "testsecret", true);
            var provider = new KubernetesSecretProvider(client, settings);

            // act
            provider.Load();

            // assert
            Assert.True(provider.TryGet("testKey", out var testValue));
            Assert.Equal("testValue", testValue);
        }
    }
}