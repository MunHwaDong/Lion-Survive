// 최적화된 NavGraphGenerator.cs
// 에디터에서 노드를 생성하고, 공간 분할 기반 연결을 수행하여 성능 개선

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;

public class NavGraphGenerator : EditorWindow
{
    private const int INF = (int)1e9;

    private float minX = -400f, maxX = 400f;
    private float minZ = -400f, maxZ = 400f;
    private float spacing = 1f;

    private float raycastHeight = 10f;
    private float raycastLength = 20f;
    private LayerMask groundMask = ~0;
    private LayerMask obstacleMask = 0;
    private float connectDistance = 1.5f;
    private float clearanceRadius = 2f; // 추가: 장애물로부터의 최소 거리

    private int regionWidth = 20;
    private int regionHeight = 20;

    private List<NavNode> generatedNodes = new();
    private List<NavRegion> regions = new();

    [MenuItem("Tools/Generate NavGraph Optimized")]
    public static void ShowWindow()
    {
        GetWindow<NavGraphGenerator>("NavGraph Generator Optimized");
    }

    private void OnGUI()
    {
        GUILayout.Label("노드 생성 영역", EditorStyles.boldLabel);
        minX = EditorGUILayout.FloatField("Min X", minX);
        maxX = EditorGUILayout.FloatField("Max X", maxX);
        minZ = EditorGUILayout.FloatField("Min Z", minZ);
        maxZ = EditorGUILayout.FloatField("Max Z", maxZ);
        spacing = EditorGUILayout.FloatField("Spacing", spacing);

        GUILayout.Space(10);
        raycastHeight = EditorGUILayout.FloatField("Raycast Height", raycastHeight);
        raycastLength = EditorGUILayout.FloatField("Raycast Length", raycastLength);
        groundMask = LayerMaskField("Ground Layer Mask", groundMask);
        obstacleMask = LayerMaskField("Obstacle Mask", obstacleMask);
        connectDistance = EditorGUILayout.FloatField("Connection Distance", connectDistance);
        clearanceRadius = EditorGUILayout.FloatField("Clearance Radius", clearanceRadius);

        GUILayout.Space(20);
        
        regionWidth = EditorGUILayout.IntField("Region Width Distance", regionWidth);
        regionHeight = EditorGUILayout.IntField("Region Height Radius", regionHeight);

        if (GUILayout.Button("Generate Nodes"))
        {
            GenerateNodes();
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("Save As NavGraph"))
        {
            SaveAsNavGraph();
        }
        
        if (GUILayout.Button("Save As NavRegionGraph"))
        {
            SaveAsNavRegionGraph();
        }
    }

    private void GenerateNodes()
{
    generatedNodes.Clear();
    regions.Clear();

    int idCounter = 0;
    Dictionary<Vector2Int, List<NavNode>> spatialMap = new();
    Dictionary<Vector2Int, NavRegion> regionMap = new();
    Dictionary<string, List<NavNode>> nodesByRegion = new();

    Vector2Int GridPos(Vector3 pos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(pos.x / connectDistance),
            Mathf.FloorToInt(pos.z / connectDistance));
    }

    Vector2Int GetRegionCoord(Vector3 position)
    {
        int regionX = Mathf.FloorToInt((position.x - minX) / regionWidth);
        int regionZ = Mathf.FloorToInt((position.z - minZ) / regionHeight);
        return new Vector2Int(regionX, regionZ);
    }

