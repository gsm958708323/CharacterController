using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

/// <summary>
/// 最大最小值
/// </summary>
public class AABB : MonoBehaviour
{
    public Vector3 Pos;
    public float halfWidth, halfHeight;

    public Vector3 Min;
    public Vector3 Max;

    public Vector3 RightUp, RightDown, LeftUp, LeftDown;

    private void Start()
    {
        InitBox();
    }

    private void Update()
    {
        // if (Input.GetKeyUp(KeyCode.Q))
        // {
        //     InitBox();
        // }

        InitBox();
    }

    private void InitBox()
    {
        var box = GetComponent<BoxCollider>();
        var scale = Vector3.Scale(transform.lossyScale, box.size);
        var pos = transform.position + box.center;
        Pos = pos;
        halfWidth = scale.x / 2;
        halfHeight = scale.z / 2;

        UpdateBound(pos);
    }

    /// <summary>
    /// 位置发生变化，更新包围盒信息
    /// </summary>
    /// <param name="pos"></param>
    void UpdateBound(Vector3 pos)
    {
        // if (pos.x == 0 && pos.z == 0)
        //     return;

        Min.x = float.MaxValue;
        Min.z = float.MaxValue;
        Max.x = float.MinValue;
        Max.z = float.MinValue;

        RightUp = new Vector3(halfWidth + pos.x, 0, halfHeight + pos.z);
        RightDown = new Vector3(halfWidth + pos.x, 0, -halfHeight + pos.z);
        LeftUp = new Vector3(-halfWidth + pos.x, 0, halfHeight + pos.z);
        LeftDown = new Vector3(-halfWidth + pos.x, 0, -halfHeight + pos.z);
        foreach (var point in new[] { RightUp, RightDown, LeftUp, LeftDown })
        {
            Min.x = Math.Min(point.x, Min.x);
            Min.z = Math.Min(point.z, Min.z);

            Max.x = Math.Max(point.x, Max.x);
            Max.z = Math.Max(point.z, Max.z);
        }
    }
}
