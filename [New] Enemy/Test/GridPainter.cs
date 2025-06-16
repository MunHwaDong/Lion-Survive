using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPainter : MonoBehaviour
{
    [SerializeField] private GameObject point;
    
    [SerializeField] private NavGraph navGraph;
    
    [SerializeField] private Material blockedMat;
    [SerializeField] private Material unblockedMat;
    
    [SerializeField] private NavRegionGraph navRegionGraph;
    private List<NavRegion> regions;
    
    void Start()
    {
        regions = navRegionGraph.graph;

        // foreach (var navRegion in navRegionGraph.graph)
        // {
        //     Instantiate(point, new Vector3(navRegion.center.x, 0, navRegion.center.y), Quaternion.identity);
        // }

        // foreach (var p in navGraph.nodes)
        // {
        //     var meshRenderer = Instantiate(point, p.worldPosition, Quaternion.identity).GetComponent<MeshRenderer>();
        //
        //     if (p.type == NodeType.BLOCKED)
        //     {
        //         meshRenderer.material = blockedMat;
        //     }
        //     else
        //     {
        //         meshRenderer.material = unblockedMat;
        //     }
        // }
    }
    
    private void OnDrawGizmos()
    {
        if (regions == null) return;

        Gizmos.color = Color.cyan;
        foreach (var region in regions)
        {
            Vector3 min = new Vector3(region.min.x, 0f, region.min.y);
            Vector3 max = new Vector3(region.max.x, 0f, region.max.y);
            Vector3 center = new Vector3(region.center.x, 0f, region.center.y);
            Vector3 size = max - min;

            Gizmos.DrawWireCube(center + Vector3.up * 0.5f, new Vector3(size.x, 1f, size.z));
        }
    }
}
