/*
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
    using System;

    public class TopicName
    {
        private const string PublicTenant = "public";
        private const string DefaultNamespace = "default";
        private const string PartitionedTopicSuffix = "-partition-";
        private const string Persistent = "persistent";
        private const string NonPersistent = "non-persistent";

        private readonly string _domain;
        private readonly string _tenant;
        private readonly string? _cluster;
        private readonly string _namespacePortion;
        private readonly string _localName;
        private readonly NamespaceName _namespaceName;
        private readonly int _partitionIndex;
        private readonly string _completeTopicName;

        public TopicName(string completeTopicName)
        {
            // The topic name can be in two different forms, one is fully qualified topic name,
            // the other one is short topic name
            string[] parts;

            if (!completeTopicName.Contains("://"))
            {
                // The short topic name can be:
                // - <topic>
                // - <property>/<namespace>/<topic>
                parts = completeTopicName.Split('/');

                completeTopicName = parts.Length switch
                {
                    3 => Persistent + "://" + completeTopicName,
                    1 => Persistent + "://" + PublicTenant + "/" + DefaultNamespace + "/" + parts[0],
                    _ => throw new ArgumentException("Invalid short topic name '" + completeTopicName +
                                                     "', it should be in the format of " +
                                                     "<tenant>/<namespace>/<topic> or <topic>")
                };
            }

            // The fully qualified topic name can be in two different forms:
            // new:    persistent://tenant/namespace/topic
            // legacy: persistent://tenant/cluster/namespace/topic

            parts = completeTopicName.Split(new[] { ':', '/', '/' }, 2, StringSplitOptions.RemoveEmptyEntries);
            _domain = Persistent.Equals(parts[0]) ? Persistent : NonPersistent;

            string rest = parts[1];

            // The rest of the name can be in different forms:
            // new:    tenant/namespace/<localName>
            // legacy: tenant/cluster/namespace/<localName>
            // Examples of localName:
            // 1. some, name, xyz
            // 2. xyz-123, feeder-2

            parts = rest.Split(new[] { ':', '/', '/' }, 4, StringSplitOptions.RemoveEmptyEntries);

            switch (parts.Length)
            {
                case 3:
                    // New topic name without cluster name
                    _tenant = parts[0];
                    _cluster = null;
                    _namespacePortion = parts[1];
                    _localName = parts[2];
                    _partitionIndex = GetPartitionIndex(completeTopicName);
                    _namespaceName = NamespaceName.Get(_tenant, _namespacePortion);
                    break;
                case 4:
                    // Legacy topic name that includes cluster name
                    _tenant = parts[0];
                    _cluster = parts[1];
                    _namespacePortion = parts[2];
                    _localName = parts[3];
                    _partitionIndex = GetPartitionIndex(completeTopicName);
                    _namespaceName = NamespaceName.Get(_tenant, _cluster, _namespacePortion);
                    break;
                default: throw new ArgumentException("Invalid topic name: " + completeTopicName);
            }

            if (string.IsNullOrEmpty(_localName))
            {
                throw new ArgumentException("Invalid topic name: " + completeTopicName);
            }

            _completeTopicName =
                IsV2()
                    ? $"{_domain}://{_tenant}/{_namespacePortion}/{_localName}"
                    : $"{_domain}://{_tenant}/{_cluster}/{_namespacePortion}/{_localName}";
        }

        private bool IsV2()
        {
            return _cluster == null;
        }

        public string GetDomain()
        {
            return _domain;
        }

        public string GetTenant()
        {
            return _tenant;
        }

        public string GetNamespacePortion()
        {
            return _namespacePortion;
        }

        public int GetPartitionIndex()
        {
            return _partitionIndex;
        }

        public string GetLocalName()
        {
            return _localName;
        }

        public NamespaceName GetNamespaceName()
        {
            return _namespaceName;
        }

        private static string SubstringAfterLast(string str, string separator)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            if (string.IsNullOrEmpty(separator))
            {
                return "";
            }

            var pos = str.LastIndexOf(separator, StringComparison.Ordinal);
            return pos != -1 && pos != str.Length - separator.Length ? str.Substring(pos + separator.Length) : "";
        }

        private static int GetPartitionIndex(string topic)
        {
            var partitionIndex = -1;

            if (!topic.Contains(PartitionedTopicSuffix))
                return partitionIndex;

            try
            {
                var idx = SubstringAfterLast(topic, PartitionedTopicSuffix);
                partitionIndex = int.Parse(idx);

                if (partitionIndex < 0)
                {
                    // for the "topic-partition--1"
                    partitionIndex = -1;
                }
                else if (idx.Length != partitionIndex.ToString().Length)
                {
                    // for the "topic-partition-01"
                    partitionIndex = -1;
                }
            }
            catch (Exception)
            {
                // ignore exception
            }

            return partitionIndex;
        }

        public override string ToString()
        {
            return _completeTopicName;
        }
    }
}
