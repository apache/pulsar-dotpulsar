#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using pbr = Google.Protobuf.Reflection;

public enum ProducerAccessMode {
    /// <summary>
    /// By default multiple producers can publish on a topic
    /// </summary>
    [pbr::OriginalName("Shared")] Shared = 0,
    /// <summary>
    /// Require exclusive access for producer. Fail immediately if there's already a producer connected.
    /// </summary>
    [pbr::OriginalName("Exclusive")] Exclusive = 1,
    /// <summary>
    /// Producer creation is pending until it can acquire exclusive access
    /// </summary>
    [pbr::OriginalName("WaitForExclusive")] WaitForExclusive = 2,
    /// <summary>
    /// Require exclusive access for producer. Fence out old producer.
    /// </summary>
    [pbr::OriginalName("ExclusiveWithFencing")] ExclusiveWithFencing = 3,
}
