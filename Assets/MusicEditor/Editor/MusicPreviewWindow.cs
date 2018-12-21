using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MusicEditorSpace
{
   
    internal class MusicPreviewWindow : EditorWindow
    {
        private MusicEditor m_editor;

        //下落时间等于2秒
        private float fallTime;
        private float trackLength;

        private clsMCI v_hit;

        private string infoText;
        private GUIStyle infoStyle;
       
        readonly private float maxTrackWidth = 100;
        readonly private float buttonHeight = 15;

        //输入提前量超过这个时间 + inHitTime的会被miss掉,单位为秒
        readonly private float earlyMissTime = 0.2f;
        //只有在+-这个时间内的输入才会进行判定，单位为秒
        readonly private float inHitTime = 0.1f;

        readonly private float textGradualTime = 0.3f;

        private int combol;


        public static MusicPreviewWindow getNewPreview(MusicEditor editor)
        {
            MusicPreviewWindow mpw = GetWindow<MusicPreviewWindow>();
            mpw.titleContent = new GUIContent("MusicGameEditor");
            mpw.m_editor = editor;
            mpw.v_hit = new clsMCI();
            mpw.v_hit.FileName = editor.m_hit.FileName;
            mpw.fallTime = 2.0f;
            if (editor.m_Music != null)
                mpw.trackLength = mpw.fallTime / editor.m_Music.length;
            else
                mpw.trackLength = mpw.fallTime / 60;

            mpw.infoStyle = new GUIStyle();
            mpw.infoStyle.fontSize = 50;
            mpw.infoStyle.alignment = TextAnchor.MiddleCenter;
            mpw.infoStyle.normal.textColor = Color.white;

            mpw.OnFocus();
            return mpw;
        }

        void OnFocus()
        {
            if (m_editor != null && !m_editor.s_isTest)
            {
                m_editor.s_isTest = true;
                foreach (var tRect in m_editor.t_RectManager.trackRectList)
                {
                    tRect.playPosRecauculate(m_editor.m_Time);
                }
            }
        }

        void OnLostFocus()
        {
            if (m_editor != null && m_editor.s_isTest)
            {
                m_editor.s_isTest = false;
                foreach (var tRect in m_editor.t_RectManager.trackRectList)
                {
                    tRect.playPosRecauculate(m_editor.m_Time);
                }
            }
        }

        void OnDestroy()
        {
            foreach (var tRect in m_editor.t_RectManager.trackRectList)
            {
                foreach (var tsRect in tRect.timeStampRects)
                {
                    tsRect.testState = TimeStampRect.TestState.none;
                    tsRect.testValue = 0;
                    tsRect.testState_t = TimeStampRect.TestState.none;
                    tsRect.testValue_t = 0;
                }
            }
        }

        void Update()
        {
            //渐变效果

        }

        void OnGUI()
        {
            if (m_editor.m_Tracks == null || m_editor.m_Music == null)
            {
                GUILayout.Label("等待输入文件");
                return;
            }
            for (int i = 0; i < m_editor.t_RectManager.trackRectList.Count; i++)
            {
                int playPos = m_editor.t_RectManager.trackRectList[i].playPos;
                bool isPress = false;
                if (playPos < m_editor.t_RectManager.trackRectList[i].t.timestamps.Count)
                {
                    float p_time = m_editor.t_RectManager.trackRectList[i].t.timestamps[playPos].time;
                    float p_length = m_editor.t_RectManager.trackRectList[i].t.timestamps[playPos].length;

                    if (Event.current.type == EventType.KeyDown &&
                        Event.current.keyCode == m_editor.t_RectManager.trackRectList[i].t.controller &&
                        m_editor.t_RectManager.trackRectList[i].t.controller != KeyCode.None && !m_editor.t_RectManager.trackRectList[i].isHeadPlayed)
                    {
                        isPress = true;
                        //按下了对应的控制键
                        if (p_time * m_editor.m_Music.length - m_editor.m_Time * m_editor.m_Music.length > inHitTime &&
                            p_time * m_editor.m_Music.length - m_editor.m_Time * m_editor.m_Music.length < earlyMissTime)
                        {
                            ++m_editor.t_RectManager.trackRectList[i].playPos;
                            infoText = "Miss";
                            infoStyle.normal.textColor = Color.red;
                            m_editor.t_RectManager.trackRectList[i].timeStampRects[playPos].testState =
                                TimeStampRect.TestState.miss;
                          
                        }
                        else
                        {
                            if (Mathf.Abs(p_time * m_editor.m_Music.length - m_editor.m_Time * m_editor.m_Music.length) < inHitTime)
                            {
                                v_hit.CurrentPosition = 0;
                                v_hit.play();
                                infoText = (100.0f - Mathf.Abs(p_time * m_editor.m_Music.length - m_editor.m_Time * m_editor.m_Music.length) / inHitTime * 100.0f).ToString("0.0") + "%";
                                infoStyle.normal.textColor = Color.white;
                                m_editor.t_RectManager.trackRectList[i].timeStampRects[playPos].testState =
                                    TimeStampRect.TestState.hit;
                                m_editor.t_RectManager.trackRectList[i].timeStampRects[playPos].testValue =
                                    1 - Mathf.Abs(p_time * m_editor.m_Music.length - m_editor.m_Time * m_editor.m_Music.length) / inHitTime;

                                if (p_length == 0)
                                    ++m_editor.t_RectManager.trackRectList[i].playPos;
                                else
                                    m_editor.t_RectManager.trackRectList[i].isHeadPlayed = true;
                               
                            }
                        }
                    }

                    if (Event.current.type == EventType.KeyUp &&
                        Event.current.keyCode == m_editor.t_RectManager.trackRectList[i].t.controller &&
                        m_editor.t_RectManager.trackRectList[i].t.controller != KeyCode.None &&
                        m_editor.t_RectManager.trackRectList[i].isHeadPlayed)
                    {
                        if (Mathf.Abs((p_time + p_length) * m_editor.m_Music.length -
                                      m_editor.m_Time * m_editor.m_Music.length) > inHitTime)
                        {
                            ++m_editor.t_RectManager.trackRectList[i].playPos;
                            m_editor.t_RectManager.trackRectList[i].isHeadPlayed = false;
                            infoText = "Miss";
                            infoStyle.normal.textColor = Color.red;
                            m_editor.t_RectManager.trackRectList[i].timeStampRects[playPos].testState_t =
                                TimeStampRect.TestState.miss;
                           
                            break;

                        }
                        else
                        {
                            v_hit.CurrentPosition = 0;
                            v_hit.play();
                            infoText = (100.0f - Mathf.Abs((p_time + p_length) * m_editor.m_Music.length -
                                                           m_editor.m_Time * m_editor.m_Music.length) / inHitTime *
                                        100.0f).ToString("0.0") + "%";
                            infoStyle.normal.textColor = Color.white;
                            m_editor.t_RectManager.trackRectList[i].timeStampRects[playPos].testState_t =
                                TimeStampRect.TestState.hit;
                            m_editor.t_RectManager.trackRectList[i].timeStampRects[playPos].testValue_t =
                                1 - Mathf.Abs((p_time + p_length) * m_editor.m_Music.length -
                                              m_editor.m_Time * m_editor.m_Music.length) / inHitTime;

                            ++m_editor.t_RectManager.trackRectList[i].playPos;
                            m_editor.t_RectManager.trackRectList[i].isHeadPlayed = false;

                            break;
                        }
                    }

                    if (!m_editor.t_RectManager.trackRectList[i].isHeadPlayed && m_editor.m_Time * m_editor.m_Music.length - p_time * m_editor.m_Music.length > inHitTime )
                    {
                        ++m_editor.t_RectManager.trackRectList[i].playPos;
                        infoText = "Miss";
                        infoStyle.normal.textColor = Color.red;
                        m_editor.t_RectManager.trackRectList[i].timeStampRects[playPos].testState =
                            TimeStampRect.TestState.hit;
                      
                    }
                }

                float trackWidth = Mathf.Min(position.width / m_editor.m_Tracks.Tracks.Count, maxTrackWidth);
                float trackAreaX = (position.width - trackWidth * m_editor.m_Tracks.Tracks.Count) * 0.5f;
                Rect baseRect = new Rect(trackAreaX + trackWidth * i, 0, trackWidth, position.height);
                Rect trackRect = new Rect(baseRect.position + 0.2f * baseRect.width * Vector2.right,new Vector2(baseRect.width * 0.6f,baseRect.height-40));
                Rect buttonRect = new Rect(trackRect.position + new Vector2(0.1f * trackRect.width,baseRect.height - 30),new Vector2(0.8f*trackRect.width, buttonHeight));
                //绘制轨道底纹
                MusicEditor.GLDrawRect(baseRect, Color.black);
                MusicEditor.GLDrawRect(trackRect,new Color(0.7f,0.7f,0.7f,1.0f));
                //绘制控制按钮
                MusicEditor.GLDrawRect(buttonRect, isPress ? Color.white : Color.gray);
                //绘制轨道中的所有位于有效区域内的键
                for (int j = m_editor.t_RectManager.trackRectList[i].playPos; j < m_editor.t_RectManager.trackRectList[i].t.timestamps.Count; j++)
                {
                    float time = m_editor.t_RectManager.trackRectList[i].t.timestamps[j].time;
                    float length = m_editor.t_RectManager.trackRectList[i].t.timestamps[j].length;
                    bool isDid = m_editor.t_RectManager.trackRectList[i].t.timestamps[j].isDid;

                    if (!isDid && time < m_editor.m_Time + 1.2f * trackLength)
                    {
                        //绘制键
                        Rect keyRect = new Rect(baseRect.x + baseRect.width * 0.25f,
                            baseRect.y + baseRect.height - (time - m_editor.m_Time) / trackLength * baseRect.height - buttonHeight * 0.5f + buttonHeight - 30,
                            baseRect.width * 0.5f, buttonHeight);
                        if (length != 0)
                        {
                            Rect endKeyRect = new Rect(keyRect.position + Vector2.down * length /trackLength * baseRect.height,keyRect.size);
                            //绘制中间区域
                            MusicEditor.GLDrawRect(new Rect(endKeyRect.position + Vector2.right * 5,new Vector2(keyRect.size.x - 10,keyRect.y - endKeyRect.y)),Color.gray);
                            //绘制结束
                            MusicEditor.GLDrawRect(endKeyRect,Color.gray);
                        }
                        MusicEditor.GLDrawRect(keyRect,Color.gray);
                    }
                }

                Repaint();
            }

            //绘制记分板
            GUI.Label(new Rect(position.width * 0.5f - 100,20,200,100), infoText,infoStyle);
        }
    }
}
