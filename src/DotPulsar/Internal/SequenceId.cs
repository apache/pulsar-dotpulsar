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

using System.Threading;

namespace DotPulsar.Internal
{
    public sealed class SequenceId
    {
        private long _current;

        public SequenceId(ulong initialSequenceId)
        {
            // Subtracting one because Interlocked.Increment will return the post-incremented value
            // which is expected to be the initialSequenceId for the first call
            _current = unchecked((long)initialSequenceId - 1);
        }

        public ulong FetchNext()
        {
            return unchecked((ulong)Interlocked.Increment(ref _current));
        }
    }
}
