using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using UnityEditor;
using UnityEngine.Networking;
using UnityEngine.Timeline;
using Vector2 = UnityEngine.Vector2;
using System;
using System.Deployment.Internal;
using UnityEditor.Graphs;
using UnityEditor.UI;
using UnityEngine.Experimental.UIElements;

namespace MusicEditorSpace
{
    internal class InteractableRectManager
    {
        internal MusicEditor m_editor;

        public List<ScriptableObject> selectedObjectList;
        internal List<InteractableRect> rectList;
        internal List<trackRect> trackRectList;

        internal static TimeRulerRect tr;
        internal Rect draggingRect;
        private eState eventState;
        internal Vector2 clickOnPos;
        
        internal bool isClickOnButton;
        internal bool isObjectDirty;

        internal InteractableRectManager(MusicEditor f)
        {
            m_editor = f;
            selectedObjectList = new List<ScriptableObject>();
            rectList = new List<InteractableRect>();
            trackRectList = new List<trackRect>();

            tr = new TimeRulerRect(this);
            draggingRect = Rect.zero;
            eventState = eState.none;
            clickOnPos = Vector2.zero;
            
            isClickOnButton = false;
            isObjectDirty = false;
        }

        public void OnInternalGUI(Event e, Vector2 offset, Rect mainRect)
        {
            //绘制顺序：时间戳 框 左侧地板 左侧
            List<InteractableRect> tempList = new List<InteractableRect>(rectList);
            foreach (var r in tempList)
            {
                r.RectOnGUI();
            }

            //绘制公共事件列表
            MusicEditor.GLDrawRect(new Rect(280, 36, MusicEditorWindow.MainRect.width - 280, 16), Color.gray);
            MusicEditor.GLDrawRect(new Rect(280, 36, MusicEditorWindow.MainRect.width - 280, 2), Color.black);
            MusicEditor.GLDrawRect(new Rect(0, 52, MusicEditorWindow.MainRect.width, 2), Color.black);

            rectPosRedo();

            if (draggingRect != Rect.zero)
            {
                MusicEditor.GLDrawRect(draggingRect, new Color(0f, 0.5f, 1.0f, 0.3f));
            }

            isClickOnButton = false;

            nextState();

            if (m_editor.t_CommandBuffer.isDirty && Event.current.type == EventType.MouseUp)
            {
                m_editor.t_CommandBuffer.DoCommandList();
            }

            if (isObjectDirty)
            {
                isObjectDirty = false;
            }
        }

        protected enum eState
        {
            l_Click_On = 0,
            l_Click_Miss = 1,
            r_Click = 2,
            dragMove = 3,
            areaSelect = 4,
            barKey = 5,
            none = 6
        }

        internal void deleteSelection()
        {
            for (int i = 0; i < rectList.Count; ++i)
            {
                if (rectList[i].deleteRectSelected())
                {
                    rectList.RemoveAt(i);
                    --i;
                }
            }
        }

