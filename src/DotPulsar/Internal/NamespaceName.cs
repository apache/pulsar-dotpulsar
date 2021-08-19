namespace DotPulsar.Internal
{
    using System;
    using System.Text.RegularExpressions;

    public class NamespaceName
    {
        // allowed characters for property, namespace, cluster and topic names are
        // alphanumeric (a-zA-Z_0-9) and these special chars -=:.
        // % is allowed as part of valid URL encoding
        private static Regex _NAMED_ENTITY_PATTERN = new("^[-=:.\\w]*$");
        private readonly string _tenant;
        private readonly string? _cluster;
        private readonly string _localName;
        private readonly string _ns;
        private readonly bool _isV2;

        private NamespaceName(string ns)
        {
            if (string.IsNullOrEmpty(ns))
            {
                throw new ArgumentException("Invalid null namespace: " + ns);
            }

            // Verify it's a proper namespace
            // The namespace name is composed of <tenant>/<namespace>
            // or in the legacy format with the cluster name:
            // <tenant>/<cluster>/<namespace>
            try
            {
                string[] parts = ns.Split('/');

                switch (parts.Length)
                {
                    case 2:
                        // New style namespace : <tenant>/<namespace>
                        ValidateNamespaceName(parts[0], parts[1]);

                        _tenant = parts[0];
                        _cluster = null;
                        _localName = parts[1];
                        _isV2 = true;
                        break;
                    case 3:
                        // Old style namespace: <tenant>/<cluster>/<namespace>
                        ValidateNamespaceName(parts[0], parts[1], parts[2]);

                        _tenant = parts[0];
                        _cluster = parts[1];
                        _localName = parts[2];
                        _isV2 = false;
                        break;
                    default: throw new ArgumentNullException("Invalid namespace format. namespace: " + ns);
                }
            }
            catch (Exception e)
            {
                throw new AggregateException("Invalid namespace format."
                                             + " expected <tenant>/<namespace> or <tenant>/<cluster>/<namespace> "
                                             + "but got: " + ns, e);
            }

            this._ns = ns;
        }

        public static NamespaceName Get(string tenant, string ns)
        {
            ValidateNamespaceName(tenant, ns);
            return new NamespaceName(tenant + '/' + ns);
        }

        public static NamespaceName Get(string tenant, string cluster, string ns)
        {
            ValidateNamespaceName(tenant, cluster, ns);
            return new NamespaceName(tenant + '/' + cluster + '/' + ns);
        }

        public bool IsV2Namespace()
        {
            return _isV2;
        }

        public override string ToString()
        {
            return _ns;
        }

        private static void CheckName(string name)
        {
            if (!_NAMED_ENTITY_PATTERN.IsMatch(name))
            {
                throw new ArgumentException("Invalid named entity: " + name);
            }
        }

        private static void ValidateNamespaceName(string tenant, string ns)
        {
            if (string.IsNullOrEmpty(tenant) || string.IsNullOrEmpty(ns))
            {
                throw new ArgumentException($"Invalid namespace format. namespace: {tenant}/{ns}");
            }

            CheckName(tenant);
            CheckName(ns);
        }

        private static void ValidateNamespaceName(string tenant, string cluster, string ns)
        {
            if (string.IsNullOrEmpty(tenant) || string.IsNullOrEmpty(cluster) || string.IsNullOrEmpty(ns))
            {
                throw new ArgumentException($"Invalid namespace format. namespace: {tenant}/{cluster}/{ns}");
            }

            CheckName(tenant);
            CheckName(cluster);
            CheckName(ns);
        }
    }
}
