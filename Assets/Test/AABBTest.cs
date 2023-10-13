using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABBTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var aabb = FindObjectsOfType<AABB>();
        foreach (var box1 in aabb)
        {
            foreach (var box2 in aabb)
            {
                if (box1 == box2)
                    continue;
                var intersect = Intersect(box1, box2);
                print(intersect);
            }
        }
    }

    private bool Intersect(AABB box1, AABB box2)
    {
        if (!IntersectLine(box1.Min.x, box1.Max.x, box2.Min.x, box2.Max.x))
            return false;

        if (!IntersectLine(box1.Min.z, box1.Max.z, box2.Min.z, box2.Max.z))
            return false;

        return true;
    }

    /// <summary>
    /// 线段是否有相交区间
    /// </summary>
    bool IntersectLine(float min1, float max1, float min2, float max2)
    {
        // 不相交判断：box1在box2的右边 box1在box2的左边
        var intersect = min1 > max2 || max1 < min2;
        return !intersect;
    }
}