        protected void nextState()
        {
            Rect araeSelectable = new Rect(280, 18, MusicEditorWindow.MainRect.width - 280, MusicEditorWindow.MainRect.height - 18);
            List<InteractableRect> tempList = new List<InteractableRect>(rectList);
            switch (eventState)
            {
                case eState.none:
                    if (Event.current.type == EventType.MouseDown && !isClickOnButton)
                    {
                        clickOnPos = Event.current.mousePosition;
                        foreach (var r in tempList)
                        {
                            r.touchDelta = Event.current.mousePosition - r.rect.position;
                        }
                        if (Event.current.button == 0)
                        {
                            bool isClickOn = false;
                            foreach (var r in tempList)
                            {
                                isClickOn = isClickOn || r.getSelection();
                            }
                            //如果这次点击命中任何一个被选中的物体
                            if (isClickOn)
                            {
                                eventState = eState.l_Click_On;
                            }
                            else
                            {
                                eventState = eState.l_Click_Miss;
                            }
                        }

                        if (Event.current.button == 1)
                        {
                            eventState = eState.r_Click;
                        }
                    }
                    break;
                case eState.l_Click_On:
                    if (Event.current.type == EventType.MouseUp)
                    {
                        //选择/反选物体
                        foreach (var r in tempList)
                        {
                            r.doLeftClick();
                        }

                        eventState = eState.none;
                    }

                    if (Event.current.type == EventType.MouseDrag)
                    {
                        //拖动物体
                        eventState = eState.dragMove;
                        foreach (var r in tempList)
                        {
                            draggingRect.position = Event.current.mousePosition;
                            r.DragStart();
                            r.doDrag();
                        }
                    }
                    break;
                case eState.l_Click_Miss:
                    if (Event.current.type == EventType.MouseUp)
                    {
                        //反选所有物体
                        foreach (var r in tempList)
                        {
                            if (r.isSelected)
                                m_editor.t_CommandBuffer.AddCommand(new SelectionChangge(r, false));
                        }
                        foreach (var r in tempList)
                        {
                            r.doLeftClick();
                        }

                        eventState = eState.none;
                    }

                    if (Event.current.type == EventType.MouseDrag)
                    {
                        if (araeSelectable.Contains(clickOnPos))
                        {
                            draggingRect.position = clickOnPos;
                            eventState = eState.areaSelect;
                        }

                        else
                        {
                            eventState = eState.none;
                        }
                    }
                    break;
                case eState.r_Click:
                    if (Event.current.type == EventType.MouseUp)
                    {
                        eventState = eState.none;
                        foreach (var r in tempList)
                        {
                            if (r.isSelected)
                                m_editor.t_CommandBuffer.AddCommand(new SelectionChangge(r, false));
                        }
                        foreach (var r in tempList)
                        {
                            r.doRightClick();
                        }
                    }

                    if (Event.current.type == EventType.MouseDrag)
                    {

                        foreach (var r in tempList)
                        {
                            r.doRightDragStart();
                        }
                        eventState = eState.barKey;

                    }
                    break;
                case eState.areaSelect:
                    //绘制选择框
                    if (Event.current.type == EventType.MouseDrag)
                    {
                        float minX = Mathf.Min(clickOnPos.x, Event.current.mousePosition.x);
                        float minY = Mathf.Min(clickOnPos.y, Event.current.mousePosition.y);
                        Vector2 size = new Vector2(Mathf.Max(clickOnPos.x, Event.current.mousePosition.x) - minX,
                            Mathf.Max(clickOnPos.y, Event.current.mousePosition.y) - minY);
                        draggingRect = new Rect(minX, minY, size.x, size.y);
                    }

                    if (Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseLeaveWindow)
                    {
                        //选择区域内的所有目标
                        foreach (var r in tempList)
                        {
                            if (r.isSelected)
                                m_editor.t_CommandBuffer.AddCommand(new SelectionChangge(r, false));
                        }
                        foreach (var r in tempList)
                        {
                            r.doAreaSelect();
                        }
                        draggingRect = Rect.zero;
                        eventState = eState.none;

                    }

                    break;
                case eState.dragMove:
                    if (Event.current.type == EventType.MouseDrag)
                    {
                        //所有rect进行drag操作
                        foreach (var r in tempList)
                        {
                            r.doDrag();
                        }
                    }

                    if (Event.current.type == EventType.MouseUp)
                    {
                        foreach (var r in tempList)
                        {
                            r.DragEnd();
                        }
                        eventState = eState.none;
                        draggingRect = Rect.zero;
                    }
                    break;
                case eState.barKey:

                    if (Event.current.type == EventType.MouseDrag)
                    {

                        foreach (var r in tempList)
                        {
                            r.doRightDrag();
                        }
                    }

                    if (Event.current.type == EventType.MouseUp)
                    {
                        eventState = eState.none;
                        foreach (var r in tempList)
                        {

                            r.doRightDragEnd();
                        }
                        if (m_editor.t_CommandBuffer.isDirty)
                            m_editor.t_CommandBuffer.DoCommandList();
                    }
                    break;
                default:
                    break;
            }
        }


        internal void rectPosRedo()
        {
            for (int i = 0; i < trackRectList.Count; ++i)
            {
                trackRectList[i].rect = new Rect(0, 85 * i + 60 + 20, MusicEditorWindow.MainRect.width, 50);
                trackRectList[i].headRect = new Rect(0, 85 * i + 60 + 20, 280, 50);
                trackRectList[i].eventRect = new Rect(280, 85 * i + 60 + 25, trackRectList[i].rect.width - 280, 40);
            }
        }
    }

    abstract internal class InteractableRect
    {
        public Rect rect;
        public bool isSelected;
        public Vector2 touchDelta;

        internal InteractableRectManager m_manager;

        internal static Rect getSuitableRect(Rect raw)
        {
            Rect r = new Rect(raw);
            if (r.yMin < 54)
            {
               r.height = Mathf.Max(raw.height + raw.y - 54,0);
               r.y = 54;
            }

            return r;
        }

        internal InteractableRect(InteractableRectManager irm)
        {
            m_manager = irm;
            irm.rectList.Add(this);
        }

        internal abstract void RectOnGUI();
        internal abstract void doLeftClick();
        internal abstract void doRightClick();

        internal abstract void doDrag();
        internal abstract void DragStart();
        internal abstract void DragEnd();
        internal abstract void doAreaSelect();

        internal abstract void doRightDrag();
        internal abstract void doRightDragStart();
        internal abstract void doRightDragEnd();

        internal abstract bool getSelection();
        internal abstract bool deleteRectSelected();

