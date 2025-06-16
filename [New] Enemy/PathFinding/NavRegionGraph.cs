using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Navigation/NavGraph")]
public class NavRegionGraph : ScriptableObject
{
    public List<NavRegion> graph = new();

    private const float MIN_DIRECTION_THRESHOLD = 0.3f;

    public List<NavRegion> FindPathOnBFS(NavRegion start, NavRegion end)
    {
        Dictionary<NavRegion, int> visited = new();
        Dictionary<NavRegion, NavRegion> cameFrom = new();
        Queue<NavRegion> queue = new();
        
        queue.Enqueue(start);
        visited[start] = 0;

        while (queue.Count > 0)
        {
            NavRegion current = queue.Dequeue();

            if (current.neighbors is null) continue;

            Vector2 curToEndDir = (new Vector2(end.center.x, end.center.y) - new Vector2(current.center.x, current.center.y)).normalized;

            foreach (var neighbor in current.neighbors)
            {
                Vector2 curToNeighborDir = (new Vector2(neighbor.center.x, neighbor.center.y) - new Vector2(current.center.x, current.center.y)).normalized;
                float toEndDirScalar = Vector2.Dot(curToEndDir, curToNeighborDir);

                if (toEndDirScalar < MIN_DIRECTION_THRESHOLD) continue;
                if (visited.ContainsKey(neighbor)) continue;

                visited[neighbor] = visited[current] + 1;
                cameFrom[neighbor] = current;
                queue.Enqueue(neighbor);

                if (neighbor == end)
                    return ReconstructPath(end, cameFrom);
            }
        }

        return new();
    }

    private List<NavRegion> ReconstructPath(NavRegion destination, Dictionary<NavRegion, NavRegion> cameFrom)
    {
        List<NavRegion> path = new();
        NavRegion current = destination;

        while (current != null)
        {
            path.Add(current);
            cameFrom.TryGetValue(current, out current);
        }

        path.Reverse();
        return path;
    }
}
