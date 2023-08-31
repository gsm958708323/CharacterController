using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float JumpTime = 1.0f;
    private float curJumpTime = 0.0f;
    private bool isJump = false;
    public float Height = 2.0f;
    public float JumpSpeed = 10f;

    private void OnValidate()
    {
        var dis = (startPoint.position - endPoint.position).magnitude;
        JumpTime = dis / JumpSpeed;
    }

    void Start()
    {
        var dis = (startPoint.position - endPoint.position).magnitude;
        JumpTime = dis / JumpSpeed;
    }

    void Update()
    {
        // if (Input.GetKey(KeyCode.Space))
        // {
        //     Test2();
        // }
        Test1();
    }

    void Test1()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            maxHeight = 0;
            ChangeState(true);
        }
        Jump();
    }

    void Test2()
    {
        //面朝向移动
        this.transform.Translate(Vector3.forward * 5 * Time.deltaTime);
        //左右曲线移动
        curJumpTime += Time.deltaTime * 3;
        this.transform.Translate(Vector3.right * Time.deltaTime * Mathf.Sin(curJumpTime) * 3);
    }

    public float maxHeight;
    public float maxHeightTemp;

    void Jump()
    {
        if (isJump)
        {
            curJumpTime += Time.deltaTime;
            if (curJumpTime < JumpTime)
            {
                float t = curJumpTime / JumpTime;
                transform.position = Vector3.Lerp(startPoint.position, endPoint.position, t);
                // transform.position += (endPoint.position - startPoint.position).normalized * JumpSpeed * Time.deltaTime;
                var x = t * Mathf.PI;
                var heightOffet = Height - 3 > 0 ? Height -3 : 0;
                float posY = Mathf.Sin(x) * heightOffet;
                // float posY = sinCurvy.GetZhengXianValue(x);
                transform.position += posY * Vector3.up;
                if (transform.position.y > maxHeight)
                {
                    maxHeight = transform.position.y;
                    maxHeightTemp = maxHeight - endPoint.position.y;
                }
                Debug.Log($"{t} =====  {Mathf.Rad2Deg * x} ===== {Mathf.Sin(x)}");
            }
            else
            {
                ChangeState(false);
                curJumpTime = 0.0f;
            }
        }
    }

    void ChangeState(bool isJump)
    {
        if (this.isJump != isJump)
        {
            OnStateEnter(isJump);
            this.isJump = isJump;
        }
    }

    void OnStateEnter(bool isJump)
    {
        if (isJump)
        {
        }
        else
        {
            gameObject.transform.position = startPoint.position;
        }
    }
}