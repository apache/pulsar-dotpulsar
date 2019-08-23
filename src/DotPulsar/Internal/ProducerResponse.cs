namespace DotPulsar.Internal
{
    public sealed class ProducerResponse
    {
        public ProducerResponse(ulong producerId, string producerName)
        {
            ProducerId = producerId;
            ProducerName = producerName;
        }

        public ulong ProducerId { get; }
        public string ProducerName { get; }
    }
}
