using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;


public class MyCustomEditor : EditorWindow
{
    [MenuItem("Window/UI Toolkit/MyCustomEditor")]
    public static void ShowExample()
    {
        MyCustomEditor wnd = GetWindow<MyCustomEditor>();
        wnd.titleContent = new GUIContent("MyCustomEditor");
    }

    [SerializeField]
    private VisualTreeAsset m_UXMLTree;

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // // VisualElements objects can contain other VisualElement following a tree hierarchy.
        // VisualElement label = new Label("Hello World! From C#");
        // root.Add(label);

        // // Import UXML
        // var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Test/UIToolKit/MyCustomEditor.uxml");
        // VisualElement labelFromUXML = visualTree.Instantiate();
        // root.Add(labelFromUXML);

        // root.Add(m_UXMLTree.Instantiate());

        Label label = new Label("These controls were created using C# code.");
        root.Add(label);
        Button button = new Button();
        button.name = "button3";
        button.text = "This is button3.";
        root.Add(button);

        root.Add(new Button() { name = "button2", text = "button2" });

        SetButtonHandler();
    }

    private void SetButtonHandler()
    {
        var root = rootVisualElement;
        var buttons = root.Query<Button>();
        buttons.ForEach((button) =>
        {
            button.RegisterCallback<ClickEvent>(ButtonClick);
        });
    }

    private void ButtonClick(ClickEvent evt)
    {
        Button button = evt.currentTarget as Button;
        Debug.Log(button.name);
    }
}