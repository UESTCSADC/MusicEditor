using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = System.Diagnostics.Debug;



namespace MusicEditorSpace
{
    internal class MusicCommandBuffer
    {
        private List<MusicCommand>[] commandRingBuffer = new List<MusicCommand>[100];
        private uint writePos = 0;//目前的写入位置
        private uint readPos = 0;//目前的读取位置
        private List<MusicCommand> temp;

        internal bool isDirty;
        internal MusicEditor father;

        internal MusicCommandBuffer(MusicEditor f)
        {
            temp = new List<MusicCommand>();
            commandRingBuffer = new List<MusicCommand>[100];
            writePos = 0;
            readPos = 1;
            isDirty = false;
            father = f;
        }

        internal void AddCommand(MusicCommand mc)
        {
            temp.Add(mc);
            isDirty = true;
        }

        internal void DoCommandList()
        {
            foreach (var mc in temp)
            {
                mc.doCommand();
            }

            //do了就不能next了
            if (readPos + 1 == uint.MaxValue)
                readPos = (readPos) % 100;

            ++readPos;
            commandRingBuffer[readPos % 100] = new List<MusicCommand>(temp);
            writePos = (readPos + 1);

            temp.Clear();
            isDirty = false;
        }

        internal void CancelCommandList()
        {
            temp.Clear();
            isDirty = false;
        }

        internal void preCommand()
        {
            //上一步，将readPos回推，直到writePos + 1或者为空
            if ((int)readPos > (int)(writePos - 100) && readPos > 0)
            {
                //倒着执行列表操作
                for (int i = commandRingBuffer[readPos % 100].Count - 1; i >= 0; --i)
                {
                    commandRingBuffer[readPos % 100][i].reDoCommand();
                }

                readPos -= 1;
            }
        }

        internal void nextCommand()
        {
            //下一步
            if (readPos + 1 < writePos && commandRingBuffer[(readPos + 1) % 100] != null)
            {
                readPos += 1;
                foreach (var mc in commandRingBuffer[readPos % 100])
                {
                    mc.doCommand();
                }
            }
        }
    }

    internal abstract class MusicCommand
    {
        
        internal abstract void doCommand();
        internal abstract void reDoCommand();

        
    }

    internal class AddKeyCommand : MusicCommand
    {
        private trackRect tRect;
        private float p;
        private TimeStamp timeStamp;
        private TimeStampRect ts;

        internal AddKeyCommand(trackRect t,float p)
        {
            tRect = t;
            this.p = p;
            timeStamp = tRect.removeKeyCoverd(p);
        }

        //添加长条
        internal AddKeyCommand(trackRect t, TimeStampRect tsr)
        {
            tRect = t;
            ts = tsr;
            p = tsr.ts.time;

            //插入并删除重复、合并
            tRect.removeKeyCoverd(tsr.ts);
        }

        //做
        internal override void doCommand()
        {
            if (ts == null)
            {
                ts = new TimeStampRect(timeStamp, tRect);
            }

            ts.isSelected = true;
            tRect.timeStampRects.Add(ts);
        }

        //反做
        internal override void reDoCommand()
        {
            tRect.t.removeTimeStamp(p);
            tRect.timeStampRects.Remove(ts);
        }
    }

    internal class RemoveKeyCommand : MusicCommand
    {
        private TimeStampRect key;
        private trackRect tRect;

        internal RemoveKeyCommand(TimeStampRect k,trackRect tr)
        {
            key = k;
            tRect = tr;
        }

        internal override void doCommand()
        {
            tRect.timeStampRects.Remove(key);
            tRect.t.removeTimeStamp(key.ts.time);

        }

        internal override void reDoCommand()
        {
            new AddKeyCommand(tRect,key);
            tRect.timeStampRects.Add(key);
        }
    }

    internal class SelectionChangge : MusicCommand
    {
        private InteractableRect target;
        private bool state;
        private bool preState;

        internal SelectionChangge(InteractableRect tar,bool st)
        {
            target = tar;
            state = st;
            preState = tar.isSelected;
        }

