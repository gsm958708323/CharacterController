using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Vector3 offset;
    Transform cameraTrans;

    // Start is called before the first frame update
    void Start()
    {
        cameraTrans = Camera.main.transform;
        offset = transform.position - cameraTrans.position;
    }

    // Update is called once per frame
    void Update()
    {
        cameraTrans.position = transform.position - offset;
    }
}
