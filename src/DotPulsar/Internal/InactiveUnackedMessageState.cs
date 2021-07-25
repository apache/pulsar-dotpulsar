﻿/*
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
    using Abstractions;
    using PulsarApi;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class InactiveUnackedMessageState : IUnackedMessageState
    {

        public void Acknowledge(MessageIdData messageId)
        {
            return;
        }

        public void Add(MessageIdData messageId)
        {
            return;
        }

        public IEnumerable<MessageIdData> CheckUnackedMessages()
        {
            return Enumerable.Empty<MessageIdData>();
        }

        public void Remove(MessageIdData messageId)
        {
            return;
        }
    }
}