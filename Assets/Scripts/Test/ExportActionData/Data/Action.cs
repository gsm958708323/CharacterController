using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using XLua;

[HideReferenceObjectPicker]
public class Action : SerializeData
{
    [LabelText("行为")]
    [ValueDropdown("ActionList", DropdownTitle = "行为列表")]
    [OnValueChanged("OnActionChange")]
    public int m_ActionId;

    [LabelText("延迟时间")]
    [PropertyRange(0, 60)]
    public float m_DelayTime;

    [LabelText("参数列表")]
    [ListDrawerSettings(Expanded = true, DraggableItems = false, HideRemoveButton = true, HideAddButton = true)]
    public List<ParamInfo> m_ParameterList = new List<ParamInfo>();

    public virtual IEnumerable<ValueDropdownItem> ActionList()
    {
        List<ValueDropdownItem> list = new List<ValueDropdownItem>();
        var tab = ConfigHelper.GetBulletEventConfig();
        var data = tab.Get<LuaTable>("Data");
        data.ForEach<int, LuaTable>((k, v) =>
        {
            ValueDropdownItem item = new ValueDropdownItem
            {
                Value = k,
                Text = k.ToString() + ":" + v.GetInPath<string>("Name")
            };
            list.Add(item);
        });

        return list;
    }

    public virtual void OnActionChange()
    {
        m_ParameterList.Clear();

        List<ParamInfo> parameterList = new List<ParamInfo>();
        var tab = ConfigHelper.GetBulletEventConfig();
        var data = tab.Get<LuaTable>("Data");
        var actionData = data.Get<int, LuaTable>(m_ActionId);
        if (actionData == null)
            return;

        for (int i = 1; i <= 10; i++)
        {
            var editDesc = actionData.Get<string, object>("EditDesc" + i);
            var editType = actionData.Get<string, object>("EditType" + i);
            if (editDesc == null || editType == null)
                continue;

            var desc = editDesc.ToString();
            var valType = Convert.ToInt32(editType);
            var value = ParamUtil.CreateParam(valType, desc);
            if (value != null)
            {
                var param = new ParamInfo(valType, value, desc);
                m_ParameterList.Add(param);
            }
        }
    }

    public override object GetOutputData()
    {
        base.GetOutputData();
        m_Result.Add("ActionId", m_ActionId);
        m_Result.Add("DelayTime", new Fixed(m_DelayTime));

        List<object> paramList = new List<object>();
        foreach (var item in m_ParameterList)
        {
            var paramData = item.GetOutputData();
            paramList.Add(paramData);
        }

        m_Result.Add("ParamInfo", paramList);

        return m_Result;
    }

    public override void SetLuaData(LuaTable luaData)
    {
        base.SetLuaData(luaData);
        m_ActionId = luaData.Get<int>("ActionId");
        m_DelayTime = luaData.Get<float>("DelayTime");
        var paramInfoData = luaData.Get<LuaTable>("ParamInfo");
        m_ParameterList.Clear();
        for (int i = 0; i < paramInfoData.Length; i++)
        {
            var paramData = paramInfoData.Get<int, LuaTable>(i + 1);
            if (paramData != null)
            {
                var paramInfo = new ParamInfo();
                paramInfo.SetLuaData(paramData);
                m_ParameterList.Add(paramInfo);
            }
        }
    }
}
