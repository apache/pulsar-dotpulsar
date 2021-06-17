namespace DotPulsar.IntegrationTests.Services
{
    using Abstraction;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public sealed class StandaloneExternalService : PulsarServiceBase
    {
        public override Uri GetBrokerUri()
            => new ("pulsar://localhost:6650");

        public override Uri GetWebServiceUri()
            => new("http://localhost:8080");
    }
}