        internal abstract ScriptableObject getObject();

        internal abstract bool isInRect(Vector2 pos);

        
    }

    internal class trackRect : InteractableRect
    {
        public MusicEditorSpace.Track t;

        internal Rect headRect;
        internal Rect eventRect;
        internal List<TimeStampRect> timeStampRects;

        internal bool tsRectSortDirty;

        internal bool isActive;

        private int lastIndex;
        internal TimeStampRect longST;

        private clsMCI v_DefaultHit;
        internal int playPos;
        internal bool isHeadPlayed;

        internal trackRect(Track c,InteractableRectManager irm) : base(irm)
        {
            t = c;
            playPos = 0;
            isHeadPlayed = false;
            isActive = true;
            rect = new Rect(0, 0, MusicEditorWindow.MainRect.width, 50);
            isSelected = false;

            touchDelta = new UnityEngine.Vector2(0, 0);

            headRect = new Rect(0, 0, 280, 50);
            eventRect = new Rect(280, 0, MusicEditorWindow.MainRect.width - 280, 40);

            m_manager.rectPosRedo();
            timeStampRects = new List<TimeStampRect>();

            tsRectSortDirty = false;
            longST = null;

            for (int i = 0; i < t.timestamps.Count; ++i)
            {
                t.timestamps[i].isDid = false;
                timeStampRects.Add(new TimeStampRect(t.timestamps[i], this));
            }

            v_DefaultHit = new clsMCI();
            if (t.defaultHitAudio != null)
                v_DefaultHit.FileName = AssetDatabase.GetAssetPath(t.defaultHitAudio);
        }

        internal void removeKeyCoverd(TimeStamp ts)
        {
            bool isAdd = false;
            for (int i = 0; i < timeStampRects.Count; ++i)
            {
                if (timeStampRects[i].ts.time + timeStampRects[i].ts.length >= ts.time)
                {
                    float left = ts.time, right = left + ts.length;
                    bool remove = false;
                    //融合前边
                    if (timeStampRects[i].ts.time <= ts.time)
                    {
                        left = timeStampRects[i].ts.time;
                        m_manager.m_editor.t_CommandBuffer.AddCommand(new RemoveKeyCommand(timeStampRects[i], this));
                        remove = true;
                    }

                    //融合右边
                    for (int j = i; j < timeStampRects.Count; ++j)
                    {
                        if (timeStampRects[j].ts.time > ts.time + ts.length)
                        {
                            break;
                        }
                        if (!remove)
                            m_manager.m_editor.t_CommandBuffer.AddCommand(new RemoveKeyCommand(timeStampRects[j], this));
                        right = Mathf.Max(right, timeStampRects[j].ts.time + timeStampRects[j].ts.length);
                    }

                    ts.time = left;
                    ts.length = right - left;

                    t.timestamps.Insert(i, ts);
                    isAdd = true;
                    break;
                }
            }
            if (!isAdd)
                t.timestamps.Add(ts);
        }

        internal TimeStamp removeKeyCoverd(float p)
        {
            TimeStamp timeStamp = ScriptableObject.CreateInstance<TimeStamp>();
            timeStamp.init(p,0);
            bool isAdd = false;
            for (int i = 0; i < timeStampRects.Count; ++i)
            {
                if (timeStampRects[i].ts.time + timeStampRects[i].ts.length >= p)
                { 
                    if (timeStampRects[i].ts.time <= p)
                        m_manager.m_editor.t_CommandBuffer.AddCommand(new RemoveKeyCommand(timeStampRects[i], this));

                    t.timestamps.Insert(i, timeStamp);
                    isAdd = true;
                    break;
                }
            }
            if (!isAdd)
                t.timestamps.Add(timeStamp);

            return timeStamp;
        }

        internal void playPosRecauculate(float nTime)
        {
            playPos = t.timestamps.Count;
            isHeadPlayed = false;
            for (int i = 0; i < t.timestamps.Count; ++i)
            {
                if (t.timestamps[i].time + t.timestamps[i].length >= nTime)
                {
                    if (t.timestamps[i].time <= nTime)
                        isHeadPlayed = true;
                    playPos = i;
                    break;
                }
            }
        }

        internal override bool deleteRectSelected()
        {
            if (isSelected)
            {
                //依次删除时间戳
                foreach (var tsr in timeStampRects)
                {
                    m_manager.m_editor.t_CommandBuffer.AddCommand(new RemoveKeyCommand(tsr,this));
                }

                //删除整条轨道
                m_manager.m_editor.t_CommandBuffer.AddCommand(new RemoveTrackCommand(this));
                return true;
            }
            else
            {
                foreach (var tsr in timeStampRects)
                {
                    if(tsr.isSelected)
                        m_manager.m_editor.t_CommandBuffer.AddCommand(new RemoveKeyCommand(tsr,this));
                }
                return false;
            }
        }


