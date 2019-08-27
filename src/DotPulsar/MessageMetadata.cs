namespace DotPulsar
{
    public sealed class MessageMetadata
    {
        public MessageMetadata() => Metadata = new Internal.PulsarApi.MessageMetadata();

        internal Internal.PulsarApi.MessageMetadata Metadata;

        public long DeliverAtTime
        {
            get => Metadata.DeliverAtTime;
            set => Metadata.DeliverAtTime = value;
        }

        public ulong EventTime
        {
            get => Metadata.EventTime;
            set => Metadata.EventTime = value;
        }

        public string this[string key]
        {
            get
            {
                for (var i = 0; i < Metadata.Properties.Count; ++i)
                {
                    var keyValye = Metadata.Properties[i];
                    if (keyValye.Key == key)
                        return keyValye.Value;
                }

                return null;
            }
            set
            {
                for (var i = 0; i < Metadata.Properties.Count; ++i)
                {
                    var keyValye = Metadata.Properties[i];
                    if (keyValye.Key != key)
                        continue;
                    keyValye.Value = value;
                    return;
                }

                Metadata.Properties.Add(new Internal.PulsarApi.KeyValue { Key = key, Value = value });
            }
        }

        public ulong SequenceId
        {
            get => Metadata.SequenceId;
            set => Metadata.SequenceId = value;
        }
    }
}
