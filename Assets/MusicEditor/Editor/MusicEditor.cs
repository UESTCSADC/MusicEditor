using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEditor;

using GUILayout = UnityEngine.GUILayout;
using GUIStyle = UnityEngine.GUIStyle;

//用于计算bmp的第三方库
using System.Numerics;
using DSPLib;


namespace MusicEditorSpace
{
    internal class MusicEditor :ScriptableObject
    {
        private readonly List<int> stepList = new List<int>() { 1, 2, 5, 10, 30, 60 };

        //资源相关
        internal AudioClip m_Music;
        internal TrackScriptableObject m_Tracks;
        internal clsMCI m_hit;

        //资源相关
        internal float m_BPM;
        internal float m_BPMOffset;
        private float m_musicClipLength;

        //播放相关变量
        internal float m_Time; //播放进度
        internal bool m_isPlaying;//播放状态
        internal bool m_isLoop = false;
        internal float m_loopS = 0.0f;
        internal float m_loopE = 1.0f;

        //时间尺相关变量
        private float m_timeRulerScale; //初始状态大概在全屏下显示10s，缩小到最小时可以显示整首曲子
        internal Vector2 m_ScrollPosition;
        internal float m_TimeRulerLength;
        private int m_timeRulerStep;

        //工具
        internal MusicCommandBuffer t_CommandBuffer;
        internal InteractableRectManager t_RectManager;

        //选项变量
        internal bool s_isMagnat;
        internal bool s_isKeyAdd;
        internal bool s_isKeep;
        internal bool s_isGoPlay;

        internal bool s_isTest;
        internal MusicPreviewWindow m_testWindow;

       static Material m_rectMaterial;

        public void OnEditorAwake()
        {    
            t_CommandBuffer = new MusicCommandBuffer(this);
            t_RectManager = new InteractableRectManager(this);

            m_musicClipLength = 0.0f;
            m_timeRulerScale = 1.0f;
            m_timeRulerStep = 1;

            s_isMagnat = true;
            s_isKeep = true;
            s_isKeyAdd = true;

            s_isTest = false;

            m_Time = 0.0f;
            m_ScrollPosition = Vector2.zero;

            m_hit = new clsMCI();
            m_hit.FileName = "Assets/MusicEditor/Resources/SampleHit.wav";

            m_Tracks = null;
            m_Music = null;

            CreateRectMaterial();
        }

        void CreateRectMaterial()
        {
            if (!m_rectMaterial)
            {
                // Unity has a built-in shader that is useful for drawing
                // simple colored things.
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                m_rectMaterial = new Material(shader);
                m_rectMaterial.hideFlags = HideFlags.HideAndDontSave;
                // Turn on alpha blending
                m_rectMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                m_rectMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                // Turn backface culling off
                m_rectMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                // Turn off depth writes
                m_rectMaterial.SetInt("_ZWrite", 0);
            }
        }

        public void OnEditorClose()
        {
            AudioUtility.StopAllClips();
        }

