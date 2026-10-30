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

namespace DotPulsar;

/// <summary>
/// How the process activity is correlated with the message's send activity, if tracing is enabled.
/// </summary>
public enum TraceCorrelation : byte
{
    /// <summary>
    /// The process activity is not correlated with the message's send activity.
    /// </summary>
    None = 0,

    /// <summary>
    /// The process activity links to the message's send activity. The process activity is a child of the ambient activity (if any).
    /// This is the correlation recommended by the OpenTelemetry semantic conventions for messaging.
    /// </summary>
    Link = 1,

    /// <summary>
    /// The process activity is a child of the message's send activity, so both end up in the same trace.
    /// The process activity also links to the message's send activity and to the ambient activity (if any).
    /// </summary>
    Parent = 2
}
