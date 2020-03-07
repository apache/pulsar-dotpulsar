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

using System.Collections.Generic;

namespace DotPulsar.Internal
{
    public sealed class IdLookup<T> where T : class
    {
        private T?[] _items;

        public IdLookup() => _items = new T[1];

        public bool IsEmpty()
        {
            lock (_items)
            {
                for (var i = 0; i < _items.Length; ++i)
                {
                    if (_items[i] != null)
                        return false;
                }

                return true;
            }
        }

        public ulong Add(T item)
        {
            lock (_items)
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
        }

        public T? Remove(ulong id)
        {
            lock (_items)
            {
                var item = _items[(int)id];
                _items[(int)id] = null;
                return item;
            }
        }

        public T[] RemoveAll()
        {
            lock (_items)
            {
                var items = new List<T>();
                for (var i = 0; i < _items.Length; ++i)
                {
                    var item = _items[i];
                    if (item != null)
                    {
                        items.Add(item);
                        _items[i] = null;
                    }
                }
                return items.ToArray();
            }
        }

        public T? this[ulong id]
        {
            get
            {
                lock (_items)
                {
                    return _items[(int)id];
                }
            }
        }
    }
}
