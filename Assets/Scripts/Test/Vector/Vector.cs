using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector : MonoBehaviour
{
    public Transform player;
    public Transform target;
    Vector3 initPos;

    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(player.position, player.forward * 3, Color.red);

        var targetDir = target.position - player.position;
        Debug.DrawRay(player.position, targetDir, Color.red);

        if (Vector3.Cross(player.forward, targetDir.normalized).y > 0)
        {
            print(111);
        }
        else
        {
            print(222);
        }
        print($"{player.forward } {player.forward * 10}");

        var vect1 = player.position;
        var vec2 = player.right;
        Debug.DrawRay(Vector3.zero, vec2, Color.green);
        var vec3 = player.forward;
        Debug.DrawLine(Vector3.zero, vec3, Color.red);

        var vectNew = vec2 + vec3;
        Debug.DrawLine(Vector3.zero, vectNew, Color.black);

        Debug.DrawLine(vect1, vect1 + vectNew, Color.yellow);

        // （计算我前方10m远的坐标）计算a,b两点的相对位移，并在我的正前方计算出目标点b的相对坐标
        var offsetXZ = player.position - target.position;
        var offsetY = offsetXZ.y;
        offsetXZ.y = 0;

        var targetPos = transform.position + transform.forward * offsetXZ.magnitude;
        targetPos.y -= offsetY;
        Debug.DrawLine(transform.position, targetPos, Color.yellow);
    }
}
