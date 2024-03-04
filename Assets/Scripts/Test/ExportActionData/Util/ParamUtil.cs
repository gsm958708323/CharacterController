using UnityEngine;
using UnityEditor;
using XLua;
using System;
using System.Linq;
using System.Text;
using System.Collections;

public static class ParamUtil
{
    public static bool LuaTableToVector3(LuaTable table, out Vector3 result)
    {
        if (table == null || table.Length < 3)
        {
            result = Vector3.zero;
            return false;
        }
        float x, y, z = 0;
        table.Get<int, float>(1, out x);
        table.Get<int, float>(2, out y);
        table.Get<int, float>(3, out z);
        result = new Vector3(x, y, z);
        return true;
    }

    public static T GetValueOrDefault<T>(this LuaTable data, string key)
    {
        if (data.ContainsKey(key))
        {
            return data.Get<T>(key);
        }
        return default(T);
    }

    /// <summary>
    /// 获取table中的值，如果为空返回默认值
    /// </summary>
    /// <typeparam name="R">期望类型</typeparam>
    /// <typeparam name="I">输入类型</typeparam>
    /// <param name="data"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static TValue GetValueOrDefault<TKey, TValue>(this LuaTable data, TKey key)
    {
        if (data.ContainsKey(key))
        {
            return data.Get<TKey, TValue>(key);
        }
        return default(TValue);
    }

    public static IParamBase CreateParam(int valType, string desc)
    {
        string typeStr;
        if (ConfigHelper.GetActionParamType().TryGetValue(valType, out typeStr))
        {
            typeStr = typeStr.ToLower();
            if (typeStr == "bool")
            {
                var boolParam = new BoolParam();
                return boolParam;
            }
            else if (typeStr == "fixed")
            {
                var fixedParam = new FixedParam();
                return fixedParam;
            }
            else if (typeStr == "fixed_int")
            {
                var intParam = new FixedIntParam();
                return intParam;
            }
            else if (typeStr == "int")
            {
                var intParam = new IntParam();
                return intParam;
            }
            else if (typeStr == "string")
            {
                var strParam = new StringParam();
                return strParam;
            }
            else if (typeStr == "vector3")
            {
                var v3Param = new Vector3FixedParam();
                return v3Param;
            }
            else if (typeStr == "fixed_vector3")
            {
                var v3Param = new Vector3FixedParam();
                return v3Param;
            }
            else if (typeStr == "enum")
            {
                var enumParam = new EnumParam(desc);
                // 默认初始化取第一个枚举
                var enumsList = enumParam.SelectEnum();
                enumParam.SetValue((int)(enumsList.First().Value));
                return enumParam;
            }
            else if (typeStr == "int_list")
            {
                var tableParam = new IntListParam();
                return tableParam;
            }
            else if (typeStr == "fixed_int_list")
            {
                var tableParam = new FixedIntListParam();
                return tableParam;
            }
            else
            {
                Debug.LogError("暂不支持的数据类型 ：" + typeStr);
            }
        }
        return null;
    }

