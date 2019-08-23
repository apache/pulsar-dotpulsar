using System;

namespace DotPulsar.Abstractions
{
    /// <summary>
    /// A pulsar client building abstraction
    /// </summary>
    public interface IPulsarClientBuilder
    {
        /// <summary>
        /// The service URL for the Pulsar cluster
        /// </summary>
        IPulsarClientBuilder ServiceUrl(Uri uri);

        /// <summary>
        /// Create the client
        /// </summary>
        IPulsarClient Build();
    }
}
