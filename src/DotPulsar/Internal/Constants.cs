using System.Reflection;

namespace DotPulsar.Internal
{
    public static class Constants
    {
        static Constants()
        {
            var assemblyName = Assembly.GetCallingAssembly().GetName();
            ClientVersion = assemblyName.Name + " " + assemblyName.Version.ToString(3);
            ProtocolVersion = 14;
            PulsarScheme = "pulsar";
            PulsarSslScheme = "pulsar+ssl";
            DefaultPulsarPort = 6650;
            DefaultPulsarSSLPort = 6651;
            MagicNumber = new byte[] { 0x0e, 0x01 };
        }

        public static string ClientVersion { get; }
        public static int ProtocolVersion { get; }
        public static string PulsarScheme { get; }
        public static string PulsarSslScheme { get; }
        public static int DefaultPulsarPort { get; }
        public static int DefaultPulsarSSLPort { get; }
        public static byte[] MagicNumber { get; }
    }
}
