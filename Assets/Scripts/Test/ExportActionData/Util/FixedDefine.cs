using System.Globalization;
using UnityEngine;

public struct Fixed
{
    public float m_Value;
    public Fixed(float value)
    {
        m_Value = value;
    }
}

public struct Fixed2d
{
    public Vector2 m_Value;
    public Fixed2d(Vector2 value)
    {
        m_Value = value;
    }

    public float this[int index]
    {
        get { return m_Value[index]; }
        set { m_Value[index] = value; }
    }
}

public struct Fixed3d
{
    public Vector3 m_Value;
    public Fixed3d(Vector3 value)
    {
        m_Value = value;
    }

    public float this[int index]
    {
        get { return m_Value[index]; }
        set { m_Value[index] = value; }
    }
    public override string ToString()
    {
        return string.Format("({0:F3}, {1:F3}, {2:F3})", m_Value.x, m_Value.y, m_Value.z);
    }
}


public struct Fixed4d
{
    public Vector4 m_Value;
    public Fixed4d(Vector4 value)
    {
        m_Value = value;
    }

    public float this[int index]
    {
        get { return m_Value[index]; }
        set { m_Value[index] = value; }
    }
}


