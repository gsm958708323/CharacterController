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
    }
}
