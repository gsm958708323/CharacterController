using UnityEngine;
using UnityEditor;
using XLua;
using System;

[LuaCallCSharp]
public class MathUtil
{
    //异或
    public static void MathXor(int num1, int num2, out int value)
    {
        value = num1 ^= num2;
    }

    //与
    public static void MathAnd(int num1, int num2, out int value)
    {
        value = num1 &= num2;
    }

    //或
    public static void MathOr(int num1, int num2, out int value)
    {
        value = num1 |= num2;
    }

    //取反
    public static void MathNot(int num, out int value)
    {
        value = ~num;
    }

    //移位
    public static void MathShift(int num, out int value)
    {
        value = 1 << num;
    }

    //保留指定小数位,默认保留一位
    public static float ReserveDecimals(float num, int digits = 1)
    {
        return (float)Math.Round(num, digits);
    }

    public static bool Approximately(float a, float b)
    {
        return Mathf.Abs(a - b) < 0.001;
    }

    //0-360
    public static float GetFinalAngle(float angle)
    {
        angle = angle % 360;
        return angle >= 0 ? angle : angle + 360;
    }

    //获取面向向量
    public static Vector3 GetDir(float angle)
    {
        var rad = Mathf.Deg2Rad * angle;
        Vector3 v3 = new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad));
        return v3.normalized;
    }

    public static void GetEulerByDir(float dirX, float dirY, float dirZ, out float eulerX, out float eulerY, out float eulerZ)
    {
        var eulerAngles = Quaternion.LookRotation(new Vector3(dirX, dirY, dirZ)).eulerAngles;
        eulerX = eulerAngles.x;
        eulerY = eulerAngles.y;
        eulerZ = eulerAngles.z;

    }
}
