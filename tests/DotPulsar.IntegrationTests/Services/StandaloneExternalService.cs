﻿/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace DotPulsar.IntegrationTests.Services;

using System;
using System.Threading.Tasks;
using Xunit.Abstractions;

public sealed class StandaloneExternalService : PulsarServiceBase
{
    public StandaloneExternalService(IMessageSink messageSink) : base(messageSink) { }
    public override Task InitializeAsync() => Task.CompletedTask;

    public override Uri GetBrokerUri()
        => new("pulsar://localhost:6650");

    public override Uri GetWebServiceUri()
        => new("http://localhost:8080");
}