    public static IParamBase CreateParam(int valType, LuaTable luaData, string desc)
    {
        string typeStr;
        if (ConfigHelper.GetActionParamType().TryGetValue(valType, out typeStr))
        {
            typeStr = typeStr.ToLower();
            if (typeStr == "bool")
            {
                var valData = luaData.Get<string, bool>("Value");
                var boolParam = new BoolParam();
                boolParam.SetValue(valData);
                return boolParam;
            }
            else if (typeStr == "fixed")
            {
                var valData = luaData.Get<string, float>("Value");
                var fixedParam = new FixedParam();
                fixedParam.SetValue(valData);
                return fixedParam;
            }
            else if (typeStr == "fixed_int")
            {
                var valData = luaData.Get<string, float>("Value");
                var fixedIntParam = new FixedIntParam();
                fixedIntParam.SetValue((int)valData);
                return fixedIntParam;
            }
            else if (typeStr == "int")
            {
                var valData = luaData.Get<string, int>("Value");
                var intParam = new IntParam();
                intParam.SetValue(valData);
                return intParam;
            }
            else if (typeStr == "string")
            {
                var valData = luaData.Get<string, string>("Value");
                var strParam = new StringParam();
                strParam.SetValue(valData);
                return strParam;
            }
            else if (typeStr == "vector3")
            {
                var valData = luaData.Get<string, LuaTable>("Value");
                var v3Param = new Vector3FixedParam();
                var x = valData.Get<int, float>(1);
                var y = valData.Get<int, float>(2);
                var z = valData.Get<int, float>(3);
                v3Param.SetValue(x, y, z);
                return v3Param;
            }
            else if (typeStr == "fixed_vector3")
            {
                var valData = luaData.Get<string, LuaTable>("Value");
                var v3Param = new Vector3FixedParam();
                var x = valData.Get<int, float>(1);
                var y = valData.Get<int, float>(2);
                var z = valData.Get<int, float>(3);
                v3Param.SetValue(x, y, z);
                return v3Param;
            }
            else if (typeStr == "enum")
            {
                var valData = luaData.Get<string, int>("Value");
                var enumParam = new EnumParam(desc);
                enumParam.SetValue(valData);
                return enumParam;
            }
            else if (typeStr == "int_list")
            {
                var valData = luaData.Get<string, LuaTable>("Value");
                var tableParam = new IntListParam();
                tableParam.SetValue(valData);
                return tableParam;
            }
            else if (typeStr == "fixed_int_list")
            {
                var valData = luaData.Get<string, LuaTable>("Value");
                var fixedParam = new FixedIntListParam();
                fixedParam.SetValue(valData);
                return fixedParam;
            }
            else
            {
                Debug.LogError("暂不支持的数据类型 ：" + typeStr);
            }
        }
        return null;
    }

    private const int BUILDER_CAPACITY = 2000;

    public static string Encode(object lua)
    {
        StringBuilder builder = new StringBuilder(BUILDER_CAPACITY);
        int layer = 0;
        builder.Append("return ");
        bool success = SerializeValue(lua, builder, ref layer);
        return (success ? builder.ToString() : null);
    }

    public static bool SerializeValue(object value, StringBuilder builder, ref int layer)
    {
        layer++;
        bool success = true;
        try
        {
            if (value == null)
            {
                builder.Append("nil");
            }
            else if (value is IntListParam)
            {
                success = SerializeIntList(value, builder);
            }
            else if (value is FixedIntListParam)
            {
                success = SerializeFixedIntList(value, builder);
            }
            else if (value is string)
            {
                success = SerializeString((string)value, builder);
            }
            else if (value is IDictionary)
            {
                success = SerializeObject((IDictionary)value, builder, layer);
            }
            else if (value is IList)
            {
                success = SerializeArray(value as IList, builder, layer);
            }
            else if ((value is Boolean) && ((Boolean)value == true))
            {
                builder.Append("true");
            }
            else if ((value is Boolean) && ((Boolean)value == false))
            {
                builder.Append("false");
            }
            else if (value.GetType() == typeof(float))
            {
                float result = (float)value;
                success = SerializeNumber(result, builder);
            }
            else if (value.GetType() == typeof(double))
            {
                double result = (double)value;
                success = SerializeNumber(result, builder);
            }
            else if (value.GetType() == typeof(int))
            {
                success = SerializeNumber((int)value, builder);
            }
            else if (value.GetType() == typeof(long))
            {
                success = SerializeNumber((long)value, builder);
            }
            else if (value.GetType() == typeof(Color))
            {
                success = SerializeColor((Color)value, builder);
            }
            else if (value.GetType() == typeof(Vector3))
            {
                success = SerializeVector3((Vector3)value, builder);
            }
            else if (value.GetType() == typeof(Vector3Int))
            {
                success = SerializeVector3Int((Vector3Int)value, builder);
            }
            else if (value.GetType() == typeof(Vector2))
            {
                success = SerializeVector2((Vector2)value, builder);
            }
            else if (value.GetType() == typeof(Vector2Int))
            {
                success = SerializeVector2Int((Vector2Int)value, builder);
            }
            else if (value.GetType() == typeof(Vector4))
            {
                success = SerializeVector4((Vector4)value, builder);
            }
            else if (value.GetType() == typeof(Fixed))
            {
                Fixed result = (Fixed)value;
                success = SerializeFixedNumber(result.m_Value, builder);
            }
            else if (value.GetType() == typeof(Fixed2d))
            {
                Fixed2d result = (Fixed2d)value;
                success = SerializeFixedVector2((Fixed2d)value, builder);
            }
            else if (value.GetType() == typeof(Fixed3d))
            {
                Fixed3d result = (Fixed3d)value;
                success = SerializeFixedVector3((Fixed3d)value, builder);
            }
            else if (value.GetType() == typeof(Fixed4d))
            {
                Fixed4d result = (Fixed4d)value;
                success = SerializeFixedVector4((Fixed4d)value, builder);
            }
            else
            {
                Debug.LogError("ObjectToLua Wrong :" + value.ToString());
                success = false;
            }
            layer--;
        }
        catch (Exception e)
        {
            Debug.LogError("ObjectToLua Wrong :" + value.GetType());
            success = false;
        }
        return success;
    }

