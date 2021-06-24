using System;
using System.Collections;
using System.Collections.Generic;

public class Dequeue<T> : IEnumerable<T>
{
    private const int DefaultCapacity = 16;
    private T[] items;
    private int left;
    private int right;

    public Dequeue()
    {
        this.items = new T[DefaultCapacity];
        this.left = 0;
        this.right = 0;
    }

    public int Count
    {
        get
        {
            var d = (right - left);
            if (d >= 0)
                return d;
            else
                return items.Length + d;
        }
    }

    private bool IsEmpty => left == right;

    public T Front
    {
        get
        {
            if (IsEmpty)
                throw new InvalidOperationException();
            return items[left];
        }
    }

    public T Back
    {
        get
        {
            if (IsEmpty)
                throw new InvalidOperationException();
            return items[Bound(right - 1)];
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = left; i != right; i = Bound(i + 1))
        {
            yield return items[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Clear()
    {
        left = right = 0;
        Array.Clear(items, 0, items.Length);
    }

    private void GrowIfFull()
    {
        if ((left + 1) % items.Length == right)
        {
            var newItems = new T[items.Length * 2];
            int j = 0;
            for (int i = left; i < right; ++i, ++j)
            {
                if (i == items.Length)
                    i = 0;
                newItems[j] = items[i];
            }
            left = 0;
            right = j;
            items = newItems;
        }
    }

    public T PopFront()
    {
        if (IsEmpty)
            throw new InvalidOperationException("There are no items to remove.");
        T item = items[left];
        items[left] = default;
        left = Bound(left + 1);
        return item;
    }

    public T PopBack()
    {
        if (IsEmpty)
            throw new InvalidOperationException("There are no items to remove.");
        right = Bound(right - 1);
        T item = items[right];
        items[right] = default;
        return item;
    }

    public void PushFront(in T item)
    {
        GrowIfFull();
        left = Bound(left - 1);
        items[left] = item;
    }

    public void PushBack(in T item)
    {
        GrowIfFull();
        items[right] = item;
        right = Bound(right + 1);
    }

    private int Bound(int index)
    {
        int len = items.Length;
        if (index < 0)
            return index + len;
        else if (index >= len)
            return index - len;
        else
            return index;
    }
}