using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PathFindingManager : MonoBehaviour
{
    [SerializeField] private NavRegionGraph navRegionGraph;
    [SerializeField] private NavGraph navGraph;
    
    private Dictionary<string, NavRegion> _regionMap = new();
    private SemaphoreSlim _semaphore = new(4);
    
    private void Awake()
    {
        InitializeGraph();
    }

    private void InitializeGraph()
    {
        // Region Map 초기화
        foreach (var region in navRegionGraph.graph)
        {
            _regionMap[region.id] = region;
        }

        // Node - Region 연결 복원
        foreach (var node in navGraph.nodes)
        {
            if (_regionMap.TryGetValue(node.regionId, out var region))
            {
                node.region = region;
            }
        }

        // Region - Neighbors 복원
        foreach (var region in navRegionGraph.graph)
        {
            region.neighbors = new List<NavRegion>();
            foreach (var neighborId in region.neighborIds)
            {
                if (_regionMap.TryGetValue(neighborId, out var neighbor))
                {
                    region.neighbors.Add(neighbor);
                }
            }
        }

        Debug.Log("NavGraph runtime data initialized.");
    }
    
    public void RequestPath(Transform start, Transform end, ConcurrentQueue<Vector3> targetQueue, CancellationTokenSource cts)
    {
        if (targetQueue.Count > 0) return;
        
        var startPos = start.position;
        var endPos = end.position;
        
        _ = ProcessRequestWithSemaphoreAsync(new PathRequest(startPos, endPos, targetQueue, cts));
    }
    
    private async UniTaskVoid ProcessRequestWithSemaphoreAsync(PathRequest request)
    {
        // 슬롯이 열릴 때까지 대기
        await _semaphore.WaitAsync();

        try
        {
            await ProcessRequestAsync(request);
        }
        catch (OperationCanceledException)
        {
            Debug.LogWarning("Pathfinding was cancelled.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Pathfinding error: {e}");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async UniTask ProcessRequestAsync(PathRequest request)
    {
        try
        {
            if (request.Cts is not null && request.Cts is { IsCancellationRequested: true }) return;
            
            var startRegion = navGraph.GetRegion(request.Start);
            var endRegion = navGraph.GetRegion(request.End);
            
            var regionPathPos = await UniTask.RunOnThreadPool(() =>
                navRegionGraph.FindPathOnBFS(startRegion, endRegion));

            for (int i = 0; i < regionPathPos.Count - 1; i++)
            {
                Debug.DrawLine(
                    new Vector3(regionPathPos[i].center.x, 1, regionPathPos[i].center.y),
                    new Vector3(regionPathPos[i + 1].center.x, 1, regionPathPos[i + 1].center.y),
                    Color.cyan, 10f);
            }

            if (request.Cts is not null && request.Cts is { IsCancellationRequested: true }) return;
            
            await AStarPathFinder.FindPath(regionPathPos, request.Start, request.End, request.TargetQueue, request.Cts);
        }
        catch (Exception e)
        {
            Debug.LogError($"ProcessRequestAsync error: {e}");
        }
    }

    private readonly struct PathRequest
    {
        public readonly Vector3 Start;
        public readonly Vector3 End;
        public readonly ConcurrentQueue<Vector3> TargetQueue;
        public readonly CancellationTokenSource Cts;

        public PathRequest(Vector3 start, Vector3 end, ConcurrentQueue<Vector3> targetQueue, CancellationTokenSource cts)
        {
            Start = start;
            End = end;
            TargetQueue = targetQueue;
            Cts = cts;
        }
    }
}