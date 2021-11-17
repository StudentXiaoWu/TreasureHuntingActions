using System.Collections.Generic;
using UnityEngine;

public static class ExtraClass
{
    public static Vector3[] ToVector3Array(this List<Point> list)
    {
        Vector3[] v3s = new Vector3[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            v3s[i] = new Vector3(list[i].x, list[i].y, 0);
        }
        return v3s;
    }

    public static Vector3Int ToVector3Int(this Vector3 v)
    {
        int x = v.x - Mathf.FloorToInt(v.x) >= 0.5f ? Mathf.CeilToInt(v.x) : Mathf.FloorToInt(v.x);
        int y = v.y - Mathf.FloorToInt(v.y) >= 0.5f ? Mathf.CeilToInt(v.y) : Mathf.FloorToInt(v.y);
        return new Vector3Int(x, y, 0);
    }
}
