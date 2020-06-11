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

namespace DotPulsar.Internal
{
    using System.Reflection;

    public static class Constants
    {
        static Constants()
        {
            var assemblyName = Assembly.GetCallingAssembly().GetName();
            ClientVersion = $"{assemblyName.Name} {assemblyName.Version.ToString(3)}";
            ProtocolVersion = 14;
            PulsarScheme = "pulsar";
            PulsarSslScheme = "pulsar+ssl";
            DefaultPulsarPort = 6650;
            DefaultPulsarSSLPort = 6651;
            MagicNumber = new byte[] { 0x0e, 0x01 };
            MetadataSizeOffset = 6;
            MetadataOffset = 10;
        }

        public static string ClientVersion { get; }
        public static int ProtocolVersion { get; }
        public static string PulsarScheme { get; }
        public static string PulsarSslScheme { get; }
        public static int DefaultPulsarPort { get; }
        public static int DefaultPulsarSSLPort { get; }
        public static byte[] MagicNumber { get; }
        public static int MetadataSizeOffset { get; }
        public static int MetadataOffset { get; }
    }
}
