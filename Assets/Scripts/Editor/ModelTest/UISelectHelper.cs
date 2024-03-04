using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISelectHelper : MonoBehaviour
{
    private static List<RaycastResult> Results = new List<RaycastResult>();

    [MenuItem("Tools/Select #X")]
    static void Select()
    {
        if (!EditorApplication.isPlaying)
            return;

        Results.Clear();
        PointerEventData data = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        EventSystem.current.RaycastAll(data, Results);
        if (Results.Count == 0)
            return;

        var go = Results[0].gameObject;
        EditorGUIUtility.PingObject(go);
    }
}
