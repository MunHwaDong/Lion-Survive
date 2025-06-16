using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class AStarPathFinder
{
    private static NavGraph graph;

    public static async UniTask FindPath(List<NavRegion> regionPath, Vector3 start, Vector3 end, ConcurrentQueue<Vector3> waypoints, CancellationTokenSource cts = null)
    {
        graph ??= GameManager.Instance.mapData;

        await UniTask.RunOnThreadPool(() =>
        {
            var st = graph.FindClosestNode(start);
            var en = graph.FindClosestNode(end);

            if (cts is not null && cts.IsCancellationRequested) return;

            var path = AStar(st, en, regionPath, cts);

            if (cts is not null && cts.IsCancellationRequested) return;

            if (path is null) return;
            
            foreach (var node in path)
            {
                waypoints.Enqueue(node.worldPosition);
            }
        });
    }

    private static List<NavNode> AStar(NavNode start, NavNode goal, List<NavRegion> allowedRegions, CancellationTokenSource cts = null)
    {
        if (cts is not null && cts.IsCancellationRequested) return null;

        var allowedRegionSet = allowedRegions.ToHashSet();
        
        var candidateNodes = graph.nodes.Where(n => allowedRegionSet.Contains(n.region));

        foreach (var node in candidateNodes)
        {
            node.gScore = float.MaxValue;
            node.fScore = float.MaxValue;
            node.visited = false;
            node.cameFrom = null;
        }

        start.gScore = 0;
        start.fScore = Heuristic(start, goal);

        var openSet = new Priority_Queue<PriorityNode>();
        var inOpenSet = new HashSet<NavNode>();
        openSet.Enqueue(new PriorityNode(start.fScore, start));
        inOpenSet.Add(start);

        while (!openSet.IsEmpty())
        {
            if (cts is not null && cts.IsCancellationRequested) return null;

            var current = openSet.Dequeue().node;
            inOpenSet.Remove(current);
            
            if (current == goal)
                return ReconstructPath(goal);

            current.visited = true;

            foreach (var neighbor in current.connectedNodes)
            {
                if (neighbor.visited || !allowedRegionSet.Contains(neighbor.region))
                    continue;

                float tentativeG = current.gScore + (current.worldPosition - neighbor.worldPosition).sqrMagnitude;

                if (tentativeG < neighbor.gScore)
                {
                    neighbor.cameFrom = current;
                    neighbor.gScore = tentativeG;
                    neighbor.fScore = tentativeG + Heuristic(neighbor, goal);

                    if (!inOpenSet.Contains(neighbor))
                    {
                        openSet.Enqueue(new PriorityNode(neighbor.fScore, neighbor));
                        inOpenSet.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    private static float Heuristic(NavNode a, NavNode b)
    {
        return (a.worldPosition - b.worldPosition).sqrMagnitude + a.weight;
    }

    private static List<NavNode> ReconstructPath(NavNode current)
    {
        List<NavNode> path = new();
        while (current != null)
        {
            path.Add(current);
            current = current.cameFrom;
        }
        path.Reverse();
        return path;
    }
}