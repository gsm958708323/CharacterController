using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class SerializeData
{
    protected Dictionary<string, object> m_Result = new Dictionary<string, object>();

    public virtual object GetOutputData()
    {
        if (m_Result == null)
            m_Result = new Dictionary<string, object>();
        m_Result.Clear();
        return null;
    }

    public virtual void SetLuaData(LuaTable luaData)
    {

    }
}
