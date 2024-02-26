using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
using XLua;

public class ExportActionData : SerializedMonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    [DictionaryDrawerSettings(KeyLabel = "事件类型")]
    public List<ActionGroup> EventGroupList = new List<ActionGroup>();

    [Button("清空")]
    void Clear()
    {
        EventGroupList.Clear();
    }

    [Button("保存")]
    void Save()
    {
        var bulletData = new Dictionary<string, object>();

        var eventDict = new Dictionary<int, object>();
        foreach (var item in EventGroupList)
        {
            eventDict.Add(item.EventType, item.GetOutputData());
        }
        bulletData.Add("event", eventDict);

        var luaStr = ParamUtil.Encode(bulletData);
        if (string.IsNullOrEmpty(luaStr))
        {
            return;
        }
        var filePath = ConfigHelper.GetBulletFilePath(1001);
        if (!File.Exists(filePath))
        {
            using (File.Create(filePath)) { }
        }
        File.WriteAllText(filePath, luaStr);

        Debug.Log("保存成功");
    }

    [Button("加载")]
    void Load()
    {
        var table = ConfigHelper.GetBulletConfig(1001);
        if (table is null)
            return;

        EventGroupList.Clear();
        var eventData = table.GetValueOrDefault<LuaTable>("event");
        eventData.ForEach<int, LuaTable>((eventType, eventData) =>
        {
            var groupData = new ActionGroup()
            {
                EventType = eventType,
            };
            groupData.SetLuaData(eventData);
            EventGroupList.Add(groupData);
        });

        Debug.Log("加载成功");
    }
}
