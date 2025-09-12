#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using pbr = Google.Protobuf.Reflection;

/// <summary>
/// Each protocol version identify new features that are
/// incrementally added to the protocol
/// </summary>
public enum ProtocolVersion {
    /// <summary>
    /// Initial versioning
    /// </summary>
    [pbr::OriginalName("v0")] V0 = 0,
    /// <summary>
    /// Added application keep-alive
    /// </summary>
    [pbr::OriginalName("v1")] V1 = 1,
    /// <summary>
    /// Added RedeliverUnacknowledgedMessages Command
    /// </summary>
    [pbr::OriginalName("v2")] V2 = 2,
    /// <summary>
    /// Added compression with LZ4 and ZLib
    /// </summary>
    [pbr::OriginalName("v3")] V3 = 3,
    /// <summary>
    /// Added batch message support
    /// </summary>
    [pbr::OriginalName("v4")] V4 = 4,
    /// <summary>
    /// Added disconnect client w/o closing connection
    /// </summary>
    [pbr::OriginalName("v5")] V5 = 5,
    /// <summary>
    /// Added checksum computation for metadata + payload
    /// </summary>
    [pbr::OriginalName("v6")] V6 = 6,
    /// <summary>
    /// Added CommandLookupTopic - Binary Lookup
    /// </summary>
    [pbr::OriginalName("v7")] V7 = 7,
    /// <summary>
    /// Added CommandConsumerStats - Client fetches broker side consumer stats
    /// </summary>
    [pbr::OriginalName("v8")] V8 = 8,
    /// <summary>
    /// Added end of topic notification
    /// </summary>
    [pbr::OriginalName("v9")] V9 = 9,
    /// <summary>
    /// Added proxy to broker
    /// </summary>
    [pbr::OriginalName("v10")] V10 = 10,
    /// <summary>
    /// C++ consumers before this version are not correctly handling the checksum field
    /// </summary>
    [pbr::OriginalName("v11")] V11 = 11,
    /// <summary>
    /// Added get topic's last messageId from broker
    /// </summary>
    [pbr::OriginalName("v12")] V12 = 12,
    /// <summary>
    /// Added CommandActiveConsumerChange
    /// Added CommandGetTopicsOfNamespace
    /// </summary>
    [pbr::OriginalName("v13")] V13 = 13,
    /// <summary>
    /// Add CommandAuthChallenge and CommandAuthResponse for mutual auth
    /// </summary>
    [pbr::OriginalName("v14")] V14 = 14,
    /// <summary>
    /// Added Key_Shared subscription
    /// </summary>
    [pbr::OriginalName("v15")] V15 = 15,
    /// <summary>
    /// Add support for broker entry metadata
    /// </summary>
    [pbr::OriginalName("v16")] V16 = 16,
    /// <summary>
    /// Added support ack receipt
    /// </summary>
    [pbr::OriginalName("v17")] V17 = 17,
    /// <summary>
    /// Add client support for broker entry metadata
    /// </summary>
    [pbr::OriginalName("v18")] V18 = 18,
    /// <summary>
    /// Add CommandTcClientConnectRequest and CommandTcClientConnectResponse
    /// </summary>
    [pbr::OriginalName("v19")] V19 = 19,
    /// <summary>
    /// Add client support for topic migration redirection CommandTopicMigrated
    /// </summary>
    [pbr::OriginalName("v20")] V20 = 20,
    /// <summary>
    /// Carry the AUTO_CONSUME schema to the Broker after this version
    /// </summary>
    [pbr::OriginalName("v21")] V21 = 21,
}
