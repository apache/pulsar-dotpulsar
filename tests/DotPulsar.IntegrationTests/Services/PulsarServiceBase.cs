/*
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

using Abstraction;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class PulsarServiceBase : IPulsarService
{
    private readonly CancellationTokenSource _cts;

    protected PulsarServiceBase()
    {
        _cts = new CancellationTokenSource();
    }

    public virtual Task InitializeAsync()
        => Task.CompletedTask;

    public virtual Task DisposeAsync()
    {
        _cts.Dispose();
        return Task.CompletedTask;
    }

    public virtual Uri GetBrokerUri()
        => throw new NotImplementedException();

    public virtual Uri GetWebServiceUri()
        => throw new NotImplementedException();

    public async Task CreatePartitionedTopic(string restTopic, int numPartitions, string? token = null)
    {
        using var adminClient = new HttpClient();
        if (token != null)
        {
            adminClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        var content = new StringContent(numPartitions.ToString(), Encoding.UTF8, "application/json");
        var response = await adminClient.PutAsync($"{GetWebServiceUri()}admin/v2/{restTopic}/partitions", content, _cts.Token).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }
}
