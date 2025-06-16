using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NavNode
{
    public string id;
    public Vector3 worldPosition;
    public List<string> connectedNodeIds;
    public NodeType type;
    public LayerMask layer;
    public float weight;
    
    public string regionId;
    
    [NonSerialized] public List<NavNode> connectedNodes;
    
    // A* 상태값 캐싱
    [NonSerialized] public float gScore;
    [NonSerialized] public float fScore;
    [NonSerialized] public NavNode cameFrom;
    [NonSerialized] public bool visited;
    
    [NonSerialized] public NavRegion region;
}