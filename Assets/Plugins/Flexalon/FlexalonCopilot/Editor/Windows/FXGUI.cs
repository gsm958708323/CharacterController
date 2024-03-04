using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FlexalonCopilot.Editor
{
    internal static class FXGUI
    {
        internal static void Vertical(Action action)
        {
            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            action();
            EditorGUILayout.EndVertical();
        }

        internal static void Vertical(float width, Action action)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(width));
            action();
            EditorGUILayout.EndVertical();
        }

        internal static void Vertical(GUIStyle style, Action action)
        {
            EditorGUILayout.BeginVertical(style, GUILayout.ExpandWidth(true));
            action();
            EditorGUILayout.EndVertical();
        }

        internal static void Horizontal(Action action)
        {
            EditorGUILayout.BeginHorizontal();
            action();
            EditorGUILayout.EndHorizontal();
        }

        internal static void Horizontal(GUIStyle style, Action action)
        {
            EditorGUILayout.BeginHorizontal(style);
            action();
            EditorGUILayout.EndHorizontal();
        }

        internal static Vector2 Scroll(Vector2 scrollPosition, Action action)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandWidth(true));
            action();
            EditorGUILayout.EndScrollView();
            return scrollPosition;
        }

        internal static Vector2 Scroll(Vector2 scrollPosition, GUIStyle style, Action action)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, style, GUILayout.ExpandWidth(true));
            action();
            EditorGUILayout.EndScrollView();
            return scrollPosition;
        }

        internal static void DisableGroup(bool disable, Action action)
        {
            EditorGUI.BeginDisabledGroup(disable);
            action();
            EditorGUI.EndDisabledGroup();
        }

        private static Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();

        internal static bool ImageButton(string guid, int width, int height)
        {
            if (!_textures.TryGetValue(guid, out var texture))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                _textures[guid] = texture;
            }

            return GUILayout.Button(texture, GUILayout.Width(width), GUILayout.Height(height));
        }

        public static void Image(string guid, int width, int height)
        {
            if (!_textures.TryGetValue(guid, out var texture))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                _textures[guid] = texture;
            }

            GUILayout.Label(texture, GUILayout.Width(width), GUILayout.Height(height));
        }
    }
}