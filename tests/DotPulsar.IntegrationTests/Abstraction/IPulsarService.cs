namespace DotPulsar.IntegrationTests.Abstraction
{
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
        Task<HttpResponseMessage?> CreatePartitionedProducer(string restTopic, int numPartitions);
    }
}
