using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UniTaskQueue2
{
    private readonly Queue<Func<UniTask>> _taskQueue = new();
    private readonly object _lock = new();
    
    private bool _isProcessing = false;

    public event Action OnQueueEmptyEvent;
    
    public void Enqueue(Func<UniTask> task)
    {
        lock (_lock)
        {
            _taskQueue.Enqueue(task);
        }
    }

    public void RunTaskForget(Action task)
    {
        Enqueue(() => UniTask.RunOnThreadPool(task));
    }

    public void RunTaskForget<T>(Func<T> task, Action<T> onComplete = null)
    {
        Enqueue(async () =>
        {
            var result = await UniTask.RunOnThreadPool(task);
            
            onComplete?.Invoke(result);
        });
    }

    public UniTask<T> RunTask<T>(Func<T> task)
    {
        var source = new UniTaskCompletionSource<T>();
        
        Enqueue(async () =>
        {
            try
            {
                var result = await UniTask.RunOnThreadPool(task);
                
                source.TrySetResult(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        });
        
        return source.Task;
    }

    public async void RunNextTask()
    {
        while (true)
        {
            Func<UniTask> task = null;

            lock (_lock)
            {
                if (_taskQueue.Count <= 0)
                {
                    _isProcessing = false;
                    OnQueueEmptyEvent?.Invoke();
                    return;
                }

                _isProcessing = true;
                task = _taskQueue.Dequeue();
            }

            try
            {
                await task();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }
    }
}
