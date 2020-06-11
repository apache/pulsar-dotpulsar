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
    using DotPulsar.Abstractions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public sealed class ExceptionHandlerPipeline : IHandleException
    {
        private readonly IHandleException[] _handlers;

        public ExceptionHandlerPipeline(IEnumerable<IHandleException> handlers)
            => _handlers = handlers.ToArray();

        public async ValueTask OnException(ExceptionContext exceptionContext)
        {
            foreach (var handler in _handlers)
            {
                await handler.OnException(exceptionContext).ConfigureAwait(false);

                if (exceptionContext.ExceptionHandled)
                    break;
            }
        }
    }
}