        public void OnEditorGUI(Rect mainRect)
        {
          
            if (m_Music != null && m_isPlaying)
                m_Time = (float)AudioUtility.GetClipSamplePosition(m_Music) / (float)AudioUtility.GetSampleCount(m_Music);
            if (m_isLoop &&(m_Time > m_loopE || m_Time < m_loopS))
            {
                m_Time = m_loopS + 0.001f;
                AudioUtility.SetClipSamplePosition(m_Music,(int)(m_Time * (float)AudioUtility.GetSampleCount(m_Music)));
                foreach (var tRect in t_RectManager.trackRectList)
                {
                    tRect.playPosRecauculate(m_Time);
                }
            }

            m_TimeRulerLength = (MusicEditorWindow.MainRect.width - 300) / m_timeRulerScale;

            Rect rect2 = new Rect(280, 0, (mainRect.width - 300) / m_timeRulerScale + 20, 16);
            this.BPMRulerOnGUI(rect2, mainRect, m_ScrollPosition);

            //绘制所有的事件列表
            GLDrawRect(new Rect(0, 45, 280, mainRect.height - 45), new Color(0.8f, 0.8f, 0.8f, 1.0f));
            this.TrackListOnGUI(mainRect);//所有的轨道序列绘制

            Rect rect1 = new Rect(280, 18, (mainRect.width - 300) / m_timeRulerScale + 20, 18);
            this.TimeRulerOnGUI(rect1, mainRect, m_ScrollPosition); //绘制时间尺


            //绘制时间指针
            Rect timeMark = new Rect(290 + m_Time * m_TimeRulerLength - m_ScrollPosition.x, 36, 2, mainRect.height - 36);
            if (s_isKeep && m_isPlaying)
            {
                m_ScrollPosition.x = m_Time * m_TimeRulerLength - 30;
                timeMark.x = 320;
            }

            GLDrawRect(timeMark,Color.white);

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal(EditorStyles.toolbarButton, new GUILayoutOption[1] { GUILayout.Width(280) });
            this.PlayControlsOnGUI(); //播放操作界面
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(EditorStyles.toolbarButton, new GUILayoutOption[1] { GUILayout.Width(280) });
            this.MusicControlsOnGUI();//绘制音乐管理界面
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(EditorStyles.toolbarButton, new GUILayoutOption[1] { GUILayout.Width(280) });
            this.TrackSourceControlsOnGUI();//绘制音轨操作面板
            GUILayout.EndHorizontal();

            

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal(EditorStyles.toolbarButton, new GUILayoutOption[1] { GUILayout.Width(280) });
            this.OptionControlsOnGUI(); //绘制底部选项栏
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            m_ScrollPosition =
                GUILayout.BeginScrollView(m_ScrollPosition, new GUILayoutOption[1] {GUILayout.ExpandWidth(true) });
            

                
            if (m_Tracks!=null)
                GUILayout.Label("", EditorStyles.label, new GUILayoutOption[3] { GUILayout.Width(mainRect.width - 280), GUILayout.Height(m_Tracks.Tracks.Count * 85 + 85),GUILayout.Width(rect1.width)});
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void BPMRulerOnGUI(Rect rect, Rect mainRect, Vector2 offset)
        {
            if (m_BPM != 0 && m_musicClipLength != 0.0f)
            {
                float step = m_musicClipLength / m_BPM;
                float stepWidth = (rect.width - 20) / m_musicClipLength;

                for (float i = -step; i <= m_musicClipLength; i += step)
                {
                    Rect rulerMark = new Rect(rect.position, new Vector2(1.0f, mainRect.height));
                    rulerMark.x = rect.x + 10.0f + (i + m_BPMOffset * step) * stepWidth - offset.x;
                    rulerMark.y += 20;
                    float count = (mainRect.width - 300) / stepWidth;

                    float g = 0.3f;
                    if (count > 80)
                        g = 0.3f + 0.15f * (1 - (count - 80) / (step - 80));
                    GLDrawRect(rulerMark, new Color(g, g, g, 0.5f));

                    for (int j = 0; j < 7; ++j)
                    {
                        rulerMark.x += stepWidth * step * 0.125f;
                        if (j % 4 == 3 && count < 80)
                        {
                            GLDrawRect(rulerMark, new Color(0.6f, 0.6f, 0.6f, 0.5f));
                            continue;
                        }

                        if (j % 2 == 1 && count < 40)
                        {
                           
                            GLDrawRect(rulerMark, new Color(0.6f, 0.6f, 0.6f, 0.5f));
                            continue;
                        }

                        if (count < 20)
                        {
                            
                            GLDrawRect(rulerMark, new Color(0.6f, 0.6f, 0.6f, 0.5f));
                        }
                    }
                }
            }
        }

        private void TimeRulerOnGUI(Rect rect,Rect mainRect,Vector2 offset)
        {
            //绘制方块体
            GLDrawRect(rect,new Color(0.5f,0.5f,0.5f,1.0f));

            if (m_musicClipLength != 0)
            {
                //绘制有效部分
                float lX = 290 - m_ScrollPosition.x;
                float rX = 290 + m_TimeRulerLength - m_ScrollPosition.x;
                Rect r = new Rect(rect);
                r.x = Mathf.Max(280, lX);
                r.width = Mathf.Min(rX - r.x, r.width);
                GLDrawRect(r, new Color(0.7f, 0.7f, 0.7f, 1.0f));
                //绘制循环部分
                if (m_isLoop)
                {
                    lX = 290 - m_ScrollPosition.x + m_loopS * m_TimeRulerLength;
                    rX = 290 - m_ScrollPosition.x + m_loopE * m_TimeRulerLength;
                    r = new Rect(rect);
                    r.x = Mathf.Max(280, lX);
                    r.width = Mathf.Min(rX - r.x, r.width);
                    GLDrawRect(r, new Color(0.0f, 0.5f, 1.0f, 1.0f));
                }

                float stepWidth = (rect.width - 20) / m_musicClipLength;

                m_timeRulerStep = 1;
                for (int i = 0; i < stepList.Count; ++i)
                {
                    //超过需要的值,尽量以最高的精度来绘制尺子
                    if ((mainRect.width - 360.0f) / (stepWidth * stepList[i]) <= 15)
                    {
                        m_timeRulerStep = stepList[i];
                        break;
                    }
                }

                //绘制尺标和数字
                for (int i = 0; i <= m_musicClipLength; i += (int) m_timeRulerStep)
                {
                    Rect rulerMark = new Rect(rect.position, new Vector2(1.0f, rect.height * 0.5f));
                    rulerMark.x = rect.x + 10.0f + i * stepWidth - offset.x;
                    rulerMark.y += rect.height * 0.5f;
                    GLDrawRect(rulerMark, new Color(0.3f, 0.3f, 0.3f, 1));

                    Rect labelRect = new Rect(rulerMark.position,
                        new Vector2(stepWidth * m_timeRulerStep, rect.height * 0.8f));
                    labelRect.y -= rect.height * 0.5f;
                    GUI.Label(labelRect, (i / 60 < 10 ? "0" : "") + i / 60 + ":" + (i % 60 < 10 ? "0" : "") + i % 60,
                        EditorStyles.label);

                    rulerMark.y += rect.height * 0.3f;
                    rulerMark.height = rect.height * 0.2f;
                    for (int j = 0; j < 4; ++j)
                    {
                        rulerMark.x += stepWidth * m_timeRulerStep * 0.2f;
                        GLDrawRect(rulerMark, new Color(0.6f, 0.6f, 0.6f, 1));
                    }
                }
            }
        }

        private void PlayControlsOnGUI()
        {            
            GUIContent goBack = new GUIContent(EditorGUIUtility.IconContent("Animation.FirstKey"));
            goBack.tooltip = "重新播放";
            if (GUILayout.Button(goBack, EditorStyles.toolbarButton,
                new GUILayoutOption[1] { GUILayout.Width(30) }))
            {
                //按钮-重新播放
                m_Time = 0.0f;
                AudioUtility.StopClip(m_Music);
                m_isPlaying = false;
                foreach (var tRect in t_RectManager.trackRectList)
                {
                    tRect.playPosRecauculate(0);
                }
            }

            GUIContent goPlay = new GUIContent(EditorGUIUtility.IconContent("Animation.Play"));
            goPlay.tooltip = "播放";
            if (GUILayout.Button(goPlay, EditorStyles.toolbarButton,
                new GUILayoutOption[1] { GUILayout.Width(30) }))
            {
                //按钮-播放
                if (m_Music != null)
                {
                    if (m_isPlaying)
                    {
                        AudioUtility.PauseClip(m_Music);
                    }
                    else
                    {
                        if (AudioUtility.IsClipPlaying(m_Music))
                            AudioUtility.ResumeClip(m_Music);
                        else
                        {

                            AudioUtility.PlayClip(m_Music);
                            AudioUtility.SetClipSamplePosition(m_Music,
                                (int) (m_Time * (float) AudioUtility.GetSampleCount(m_Music)));
                        }

                    }
                }

                m_isPlaying = !m_isPlaying;
                
               
            }

            //BMP参数
            GUILayout.Label("BPM",GUILayout.Width(30));
            m_BPM = EditorGUILayout.DelayedFloatField(m_BPM, new GUILayoutOption[1] { GUILayout.Width(40) });
            if (m_Tracks!= null && m_Tracks.m_bpm != m_BPM)
                m_Tracks.m_bpm = m_BPM;
            m_BPMOffset = GUILayout.HorizontalSlider(m_BPMOffset, 0, 1);
            if (m_Tracks != null && m_Tracks.m_bpmOffset != m_BPMOffset)
                m_Tracks.m_bpmOffset = m_BPMOffset;
        }

        private void MusicControlsOnGUI()
        {
            
            //this.LoadMusicButtonOnGUI();//按钮-导入音乐文件
            AudioClip clip = EditorGUILayout.ObjectField(m_Music, typeof(AudioClip), false) as AudioClip;
            if (clip != m_Music)
            {   
                m_Music = clip;
                //bpmCalculate();
                m_musicClipLength = m_Music.length;
                if (m_Tracks != null)
                    m_Tracks.boundingMusic = m_Music;
            }

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("AudoKey", EditorStyles.toolbarButton))
            {
                //按钮-进入测试模式
                if (t_RectManager.trackRectList.Count > 0)
                {
                    List<float> autoKey = bpmCalculate(100);
                    Debug.Log(autoKey.Count);
                    foreach (var key in autoKey)
                    {
                        t_CommandBuffer.AddCommand(new AddKeyCommand(t_RectManager.trackRectList[0], key));
                    }
                }
            }
        }

