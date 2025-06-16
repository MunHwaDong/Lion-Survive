using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskQueue
{
    private Queue<Action> _taskQueue = new();
    private bool _processing;

    public void RunTask(Action task)
    {
        if (_processing is false)
        {
            _processing = true;
            task?.Invoke();
        }
        else
        {
            _taskQueue.Enqueue(task);
        }
    }

    public void DoneTask()
    {
        _processing = false;

        if (_taskQueue.Count <= 0) return;
        
        _taskQueue.Dequeue()?.Invoke();
    }
}
