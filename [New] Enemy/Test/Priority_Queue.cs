using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Priority_Queue<T> where T : IComparable<T>
{
    private readonly List<T> Heap = new();

    public Priority_Queue()
    {
        Heap.Add(default);
    }

    public bool IsEmpty()
    {
        return Heap.Count == 1;
    }

    public void Enqueue(T data)
    {
        Heap.Add(data);
        var idx = Heap.Count - 1;
        HeapifyUp(idx);
    }

    public T Dequeue()
    {
        if (Heap.Count == 1) throw new Exception("Queue is empty");
        var returnData = Heap[1];
        Heap[1] = Heap[Heap.Count - 1];
        Heap.RemoveAt(Heap.Count - 1);
        HeapifyDown(1);

        return returnData;
    }

    public void Clear()
    {
        Heap.Clear();
    }

    private void HeapifyUp(int idx)
    {
        while (idx > 1)
        {
            var parentIdx = idx / 2;

            //해당 인스턴스(여기선 Heap[idx])가 크면 양수, 같으면 0, 작으면 음수
            if (Heap[idx].CompareTo(Heap[parentIdx]) >= 0)
                break;

            Swap(idx, parentIdx);
            idx = parentIdx;
        }
    }

    private void HeapifyDown(int idx)
    {
        var lastIdx = Heap.Count - 1;

        while (true)
        {
            var leftChildIdx = 2 * idx;
            var rightChildIdx = 2 * idx + 1;
            var smallestIdx = idx;

            // 왼쪽 자식이 존재하고 현재 노드보다 작을 때
            if (leftChildIdx <= lastIdx && Heap[leftChildIdx].CompareTo(Heap[smallestIdx]) < 0)
                smallestIdx = leftChildIdx;

            // 오른쪽 자식이 존재하고 가장 작은 값보다 더 작을 때
            if (rightChildIdx <= lastIdx && Heap[rightChildIdx].CompareTo(Heap[smallestIdx]) < 0)
                smallestIdx = rightChildIdx;

            // 더 이상 아래로 내려갈 필요가 없을 때
            if (smallestIdx == idx) break;

            // Swap
            Swap(idx, smallestIdx);
            idx = smallestIdx;
        }
    }

    private void Swap(int a, int b)
    {
        var tmp = Heap[a];
        Heap[a] = Heap[b];
        Heap[b] = tmp;
    }
}