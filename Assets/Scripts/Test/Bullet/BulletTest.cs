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

    void CalColliderInfo()
    {
        var goArray = FindObjectsOfType<GameObject>();
        var targetLayer = LayerMask.NameToLayer("Wall");

        foreach (var item in goArray)
        {
            if (item.layer == targetLayer)
            {
                // Debug.Log($"2====  {item.gameObject.name}" );
                var boxs = item.GetComponents<BoxCollider>();
                foreach (var box in boxs)
                {
                    var pos = box.transform.position + box.center;
                    var realScale = Vector3.Scale(box.transform.lossyScale, box.size);
                    print($"{pos}  {realScale}");
                }
            }
        }
    }

    // 递归计算最底层物体的真实缩放和位置
    public void CalculateColliderInfo(Transform transform, out Vector3 realScale, out Vector3 realPosition)
    {
        realScale = transform.localScale;
        realPosition = transform.localPosition;

        // 遍历父级直到找到指定图层的物体或没有父级
        while (transform.parent != null)
        {
            transform = transform.parent;
            // 累积缩放
            realScale = Vector3.Scale(realScale, transform.localScale);
            // 累积位置
            realPosition += transform.localPosition;
        }
    }

    Vector3 prePos;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CalColliderInfo();
        }

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
