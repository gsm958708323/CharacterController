using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinCurvy : MonoBehaviour
{
/// <summary>
    /// 周期
    /// </summary>
    private float lifeTime = 2;
    /// <summary>
    /// 波峰
    /// </summary>
    private float MaxPos = 0;
    /// <summary>
    /// 波谷
    /// </summary>
    private float MinPos = 0;
    /// <summary>
    /// 计算时使用的周期，避免周期为小于零的情况
    /// </summary>
    private float ZQ
    {
        get
        {
            if (lifeTime <= 0)
            {
                return 2;
            }
            else
            {
                return lifeTime;
            }
        }
    }
    /// <summary>
    /// 构造一个正弦曲线
    /// </summary>
    /// <param name="zhouqi"></param>
    /// <param name="minp"></param>
    /// <param name="maxp"></param>
    public SinCurvy(float zhouqi, float minp, float maxp)
    {
        this.lifeTime = zhouqi;
        this.MinPos = minp;
        this.MaxPos = maxp;
    }
    /// <summary>
    /// 根据X轴获取对应Y轴的值
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public float GetZhengXianValue(float x)
    {
        float innner = (2 * (float)Mathf.PI) / ZQ;
        float ya = (float)Mathf.Sin(innner * x);
        float maall = (MaxPos - MinPos) * 0.5f;
        float middle = maall + MinPos;
        return (middle + maall * ya);
    }
    /// <summary>
    /// 修改正弦参数
    /// </summary>
    /// <param name="zhouqi"></param>
    /// <param name="minp"></param>
    /// <param name="maxp"></param>
    public void SetCurvyZhengXian(float zhouqi, float minp, float maxp)
    {
        this.lifeTime = zhouqi;
        this.MinPos = minp;
        this.MaxPos = maxp;
    }
}
