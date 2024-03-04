using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [Header("===========point============")]
    public Transform startPoint;
    public Transform endPoint;
    [Header("===========test1============")]
    public float JumpTime = 1.0f;
    private float curJumpTime = 0.0f;
    private bool isJump = false;
    public float Height = 2.0f;
    public float JumpSpeed = 10f;

    private void OnValidate()
    {
        var dis = (startPoint.position - endPoint.position).magnitude;
        // JumpTime = dis / JumpSpeed;

        Test2Speed = dis / JumpTime;
        paramH = dis / 2;
        paramA = paramK / (paramH * paramH);
    }

    void Start()
    {
        OnValidate();
    }

    void Update()
    {
        Test1();
    }

    void Test1()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // maxHeight = 0;
            ChangeState(true);
        }

        if (isJump)
        {
            Jump();
        }
    }


    void Jump()
    {
        curJumpTime += Time.fixedDeltaTime;
        if (curJumpTime < JumpTime)
        {
            Vector3 offset = GetOffset2();
            transform.position +=  offset;
        }
        else
        {
            ChangeState(false);
            curJumpTime = 0.0f;
        }
    }


    [HideInInspector] public float paramA;
    [HideInInspector] public float paramH;
    [Header("===========test2============")]

    public float paramK;
    [HideInInspector] public float Test2Speed;

    private Vector3 GetOffset2()
    {
        var x = Test2Speed * Time.fixedDeltaTime;
        // y = a(x - h)^2 + k
        var y = -paramA * (x - paramH) * (x - paramH) + paramK;

        Vector3 offset = new Vector3(0, y, x);
        return offset;
    }

    Vector3 GetOffset()
    {
        float t = curJumpTime / JumpTime; // 时间因子（0-1之间）
        var nextPos = Lerp(startPoint.position, endPoint.position, t);
        var x = nextPos.x - transform.position.x;
        var y = nextPos.y - transform.position.y;
        var z = nextPos.z - transform.position.z;

        var heightOffet = Height - 3 > 0 ? Height - 3 : 0;
        float posY = Mathf.Sin(t * Mathf.PI) * heightOffet;

        return new Vector3(x, y + posY, z);
    }

    public Vector3 Lerp(Vector3 value1, Vector3 value2, float amount)
    {
        return new Vector3(
            value1.x + (value2.x - value1.x) * amount,
            value1.y + (value2.y - value1.y) * amount,
            value1.z + (value2.z - value1.z) * amount);
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