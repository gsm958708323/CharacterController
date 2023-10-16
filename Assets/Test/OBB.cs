using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBB : MonoBehaviour
{
    /// <summary>
    /// 本地x,y,z轴的方向
    /// </summary>
    public Vector3[] Axis = new Vector3[3];
    /// <summary>
    /// 沿着x,y,z轴的的半径范围
    /// </summary>
    public Vector3 Size;
    /// <summary>
    /// 中心点
    /// </summary>
    public Vector3 Point;

    // Start is called before the first frame update
    void Start()
    {
        UpdateBox();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBox();
    }

    private void UpdateBox()
    {
        Size = transform.lossyScale / 2;
        Point = transform.position;

        Axis[0] = transform.right;
        Axis[1] = transform.up;
        Axis[2] = transform.forward;
    }
}
