using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTest : MonoBehaviour
{
    public float speed = 10;
    bool isShoot = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    Vector3 prePos;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isShoot = true;
            transform.position = Vector3.zero;
        }

        if (isShoot)
        {
            prePos = transform.position;
            var velocity = transform.forward * Time.deltaTime * speed;
            transform.Translate(velocity);

            var dis = (transform.position - prePos).magnitude;
            if (Physics.Raycast(prePos, transform.position - prePos, dis, LayerMask.GetMask("Wall")))
            {
                isShoot = false;
                Debug.Log("发生碰撞222");
            }
            Debug.DrawLine(prePos, transform.position, Color.red);
        }
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     isShoot = false;
    //     Debug.Log("发生碰撞");
    // }
}
