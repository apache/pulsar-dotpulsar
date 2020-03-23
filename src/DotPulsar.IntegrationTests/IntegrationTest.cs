using Xunit;

namespace DotPulsar.IntegrationTests
{
    [CollectionDefinition(nameof(IntegrationTest))]
    public class IntegrationTest : ICollectionFixture<IntegrationTestFixture>
    {

    }
}
