using System;
using System.Collections.Concurrent;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PathFindingMovementActionNode : BTNode
{
    private ConcurrentQueue<Vector3> path;
    private CancellationTokenSource cts;
    private bool isRequestingPath = false;

    public PathFindingMovementActionNode(Blackboard blackboard, IMonster monster)
    {
        Blackboard = blackboard;
        Monster = monster;

        monster.OnDeath += CancelAndCleanUp;
    }

    public override NodeState EvaluateBehaviour()
    {
        if (path is null || (path.IsEmpty && !isRequestingPath))
        {
            path = new ConcurrentQueue<Vector3>();
            cts = new CancellationTokenSource();
            isRequestingPath = true;

            GameManager.Instance.pathFindingManager.RequestPath(Monster.transform, GameManager.Instance.PlayerPosition, path, cts);

            // 안전하게 awaitable 로직을 TryCatch로 감싸기 위해 Task로 분리
            MonitorPathCompletionAsync(path, cts).Forget();
        }

        // 장애물 없는 경우 취소
        if(!Physics.Linecast(Monster.transform.position, GameManager.Instance.PlayerPosition.transform.position, out var hit, 1 << LayerMask.NameToLayer("Obstacle")))
        {
            CancelAndCleanUp();
            
            return NodeState.FAILURE;
        }

        // 경로를 따라 이동
        if (path is not null)
        {
            if (path.TryDequeue(out Vector3 point))
            {
                //TODO: 임시로 확인하기 위해 Path Movement를 아래처럼 구현함. Movetoward와 같은 것으로 변경해줘야함
                Monster.transform.position = point;
                return path.IsEmpty ? NodeState.SUCCESS : NodeState.RUNNING;
            }
        }

        return isRequestingPath ? NodeState.RUNNING : NodeState.FAILURE;
    }

    private async UniTaskVoid MonitorPathCompletionAsync(ConcurrentQueue<Vector3> requestPath, CancellationTokenSource tokenSource)
    {
        try
        {
            //경로 연산을 5초 동안 기다림
            await UniTask.Delay(TimeSpan.FromSeconds(5), cancellationToken: tokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            Debug.LogWarning($"Path Finding Is Cancelled");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[MonitorPathCompletion] Unexpected error: {ex}");
        }
        finally
        {
            if (path == requestPath)
            {
                isRequestingPath = false;
                tokenSource.Dispose();

                path = null;
                cts = null;
            }
        }
    }

    private void CancelAndCleanUp()
    {
        if (cts != null && !cts.IsCancellationRequested)
        {
            try
            {
                cts.Cancel();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        isRequestingPath = false;
        path?.Clear();
        path = null;
        
        cts?.Dispose();
        cts = null;
    }
}
