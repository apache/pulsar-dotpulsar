namespace DotPulsar.IntegrationTests.Fixtures;

using Xunit;

[CollectionDefinition(nameof(StandaloneTokenClusterTest))]
public class StandaloneTokenClusterTest : ICollectionFixture<TokenClusterFixture> { }
