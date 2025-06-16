using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Diagnostics;
using System.Threading;
using Debug = UnityEngine.Debug;

public static class Astar
{
    public static async UniTask<List<NavNode>> FindPath(NavGraph graph, NavNode start, NavNode goal, CancellationTokenSource cts = default)
    {
        foreach (var node in graph.nodes)
        {
            node.gScore = float.MaxValue;
            node.fScore = float.MaxValue;
            node.visited = false;
            node.cameFrom = null;
        }

        start.gScore = 0;
        start.fScore = Heuristic(start, goal);

        var openSet = new Priority_Queue<PriorityNode>();
        openSet.Enqueue(new PriorityNode(start.fScore, start));

        while (!openSet.IsEmpty())
        {
            cts?.Token.ThrowIfCancellationRequested();
            
            var current = openSet.Dequeue().node;

            if (current == goal)
                return ReconstructPath(goal);

            current.visited = true;

            foreach (var neighbor in current.connectedNodes)
            {
                if (neighbor.visited) continue;

                float tentativeG = current.gScore + (current.worldPosition - neighbor.worldPosition).sqrMagnitude;

                if (tentativeG < neighbor.gScore)
                {
                    neighbor.cameFrom = current;
                    neighbor.gScore = tentativeG;
                    neighbor.fScore = tentativeG + Heuristic(neighbor, goal);

                    openSet.Enqueue(new PriorityNode(neighbor.fScore, neighbor));
                }
            }
        }

        return null;
    }

    private static float Heuristic(NavNode a, NavNode b)
    {
        return (a.worldPosition - b.worldPosition).sqrMagnitude;
    }

    private static List<NavNode> ReconstructPath(NavNode current)
    {
        List<NavNode> path = new List<NavNode> { current };
        
        while (current is not null)
        {
            path.Add(current);
            
            current = current.cameFrom;
        }
        
        path.Reverse();
        return path;
    }
}
