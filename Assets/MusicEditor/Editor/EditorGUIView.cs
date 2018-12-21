using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class EditorGUIView :  EditorWindow
{
    private static List<GUIStyle> styles = null;

    [MenuItem("Window/EditorStylesView")]
    static void EditorStyleView()
    {
        EditorWindow.GetWindow<EditorGUIView>("EditorStyles");

        styles = new List<GUIStyle>();
        foreach (PropertyInfo fi in typeof(EditorStyles).GetProperties(BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic))
        {
            object o = fi.GetValue(null, null);
            if (o.GetType() == typeof(GUIStyle))
            {
                styles.Add(o as GUIStyle);
            }
        }

        for (int i = 0; i < styles.Count; i++)
        {
            Debug.Log("Editor Style : " + styles[i].name);
        }
    }

    public Vector2 scrollPosition = Vector2.zero;

    void OnGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        for (int i = 0; i < styles.Count; i++)
        {
            GUILayout.Label("Editor Style : " + styles[i].name, styles[i]);
            
        }
        GUILayout.EndScrollView();
    }
}
