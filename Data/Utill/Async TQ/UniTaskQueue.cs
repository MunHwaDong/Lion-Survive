using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

//UniTask 라이브러리가 필요합니다.

public class UniTaskQueue
{
    private readonly Queue<Func<UniTask>> _taskQueue = new();
    private readonly object _lock = new();
    
    private bool _isProcessing = false;
    public int TaskCount => _taskQueue.Count + (_isProcessing ? 1 : 0);
    public event Action OnDoneTask;
    public event Action OnQueueEmpty;

    /// <summary>
    /// 비동기로 백그라운드 스레드에서 Task를 실행합니다. 비동기 작업의 순서를 보장하지 않습니다.
    /// </summary>
    /// <param name="task">백그라운드 스레드에서 실행할 Task</param>
    /// <param name="onComplete">Task의 반환 값을 받을 Call-back</param>
    /// <typeparam name="T">Task의 반환 값</typeparam>
    public void RunTaskForget<T>(Func<T> task, Action<T> onComplete = null)
    {
        lock (_lock)
        {
            _taskQueue.Enqueue(async () =>
            {
                try
                {
                    var result = await UniTask.RunOnThreadPool(task);
                    
                    await UniTask.SwitchToMainThread();
                    
                    onComplete?.Invoke(result);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                finally
                {
                    await DoneTask();
                }
            });
        
            if (_isProcessing is false)
            {
                _isProcessing = true;
                _taskQueue.Dequeue().Invoke();
            }
        }
    }
    
    /// <summary>
    /// 비동기로 백그라운드 스레드에서 Task를 실행합니다. 비동기 작업의 순서를 보장하지 않습니다. 반환값을 사용하지 않습니다.
    /// </summary>
    /// <param name="task">백그라운드 스레드에서 실행할 Task</param>
    public void RunTaskForget(Action task)
    {
        lock (_lock)
        {
            _taskQueue.Enqueue(async () =>
            {
                try
                {
                    await UniTask.RunOnThreadPool(task);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                finally
                {
                    await DoneTask();
                }
            });
        
            if (_isProcessing is false)
            {
                _isProcessing = true;
                _taskQueue.Dequeue().Invoke();
            }
        }
    }
    
    /// <summary>
    /// 비동기로 Task를 실행합니다. 비동기 작업의 순서를 보장하며, 값을 반환합니다.
    /// </summary>
    /// <param name="task">실행할 Task</param>
    /// <typeparam name="T">Task의 반환값</typeparam>
    /// <returns>T-Type의 값을 반환합니다.</returns>
    public UniTask<T> RunTask<T>(Func<UniTask<T>> task)
    {
        var source = new UniTaskCompletionSource<T>();
        
        lock (_lock)
        {
            _taskQueue.Enqueue(async () =>
            {
                try
                {
                   var result = await task();
        
                    source.TrySetResult(result);
                }
                catch (Exception e)
                {
                    source.TrySetException(e);
                }
                finally
                {
                    await DoneTask();
                }
            });
        
            if (_isProcessing is false)
            {
                _isProcessing = true;
                _taskQueue.Dequeue()?.Invoke();
            }
        }
        
        return source.Task;
    }
    
    public void RunTask(Func<UniTask> task)
    {
        lock (_lock)
        {
            _taskQueue.Enqueue(async () =>
            {
                try
                {
                    await task();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    throw;
                }
                finally
                {
                    await DoneTask();
                }
            });
        
            if (_isProcessing is false)
            {
                _isProcessing = true;
                _taskQueue.Dequeue()?.Invoke();
            }
        }
    }

    public async UniTask DoneTask()
    {
        Func<UniTask> lockTask = null;

        lock (_lock)
        {
            _isProcessing = false;
            
            OnDoneTask?.Invoke();
        
            //등록된 작업이 없는데 DoneTask()하는 경우
            if (_taskQueue.Count <= 0)
            {
                OnQueueEmpty?.Invoke();
                return;
            }
        
            _isProcessing = true;

            lockTask = _taskQueue.Dequeue();
        }
        
        if(lockTask is not null)
            await lockTask();
    }

    public bool IsEmpty()
    {
        return _taskQueue.Count <= 0;
    }

    public bool IsProcessing()
    {
        return _isProcessing;
    }
}