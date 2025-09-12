#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using pbr = Google.Protobuf.Reflection;

public enum CompressionType {
    [pbr::OriginalName("NONE")] None = 0,
    [pbr::OriginalName("LZ4")] Lz4 = 1,
    [pbr::OriginalName("ZLIB")] Zlib = 2,
    [pbr::OriginalName("ZSTD")] Zstd = 3,
    [pbr::OriginalName("SNAPPY")] Snappy = 4,
}
