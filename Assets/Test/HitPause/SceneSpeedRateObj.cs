using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSpeedRateObj : TimeScaleObj
{
    float defaultValue;
    float baseValue;
    float targetValue;
    float curValue;

    public void Enter(float fadeIn = 0, float duration = 0, float fadeOut = 0, float targetValue = 0)
    {
        defaultValue = baseValue = curValue = 1;
        this.targetValue = targetValue;
        base.Enter(fadeIn, duration, fadeOut);
    }

    public override void Exit()
    {

        base.Exit();
    }

    protected override void FadeInTime(float v)
    {
        base.FadeInTime(v);
        curValue = Mathf.Lerp(baseValue, targetValue, v);

        Debugger.Log($"渐入 {curTime} {v} {curValue}");
    }

    protected override void DurationTime()
    {
        base.DurationTime();
        curValue = targetValue;
        baseValue = targetValue;

        Debugger.Log($"持续 {curTime} {curValue}");
    }

    protected override void FadeOutTime(float v)
    {
        base.FadeOutTime(v);
        // 从targetValue淡出到defaultValue，这里用baseValue防止targetdValue被污染
        curValue = Mathf.Lerp(baseValue, defaultValue, v);

        Debugger.Log($"渐出 {curTime} {v} {curValue}");
    }

    protected override void FinishTime()
    {
        base.FinishTime();
        defaultValue = baseValue = targetValue = curValue = 1;

        Debugger.Log($"结束 {curTime} {curValue}");
    }

    public float GetRate()
    {
        return curValue;
    }
}
