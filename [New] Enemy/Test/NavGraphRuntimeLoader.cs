using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavGraphRuntimeLoader : MonoBehaviour
{
    public NavGraph navGraph;
    public NavRegionGraph regionGraph;

    private Dictionary<string, NavRegion> regionMap = new();

    private void Awake()
    {
        InitializeGraph();
    }

    public void InitializeGraph()
    {
        // Region Map 초기화
        foreach (var region in regionGraph.graph)
        {
            regionMap[region.id] = region;
        }

        // Node - Region 연결 복원
        foreach (var node in navGraph.nodes)
        {
            if (regionMap.TryGetValue(node.regionId, out var region))
            {
                node.region = region;
            }
        }

        // Region - Neighbors 복원
        foreach (var region in regionGraph.graph)
        {
            region.neighbors = new List<NavRegion>();
            foreach (var neighborId in region.neighborIds)
            {
                if (regionMap.TryGetValue(neighborId, out var neighbor))
                {
                    region.neighbors.Add(neighbor);
                }
            }
        }

        Debug.Log("NavGraph runtime data initialized.");
    }
}