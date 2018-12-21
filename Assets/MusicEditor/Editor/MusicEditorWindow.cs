using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using UnityEngine.Events;

namespace MusicEditorSpace
{
    public class MusicEditorWindow : EditorWindow
    {
        private MusicEditor m_Editor;
        internal static Rect MainRect = new Rect();
        internal static MusicEditorWindow meWindow;

        private static Object[] preObjs;

        [MenuItem("Window/MusicGameEditor")]
        public static void ShowWindow()
        {
            if (meWindow == null)
            {
              
               meWindow = GetWindow<MusicEditorWindow>();
               meWindow.titleContent = new GUIContent("MusicGameEditor");

               preObjs = Selection.objects;
            }

        }

        public void OnEnable()
        {
            MainRect = this.position;
            if ((UnityEngine.Object) this.m_Editor == (UnityEngine.Object) null)
            {
                this.m_Editor = ScriptableObject.CreateInstance(typeof(MusicEditor)) as MusicEditor;
                this.m_Editor.hideFlags = HideFlags.HideAndDontSave;
                this.m_Editor.OnEditorAwake();
            }
        }

        void OnGUI()
        {    
            m_Editor.OnEditorGUI(this.position);

            if (Event.current.rawType == EventType.KeyDown || Event.current.rawType == EventType.KeyUp)
                keyBoardInput(Event.current);

            Repaint();
        }

        void keyBoardInput(Event e)
        {
            //删除所有项目
            if (!m_Editor.m_isPlaying)
            switch (e.keyCode)
            {
                case KeyCode.Z:
                        m_Editor.t_CommandBuffer.preCommand();
                    break;
                case KeyCode.X:
                        m_Editor.t_CommandBuffer.nextCommand();
                    break;
                case KeyCode.Delete:
                    m_Editor.t_RectManager.deleteSelection();
                    Selection.objects = new Object[0];
                    break;
            }
            else
            {
                m_Editor.doTrackC(e);
            }
        }

        void Update()
        {
            MainRect = this.position;
        }

        void OnDestroy()
        {
            
            m_Editor.OnEditorClose();
            meWindow = null;
            MainRect = Rect.zero;
            if(preObjs!=null)
                Selection.objects = preObjs;
            else
                Selection.objects = new Object[0];
        }
    }
}