    public static bool SerializeString(string str, StringBuilder builder)
    {
        builder.Append("\"");
        for (int i = 0; i < str.Length; ++i)
        {
            if (str[i] < ' ' || str[i] == '"' || str[i] == '\\')
            {
                builder.Append('\\');
                int j = "\"\\\n\r\t\b\f".IndexOf(str[i]);
                if (j >= 0)
                    builder.Append("\"\\nrtbf"[j]);
                else
                    builder.AppendFormat("u{0:X4}", (UInt32)str[i]);
            }
            else
            {
                builder.Append(str[i]);
            }

        }
        builder.Append("\"");
        return true;
    }

    public static bool SerializeObject(IDictionary anObject, StringBuilder builder, int layer)
    {
        builder.Append("{");
        builder.Append("\n");

        bool first = true;

        var keys = anObject.Keys;
        foreach (var key in keys)
        {
            object value = anObject[key];

            if (!first)
            {
                builder.Append("\n");
            }
            for (int i = 0; i < layer; i++)
            {
                builder.Append("\t");
            }
            builder.Append("[");
            if (key.GetType() == typeof(int))
            {
                SerializeNumber((int)key, builder);
            }
            else if (key.GetType() == typeof(long))
            {
                SerializeNumber((long)key, builder);
            }
            else
            {
                SerializeString(key.ToString(), builder);
            }
            builder.Append("]");
            builder.Append(" = ");
            if (!SerializeValue(value, builder, ref layer))
            {
                return false;
            }
            builder.Append(",");
            first = false;
        }

        builder.Append("\n");
        for (int i = 0; i < layer; i++)
        {
            builder.Append("\t");
        }
        builder.Append("}");
        return true;
    }

    public static bool SerializeArray(IList anArray, StringBuilder builder, int layer)
    {
        builder.Append("{");
        builder.Append("\n");

        bool first = true;
        for (int i = 0; i < anArray.Count; i++)
        {
            object value = anArray[i];
            if (!first)
            {
                builder.Append("\n");
            }

            for (int j = 0; j < layer; j++)
            {
                builder.Append("\t");
            }

            builder.Append("[");
            SerializeNumber(i + 1, builder);
            builder.Append("]");
            builder.Append(" = ");

            if (!SerializeValue(value, builder, ref layer))
            {
                return false;
            }
            builder.Append(",");
            first = false;
        }
        builder.Append("\n");
        for (int i = 0; i < layer; i++)
        {
            builder.Append("\t");
        }
        builder.Append("}");
        return true;
    }

    public static bool SerializeNumber(int number, StringBuilder builder)
    {
        builder.Append(number);
        return true;
    }

    public static bool SerializeNumber(long number, StringBuilder builder)
    {
        builder.Append(number);
        return true;
    }

    public static bool SerializeNumber(float number, StringBuilder builder)
    {
        builder.Append(number);
        return true;
    }

    public static bool SerializeNumber(double number, StringBuilder builder)
    {
        builder.Append(number);
        return true;
    }

    public static bool SerializeColor(Color color, StringBuilder builder)
    {
        builder.Append("{");
        SerializeNumber(color.r, builder);
        builder.Append(",");
        SerializeNumber(color.g, builder);
        builder.Append(",");
        SerializeNumber(color.b, builder);
        builder.Append(",");
        SerializeNumber(color.a, builder);
        builder.Append("}");
        return true;
    }

    public static bool SerializeVector3(Vector3 vec3, StringBuilder builder)
    {
        builder.Append("{");
        SerializeNumber(vec3.x, builder);
        builder.Append(",");
        SerializeNumber(vec3.y, builder);
        builder.Append(",");
        SerializeNumber(vec3.z, builder);
        builder.Append("}");
        return true;
    }

