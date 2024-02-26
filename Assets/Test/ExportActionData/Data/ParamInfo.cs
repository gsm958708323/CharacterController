using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;
using XLua;

[Serializable]
[HideReferenceObjectPicker]
public class ParamInfo : SerializeData
{
    [HideInInspector]
    public int m_ValType;

    [LabelText("说明")]
    [ReadOnly]
    public string m_Desc = string.Empty;

    [LabelText("参数")]
    [ShowInInspector]
    [HideReferenceObjectPicker]
    [InlineProperty]
    public IParamBase m_Value;

    public ParamInfo()
    {
    }

    public ParamInfo(int valType, IParamBase val, string desc)
    {
        m_ValType = valType;
        m_Value = val;
        m_Desc = desc;
    }

    public override object GetOutputData()
    {
        base.GetOutputData();
        var val = m_Value.GetValue();
        m_Result.Add("ValType", m_ValType);
        m_Result.Add("Desc", m_Desc);
        m_Result.Add("Value", val);
        return m_Result;
    }

    public override void SetLuaData(LuaTable luaData)
    {
        base.SetLuaData(luaData);
        m_ValType = luaData.Get<int>("ValType");
        m_Desc = luaData.Get<string>("Desc");
        m_Value = ParamUtil.CreateParam(m_ValType, luaData, m_Desc);
    }
}

public class IntParam : IParamBase, ISetParamValue<int>
{
    public int value;

    public object GetValue()
    {
        return value;
    }
    public void SetValue(int data)
    {
        value = data;
    }
}

public class BoolParam : IParamBase, ISetParamValue<bool>
{
    public bool value;

    public object GetValue()
    {
        return value;
    }
    public void SetValue(bool data)
    {
        value = data;
    }
}

public class StringParam : IParamBase, ISetParamValue<string>
{
    public string value;

    public object GetValue()
    {
        if (value == null)
            return string.Empty;
        return value;
    }
    public void SetValue(string data)
    {
        value = data;
    }
}

public class FixedIntParam : IParamBase, ISetParamValue<int>
{
    public int value;

    public object GetValue()
    {
        return new Fixed(value);
    }
    public void SetValue(int data)
    {
        value = data;
    }
}

public class FixedParam : IParamBase, ISetParamValue<float>
{
    public float value;

    public object GetValue()
    {
        var val = MathUtil.ReserveDecimals(value, 3);
        return new Fixed(val);
    }
    public void SetValue(float data)
    {
        value = data;
    }
}

public class Vector3FixedParam : IParamBase
{
    public float m_X;
    public float m_Y;
    public float m_Z;

    public object GetValue()
    {
        var x = MathUtil.ReserveDecimals(m_X, 3);
        var y = MathUtil.ReserveDecimals(m_Y, 3);
        var z = MathUtil.ReserveDecimals(m_Z, 3);
        return new Fixed3d(new Vector3(x, y, z));
    }

    public void SetValue(float x, float y, float z)
    {
        m_X = x;
        m_Y = y;
        m_Z = z;
    }
}

public class EnumParam : IParamBase
{
    [HideInInspector]
    public string enumName;

    [ValueDropdown(nameof(SelectEnum), SortDropdownItems = true)]
    public int value = 1;

    public EnumParam(string name)
    {
        enumName = name;
    }

    public IEnumerable<ValueDropdownItem> SelectEnum()
    {
        return null;
        // return SceneMarkUtil.SelectEnumList(enumName);
    }

    public object GetValue()
    {
        return value;
    }

    public void SetValue(int data)
    {
        value = data;
    }
}

public class IntListParam : IParamBase
{
    public string IntList = string.Empty;

    public object GetValue()
    {
        return this;
    }

    public void SetValue(LuaTable data)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            if (i != 0)
            {
                sb.Append(",");
            }
            var value = data.Get<int, int>(i + 1);
            sb.Append(value);
        }
        IntList = sb.ToString();
    }
}

public class FixedIntListParam : IParamBase
{
    public string IntList = string.Empty;

    public object GetValue()
    {
        return this;
    }

    public void SetValue(LuaTable data)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            if (i != 0)
            {
                sb.Append(",");
            }
            var value = data.Get<int, int>(i + 1);
            sb.Append(value);
        }
        IntList = sb.ToString();
    }
}

public interface IParamBase
{
    object GetValue();
}
public interface ISetParamValue<T>
{
    void SetValue(T data);
}