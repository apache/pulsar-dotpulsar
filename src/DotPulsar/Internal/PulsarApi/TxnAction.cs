#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using pbr = Google.Protobuf.Reflection;

public enum TxnAction {
    [pbr::OriginalName("COMMIT")] Commit = 0,
    [pbr::OriginalName("ABORT")] Abort = 1,
}
