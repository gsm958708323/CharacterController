using System;
using System.Collections;
using System.Collections.Generic;
using Flexalon;
using UnityEngine;

public class OBBTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var obb = FindObjectsOfType<OBB>();
        foreach (var box1 in obb)
        {
            foreach (var box2 in obb)
            {
                if (box1 == box2)
                    continue;
                var intersect = Intersect(box1, box2);
                print(intersect);
            }
        }
    }

    private bool Intersect(OBB a, OBB b)
    {
        var t = b.Point - a.Point;
        var ax = a.Axis[0];
        var ay = a.Axis[2];
        var wa = a.Size.x;
        var ha = a.Size.z;
        var bx = b.Axis[0];
        var by = b.Axis[2];
        var wb = b.Size.x;
        var hb = b.Size.z;

        if (dot_abs(t, ax) > wa + dot_abs(wb * bx, ax) + dot_abs(hb * by, ax))
            return false;
        if (dot_abs(t, ay) > ha + dot_abs(wb * bx, ay) + dot_abs(hb * by, ay))
            return false;
        if (dot_abs(t, bx) > wb + dot_abs(wa * ax, bx) + dot_abs(ha * ay, bx))
            return false;
        if (dot_abs(t, by) > hb + dot_abs(wa * ax, by) + dot_abs(ha * ay, by))
            return false;

        return true;
    }

    float dot_abs(Vector3 a, Vector3 b)
    {
        return Mathf.Abs(Vector3.Dot(a, b));
    }

    /*
    https://blog.csdn.net/silangquan/article/details/50812425
    */
}
