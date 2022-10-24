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

using System;
using System.Threading.Tasks;

/// <summary>
/// The processing options.
/// </summary>
public sealed class ProcessingOptions
{
    /// <summary>
    /// The value used to represent unlimited processing of messages per task.
    /// </summary>
    public const int Unbounded = -1;

    private bool _ensureOrderedAcknowledgement;
    private int _maxDegreeOfParallelism;
    private int _maxMessagesPerTask;
    private TaskScheduler _taskScheduler;

    /// <summary>
    /// Initializes a new instance with the default values.
    /// </summary>
    public ProcessingOptions()
    {
        _ensureOrderedAcknowledgement = true;
        _maxDegreeOfParallelism = 1;
        _maxMessagesPerTask = Unbounded;
        _taskScheduler = TaskScheduler.Default;
    }

    /// <summary>
    /// Whether ordered acknowledgement should be enforced. The default is 'true'.
    /// </summary>
    public bool EnsureOrderedAcknowledgement
    {
        get => _ensureOrderedAcknowledgement;
        set { _ensureOrderedAcknowledgement = value; }
    }

    /// <summary>
    /// The maximum number of messages that may be processed concurrently. The default is 1.
    /// </summary>
    public int MaxDegreeOfParallelism
    {
        get => _maxDegreeOfParallelism;
        set
        {
            if (value < 1)
                throw new ArgumentOutOfRangeException(nameof(value));

            _maxDegreeOfParallelism = value;
        }
    }

    /// <summary>
    /// The maximum number of messages that may be processed per task. The default is -1 (unlimited).
    /// </summary>
    public int MaxMessagesPerTask
    {
        get => _maxMessagesPerTask;
        set
        {
            if (value < 1 && value != Unbounded)
                throw new ArgumentOutOfRangeException(nameof(value));

            _maxMessagesPerTask = value;
        }
    }

    /// <summary>
    /// The TaskScheduler to use for scheduling tasks. The default is TaskScheduler.Default.
    /// </summary>
    public TaskScheduler TaskScheduler
    {
        get => _taskScheduler;
        set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            _taskScheduler = value;
        }
    }
}