        internal override void doAreaSelect()
        {

        }

        internal override void doDrag()
        {
            if (isSelected && m_manager.clickOnPos.x < 280)
            { 
                m_manager.trackRectList.Remove(this);

                int nP = Mathf.Clamp(
                    (int) ((Event.current.mousePosition.y + m_manager.m_editor.m_ScrollPosition.y - 60.0f) / 85.0f), 0,
                    m_manager.trackRectList.Count);

                m_manager.trackRectList.Insert(nP, this);
            }
        }

        internal override void doLeftClick()
        {
            Vector2 mousePos = Event.current.mousePosition + m_manager.m_editor.m_ScrollPosition;
            if (rect.Contains(mousePos))
            {
                if (headRect.Contains(mousePos))
                {
                    isActive = !isActive;
                    if(isSelected)
                        m_manager.m_editor.t_CommandBuffer.AddCommand(new SelectionChangge(this,false));
                }
            }
        }

        internal override void doRightClick()
        {
            Vector2 mousePos = Event.current.mousePosition;
            if (headRect.Contains(mousePos))
            {
                m_manager.m_editor.t_CommandBuffer.AddCommand(new SelectionChangge(this, !isSelected));
            }
            else 
            {
                if(isSelected)
                    m_manager.m_editor.t_CommandBuffer.AddCommand(new SelectionChangge(this, false));

                if (eventRect.Contains(mousePos) && isActive && !m_manager.m_editor.m_isPlaying)
                {
                    float p = Mathf.Clamp((mousePos.x +m_manager.m_editor.m_ScrollPosition.x - 290) / m_manager.m_editor.m_TimeRulerLength, 0.0f, 1.0f);
                    if (m_manager.m_editor.s_isMagnat)
                    {
                        //搜索最近的可见的时间尺
                        p = m_manager.m_editor.getMagnetTime(p);
                    }
                    m_manager.m_editor.m_hit.CurrentPosition = 0;
                    m_manager.m_editor.m_hit.play();

                    m_manager.m_editor.t_CommandBuffer.AddCommand(new AddKeyCommand(this, p));
                }
            }
        }

        internal override void RectOnGUI()
        {
            if(tsRectSortDirty)
                timeStampRects.Sort();
            eventOnGUI(m_manager.m_editor.m_ScrollPosition);
            if (longST != null)
                longST.SRectOnGUI();
            headOnGUI(m_manager.m_editor.m_ScrollPosition);

            if (t.timestamps.Count > playPos && t.timestamps[playPos].time < m_manager.m_editor.m_Time && !m_manager.m_editor.s_isTest && m_manager.m_editor.s_isGoPlay)
            {
                if (!isHeadPlayed && t.timestamps[playPos].length != 0)
                {
                    Debug.Log("play");
                    v_DefaultHit.CurrentPosition = 0;
                    v_DefaultHit.play();
                    isHeadPlayed = true;
                }

                if (t.timestamps[playPos].time + t.timestamps[playPos].length < m_manager.m_editor.m_Time)
                {
                    Debug.Log("play");
                    v_DefaultHit.CurrentPosition = 0;
                    v_DefaultHit.play();
                    ++playPos;
                    isHeadPlayed = false;
                }
            }
        }

