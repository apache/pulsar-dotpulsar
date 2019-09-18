using System;

namespace DotPulsar.Abstractions
{
    /// <summary>
    /// A pulsar client building abstraction.
    /// </summary>
    public interface IPulsarClientBuilder
    {
        /// <summary>
        /// The time to wait before retrying an operation or a reconnect. The default is 3 seconds.
        /// </summary>
        IPulsarClientBuilder RetryInterval(TimeSpan interval);

        /// <summary>
        /// The service URL for the Pulsar cluster. The default is "pulsar://localhost:6650".
        /// </summary>
        IPulsarClientBuilder ServiceUrl(Uri uri);

        /// <summary>
        /// Create the client.
        /// </summary>
        IPulsarClient Build();
    }
}
