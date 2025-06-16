using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Navigation/NavGraph")]
public class NavGraph : ScriptableObject
{
    public List<NavNode> nodes;

    private Dictionary<string, NavNode> nodeLookup;
    private Dictionary<Vector2Int, List<NavNode>> spatialHash;
    private float cellSize = 5f;
    
    public readonly int MAX_OF_WORLD_X_COORDINATE = 400;
    public readonly int MAX_OF_WORLD_Z_COORDINATE = 400;

    public void Initialize()
    {
        // ID 기반 연결 초기화
        nodeLookup = nodes.ToDictionary(n => n.id, n => n);

        foreach (var node in nodes)
        {
            node.connectedNodes = node.connectedNodeIds
                .Select(id => GetNodeById(id))
                .Where(n => n != null)
                .ToList();
        }

        // 공간 해시 초기화
        BuildSpatialHash();
    }

    public NavNode GetNodeById(string id)
    {
        if (nodeLookup is null) Initialize();
        return nodeLookup.TryGetValue(id, out var node) ? node : null;
    }

    public List<NavNode> GetConnectedNodes(NavNode node)
    {
        if (nodeLookup is null) Initialize();
        return node.connectedNodeIds
            .Select(id => GetNodeById(id))
            .Where(n => n != null)
            .ToList();
    }

    public NavNode FindClosestNode(Vector3 position)
    {
        if (spatialHash == null) BuildSpatialHash();

        Vector2Int key = GetCellKey(position);

        if (!spatialHash.TryGetValue(key, out var candidates) || candidates.Count == 0)
        {
            // 주변 셀까지 확장 검색 (필요시)
            return SlowFallback(position);
        }

        NavNode closest = null;
        float minDist = float.MaxValue;

        foreach (var node in candidates)
        {
            if (node.type == NodeType.BLOCKED) continue;

            float dist = (node.worldPosition - position).sqrMagnitude;

            if (dist < minDist)
            {
                minDist = dist;
                closest = node;
            }
        }

        return closest;
    }

    public NavRegion GetRegion(Vector3 position)
    {
        var node = FindClosestNode(position);
        return node?.region;
    }

    private void BuildSpatialHash()
    {
        spatialHash = new Dictionary<Vector2Int, List<NavNode>>();

        foreach (var node in nodes)
        {
            var key = GetCellKey(node.worldPosition);

            if (!spatialHash.TryGetValue(key, out var list))
            {
                list = new List<NavNode>();
                spatialHash[key] = list;
            }

            list.Add(node);
        }
    }

    private Vector2Int GetCellKey(Vector3 pos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(pos.x / cellSize),
            Mathf.FloorToInt(pos.z / cellSize)
        );
    }

    private NavNode SlowFallback(Vector3 position)
    {
        NavNode closest = null;
        float minDist = float.MaxValue;

        foreach (var node in nodes)
        {
            if (node.type == NodeType.BLOCKED) continue;

            float dist = (node.worldPosition - position).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                closest = node;
            }
        }

        return closest;
    }
}
