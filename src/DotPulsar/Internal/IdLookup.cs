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

﻿using System.Collections.Generic;

namespace DotPulsar.Internal
{
    public sealed class IdLookup<T> where T : class
    {
        private T?[] _items;

        public IdLookup() => _items = new T[0];

        public bool IsEmpty()
        {
            for (var i = 0; i < _items.Length; ++i)
            {
                if (_items[i] != null)
                    return false;
            }

            return true;
        }

        public ulong[] AllIds()
        {
            var activeIds = new List<ulong>();
            for (var i = 0; i < _items.Length; ++i)
            {
                if (_items[i] != null)
                    activeIds.Add((ulong)i);
            }
            return activeIds.ToArray();
        }

        public ulong Add(T item)
        {
            for (var i = 0; i < _items.Length; ++i)
            {
                if (_items[i] != null)
                    continue;

                _items[i] = item;
                return (ulong)i;
            }

            var newArray = new T[_items.Length + 1];
            _items.CopyTo(newArray, 0);
            var id = newArray.Length - 1;
            newArray[id] = item;
            _items = newArray;
            return (ulong)id;
        }

        public void Remove(ulong id) => _items[(int)id] = null;

        public T? this[ulong id]
        {
            get => _items[(int)id];
        }
    }
}
