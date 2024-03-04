using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPause : MonoBehaviour
{
    SceneSpeedRateObj timeScaleObj;
    // Start is called before the first frame update
    void Start()
    {
        // 模拟子弹时间 先慢后快 速率从0.05f渐变到1
        timeScaleObj = new SceneSpeedRateObj();
        timeScaleObj.Enter(0, 3, 0.5f, 0.05f);
    }

    // Update is called once per frame
    void Update()
    {
        timeScaleObj.Tick(Time.deltaTime);
    }
}