        private void headOnGUI(Vector2 offset)
        {
            Rect R = new Rect(headRect.position, headRect.size);
            R.y -= offset.y;
            R.size -= isActive ? Vector2.zero : new Vector2(30, 0);

            if (isSelected)
                MusicEditor.GLDrawRect(InteractableRect.getSuitableRect(R), Color.blue);
            else
                MusicEditor.GLDrawRect(InteractableRect.getSuitableRect(R), Color.gray);

            GUILayout.BeginArea(R, EditorStyles.helpBox);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            GUIStyle es = new GUIStyle(EditorStyles.label);
            es.focused.textColor = es.onNormal.textColor;
            t.name = EditorGUILayout.DelayedTextField(t.name,es,GUILayout.Width(60));

            t.controller = (KeyCode)EditorGUILayout.EnumPopup(t.controller, GUILayout.Width(60));

            GUILayout.FlexibleSpace();
            GUIContent addKeyContent = new GUIContent(EditorGUIUtility.IconContent("Animation.AddKeyFrame"));
            addKeyContent.tooltip = "添加时间戳";
            if (GUILayout.Button(addKeyContent, EditorStyles.miniButton,
                new GUILayoutOption[2] {GUILayout.Width(30), GUILayout.Height(15)}))
            {
                //添加新时间戳
                v_DefaultHit.CurrentPosition = 0;
                v_DefaultHit.play();
                m_manager.isClickOnButton = true;
                if (m_manager.m_editor.s_isMagnat)
                    m_manager.m_editor.t_CommandBuffer.AddCommand(new AddKeyCommand(this, m_manager.m_editor.getMagnetTime(m_manager.m_editor.m_Time)));
                else
                    m_manager.m_editor.t_CommandBuffer.AddCommand(new AddKeyCommand(this, m_manager.m_editor.m_Time));
                m_manager.m_editor.t_CommandBuffer.DoCommandList();
            }

            GUIContent clearContent = new GUIContent(EditorGUIUtility.IconContent("preAudioLoopOff"));
            clearContent.tooltip = "清空轨道";
            if (GUILayout.Button(clearContent, EditorStyles.miniButton,
                new GUILayoutOption[2] {GUILayout.Width(30), GUILayout.Height(15)}))
            {
                //清空轨道
                m_manager.isClickOnButton = true;
                foreach (var tr in timeStampRects)
                {
                    m_manager.m_editor.t_CommandBuffer.AddCommand(new RemoveKeyCommand(tr,this)); 
                }
                m_manager.m_editor.t_CommandBuffer.DoCommandList();
            }

            GUIContent DeleteContent = new GUIContent(EditorGUIUtility.IconContent("winbtn_win_close"));
            DeleteContent.tooltip = "删除轨道";
            if (GUILayout.Button(DeleteContent, EditorStyles.miniButton,
                new GUILayoutOption[2] {GUILayout.Width(30), GUILayout.Height(15)}))
            {
                //删除轨道
                m_manager.m_editor.t_CommandBuffer.AddCommand(new RemoveTrackCommand(this));
                m_manager.isClickOnButton = true;
                m_manager.m_editor.t_CommandBuffer.DoCommandList();
            }

            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            //绘制轨道信息
            
            GUILayout.FlexibleSpace();
            GUILayout.Label("时间戳 ：" + t.timestamps.Count);

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void eventOnGUI(Vector2 offset)
        {
            Rect R = new Rect(eventRect);
            R.y -= offset.y;
            MusicEditor.GLDrawRect(InteractableRect.getSuitableRect(R), new Color(0.7f, 0.7f, 0.7f, 0.5f));
            //绘制时间戳
            foreach (var tRect in timeStampRects)
            {
                tRect.SRectOnGUI();
            }

            //绘制蒙版
            if (!isActive)
                MusicEditor.GLDrawRect(eventRect, new Color(0.2f, 0.2f, 0.2f, 0.5f));
        }


        internal override bool isInRect(Vector2 pos)
        {
            Rect nRect = new Rect(rect);
            nRect.y -= m_manager.m_editor.m_ScrollPosition.y;
            return (nRect.Contains(pos) && isSelected);
        }

        internal override bool getSelection()
        {

            return (new Rect(headRect.position - new Vector2(0, m_manager.m_editor.m_ScrollPosition.y), headRect.size).Contains(
                       Event.current.mousePosition));
        }

        internal override ScriptableObject getObject()
        {
            return this.t;
        }

        internal override void DragStart()
        {
            lastIndex = m_manager.trackRectList.Count - 1;
            for (int i = 0; i < m_manager.trackRectList.Count; ++i)
            {
                if (m_manager.trackRectList[i] == this)
                {
                    lastIndex = i;
                    break;
                }
            }
        }

        internal override void DragEnd()
        {
            int tarP = 0;
            for (int i = 0; i < m_manager.trackRectList.Count; ++i)
            {
                if (m_manager.trackRectList[i] == this)
                {
                    tarP = i;
                    break;
                }
            }

            if(tarP != lastIndex)
                m_manager.m_editor.t_CommandBuffer.AddCommand(new MoveTrackCommand(this,tarP,lastIndex));
        }

        internal override void doRightDrag()
        {
            //头部拖动，体部创建/修改
            doDrag();
            if (eventRect.Contains(m_manager.clickOnPos) && longST != null)
            {
                longST.ts.length = Mathf.Clamp01(m_manager.m_editor.getMagnetTime((Event.current.mousePosition.x - 290 + m_manager.m_editor.m_ScrollPosition.x) / m_manager.m_editor.m_TimeRulerLength)) - longST.ts.time;
            }
        }

        internal override void doRightDragStart()
        {
            //体部则创建 /标记目标
            isSelected = false;
            if (headRect.Contains(m_manager.clickOnPos + Vector2.up * m_manager.m_editor.m_ScrollPosition.y))
            {
                isSelected = true;
                DragStart();
            }
            else if (eventRect.Contains(m_manager.clickOnPos))
            {
                if (longST == null && !m_manager.m_editor.m_isPlaying)
                {
                    var temp = m_manager.clickOnPos;
                    TimeStamp t = ScriptableObject.CreateInstance<TimeStamp>();
                    t.init(m_manager.m_editor.getMagnetTime(m_manager.m_editor.m_Time), 0);
                    longST = new TimeStampRect(t, this);
                    m_manager.clickOnPos = temp;
                    longST.ts.time = Mathf.Clamp01(m_manager.m_editor.getMagnetTime(
                        (m_manager.clickOnPos.x - 290 + m_manager.m_editor.m_ScrollPosition.x) / m_manager.m_editor.m_TimeRulerLength));
                    longST.ts.length =
                        m_manager.m_editor.getMagnetTime(
                            (Event.current.mousePosition.x - 290 + m_manager.m_editor.m_ScrollPosition.x) /
                            m_manager.m_editor.m_TimeRulerLength) - longST.ts.time;
                }
            }
            
        }

        internal override void doRightDragEnd()
        {
            //推入命令堆栈
            if (isSelected && m_manager.clickOnPos.x < 280)
            {
                isSelected = false;
                DragEnd();
                m_manager.m_editor.t_CommandBuffer.AddCommand(new SelectionChangge(this, true));
            }
            if (eventRect.Contains(m_manager.clickOnPos) && longST != null)
            {
                v_DefaultHit.CurrentPosition = 0;
                v_DefaultHit.play();
                if (m_manager.m_editor.m_BPM == 0 || longST.ts.length > (2.0f / m_manager.m_editor.m_BPM))
                {
                    if (longST.ts.length < 0)
                    {
                        longST.ts.time += longST.ts.length;
                        longST.ts.length = Mathf.Abs(longST.ts.length);
                    }

                    m_manager.m_editor.t_CommandBuffer.AddCommand(new AddKeyCommand(this, longST));
                }
                else
                {
                    m_manager.m_editor.t_CommandBuffer.AddCommand(new AddKeyCommand(this,longST.ts.time));
                    
                }
                longST = null;
            }
        }
    }

