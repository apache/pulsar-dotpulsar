﻿/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
