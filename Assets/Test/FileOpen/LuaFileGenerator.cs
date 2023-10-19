using UnityEngine;
using UnityEditor;
using System.IO;

using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using XLua;
using System.Text;

public class LuaFileGenerator : MonoBehaviour
{
    private string luaScript = "return {}"; // 存储Lua脚本内容
    private string filePath = ""; // 存储文件路径
    private string fileName = "NewLuaScript.lua"; // 默认文件名

    static LuaEnv Env = new LuaEnv();

    void OnGUI()
    {
        GUILayout.Label("Lua Script:");
        luaScript = GUILayout.TextArea(luaScript, GUILayout.MinHeight(100));

        GUILayout.Label("File Name:");
        fileName = GUILayout.TextField(fileName);

        if (GUILayout.Button("Generate Lua File"))
        {
            SaveLuaScriptToFile();
        }

        if (GUILayout.Button("显示弹窗"))
        {
            ItemTemplateWindow.OpenWindow();
        }

        if (GUILayout.Button("读取lua配置"))
        {
            ReadLuaCfg();
        }
    }

    string luaFile = @"G:\000Work\20230801_demo\Assets\EditorTool\LuaData\tab\scene_item_template\test.lua";
    LuaTable triggerCfgData;

    private void ReadLuaCfg()
    {
        var luaContent = File.ReadAllText(luaFile);
        var objs = Env.DoString(luaContent);
        if (objs == null || objs.Length == 0 || !(objs[0] is LuaTable))
        {
            return;
        }
        LuaTable allConfig = objs[0] as LuaTable;

        triggerCfgData = null;
        void ReadConf(ref LuaTable confData)
        {
            confData = allConfig;
        }
        ReadConf(ref triggerCfgData);
        var str = PrintLuaTableToString(triggerCfgData);
        print(str);


        var newTestTable = new TestTable() { A = 1, B = 2 };
        testTable = null;
        void ReadTest(ref TestTable table)
        {
            table = newTestTable;
        }
        ReadTest(ref testTable);
    }

    TestTable testTable;
    public class TestTable
    {
        public int A;
        public int B;
    }

    string PrintLuaTableToString(LuaTable table, string indentation = "")
    {
        StringBuilder result = new StringBuilder();

        table.ForEach<object, object>((key, value) =>
        {
            string formattedKey = key.ToString();
            string formattedValue = value.ToString();

            // 如果值是LuaTable，递归打印
            if (value is LuaTable)
            {
                result.AppendLine($"{indentation}{formattedKey}: (LuaTable)");
                result.Append(PrintLuaTableToString(value as LuaTable, indentation + "    "));
            }
            else
            {
                result.AppendLine($"{indentation}{formattedKey}: {formattedValue}");
            }
        });

        return result.ToString();
    }

    void Print(LuaTable table, string indentation = "")
    {
        table.ForEach<object, object>((key, value) =>
        {
            string formattedKey = key.ToString();
            string formattedValue = value.ToString();

            // 如果值是LuaTable，递归打印
            if (value is LuaTable)
            {
                print($"{indentation}{formattedKey}: (LuaTable)");
                Print(value as LuaTable, indentation + "    ");
            }
            else
            {
                print($"{indentation}{formattedKey}: {formattedValue}");
            }
        });
    }


    private void SaveLuaScriptToFile()
    {
        filePath = EditorUtility.SaveFilePanel("Save Lua File", "", fileName, "lua");
        if (!string.IsNullOrEmpty(filePath))
        {
            File.WriteAllText(filePath, luaScript);
            Debug.Log("Lua script saved to: " + filePath);
        }
    }
}

public class ItemTemplateWindow : OdinEditorWindow
{
    [MenuItem("MyTools/My Odin Window")]
    public static void OpenWindow()
    {
        GetWindow<ItemTemplateWindow>().Show();
    }

    string Save_Directory = @"G:\000Work\20230801_demo\Assets\EditorTool\LuaData\tab\scene_item_template";

    [LabelText("选择文件")]
    [HorizontalGroup("TopGroup")]
    [ValueDropdown(nameof(OnValueDropdown), DropdownTitle = "文件列表", SortDropdownItems = false)]
    public string FileName;

    [LabelText("起始Id")]
    public int startId = 100000;

    [Button("读取模板到场景")]
    [HorizontalGroup("DownGroup")]
    public void Read()
    {
        Debug.Log($"选择文件：{Save_Directory}/{FileName}.lua 起始Id：{startId}");
    }

    private IEnumerable<ValueDropdownItem> OnValueDropdown()
    {
        List<ValueDropdownItem> list = new List<ValueDropdownItem>();

        foreach (var file in Directory.GetFiles(Save_Directory, "*.lua"))
        {
            ValueDropdownItem item = new ValueDropdownItem();
            var name = Path.GetFileNameWithoutExtension(file);
            item.Value = name;
            item.Text = name;
            list.Add(item);
        }

        return list;
    }
}