    internal class TimeStampRect : InteractableRect , IComparable
    {
        internal TimeStamp ts;
        internal trackRect father;

        private float lastTime;

        internal float testValue;
        internal TestState testState;

        internal float testValue_t;
        internal TestState testState_t;

        internal enum TestState
        {
            none = 0,
            hit = 1,
            miss = 2
        }

        public TimeStampRect(TimeStamp t,trackRect f) : base(f.m_manager)
        {
            ts = t;
            isSelected = false;
            father = f;
        }

        public int CompareTo(object obj)
        {
            if(obj is TimeStampRect)
                return ts.time.CompareTo(((TimeStampRect)obj).ts.time);
            return 1;
        }

        internal override bool deleteRectSelected()
        {
            return false;
        }

        internal override void doAreaSelect()
        {
            if (father.isActive)
            {
                Rect nRect = new Rect(ts.time * m_manager.m_editor.m_TimeRulerLength + father.eventRect.x - 10, father.eventRect.y,
                    20, 40);
                nRect.position -= m_manager.m_editor.m_ScrollPosition;
                Rect nDragging = new Rect(m_manager.draggingRect.min, m_manager.draggingRect.max - m_manager.draggingRect.min);
                if (nRect.Overlaps(nDragging))
                {
                    m_manager.m_editor.t_CommandBuffer.AddCommand(new SelectionChangge(this, true));
                }
                else
                {
                    m_manager.m_editor.t_CommandBuffer.AddCommand(new SelectionChangge(this, false));

                }
            }
        }

        internal override void doDrag()
        {
            if (father.isActive && m_manager.clickOnPos.x > 280 && m_manager.clickOnPos.y > 60)
            {
                if (isSelected)
                {
                    //根据起始位置和现在的差值来确定
                    Vector2 delta = Event.current.mousePosition - m_manager.clickOnPos;

                    float t = delta.x / m_manager.m_editor.m_TimeRulerLength + lastTime;

                    if (m_manager.m_editor.s_isMagnat)
                    {
                        t = m_manager.m_editor.getMagnetTime(t);
                    }

                    ts.time = t;
                }
            }
        }

        internal override void doLeftClick()
        {
            if (isInRect(Event.current.mousePosition) && father.isActive)
            {
                m_manager.m_editor.t_CommandBuffer.AddCommand(new SelectionChangge(this, !isSelected));
            }
            else
            {
                m_manager.m_editor.t_CommandBuffer.AddCommand(new SelectionChangge(this, false));
            }
        }

        internal override void doRightClick()
        {
            m_manager.m_editor.t_CommandBuffer.AddCommand(new SelectionChangge(this,father.isActive && 
                                                              father.isSelected && 
                                                              father.headRect.Contains(Event.current.mousePosition + m_manager.m_editor.m_ScrollPosition)));
        }

        internal override void doRightDrag()
        {

        }

        internal override void doRightDragEnd()
        {
         
        }

        internal override void doRightDragStart()
        {
          
        }

