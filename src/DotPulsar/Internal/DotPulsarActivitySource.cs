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
    using DotPulsar.Abstractions;
    using System.Collections.Generic;
    using System.Diagnostics;

    public static class DotPulsarActivitySource
    {
        private const string _traceParent = "traceparent";
        private const string _traceState = "tracestate";

        static DotPulsarActivitySource()
        {
            ActivitySource = new ActivitySource(Constants.ClientName, Constants.ClientVersion);
        }

        public static ActivitySource ActivitySource { get; }

        public static Activity? StartConsumerActivity(IMessage message, string operationName, KeyValuePair<string, object?>[] tags)
        {
            if (!ActivitySource.HasListeners())
                return null;

            var properties = message.Properties;

            if (properties.TryGetValue(_traceParent, out var traceparent))
            {
                var tracestate = properties.ContainsKey(_traceState) ? properties[_traceState] : null;
                if (ActivityContext.TryParse(traceparent, tracestate, out var activityContext))
                    return ActivitySource.StartActivity(operationName, ActivityKind.Consumer, activityContext, tags);
            }

            var activity = ActivitySource.StartActivity(operationName, ActivityKind.Consumer);

            if (activity is not null && activity.IsAllDataRequested)
            {
                for (var i = 0; i < tags.Length; ++i)
                {
                    var tag = tags[i];
                    activity.SetTag(tag.Key, tag.Value);
                }
            }

            return activity;
        }

        public static Activity? StartProducerActivity(MessageMetadata metadata, string operationName, KeyValuePair<string, object?>[] tags)
        {
            if (!ActivitySource.HasListeners())
                return null;

            var activity = ActivitySource.StartActivity(operationName, ActivityKind.Producer);

            if (activity is not null && activity.IsAllDataRequested)
            {
                metadata[_traceParent] = activity.TraceId.ToHexString();
                metadata[_traceState] = activity.TraceStateString;

                for (var i = 0; i < tags.Length; ++i)
                {
                    var tag = tags[i];
                    activity.SetTag(tag.Key, tag.Value);
                }
            }

            return activity;
        }
    }
}
