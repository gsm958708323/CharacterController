using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 中心位置 + 半径
/// </summary>
public class AABB2 : MonoBehaviour
{
    /// <summary>
    /// 中心位置
    /// </summary>
    public Vector3 Center;

    /// <summary>
    /// 半径
    /// </summary>
    public Vector3 R;

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

    void UpdateBox()
    {
        var box = GetComponent<BoxCollider>();
        var scale = Vector3.Scale(transform.lossyScale, box.size);
        var pos = transform.position + box.center;

        Center = pos;
        R = scale / 2;
    }
}
