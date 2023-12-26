using System;
using System.Collections;
using System.Collections.Generic;

public enum TimeScaleType
{
    None,
    FadeIn,
    FadeOut,
    Duration,
}

/// <summary>
/// 在指定时间内调用淡入淡出方法，并传入速率值
/// </summary>
public class TimeScaleObj
{
    float fadeIn = 0;
    float duration = 0;
    float fadeOut = 0;

    float totalTime = 0;
    protected float curTime = 0;

    TimeScaleType curScaleType;

    public virtual void Enter(float fadeIn = 0, float duration = 0, float fadeOut = 0)
    {
        this.fadeIn = fadeIn;
        this.duration = duration;
        this.fadeOut = fadeOut;
        curTime = 0;
        totalTime = fadeIn + duration + fadeOut;

        // 跟随初始值找到目标状态
        if (fadeIn == 0 && duration == 0 && fadeOut == 0)
        {
            FinishTime();
        }
        else
        {
            if (fadeIn > 0)
            {
                curScaleType = TimeScaleType.FadeIn;
                FadeInTime(0);
            }
            else if (duration > 0)
            {
                curScaleType = TimeScaleType.Duration;
                DurationTime();
            }
            else if (fadeOut > 0)
            {
                curScaleType = TimeScaleType.FadeOut;
                FadeOutTime(0);
            }
        }
    }

    public virtual void Exit()
    {
        if (curScaleType != TimeScaleType.None)
        {
            curScaleType = TimeScaleType.None;
            FinishTime();
        }
    }

    public void Tick(float deltaTime)
    {
        if (curScaleType == TimeScaleType.None)
            return;
        curTime += deltaTime;

        if (curScaleType == TimeScaleType.FadeIn)
        {
            if (curTime < fadeIn)
            {
                // 小于渐入时间，调用渐入函数（趋向于1）
                FadeInTime(curTime / fadeIn);
            }
            else
            {
                if (duration > 0)
                {
                    // 进入稳定阶段
                    curScaleType = TimeScaleType.Duration;
                    DurationTime();
                }
                else if (fadeOut > 0)
                {
                    // 进入减出阶段
                    curScaleType = TimeScaleType.FadeOut;
                    FadeOutTime(0);
                }
                else
                {
                    Exit();
                }
            }
        }
        else if (curScaleType == TimeScaleType.Duration)
        {
            if (curTime >= fadeIn + duration)
            {
                if (fadeOut > 0)
                {
                    curScaleType = TimeScaleType.FadeOut;
                    FadeOutTime(0);
                }
                else
                {
                    Exit();
                }
            }
        }
        else if (curScaleType == TimeScaleType.FadeOut)
        {
            var time = curTime - fadeIn - duration;
            if (time < fadeOut)
            {
                FadeOutTime(time / fadeOut);
            }
            else
            {
                Exit();
            }
        }

    }

    protected virtual void FadeOutTime(float v)
    {
    }

    protected virtual void DurationTime()
    {
    }

    protected virtual void FadeInTime(float v)
    {
    }

    protected virtual void FinishTime()
    {
    }
}
