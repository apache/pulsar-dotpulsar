using System.Collections.Generic;

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
