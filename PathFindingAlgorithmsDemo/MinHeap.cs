using System;
using System.Collections.Generic;

namespace PathFindingAlgorithmsDemo
{
    public class MinHeap<T>
    {
        private const int _initialCapacity = 4;
        private T[] _array;
        private int _lastItemIndex;
        private readonly IComparer<T> _comparer;

        public MinHeap() : this(_initialCapacity, Comparer<T>.Default)
        {
        }

        public MinHeap(int capacity) : this(capacity, Comparer<T>.Default)
        {
        }

        public MinHeap(Comparison<T> comparison) : this(_initialCapacity, Comparer<T>.Create(comparison))
        {
        }

        public MinHeap(IComparer<T> comparer) : this(_initialCapacity, comparer)
        {
        }

        public MinHeap(int capacity, IComparer<T> comparer)
        {
            _array = new T[capacity];
            _lastItemIndex = -1;
            _comparer = comparer;
        }

        public int Count => _lastItemIndex + 1;

        public void Add(T item)
        {
            if (_lastItemIndex == _array.Length - 1)
            {
                Resize();
            }

            _lastItemIndex++;
            _array[_lastItemIndex] = item;

            MinHeapifyUp(_lastItemIndex);
        }

        public T Remove()
        {
            if (_lastItemIndex == -1)
            {
                throw new InvalidOperationException("The heap is empty");
            }

            T removedItem = _array[0];
            _array[0] = _array[_lastItemIndex];
            _lastItemIndex--;

            MinHeapifyDown(0);

            return removedItem;
        }

        public T Peek()
        {
            if (_lastItemIndex == -1)
            {
                throw new InvalidOperationException("The heap is empty");
            }

            return _array[0];
        }

        public void Clear()
        {
            _lastItemIndex = -1;
        }

        private void MinHeapifyUp(int index)
        {
            if (index == 0)
            {
                return;
            }

            int childIndex = index;
            int parentIndex = (index - 1) / 2;

            if (_comparer.Compare(_array[childIndex], _array[parentIndex]) < 0)
            {
                // swap the parent and the child
                T temp = _array[childIndex];
                _array[childIndex] = _array[parentIndex];
                _array[parentIndex] = temp;

                MinHeapifyUp(parentIndex);
            }
        }

        private void MinHeapifyDown(int index)
        {
            int leftChildIndex = index * 2 + 1;
            int rightChildIndex = index * 2 + 2;
            int smallestItemIndex = index; // The index of the parent

            if (leftChildIndex <= _lastItemIndex &&
                _comparer.Compare(_array[leftChildIndex], _array[smallestItemIndex]) < 0)
            {
                smallestItemIndex = leftChildIndex;
            }

            if (rightChildIndex <= _lastItemIndex &&
                _comparer.Compare(_array[rightChildIndex], _array[smallestItemIndex]) < 0)
            {
                smallestItemIndex = rightChildIndex;
            }

            if (smallestItemIndex != index)
            {
                // swap the parent with the smallest of the child items
                T temp = _array[index];
                _array[index] = _array[smallestItemIndex];
                _array[smallestItemIndex] = temp;

                MinHeapifyDown(smallestItemIndex);
            }
        }

        private void Resize()
        {
            T[] newArr = new T[_array.Length * 2];
            for (int i = 0; i < _array.Length; i++)
            {
                newArr[i] = _array[i];
            }

            _array = newArr;
        }
    }
}
