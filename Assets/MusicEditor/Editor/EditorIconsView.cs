using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using UnityEngine;

public class EditorIconsView : EditorWindow {

    [MenuItem("Window/IconsPreview")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<EditorIconsView>("Icons");
    }

    private Vector2 m_Scroll;
    private List<string> m_Icons = null;

    void Awake()
    {
        m_Icons = new List<string>();
        Texture2D[] t = Resources.FindObjectsOfTypeAll<Texture2D>();
        foreach (var x in t)
        {
            Debug.unityLogger.logEnabled = false;
            GUIContent go = EditorGUIUtility.IconContent(x.name);
            Debug.unityLogger.logEnabled = true;
            if (go != null && go.image != null)
            {
                m_Icons.Add(x.name);
            }
        }
        m_Icons.Sort();
        
    }

    void OnGUI()
    {
        m_Scroll = GUILayout.BeginScrollView(m_Scroll);
        float width = 50f;
        int count = (int) (position.width / width);
        for (int i = 0; i < m_Icons.Count; i += count)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < count; ++j)
            {
                int index = i + j;
                if (index < m_Icons.Count)
                {
                    if (GUILayout.Button(EditorGUIUtility.IconContent(m_Icons[index]), GUILayout.Width(width),
                        GUILayout.Height(30)))
                    {
                        Debug.Log(m_Icons[index]);
                    }
                }
            }

            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }
}
