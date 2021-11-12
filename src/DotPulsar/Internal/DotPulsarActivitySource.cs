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

namespace DotPulsar.Internal;

using DotPulsar.Abstractions;
using System.Collections.Generic;
using System.Diagnostics;

public static class DotPulsarActivitySource
{
    private const string _conversationId = "conversation_id";

    static DotPulsarActivitySource()
    {
        ActivitySource = new ActivitySource(Constants.ClientName, Constants.ClientVersion);
    }

    public static ActivitySource ActivitySource { get; }

    public static Activity? StartConsumerActivity(IMessage message, string operationName, KeyValuePair<string, object?>[] tags)
    {
        if (!ActivitySource.HasListeners())
            return null;

        var activity = ActivitySource.StartActivity(operationName, ActivityKind.Consumer);

        if (activity is not null && activity.IsAllDataRequested)
        {
            for (var i = 0; i < tags.Length; ++i)
            {
                var tag = tags[i];
                activity.SetTag(tag.Key, tag.Value);
            }


            var properties = message.Properties;
            if (properties.TryGetValue(_conversationId, out var conversationId))
                activity.SetTag(_conversationId, conversationId);
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
            for (var i = 0; i < tags.Length; ++i)
            {
                var tag = tags[i];
                activity.SetTag(tag.Key, tag.Value);
            }

            var conversationId = metadata[_conversationId];
            if (conversationId is not null)
                activity.SetTag(_conversationId, conversationId);
        }

        return activity;
    }
}