    for (float x = minX + 0.5f; x <= maxX; x += spacing)
    {
        for (float z = minZ + 0.5f; z <= maxZ; z += spacing)
        {
            Vector3 origin = new Vector3(x, raycastHeight, z);

            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, raycastLength, groundMask))
            {
                Vector3 nodePos = hit.point;
                Vector2Int regionCoord = GetRegionCoord(nodePos);

                if (!regionMap.TryGetValue(regionCoord, out NavRegion regi))
                {
                    string regionId = $"Region_{regions.Count}";

                    regi = new NavRegion
                    {
                        neighbors = new List<NavRegion>(),
                        neighborIds = new List<string>(),
                        min = new Vector2Int(
                            Mathf.FloorToInt(minX + regionCoord.x * regionWidth),
                            Mathf.FloorToInt(minZ + regionCoord.y * regionHeight)),
                        max = new Vector2Int(
                            Mathf.FloorToInt(minX + (regionCoord.x + 1) * regionWidth),
                            Mathf.FloorToInt(minZ + (regionCoord.y + 1) * regionHeight)),
                        id = regionId
                    };
                    regi.center = new Vector2Int(
                        (regi.min.x + regi.max.x) / 2,
                        (regi.min.y + regi.max.y) / 2);

                    regionMap[regionCoord] = regi;
                    regions.Add(regi);
                }

                bool isTooCloseToObstacle = Physics.CheckSphere(nodePos, clearanceRadius, obstacleMask);

                NavNode node = new NavNode
                {
                    id = $"Node_{idCounter++}",
                    worldPosition = nodePos,
                    connectedNodeIds = new List<string>(),
                    type = isTooCloseToObstacle ? NodeType.BLOCKED : NodeType.WALKABLE,
                    weight = isTooCloseToObstacle ? INF : 0,
                    regionId = regi.id
                };

                generatedNodes.Add(node);

                // 그룹핑
                if (!nodesByRegion.TryGetValue(regi.id, out var regionNodeList))
                {
                    regionNodeList = new List<NavNode>();
                    nodesByRegion[regi.id] = regionNodeList;
                }
                regionNodeList.Add(node);

                var gridPos = GridPos(nodePos);
                if (!spatialMap.ContainsKey(gridPos))
                    spatialMap[gridPos] = new List<NavNode>();
                spatialMap[gridPos].Add(node);
            }
        }
    }

    // 노드 연결
    foreach (var node in generatedNodes)
    {
        //if (node.type == NodeType.BLOCKED) continue;

        var center = GridPos(node.worldPosition);
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dz = -1; dz <= 1; dz++)
            {
                var neighborCell = new Vector2Int(center.x + dx, center.y + dz);
                if (!spatialMap.ContainsKey(neighborCell)) continue;

                foreach (var neighbor in spatialMap[neighborCell])
                {
                    if (node == neighbor) continue; // || neighbor.type == NodeType.BLOCKED) continue;
                    if (Vector3.Distance(node.worldPosition, neighbor.worldPosition) <= connectDistance)
                    {
                        if (!node.connectedNodeIds.Contains(neighbor.id))
                        {
                            node.connectedNodeIds.Add(neighbor.id);
                        }
                    }
                }
            }
        }
    }

    // Region 간 neighbor 연결 (최적화된 방식)
    foreach (var kvp in regionMap)
    {
        var coord = kvp.Key;
        var region = kvp.Value;

        if (!nodesByRegion.TryGetValue(region.id, out var nodesInRegion))
            continue;

        HashSet<string> nodeIds = new(nodesInRegion.Select(n => n.id));

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dz = -1; dz <= 1; dz++)
            {
                if (dx == 0 && dz == 0) continue;

                var neighborCoord = new Vector2Int(coord.x + dx, coord.y + dz);
                if (!regionMap.TryGetValue(neighborCoord, out var neighborRegion))
                    continue;

                if (!nodesByRegion.TryGetValue(neighborRegion.id, out var neighborNodes))
                    continue;

                bool isConnected = neighborNodes.Any(n => n.connectedNodeIds.Any(id => nodeIds.Contains(id)));

                if (isConnected && !region.neighborIds.Contains(neighborRegion.id))
                    region.neighborIds.Add(neighborRegion.id);
            }
        }
    }

    Debug.Log($"Generated {generatedNodes.Count} nodes with clearance radius {clearanceRadius}.");
    Debug.Log($"Generated {regions.Count} regions.");
}


    private void SaveAsNavGraph()
    {
        if (generatedNodes.Count == 0)
        {
            Debug.LogWarning("No nodes to save.");
            return;
        }

        NavGraph graph = CreateInstance<NavGraph>();
        graph.nodes = generatedNodes.ToList();

        string path = EditorUtility.SaveFilePanelInProject("Save NavGraph", "NavGraph", "asset", "Save navgraph asset");
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(graph, path);
            AssetDatabase.SaveAssets();
            Debug.Log("NavGraph saved at " + path);
        }
    }
    
    private void SaveAsNavRegionGraph()
    {
        if (generatedNodes.Count == 0)
        {
            Debug.LogWarning("No nodes to save.");
            return;
        }

        NavRegionGraph regionGraph = CreateInstance<NavRegionGraph>();
        regionGraph.graph = regions.ToList();

        string path = EditorUtility.SaveFilePanelInProject("Save NavRegionGraph", "NavRegionGraph", "asset", "Save NavRegionGraph asset");
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(regionGraph, path);
            AssetDatabase.SaveAssets();
            Debug.Log("NavRegionGraph saved at " + path);
        }
    }

    private LayerMask LayerMaskField(string label, LayerMask selected)
    {
        var layers = InternalEditorUtility.layers;
        int mask = 0;
        for (int i = 0; i < layers.Length; i++)
        {
            if (((1 << LayerMask.NameToLayer(layers[i])) & selected) != 0)
            {
                mask |= 1 << i;
            }
        }

        mask = EditorGUILayout.MaskField(label, mask, layers);
        int newMask = 0;
        for (int i = 0; i < layers.Length; i++)
        {
            if ((mask & (1 << i)) != 0)
            {
                newMask |= 1 << LayerMask.NameToLayer(layers[i]);
            }
        }

        return newMask;
    }
}