        internal override void DragEnd()
        {
            m_manager.m_editor.t_CommandBuffer.AddCommand(new KeyMoveCommand(lastTime,ts.time,this));
        }

        internal override void DragStart()
        {
            lastTime = ts.time;
        }

        internal override ScriptableObject getObject()
        {
            return this.ts;
        }

        internal override bool getSelection()
        {
            Rect nRect = new Rect(ts.time * m_manager.m_editor.m_TimeRulerLength + father.eventRect.x, father.eventRect.y, 40, 40);
            nRect.position -= m_manager.m_editor.m_ScrollPosition;

            return (isSelected && nRect.Contains(Event.current.mousePosition));
        }

        internal override bool isInRect(Vector2 pos)
        {
            Rect nRect = new Rect(ts.time * m_manager.m_editor.m_TimeRulerLength + father.eventRect.x, father.eventRect.y, 40, 40);
            nRect.position -= m_manager.m_editor.m_ScrollPosition;
            return nRect.Contains(Event.current.mousePosition);
        }

        internal override void RectOnGUI()
        {
            
        }

        internal void SdoDrag()
        {
           
        }

        internal void SRectOnGUI()
        {
            float time = ts.time * m_manager.m_editor.m_TimeRulerLength;
            float length = ts.length * m_manager.m_editor.m_TimeRulerLength;
            Vector2 pos = new Vector2(time + 290 - m_manager.m_editor.m_ScrollPosition.x,
                father.eventRect.y + 20 - m_manager.m_editor.m_ScrollPosition.y);
            Vector2 tail = new Vector2(time + length + 290 - m_manager.m_editor.m_ScrollPosition.x,
                father.eventRect.y + 20 - m_manager.m_editor.m_ScrollPosition.y);

            Color tsColor = Color.gray;
            if (testState == TestState.miss)
            {
                tsColor = Color.red;
                GUI.Label(new Rect(pos + new Vector2(-15, 10), new Vector2(30,20)),"miss");
            }
            else if (testState == TestState.hit)
            {
                tsColor = Color.red + (Color.green - Color.red) * testValue;
                GUI.Label(new Rect(pos + new Vector2(-15, 10), new Vector2(30,20)), (testValue * 2 - 1).ToString("0.00"));
            }

            Color tsColor_t = Color.gray;
            if (testState_t == TestState.miss)
            {
                tsColor_t = Color.red;
                GUI.Label(new Rect(tail + new Vector2(-15, 10), new Vector2(30, 20)), "miss");
            }
            else if (testState_t == TestState.hit)
            {
                tsColor_t = Color.red + (Color.green - Color.red) * testValue_t;
                GUI.Label(new Rect(tail + new Vector2(-15, 10), new Vector2(30, 20)), (testValue * 2 - 1).ToString("0.00"));
            }

            if ((m_manager.m_editor.m_BPM == 0 && ts.length > 0.005) || (m_manager.m_editor.m_BPM != 0 && length > (2.0f / m_manager.m_editor.m_BPM)))
            {
                //绘制中间
                if (isSelected)
                    MusicEditor.GLDrawRect(new Rect(pos + Vector2.down * 10, new Vector2(tail.x - pos.x, 20)), Color.blue);
                else
                    MusicEditor.GLDrawRect(new Rect(pos + Vector2.down * 10, new Vector2(tail.x - pos.x, 20)), Color.gray);

                if (isSelected)
                    MusicEditor.GLDrawTimeStamp(tail, 10, Color.blue);
                else
                    MusicEditor.GLDrawTimeStamp(tail, 10, tsColor_t);
            }

            if (isSelected)
                MusicEditor.GLDrawTimeStamp(pos, 10, Color.blue);
            else
                MusicEditor.GLDrawTimeStamp(pos, 10, tsColor);

        }

    }

    internal class TimeRulerRect : InteractableRect
    {
        internal TimeRulerRect(InteractableRectManager irm) : base(irm)
        {
            rect.position = new Vector2(280,18);
            rect.size = new Vector2(MusicEditorWindow.MainRect.width - 280,18);
        }

        internal override bool deleteRectSelected()
        {
            return false;
        }

        internal override void doAreaSelect()
        {
           
        }

        internal override void doDrag()
        {
            if (isInRect(m_manager.clickOnPos) && m_manager.m_editor.m_Music != null)
            {
                if(!m_manager.m_editor.s_isKeep)
                    m_manager.m_editor.m_Time =
                    Mathf.Clamp01((Event.current.mousePosition.x + m_manager.m_editor.m_ScrollPosition.x - 290) /
                                  m_manager.m_editor.m_TimeRulerLength);
                else
                    m_manager.m_editor.m_Time =
                        Mathf.Clamp01(Event.current.delta.x  /
                                      m_manager.m_editor.m_TimeRulerLength + m_manager.m_editor.m_Time);
                if (AudioUtility.IsClipPlaying(m_manager.m_editor.m_Music))
                {
                    AudioUtility.SetClipSamplePosition(m_manager.m_editor.m_Music,
                        (int)(m_manager.m_editor.m_Time * AudioUtility.GetSampleCount(m_manager.m_editor.m_Music)));
                }
            }
        }

