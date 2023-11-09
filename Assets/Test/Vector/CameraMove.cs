using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float Speed = 10;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Test1();
        // Test2();
    }

    void Test1()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(horizontalInput, 0, verticalInput).normalized;
        var rot = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        // 使移动方向跟摄像机角度一致
        var rotDir = rot * moveDir;
        var offsetPos = rotDir * Speed * Time.deltaTime;
        transform.position += offsetPos;
    }

    void Test2()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(horizontalInput, 0, verticalInput).normalized;
        var rotDir = moveDir;
        // 此效果为只能前进，无法后退，
        if (horizontalInput != 0 && verticalInput != 0)
        {
            rotDir += new Vector3(1, 0, 1);
            rotDir = rotDir.normalized;
        }
        var offsetPos = rotDir * Speed * Time.deltaTime;
        transform.position += offsetPos;
    }
}
