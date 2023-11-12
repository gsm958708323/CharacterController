using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector : MonoBehaviour
{
    public Transform player;
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(player.position, player.forward * 3, Color.red);

        var targetDir = target.position - player.position;
        Debug.DrawRay(player.position, targetDir, Color.red);

        if (Vector3.Cross(player.forward, target.position).y > 0)
        {
            print(111);
        }
        else
        {
            print(222);
        }

        var vect1 = player.position;
        var vec2 = player.right;
        Debug.DrawRay(Vector3.zero, vec2, Color.green);
        var vec3 = player.forward;
        Debug.DrawLine(Vector3.zero, vec3, Color.red);

        var vectNew = vec2 + vec3;
        Debug.DrawLine(Vector3.zero, vectNew, Color.black);

        Debug.DrawLine(vect1, vect1 + vectNew, Color.yellow);
    }
}
