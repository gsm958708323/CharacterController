using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityComp : MonoBehaviour
{
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true; // 开启重力
    }

    private void OnDestroy()
    {
        rb.useGravity = false; // 开启重力
    }

    // Update is called once per frame
    void Update()
    {

    }
}
