using System.Reflection;

namespace DotPulsar.Internal
{
    public static class Constants
    {
        static Constants()
        {
            var assemblyName = Assembly.GetCallingAssembly().GetName();
            ClientVersion = assemblyName.Name + " " + assemblyName.Version.ToString(3);
            ProtocolVersion = 12;
            PulsarScheme = "pulsar";
            PulsarSslScheme = "pulsar+ssl";
            DefaultPulsarPort = 6650;
            DefaultPulsarSSLPort = 6651;
        }

        public static string ClientVersion { get; }
        public static int ProtocolVersion { get; }
        public static string PulsarScheme { get; }
        public static string PulsarSslScheme { get; }
        public static int DefaultPulsarPort { get; }
        public static int DefaultPulsarSSLPort { get; }
    }
}
