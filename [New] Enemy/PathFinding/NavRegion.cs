using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NavRegion
{
    public string id;
    public List<string> neighborIds;

    public Vector2Int min;
    public Vector2Int max;
    public Vector2Int center;
    
    [NonSerialized] public List<NavRegion> neighbors;
}
