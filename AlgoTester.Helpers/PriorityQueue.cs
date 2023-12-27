using System;
using System.Collections.Generic;

namespace AlgoTester.Helpers;

public class PriorityQueue<T>
{
    private readonly SortedDictionary<int, Queue<T>> _queueDict;
    private int _count;

    public PriorityQueue()
    {
        _queueDict = new SortedDictionary<int, Queue<T>>();
        _count = 0;
    }

    public void Enqueue(T item, int priority)
    {
        if (!_queueDict.TryGetValue(priority, out Queue<T> queue))
        {
            queue = new Queue<T>();
            _queueDict[priority] = queue;
        }

        queue.Enqueue(item);
        _count++;
    }

    public T Dequeue()
    {
        if (_queueDict.Count == 0)
            throw new InvalidOperationException("Queue is empty");

        var pair = _queueDict.First();
        var queue = pair.Value;
        var item = queue.Dequeue();

        if (queue.Count == 0)
            _queueDict.Remove(pair.Key);

        _count--;
        return item;
    }

    public int Count => _count;

    public bool IsEmpty => _count == 0;
}