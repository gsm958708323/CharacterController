using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenPos : MonoBehaviour
{
    Transform player;
    Camera camera;
    RectTransform miniMap;
    RectTransform rect;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        player = GameObject.Find("Player").transform;
        miniMap = GameObject.Find("MiniMap").GetComponent<RectTransform>();
        rect = GameObject.Find("PlayerImg").GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        // 将世界坐标映射到视口坐标，范围0~1，左下角(0,0) 右上角(1,1)
        Vector3 viewPos = camera.WorldToViewportPoint(player.position);
        if (viewPos.x > 0.5F)
            print("target is on the right side!");
        else
            print("target is on the left side!");

        Vector2 vector2 = Vector2.zero;
        // 视口坐标转化为ui坐标，将视口坐标按按ui宽高进行缩放。减去偏移值为了让位置居中
        vector2.x = viewPos.x * miniMap.sizeDelta.x - miniMap.sizeDelta.x / 2;
        vector2.y = viewPos.y * miniMap.sizeDelta.y - miniMap.sizeDelta.y / 2;
        rect.anchoredPosition = vector2;
    }
}
