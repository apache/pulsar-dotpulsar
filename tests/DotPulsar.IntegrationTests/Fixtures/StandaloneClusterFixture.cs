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

namespace DotPulsar.IntegrationTests.Fixtures;

using Abstraction;
using Services;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

public class StandaloneClusterFixture : IAsyncLifetime
{
    private readonly IMessageSink _messageSink;

    public StandaloneClusterFixture(IMessageSink messageSink)
    {
        _messageSink = messageSink;
    }

    public IPulsarService? PulsarService { private set; get; }

    public async Task InitializeAsync()
    {
        PulsarService = ServiceFactory.CreatePulsarService(_messageSink);
        await PulsarService.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        if (PulsarService != null)
            await PulsarService.DisposeAsync();
    }
}
