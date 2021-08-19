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

namespace DotPulsar
{
    using Abstractions;
    using Internal.PulsarApi;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A MessageId implementation that contains a map of topic and MessageId.
    /// </summary>
    public class MultiMessageId : IMessageId
    {
        public string Topic
        {
            get { throw new NotSupportedException($"${GetType().Name} doesn't support this field"); }
        }

        public MessageIdData ToMessageIdData() => throw new NotSupportedException($"${GetType().Name} doesn't support this method");

        public Dictionary<string, IMessageId> Map { get; }

        public MultiMessageId(Dictionary<string, IMessageId> map)
        {
            Map = map ?? throw new ArgumentException("map cannot be null");
        }

        public int CompareTo(object? o)
        {
            if (o is null)
            {
                return 1;
            }

            if (o.GetType() != GetType())
            {
                throw new Exception("expected MultiMessageId object. Got instance of " + o.GetType());
            }

            var otherMap = (o as MultiMessageId)?.Map!;

            if ((Map.Count == 0) && (otherMap.Count == 0))
            {
                return 0;
            }

            if (otherMap.Count != Map.Count)
            {
                throw new ArgumentException("Current size and other size not equals");
            }

            var result = 0;

            foreach (var pair in Map)
            {
                var otherMessage = otherMap[pair.Key];

                if (otherMessage == null)
                {
                    throw new ArgumentException("Other MessageId not have topic " + pair.Key);
                }

                var currentResult = pair.Value.CompareTo(otherMessage);

                if (result == 0)
                {
                    result = currentResult;
                }
                else if (currentResult == 0)
                {
                    continue;
                }
                else if (result != currentResult)
                {
                    throw new ArgumentException(
                        "Different MessageId in Map get different compare result");
                }
                else
                {
                    continue;
                }
            }

            return result;
        }

        public bool Equals(MultiMessageId obj)
        {
            try
            {
                return CompareTo(obj) == 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
