/**
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class Awaitor<T, Result> : IDisposable
    {
        private readonly Dictionary<T, TaskCompletionSource<Result>> _items;

        public Awaitor() => _items = new Dictionary<T, TaskCompletionSource<Result>>();

        public Task<Result> CreateTask(T item)
        {
            var tcs = new TaskCompletionSource<Result>(TaskCreationOptions.RunContinuationsAsynchronously);
            _items.Add(item, tcs);
            return tcs.Task;
        }

        public void SetResult(T item, Result result)
        {
#if NETSTANDARD2_0
            var tcs = _items[item];
            _items.Remove(item);
#else
            _items.Remove(item, out var tcs);
#endif
            tcs.SetResult(result);
        }

        public void Dispose()
        {
            foreach (var item in _items.Values)
            {
                item.SetCanceled();
            }

            _items.Clear();
        }
    }
}
