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

namespace DotPulsar.IntegrationTests.Abstraction;

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

/// <summary>
/// Pulsar Service interface
/// </summary>
public interface IPulsarService : IAsyncLifetime
{
    /// <summary>
    /// Get broker binary protocol uri
    /// </summary>
    Uri GetBrokerUri();

    /// <summary>
    /// Get broker rest uri
    /// </summary>
    Uri GetWebServiceUri();

    /// <summary>
    /// Create a partitioned topic
    /// The format of the restTopic must be `{schema}/{tenant}/{namespace}/{topicName}`
    /// For example, `persistent/public/default/test-topic`
    /// </summary>
    Task<HttpResponseMessage?> CreatePartitionedTopic(string restTopic, int numPartitions);
}
