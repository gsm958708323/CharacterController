using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

public class ConfigHelper
{
    private static LuaEnv m_luaEnv = new LuaEnv();
    private static Dictionary<int, string> m_FrameEventParamType;
    private static Dictionary<string, LuaTable> m_CfgDataDict = new Dictionary<string, LuaTable>();
    private static Dictionary<string, Dictionary<int, string>> m_EnumID2CNDict =
    new Dictionary<string, Dictionary<int, string>>();

    public const string enum_cfg_path = "enum_cfg.lua";
    public const string lua_data_path = "/Test/ExportActionData/tab/";
    public const string bullet_event_cfg_path = "bullet_event_cfg.lua";


    public static LuaTable GetBulletConfig(int bulletId)
    {
        var path = $"bullet/{bulletId}.lua";
        return GetConfig(path);
    }

    public static string GetBulletFilePath(int bulletId)
    {
        var path = $"bullet/{bulletId}.lua";
        return Path.GetFullPath(Application.dataPath + lua_data_path + path);
        // return $"{Application.dataPath}{lua_data_path}{path}";
    }

    public static LuaTable GetBulletEventConfig()
    {
        return GetConfigCache(bullet_event_cfg_path);
    }

    public static Dictionary<int, string> GetEnumID2CN(string enumName)
    {
        if (enumName == null)
            enumName = string.Empty;

        if (!m_EnumID2CNDict.TryGetValue(enumName, out var enumCfg))
        {
            enumCfg = new Dictionary<int, string>();
            m_EnumID2CNDict.Add(enumName, enumCfg);
            LuaTable cfg = GetConfigCache(enum_cfg_path);
            LuaTable defineTab = cfg.GetInPath<LuaTable>("ENUM_ID2CN." + enumName);
            if (defineTab != null)
            {
                defineTab.ForEach((int key, string value) => { enumCfg.Add(key, value); });
            }
        }

        return enumCfg;
    }

    public static Dictionary<int, string> GetActionParamType()
    {
        if (m_FrameEventParamType == null)
        {
            LuaTable config = GetConfigCache(enum_cfg_path);

            m_FrameEventParamType = new Dictionary<int, string>();
            LuaTable typeTable = config.GetInPath<LuaTable>("Define.DATA_INPUT_TYPE");
            typeTable.ForEach((string key, int value) =>
            {
                m_FrameEventParamType.Add(value, key);
            });
        }

        return m_FrameEventParamType;
    }

    public static LuaTable GetConfig(string cfgPath, string cfgTableKey = null)
    {
        LuaTable data;
        string tablePath = Path.GetFullPath(Application.dataPath + lua_data_path + cfgPath);
        StreamReader reader = new StreamReader(tablePath, System.Text.Encoding.Default);
        string str = reader.ReadToEnd();
        reader.Close();

        object[] objs = m_luaEnv.DoString(str);
        data = objs[0] as LuaTable;

        if (!string.IsNullOrEmpty(cfgTableKey))
        {
            return GetTableKey(data, cfgTableKey);
        }
        return data;
    }

    public static LuaTable GetConfigCache(string cfgPath, string cfgTableKey = null)
    {
        LuaTable data;
        if (m_CfgDataDict.TryGetValue(cfgPath, out data))
        {
            return data;
        }

        string tablePath = Path.GetFullPath(Application.dataPath + lua_data_path + cfgPath);
        StreamReader reader = new StreamReader(tablePath, System.Text.Encoding.Default);
        string str = reader.ReadToEnd();
        reader.Close();

        object[] objs = m_luaEnv.DoString(str);
        data = objs[0] as LuaTable;
        m_CfgDataDict.Add(cfgPath, data);

        if (!string.IsNullOrEmpty(cfgTableKey))
        {
            return GetTableKey(data, cfgTableKey);
        }
        return data;
    }

    private static LuaTable GetTableKey(LuaTable data, string cfgTableKey)
    {
        if (data.ContainsKey(cfgTableKey))
        {
            return data.Get<string, LuaTable>(cfgTableKey);
        }
        else
        {
            Debug.LogError("GetTableKey 接口获取文件的一级子表的key找不到: " + cfgTableKey);
            return null;
        }
    }

}