    public static bool SerializeVector3Int(Vector3Int vec3, StringBuilder builder)
    {
        builder.Append("{");
        SerializeNumber(vec3.x, builder);
        builder.Append(",");
        SerializeNumber(vec3.y, builder);
        builder.Append(",");
        SerializeNumber(vec3.z, builder);
        builder.Append("}");
        return true;
    }

    public static bool SerializeVector2(Vector2 vec2, StringBuilder builder)
    {
        builder.Append("{");
        SerializeNumber(vec2.x, builder);
        builder.Append(",");
        SerializeNumber(vec2.y, builder);
        builder.Append("}");
        return true;
    }

    public static bool SerializeVector2Int(Vector2Int vec2, StringBuilder builder)
    {
        builder.Append("{");
        SerializeNumber(vec2.x, builder);
        builder.Append(",");
        SerializeNumber(vec2.y, builder);
        builder.Append("}");
        return true;
    }

    public static bool SerializeVector4(Vector4 vec2, StringBuilder builder)
    {
        builder.Append("{");
        SerializeNumber(vec2.x, builder);
        builder.Append(",");
        SerializeNumber(vec2.y, builder);
        builder.Append(",");
        SerializeNumber(vec2.z, builder);
        builder.Append(",");
        SerializeNumber(vec2.w, builder);
        builder.Append("}");
        return true;
    }

    public static bool SerializeFixedNumber(float number, StringBuilder builder)
    {
        //保留三位小数
        var num = MathUtil.ReserveDecimals(number, 3);
        builder.Append(num);
        // builder.Append("x"); // todo 定点数自定义
        return true;
    }

    public static bool SerializeFixedVector2(Fixed2d fixed2, StringBuilder builder)
    {
        Vector2 vec2 = fixed2.m_Value;
        builder.Append("{");
        SerializeFixedNumber(vec2.x, builder);
        builder.Append(",");
        SerializeFixedNumber(vec2.y, builder);
        builder.Append("}");
        return true;
    }

    public static bool SerializeFixedVector3(Fixed3d fixed3, StringBuilder builder)
    {
        Vector3 vec3 = fixed3.m_Value;
        builder.Append("{");
        SerializeFixedNumber(vec3.x, builder);
        builder.Append(",");
        SerializeFixedNumber(vec3.y, builder);
        builder.Append(",");
        SerializeFixedNumber(vec3.z, builder);
        builder.Append("}");
        return true;
    }
    public static bool SerializeFixedVector4(Fixed4d fixed4, StringBuilder builder)
    {
        Vector4 vec4 = fixed4.m_Value;
        builder.Append("{");
        SerializeFixedNumber(vec4.x, builder);
        builder.Append(",");
        SerializeFixedNumber(vec4.y, builder);
        builder.Append(",");
        SerializeFixedNumber(vec4.z, builder);
        builder.Append(",");
        SerializeFixedNumber(vec4.w, builder);
        builder.Append("}");
        return true;
    }

    public static bool SerializeIntList(object value, StringBuilder builder)
    {
        var data = value as IntListParam;

        if (data.IntList.Equals(""))
        {
            builder.Append("{}");
            return true;
        }

        var strList = data.IntList.Split(",");
        builder.Append("{");
        for (int i = 0; i < strList.Length; i++)
        {
            if (i != 0)
            {
                builder.Append(",");
            }
            try
            {
                int num = int.Parse(strList[i]);
                SerializeNumber(num, builder);
            }
            catch
            {
                Debug.LogError("整数列表 解析数字失败！" + strList[i]);
            }
        }
        builder.Append("}");
        return true;
    }

    public static bool SerializeFixedIntList(object value, StringBuilder builder)
    {
        var data = value as FixedIntListParam;

        if (data.IntList.Equals(""))
        {
            builder.Append("{}");
            return true;
        }

        var strList = data.IntList.Split(",");
        builder.Append("{");
        for (int i = 0; i < strList.Length; i++)
        {
            if (i != 0)
            {
                builder.Append(",");
            }
            try
            {
                int num = int.Parse(strList[i]);
                SerializeFixedNumber(num, builder);
            }
            catch
            {
                Debug.LogError("定点整数列表 解析数字失败！" + strList[i]);
            }
        }
        builder.Append("}");
        return true;
    }
}