        internal override void doCommand()
        {
            target.m_manager.isObjectDirty = true;
            if (state && !target.isSelected)
            {
                if(target.getObject() != null)
                    target.m_manager.selectedObjectList.Add(target.getObject());
            }
            if( !state && target.isSelected)
            {
                if (target.getObject() != null)
                    target.m_manager.selectedObjectList.Remove(target.getObject());
            }
            target.isSelected = state;
        }

        internal override void reDoCommand()
        {

            target.m_manager.isObjectDirty = true;
            if (preState && !target.isSelected)
            {
                if (target.getObject() != null)
                    target.m_manager.selectedObjectList.Add(target.getObject());
            }
            if (!preState && target.isSelected)
            { 
                if (target.getObject() != null)
                    target.m_manager.selectedObjectList.Remove(target.getObject());
            }
            target.isSelected = preState;
        }  


    }

    internal class KeyMoveCommand : MusicCommand
    {
        private float T;
        private float preT;
        private TimeStampRect target;

        internal KeyMoveCommand(float lt, float nt, TimeStampRect tar)
        {
            T = nt;
            preT = lt;
            target = tar;
        }

        internal override void doCommand()
        {
            target.ts.time = T;
            target.father.tsRectSortDirty = true;
        }

        internal override void reDoCommand()
        {
            target.ts.time = preT;

        }
    }

    internal class RemoveTrackCommand : MusicCommand
    {
        private trackRect tr;
        private int nPos;

        internal RemoveTrackCommand(trackRect t)
        {
            tr = t;
            nPos = tr.t.TrackID;
        }

        internal override void doCommand()
        {
            tr.m_manager.trackRectList.RemoveAt(nPos);
            tr.m_manager.m_editor.m_Tracks.Tracks.RemoveAt(nPos);
            tr.m_manager.rectList.Remove(tr);
        }

        internal override void reDoCommand()
        {
            tr.m_manager.trackRectList.Insert(nPos,tr);
            tr.m_manager.m_editor.m_Tracks.Tracks.Insert(nPos,tr.t);
            tr.m_manager.rectList.Add(tr);
        }
    }

    internal class AddTrackCommand : MusicCommand
    {
        private trackRect tr;
        private MusicEditor m_editor;

        internal AddTrackCommand(MusicEditor e)
        {
            m_editor = e;
        }

        internal override void doCommand()
        {
            if (tr == null)
            {
                int i = m_editor.t_RectManager.trackRectList.Count;
                Track track = ScriptableObject.CreateInstance<Track>();
                track.ObjInit(i);

                tr = new trackRect(track, m_editor.t_RectManager);
            }
            else
            {
                tr.m_manager.rectList.Add(tr);
            }
            tr.m_manager.trackRectList.Add(tr);
            tr.m_manager.m_editor.m_Tracks.Tracks.Add(tr.t);
        }

        internal override void reDoCommand()
        {
            tr.m_manager.m_editor.m_Tracks.Tracks.Remove(tr.t);
            tr.m_manager.trackRectList.Remove(tr);
            tr.m_manager.rectList.Remove(tr);
        }
    }

    internal class MoveTrackCommand : MusicCommand
    {
        private trackRect tr;
        private int prePos;
        private int tarPos;

        internal MoveTrackCommand(trackRect t,int tp,int pp)
        {
            prePos = pp;
            for (int i = 0; i < tr.m_manager.trackRectList.Count; ++i)
            {
                if (tr.m_manager.trackRectList[i] == tr)
                {
                    prePos = i;
                    break;
                }
            }

            tr = t;
            tarPos = tp;
        }

        internal override void doCommand()
        {
            tr.m_manager.m_editor.m_Tracks.Tracks.Remove(tr.t);
            tr.m_manager.m_editor.m_Tracks.Tracks.Insert(tarPos,tr.t);

            tr.m_manager.trackRectList.Remove(tr);
            tr.m_manager.trackRectList.Insert(tarPos,tr);

        }

        internal override void reDoCommand()
        {
            tr.m_manager.m_editor.m_Tracks.Tracks.Remove(tr.t);
            tr.m_manager.m_editor.m_Tracks.Tracks.Insert(prePos, tr.t);

            tr.m_manager.trackRectList.Remove(tr);
            tr.m_manager.trackRectList.Insert(prePos, tr);
        }
    }
}