        private void TrackSourceControlsOnGUI()
        {

            GUIContent newTrack = new GUIContent(EditorGUIUtility.IconContent("ol plus"));
            newTrack.tooltip = "新建音轨";
            if (GUILayout.Button(newTrack, EditorStyles.toolbarButton,
                new GUILayoutOption[1] { GUILayout.Width(30) }))
            {
                m_Tracks = ScriptableObject.CreateInstance<TrackScriptableObject>();
                m_Tracks.ObjInit();

                AssetDatabase.CreateAsset(m_Tracks,"Assets/MusicEditor/Resources/Tracks/new track.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

            }

            //载入轨道信息
            TrackScriptableObject trackTemp = EditorGUILayout.ObjectField(m_Tracks, typeof(TrackScriptableObject), false) as TrackScriptableObject;
            if (trackTemp != m_Tracks)
            {
                m_Tracks = trackTemp;
                m_BPM = m_Tracks.m_bpm;
                m_BPMOffset = m_Tracks.m_bpmOffset;

                t_RectManager = new InteractableRectManager(this);
                if(m_Tracks != null)
                foreach (var rect in m_Tracks.Tracks)
                {
                    trackRect t = new trackRect(rect,t_RectManager);
                    t_RectManager.trackRectList.Add(t);
                }
                if (m_Music == null && m_Tracks.boundingMusic != null)
                {
                    m_Music = m_Tracks.boundingMusic;
                    m_musicClipLength = m_Music.length;
                }
            }


            GUILayout.FlexibleSpace();

            if (GUILayout.Button("TestMode", EditorStyles.toolbarButton))
            {
                //按钮-进入测试模式
                if(m_testWindow == null)
                    m_testWindow = MusicPreviewWindow.getNewPreview(this);
            }
        }

        private void TrackListOnGUI(Rect mainRect)
        {
            Event e = Event.current;
            if(t_RectManager!=null)
                t_RectManager.OnInternalGUI(e, m_ScrollPosition, mainRect);
            if (m_Tracks != null)
            {
                
                GUILayout.BeginArea(new Rect(40, m_Tracks.Tracks.Count * 85 - m_ScrollPosition.y + 60, 200, 30),
                    EditorStyles.whiteLabel);
                if (GUILayout.Button("New Track", EditorStyles.miniButton))
                {
                    //添加新轨道
                    t_CommandBuffer.AddCommand(new AddTrackCommand(this));
                    t_CommandBuffer.DoCommandList();
                }

                GUILayout.EndArea();
            }
            else
            {
                Rect r = new Rect(100 + (MusicEditorWindow.MainRect.width - 280) * 0.5f, (MusicEditorWindow.MainRect.height - 60) * 0.5f,240,40);
                GUIStyle gs = new GUIStyle();
                gs.fontSize = 40;
                GUI.Label(r,"请新建或者读取音轨文件",gs);
            }

        }

        private void OptionControlsOnGUI()
        {
            m_timeRulerScale = GUILayout.HorizontalSlider(m_timeRulerScale,1,0.02f,new GUILayoutOption[1]{GUILayout.Width(100)});
            GUILayout.FlexibleSpace();
            //播放按键效果音
            GUIContent playCAudio = new GUIContent(EditorGUIUtility.IconContent("preAudioAutoPlayOff"));
            s_isGoPlay = GUILayout.Toggle(s_isGoPlay, playCAudio, EditorStyles.toolbarButton, GUILayout.Width(24));
            //使用操作键添加key
            GUIContent CAddKey = new GUIContent(EditorGUIUtility.IconContent("FilterByLabel"));
            s_isMagnat = GUILayout.Toggle(s_isMagnat, CAddKey, EditorStyles.toolbarButton, GUILayout.Width(24));
            //伴随移动
            GUIContent magnat = new GUIContent(EditorGUIUtility.IconContent("minmax slider thumb"));
            s_isKeep = GUILayout.Toggle(s_isKeep, magnat, EditorStyles.toolbarButton,GUILayout.Width(24));
        }

        internal void doTrackC(Event e)
        {
            foreach (var t in t_RectManager.trackRectList)
            {
                if (e.type == EventType.KeyDown && e.keyCode == t.t.controller && t.t.controller != KeyCode.None)
                {
                    if (s_isKeyAdd)
                    {
                        if (t.longST == null)
                        {
                            m_hit.CurrentPosition = 0;
                            m_hit.play();
                            TimeStamp tempTS = ScriptableObject.CreateInstance<TimeStamp>();
                            tempTS.init(getMagnetTime(m_Time),0);
                            t.longST = new TimeStampRect(tempTS,t);
                            t.longST.isSelected = true;
                        }
                        t.longST.ts.length = getMagnetTime(m_Time) - t.longST.ts.time;
                    }
                }  

                if (e.type == EventType.KeyUp && e.keyCode == t.t.controller && t.t.controller != KeyCode.None)
                {
                    if (s_isKeyAdd && t.longST != null)
                    {
                        if (t.longST.ts.length < 0.005)
                        {
                            t.longST.ts.length = 0;
                            t_CommandBuffer.AddCommand(new AddKeyCommand(t, t.longST.ts.time));
                        }
                        else
                        {
                            t_CommandBuffer.AddCommand(new AddKeyCommand(t, t.longST));
                            m_hit.CurrentPosition = 0;
                            m_hit.play();
                        }
                           
                        t_CommandBuffer.DoCommandList();

                        t.playPosRecauculate(m_Time);
                        t.longST = null;
                    }
                }
            }
           if(t_CommandBuffer.isDirty)
               t_CommandBuffer.DoCommandList();
        }

        internal float getMagnetTime(float t)
        {
            if (m_BPM == 0 || !s_isMagnat)
                return t;
            float bmpStep = m_TimeRulerLength / m_BPM;
            float offset = m_BPMOffset * m_TimeRulerLength / m_BPM;
            float count = (MusicEditorWindow.MainRect.width - 290 ) / bmpStep;
            if (count < 80)
                bmpStep /= 2;
            if (count < 40)
                bmpStep /= 2;
            if (count < 20)
                bmpStep /= 2;
            float lowPos = Mathf.Max((Mathf.Floor((t * m_TimeRulerLength - offset) / bmpStep) * bmpStep), 0);
            return (t * m_TimeRulerLength - lowPos) / bmpStep >= 0.5f
                ? Mathf.Clamp((lowPos + bmpStep + offset) / m_TimeRulerLength, 0.0f, 1.0f)
                : Mathf.Clamp((lowPos + offset) / m_TimeRulerLength, 0.0f, 1.0f);
        }

        private List<float> bpmCalculate(int keyCount)
        {
            int numChannels = m_Music.channels;
            int numTotalSamples = m_Music.samples;
            float clipLength = m_Music.length;
            int sampleRate = m_Music.frequency;

            float[] multiChannelSamples = new float[m_Music.samples * m_Music.channels];
            m_Music.GetData(multiChannelSamples, 0);

            float[] preProcessedSamples = new float[numTotalSamples];
            int numProcessed = 0;
            float combinedChannelAverage = 0f;

            for (int i = 0; i < multiChannelSamples.Length; i++)
            {

                combinedChannelAverage += multiChannelSamples[i];
                if ((i + 1) % numChannels == 0)
                {
                    preProcessedSamples[numProcessed] = combinedChannelAverage / numChannels;
                    numProcessed++;
                    combinedChannelAverage = 0f;
                }
            }

            int spectrumSampleSize = 1024;

            int iterations = preProcessedSamples.Length / spectrumSampleSize;

            FFT fft = new FFT();

            fft.Initialize((UInt32) spectrumSampleSize);

            double[] sampleChunk = new double[spectrumSampleSize];

            SpectralFluxAnalyzer preProcessedSpectralFluxAnalyzer = new SpectralFluxAnalyzer();

            for (int i = 0; i < iterations; i++)
            {

                // Grab the current 1024 chunk of audio sample data

                Array.Copy(preProcessedSamples, i * spectrumSampleSize, sampleChunk, 0, spectrumSampleSize);

                // Apply our chosen FFT Window

                double[] windowCoefs = DSP.Window.Coefficients(DSP.Window.Type.Hanning, (uint) spectrumSampleSize);

                double[] scaledSpectrumChunk = DSP.Math.Multiply(sampleChunk, windowCoefs);

                double scaleFactor = DSP.Window.ScaleFactor.Signal(windowCoefs);



                // Perform the FFT and convert output (complex numbers) to Magnitude

                Complex[] fftSpectrum = fft.Execute(scaledSpectrumChunk);

                double[] scaledFFTSpectrum = DSP.ConvertComplex.ToMagnitude(fftSpectrum);

                scaledFFTSpectrum = DSP.Math.Multiply(scaledFFTSpectrum, scaleFactor);

                // These 1024 magnitude values correspond (roughly) to a single point in the audio timeline
                float lengthPerSample = clipLength / numTotalSamples;
                float curSongTime = ((1f / (float)sampleRate) * i) * spectrumSampleSize;

                // Send our magnitude data off to our Spectral Flux Analyzer to be analyzed for peaks
               
                preProcessedSpectralFluxAnalyzer.analyzeSpectrum(Array.ConvertAll(scaledFFTSpectrum, x => (float)x), curSongTime);
            }

            float maxPeak = 0;
            List< SpectralFluxInfo > peakList = new List<SpectralFluxInfo>();
            for (int i = 0; i < preProcessedSpectralFluxAnalyzer.spectralFluxSamples.Count; i++)
            {
                SpectralFluxInfo sfSample = preProcessedSpectralFluxAnalyzer.spectralFluxSamples[i];
                if (sfSample.isPeak)
                {
                    maxPeak = Mathf.Max(maxPeak, sfSample.prunedSpectralFlux);
                    peakList.Add(sfSample);
                }
            }

            float threshold = 0.5f * maxPeak;
            while (peakList.Count > keyCount)
            {
                Debug.Log(threshold + " : " + peakList.Count);
                threshold *= 1.2f;
                for (int i = 0; i < peakList.Count; ++i)
                {
                    if (peakList[i].prunedSpectralFlux <threshold)
                    {
                        peakList.RemoveAt(i);
                        --i;
                    }
                }
            }

            List<float> time = new List<float>();
            foreach (var p in peakList)
            {
                time.Add(p.time / m_musicClipLength);
            }
            return time;
        }

        //绘制方块体时,应该为
        public static void GLDrawRect(Rect r, Color c)
        {
            GL.PushMatrix();
            GL.MultMatrix(UnityEditor.Handles.matrix);
            //获取canvas的默认材质绘制
            Material mat = m_rectMaterial;
            mat.SetPass(0);
            GL.Begin(GL.QUADS);
            GL.Color(c);
            GL.Vertex((Vector3)r.min);
            GL.Vertex((Vector3)new Vector2(r.xMax, r.yMin));
            GL.Vertex((Vector3)r.max);
            GL.Vertex((Vector3)new Vector2(r.xMin, r.yMax));
            GL.End();
            GL.PopMatrix();
        }

        public static void GLDrawTimeStamp(Vector2 pos,float size,Color c)
        {

            GL.PushMatrix();
            GL.MultMatrix(UnityEditor.Handles.matrix);
            //获取canvas的默认材质绘制
            Material mat = m_rectMaterial;
            mat.SetPass(0);
            //绘制外轮廓
            GL.Begin(GL.QUADS);
            GL.Color(Color.black);
            GL.Vertex(new Vector3(pos.x, pos.y - size));
            GL.Vertex(new Vector3(pos.x + size, pos.y));
            GL.Vertex(new Vector3(pos.x, pos.y + size));
            GL.Vertex(new Vector3(pos.x - size, pos.y));
            GL.End();
            //绘制内部分
            GL.Begin(GL.QUADS);
            GL.Color(c);
            GL.Vertex(new Vector3(pos.x, pos.y - size + 1f));
            GL.Vertex(new Vector3(pos.x + size - 1f, pos.y));
            GL.Vertex(new Vector3(pos.x, pos.y + size - 1f));
            GL.Vertex(new Vector3(pos.x - size + 1f, pos.y));
            GL.End();

            GL.PopMatrix();


        }
    }
}