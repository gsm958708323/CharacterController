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
            // maxHeight = 0;
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

    // public float maxHeight;
    // public float maxHeightTemp;

    void Jump()
    {
        if (isJump)
        {
            // curJumpTime += Time.deltaTime;
            // if (curJumpTime < JumpTime)
            // {
            //     float t = curJumpTime / JumpTime;
            //     transform.position = Lerp(startPoint.position, endPoint.position, t);
            //     var x = t * Mathf.PI;
            //     var heightOffet = Height - 3 > 0 ? Height - 3 : 0;
            //     float posY = Mathf.Sin(x) * heightOffet;
            //     transform.position += posY * Vector3.up;
            // }
            // else
            // {
            //     ChangeState(false);
            //     curJumpTime = 0.0f;
            // }

            curJumpTime += Time.deltaTime;
            if (curJumpTime < JumpTime)
            {
                Vector3 offset = GetOffset();
                transform.position += offset;
            }
            else
            {
                ChangeState(false);
                curJumpTime = 0.0f;
            }
        }
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

// using UnityEngine;

// public class PlatformController : MonoBehaviour
// {
//     public Transform target;
//     public float maxHeight = 2f;
//     public float duration = 2f;

//     private Vector3 startPosition;
//     private float startTime;

//     private void Start()
//     {
//         startPosition = transform.position;
//         startTime = Time.time;
//     }

//     private void Update()
//     {
//         if (target != null)
//         {
//             // 计算所需的初速度
//             Vector3 displacement = target.position - startPosition;
//             float displacementY = displacement.y;
//             displacement.y = 0f;
//             float horizontalDisplacement = displacement.magnitude;
//             float time = Mathf.Sqrt(-2f * maxHeight / Physics.gravity.y) + Mathf.Sqrt(2f * (displacementY - maxHeight) / Physics.gravity.y);
//             Vector3 velocity = new Vector3(0f, Mathf.Sqrt(-2f * Physics.gravity.y * maxHeight), 0f) + displacement.normalized * horizontalDisplacement / time;

//             // 应用初速度
//             float elapsedTime = Time.time - startTime;
//             if (elapsedTime < duration)
//             {
//                 transform.position = startPosition + velocity * elapsedTime + 0.5f * Physics.gravity * elapsedTime * elapsedTime;
//             }
//             else
//             {
//                 transform.position = target.position;
//             }
//         }
//     }
// }


// using UnityEngine;

// public class PlatformController : MonoBehaviour
// {
//     public Transform targetPosition; // 目标位置
//     public float totalTimeToReachTarget = 2.0f; // 到达目标位置所需的总时间

//     private Vector3 initialPosition;
//     private Vector3 target;
//     private float startTime;

//     private void Start()
//     {
//         initialPosition = transform.position;
//         target = targetPosition.position;
//         startTime = Time.time;
//     }

//     private void Update()
//     {
//         // 计算经过的时间
//         float elapsedTime = Time.time - startTime;

//         if (elapsedTime < totalTimeToReachTarget)
//         {
//             // 根据抛物线公式计算物体的新位置
//             Vector3 newPosition = CalculateParabolicPosition(elapsedTime);
//             transform.position = newPosition;
//         }
//     }

//     // 根据抛物线公式计算物体在给定时间内的新位置
//     private Vector3 CalculateParabolicPosition(float time)
//     {
//         float t = time / totalTimeToReachTarget;
//         Vector3 position = Vector3.Lerp(initialPosition, target, t);
//         position.y = initialPosition.y + (-Physics.gravity.y * Mathf.Pow(t, 2) * totalTimeToReachTarget) / 2;
//         return position;
//     }

// }
