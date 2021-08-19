namespace DotPulsar.Abstractions
{
    using Internal.PulsarApi;
    using System;

    /// <summary>
    /// The interface for support multiple or single message id.
    /// </summary>
    public interface IMessageId : IComparable
    {
        public string Topic { get; }
        public MessageIdData ToMessageIdData();
    }
}
