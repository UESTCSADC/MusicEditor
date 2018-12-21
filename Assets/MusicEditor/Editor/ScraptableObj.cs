using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

namespace MusicEditorSpace
{

    internal class TrackScriptableObject : ScriptableObject
    {
        //StartEvent
        [SerializeField]
        public UnityEvent startEvent;
        [SerializeField]
        public UnityEvent endEvent;
        public List<TimeStamp> MusicEvents;
        public List<Track> Tracks;
        public AudioClip boundingMusic;

        internal float m_bpm;
        internal float m_bpmOffset;

        public void ObjInit()
        {
            startEvent = null;
            endEvent = null;
            MusicEvents = new List<TimeStamp>();
            Tracks = new List<Track>();
        }

        //播放时，输入与生产判定之间的延迟

    }

    internal class Track : ScriptableObject
    {
        internal int TrackID;
        internal int nPos;
        [SerializeField]
        public UnityEvent trackInputEvent;
        [SerializeField]
        public UnityEvent trackPassEvent;
        internal List<TimeStamp> timestamps;


        internal KeyCode controller;

        internal AudioClip defaultHitAudio;

        public void ObjInit(int id)
        {
            this.name = "Track" + id;
            timestamps = new List<TimeStamp>();
            trackInputEvent = null;
            nPos = 0;
            controller = KeyCode.None;
            TrackID = id;
            defaultHitAudio = Resources.Load<AudioClip>("SampleHit");
        }

        public void InputCheck(float nowTime, KeyCode kc, float checkableEarly)
        {
            if (kc == controller)
            {
                trackInputEvent.Invoke();
                //当输入接近目标值时判断
                if (timestamps[nPos].time - checkableEarly > nowTime)
                {
                    //处理检查准确率事件
                    timestamps[nPos].isDid = true;
                    ++nPos;
                }
            }
        }

        public void MissCheck(float nowTime, float missDelay)
        {
            while (nowTime - missDelay > timestamps[nPos].time)
            {
                //处理miss事件
                timestamps[nPos].isDid = true;
                ++nPos;
            }
        }

        public TimeStamp addTimeStamp(float p)
        {
            TimeStamp st = ScriptableObject.CreateInstance<TimeStamp>();

            st.init(p,0);
            for (int i = 0; i < timestamps.Count; i++)
            {
                if (timestamps[i].time > p)
                {
                    timestamps.Insert(i,st);
                    return st;
                }
            }
            timestamps.Add(st);
            return st;
        }

        public void removeTimeStamp(float p)
        {
            foreach (var ts in timestamps)
            {
                if (ts.time == p)
                {
                    timestamps.Remove(ts);
                    break;
                }
            }
        }
    }

    internal class TimeStamp : ScriptableObject
    {
        public float time;
        public bool isDid;
        public float length;

        public void init(float t, float l)
        {
            time = t;
            isDid = false;
            length = l;
        }
    }
}
