using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace DotPulsar.Internal
{
    public sealed class SequenceBuilder<T> where T : notnull
    {
        private readonly LinkedList<ReadOnlyMemory<T>> _elements;

        public SequenceBuilder() => _elements = new LinkedList<ReadOnlyMemory<T>>();

        public SequenceBuilder<T> Prepend(ReadOnlyMemory<T> memory)
        {
            _elements.AddFirst(memory);
            return this;
        }

        public SequenceBuilder<T> Prepend(ReadOnlySequence<T> sequence)
        {
            LinkedListNode<ReadOnlyMemory<T>>? index = null;

            foreach (var memory in sequence)
            {
                if (index is null)
                    index = _elements.AddFirst(memory);
                else
                    index = _elements.AddAfter(index, memory);
            }

            return this;
        }

        public SequenceBuilder<T> Append(ReadOnlyMemory<T> memory)
        {
            _elements.AddLast(memory);
            return this;
        }

        public SequenceBuilder<T> Append(ReadOnlySequence<T> sequence)
        {
            foreach (var memory in sequence)
                _elements.AddLast(memory);

            return this;
        }

        public long Length => _elements.Sum(e => e.Length);

        public ReadOnlySequence<T> Build()
        {
            if (_elements.Count == 0)
                return new ReadOnlySequence<T>();

            Segment? start = null;
            Segment? current = null;

            foreach (var element in _elements)
            {
                if (current is null)
                {
                    current = new Segment(element);
                    start = current;
                }
                else
                    current = current.CreateNext(element);
            }

            return new ReadOnlySequence<T>(start, 0, current, current!.Memory.Length);
        }

        private sealed class Segment : ReadOnlySequenceSegment<T>
        {
            public Segment(ReadOnlyMemory<T> memory, long runningIndex = 0)
            {
                Memory = memory;
                RunningIndex = runningIndex;
            }

            public Segment CreateNext(ReadOnlyMemory<T> memory)
            {
                var segment = new Segment(memory, RunningIndex + Memory.Length);
                Next = segment;
                return segment;
            }
        }
    }
}