        internal override void doLeftClick()
        {
            if (isInRect(m_manager.clickOnPos) && m_manager.m_editor.m_Music != null )
            {
                m_manager.m_editor.m_Time =
                    Mathf.Clamp01((Event.current.mousePosition.x + m_manager.m_editor.m_ScrollPosition.x - 290) /
                                  m_manager.m_editor.m_TimeRulerLength);

                if (m_manager.m_editor.m_Time > m_manager.m_editor.m_loopE || m_manager.m_editor.m_Time < m_manager.m_editor.m_loopS)
                {
                    m_manager.m_editor.m_isLoop = false;
                    m_manager.m_editor.m_loopE = 0.0f;
                    m_manager.m_editor.m_loopS = 1.0f;
                }
                //重新计算trackRect的nPos
                
                foreach (var tRect in m_manager.trackRectList)
                {
                   tRect.playPosRecauculate(m_manager.m_editor.m_Time);
                }
                if (m_manager.m_editor.m_Music != null && AudioUtility.IsClipPlaying(m_manager.m_editor.m_Music))
                {
                    AudioUtility.SetClipSamplePosition(m_manager.m_editor.m_Music,
                        (int)(m_manager.m_editor.m_Time * AudioUtility.GetSampleCount(m_manager.m_editor.m_Music)));
                }
            }
        }

        internal override void doRightClick()
        {
            //设置循环播放
            m_manager.m_editor.m_isLoop = false;
            m_manager.m_editor.m_loopE = 0.0f;
            m_manager.m_editor.m_loopS = 1.0f;
        }

        internal override void doRightDrag()
        {
            if (rect.Contains(m_manager.clickOnPos))
            {
                float temp = Mathf.Clamp01((Event.current.mousePosition.x - 290 + m_manager.m_editor.m_ScrollPosition.x) /
                                           m_manager.m_editor.m_TimeRulerLength);
                float s = Mathf.Clamp01((m_manager.clickOnPos.x - 290 + m_manager.m_editor.m_ScrollPosition.x) /
                                        m_manager.m_editor.m_TimeRulerLength);
                m_manager.m_editor.m_loopE = Mathf.Max(s, temp);
                m_manager.m_editor.m_loopS = Mathf.Min(s, temp);
                m_manager.m_editor.m_isLoop = true;
            }
        }

        internal override void doRightDragEnd()
        {
            if (rect.Contains(m_manager.clickOnPos))
            {
                float temp = Mathf.Clamp01((Event.current.mousePosition.x - 290 + m_manager.m_editor.m_ScrollPosition.x) /
                                           m_manager.m_editor.m_TimeRulerLength);
                float s = Mathf.Clamp01((m_manager.clickOnPos.x - 290 + m_manager.m_editor.m_ScrollPosition.x) /
                                        m_manager.m_editor.m_TimeRulerLength);
                m_manager.m_editor.m_loopE = Mathf.Max(s,temp);
                m_manager.m_editor.m_loopS = Mathf.Min(s,temp);
                m_manager.m_editor.m_isLoop = true;
                m_manager.m_editor.s_isKeep = true;
            }
        }

        internal override void doRightDragStart()
        {
            if (rect.Contains(m_manager.clickOnPos))
            {
                m_manager.m_editor.m_loopS = Mathf.Clamp01((m_manager
                                                       .clickOnPos.x - 290 + m_manager.m_editor.m_ScrollPosition.x) /
                                                         m_manager.m_editor.m_TimeRulerLength);
                m_manager.m_editor.m_loopE = m_manager.m_editor.m_loopS;
                m_manager.m_editor.s_isKeep = false;
            }
        }

        internal override void DragEnd()
        {

            if (isInRect(m_manager.clickOnPos) && m_manager.m_editor.m_Music != null)
            {
                foreach (var tRect in m_manager.trackRectList)
                {
                    tRect.playPosRecauculate(m_manager.m_editor.m_Time);
                }
            }
        }

        internal override void DragStart()
        {
            
        }

        internal override ScriptableObject getObject()
        {
            return null;
        }

        internal override bool getSelection()
        {
            return isInRect(Event.current.mousePosition);
        }

        internal override bool isInRect(Vector2 pos)
        {
            return rect.Contains(pos);
        }

        internal override void RectOnGUI()
        { 
            rect.size = new Vector2(MusicEditorWindow.MainRect.width - 280, 18);
        }
    }
}
