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
using DotPulsar.Extensions;
using DotPulsar.Internal.Extensions;
using System.Diagnostics;

public static class DotPulsarActivitySource
{
    static DotPulsarActivitySource()
    {
        ActivitySource = new(Constants.ClientName, Constants.ClientVersion);
    }

    public static ActivitySource ActivitySource { get; }

    public static Activity? StartConsumerActivity(IMessage message, string operationName, KeyValuePair<string, object?>[] tags, TraceCorrelation traceCorrelation)
    {
        if (!ActivitySource.HasListeners())
            return null;

        ActivityContext parentContext = default;
        List<ActivityLink>? activityLinks = null;

        if (traceCorrelation != TraceCorrelation.None)
        {
            var creationContext = GetCreationContext(message);
            if (creationContext is not null)
            {
                activityLinks = [new ActivityLink(creationContext.Value)];

                if (traceCorrelation == TraceCorrelation.Parent)
                {
                    parentContext = creationContext.Value;

                    var ambientContext = Activity.Current?.Context;
                    if (ambientContext is not null && ambientContext.Value != default)
                        activityLinks.Add(new ActivityLink(ambientContext.Value));
                }
            }
        }

        return StartActivity(operationName, ActivityKind.Consumer, tags, activityLinks, message.GetConversationId(), parentContext);
    }

    public static Activity? StartProducerActivity(MessageMetadata metadata, string operationName, KeyValuePair<string, object?>[] tags)
    {
        if (!ActivitySource.HasListeners())
            return null;

        return StartActivity(operationName, ActivityKind.Producer, tags, null, metadata.GetConversationId());
    }

    private static ActivityContext? GetCreationContext(IMessage message)
    {
        if (message.Properties.TryGetValue(Constants.TraceParent, out var traceParent))
        {
            _ = message.Properties.TryGetValue(Constants.TraceState, out var traceState);

            if (ActivityContext.TryParse(traceParent, traceState, out var context))
                return context;
        }

        return null;
    }

    private static Activity? StartActivity(
        string operationName,
        ActivityKind kind,
        KeyValuePair<string, object?>[] tags,
        IEnumerable<ActivityLink>? activityLinks,
        string? conversationId,
        ActivityContext parentContext = default)
    {
        var activity = ActivitySource.StartActivity(kind, parentContext: parentContext, name: operationName, tags: tags, links: activityLinks);

        if (activity is not null && activity.IsAllDataRequested)
        {
            if (conversationId is not null)
                activity.SetConversationId(conversationId);
        }

        return activity;
    }
}
