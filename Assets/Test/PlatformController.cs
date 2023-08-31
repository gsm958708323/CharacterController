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
    private float MoveSpeed = 5.0f;
    public float Height = 2.0f;
    public float JumpSpeed = 1f;

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
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ChangeState(true);
        }

        if (isJump)
        {
            curJumpTime += Time.deltaTime;

            if (curJumpTime < JumpTime)
            {
                float t = curJumpTime / JumpTime;
                transform.position = Vector3.Lerp(startPoint.position, endPoint.position, t);
                // transform.position += (endPoint.position - startPoint.position).normalized * JumpSpeed * Time.deltaTime;
                // transform.position += Vector3.up * Mathf.Sin(t * Mathf.PI) * (endPoint.position.y - startPoint.position.y);
                var x = t * Mathf.PI;
                float posY = Mathf.Sin(x) * Height;
                transform.position += posY * Vector3.up;

                Debug.Log($"{t} =====  {Mathf.Rad2Deg * x} ===== {Mathf.Sin(x)}");
            }
            else
            {
                ChangeState(false);
                curJumpTime = 0.0f;
            }
        }

        // curJumpTime += Time.deltaTime;
        // if (curJumpTime < JumpTime)
        // {
        //     float t = curJumpTime / JumpTime;
        //     transform.position = Vector3.Lerp(startPoint.position, endPoint.position, t);

        //     var x = t * Mathf.PI;
        //     float posY = Mathf.Sin(x) * Height;
        //     transform.position += posY * Vector3.up;

        //     // transform.position += (endPoint.position - startPoint.position).normalized * JumpSpeed * Time.deltaTime;
        //     // transform.position += Vector3.up * Mathf.Sin(t * Mathf.PI) * Height;
        //     // Debug.Log($" {curJumpTime} {t} ===== {t * Mathf.PI} ======= {Mathf.Sin(t * Mathf.PI)} ======= {Mathf.Sin(t * Mathf.PI) * Height}");

        //     // Debug.Log($"{t} {Time.deltaTime} =====  {Mathf.Rad2Deg * x}  {Mathf.Sin(x)}");
        // }
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.gameObject.CompareTag("Platform"))
    //     {
    //         ChangeState(true);
    //     }
    // }

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
            // var comp = gameObject.GetComponent<GravityComp>();
            // if (comp)
            //     Destroy(comp);
        }
        else
        {
            // gameObject.AddComponent<GravityComp>();
            gameObject.transform.position = startPoint.position;
        }
    }

}