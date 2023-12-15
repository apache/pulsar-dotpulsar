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

namespace DotPulsar.Internal.Extensions;

using System.Diagnostics;

public static class ActivityExtensions
{
    private const string _exceptionEventName = "exception";
    private const string _exceptionStackTrace = "exception.stacktrace";
    private const string _exceptionType = "exception.type";
    private const string _exceptionMessage = "exception.message";
    private const string _messageId = "messaging.message_id";
    private const string _payloadSize = "messaging.message_payload_size_bytes";

    public static void AddException(this Activity activity, Exception exception)
    {
        activity.SetStatus(ActivityStatusCode.Error);

        var exceptionTags = new ActivityTagsCollection
        {
            { _exceptionType, exception.GetType().FullName },
            { _exceptionStackTrace, exception.ToString() }
        };

        if (!string.IsNullOrWhiteSpace(exception.Message))
            exceptionTags.Add(_exceptionMessage, exception.Message);

        var activityEvent = new ActivityEvent(_exceptionEventName, default, exceptionTags);
        activity.AddEvent(activityEvent);
    }

    public static void SetMessageId(this Activity activity, MessageId messageId)
        => activity.SetTag(_messageId, messageId.ToString());

    public static void SetConversationId(this Activity activity, string conversationId)
        => activity.SetTag(Constants.ConversationId, conversationId);

    public static void SetPayloadSize(this Activity activity, long payloadSize)
        => activity.SetTag(_payloadSize, payloadSize);
}
