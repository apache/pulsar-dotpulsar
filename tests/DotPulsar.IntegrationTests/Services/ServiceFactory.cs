namespace DotPulsar.IntegrationTests.Services
{
    using Abstraction;

    public static class ServiceFactory
    {
        private const string PulsarDeploymentType = "pulsar.deployment.type";
        private const string ContainerDeployment = "container";

        public static IPulsarService CreatePulsarService()
        {
            var deploymentType = System.Environment.GetEnvironmentVariable(PulsarDeploymentType);

            if (deploymentType == ContainerDeployment)
            {
                return new StandaloneContainerService();
            }

            return new StandaloneExternalService();
        }
    }
}
