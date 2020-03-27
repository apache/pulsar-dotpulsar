using Xunit;

namespace DotPulsar.Stress.Tests.Fixtures
{
    [CollectionDefinition(nameof(StandaloneClusterTest))]
    public class StandaloneClusterTest : ICollectionFixture<StandaloneClusterFixture>
    {

    }
}