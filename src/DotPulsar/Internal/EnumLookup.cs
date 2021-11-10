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

namespace DotPulsar.Internal;

using System;
using System.Linq;
using System.Runtime.CompilerServices;

public sealed class EnumLookup<TKey, TValue> where TKey : Enum
{
    private readonly TValue[] _values;

    public EnumLookup(TValue defaultValue)
    {
        var max = Enum.GetValues(typeof(TKey)).Cast<int>().Max() + 1;
        _values = new TValue[max];
        for (var i = 0; i < max; ++i)
            _values[i] = defaultValue;
    }

    public void Set(TKey key, TValue value)
        => _values[Unsafe.As<TKey, int>(ref key)] = value;

    public TValue Get(TKey key)
        => _values[Unsafe.As<TKey, int>(ref key)];
}
