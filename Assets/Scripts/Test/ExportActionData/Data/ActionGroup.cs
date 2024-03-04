using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using XLua;

[HideReferenceObjectPicker]
public class ActionGroup : SerializeData
{
    [LabelText("事件类型"), ValueDropdown(nameof(GetEventList))]
    public int EventType = 1;
    List<ValueDropdownItem> GetEventList()
    {
        var list = new List<ValueDropdownItem>();
        var enumDict = ConfigHelper.GetEnumID2CN("EMITTER_TRIGGER_TYPE");
        foreach (var item in enumDict)
        {
            list.Add(new ValueDropdownItem(item.Value, item.Key));
        }
        list = list.OrderBy(t => t.Value).ToList();
        return list;
    }

    [LabelText("行为列表")]
    public List<Action> m_ActionList = new List<Action>();
    // [LabelText("描述")]
    // public string m_Desc = string.Empty;

    public override object GetOutputData()
    {
        base.GetOutputData();
        m_Result.Add(nameof(EventType), EventType);

        List<object> actionList = new List<object>();

        foreach (var item in m_ActionList)
        {
            if (item.m_ActionId > 0)
            {
                var val = item.GetOutputData();
                actionList.Add(val);
            }
        }

        m_Result.Add("ActionList", actionList);

        return m_Result;
    }

    public override void SetLuaData(LuaTable luaData)
    {
        base.SetLuaData(luaData);
        EventType = luaData.GetValueOrDefault<int>("EventType");

        var actionListData = luaData.Get<LuaTable>("ActionList");
        m_ActionList.Clear();
        for (int i = 0; i < actionListData.Length; i++)
        {
            var actionData = actionListData.Get<int, LuaTable>(i + 1);
            if (actionData != null)
            {
                var action = new Action();
                action.SetLuaData(actionData);
                m_ActionList.Add(action);
            }
        }
    }
}
