
using System;

using System.Reflection;
using System.Runtime.InteropServices;
using UnityEditor;

using UnityEngine;

namespace MusicEditorSpace
{

    public static class AudioUtility
    {
        public static void PlayClip(AudioClip clip, int startSample, bool loop)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "PlayClip",

                BindingFlags.Static | BindingFlags.Public,

                null,

                new System.Type[]
                {

                    typeof(AudioClip),

                    typeof(Int32),

                    typeof(Boolean)

                },

                null

            );

            method.Invoke(

                null,

                new object[]
                {

                    clip,

                    startSample,

                    loop

                }

            );

        }



        public static void PlayClip(AudioClip clip, int startSample)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "PlayClip",

                BindingFlags.Static | BindingFlags.Public,

                null,

                new System.Type[]
                {

                    typeof(AudioClip),

                    typeof(Int32)

                },

                null

            );

            method.Invoke(

                null,

                new object[]
                {

                    clip,

                    startSample

                }

            );

        }



        public static void PlayClip(AudioClip clip)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "PlayClip",

                BindingFlags.Static | BindingFlags.Public,

                null,

                new System.Type[]
                {

                    typeof(AudioClip)

                },

                null

            );

            method.Invoke(

                null,

                new object[]
                {

                    clip

                }

            );

        }



        public static void StopClip(AudioClip clip)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "StopClip",

                BindingFlags.Static | BindingFlags.Public,

                null,

                new System.Type[]
                {

                    typeof(AudioClip)

                },

                null

            );

            method.Invoke(

                null,

                new object[]
                {

                    clip

                }

            );

        }



        public static void PauseClip(AudioClip clip)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "PauseClip",

                BindingFlags.Static | BindingFlags.Public,

                null,

                new System.Type[]
                {

                    typeof(AudioClip)

                },

                null

            );

            method.Invoke(

                null,

                new object[]
                {

                    clip

                }

            );

        }



        public static void ResumeClip(AudioClip clip)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "ResumeClip",

                BindingFlags.Static | BindingFlags.Public,

                null,

                new System.Type[]
                {

                    typeof(AudioClip)

                },

                null

            );

            method.Invoke(

                null,

                new object[]
                {

                    clip

                }

            );

        }



        public static void LoopClip(AudioClip clip, bool on)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "LoopClip",

                BindingFlags.Static | BindingFlags.Public,

                null,

                new System.Type[]
                {

                    typeof(AudioClip),

                    typeof(bool)

                },

                null

            );

            method.Invoke(

                null,

                new object[]
                {

                    clip,

                    on

                }

            );

        }



        public static bool IsClipPlaying(AudioClip clip)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "IsClipPlaying",

                BindingFlags.Static | BindingFlags.Public

            );



            bool playing = (bool) method.Invoke(

                null,

                new object[]
                {

                    clip,

                }

            );



            return playing;

        }



        public static void StopAllClips()
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "StopAllClips",

                BindingFlags.Static | BindingFlags.Public

            );



            method.Invoke(

                null,

                null

            );

        }



        public static float GetClipPosition(AudioClip clip)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "GetClipPosition",

                BindingFlags.Static | BindingFlags.Public

            );



            float position = (float) method.Invoke(

                null,

                new object[]
                {

                    clip

                }

            );



            return position;

        }



        public static int GetClipSamplePosition(AudioClip clip)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "GetClipSamplePosition",

                BindingFlags.Static | BindingFlags.Public

            );



            int position = (int) method.Invoke(

                null,

                new object[]
                {

                    clip

                }

            );



            return position;

        }



        public static void SetClipSamplePosition(AudioClip clip, int iSamplePosition)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "SetClipSamplePosition",

                BindingFlags.Static | BindingFlags.Public

            );



            method.Invoke(

                null,

                new object[]
                {

                    clip,

                    iSamplePosition

                }

            );

        }



        public static int GetSampleCount(AudioClip clip)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "GetSampleCount",

                BindingFlags.Static | BindingFlags.Public

            );



            int samples = (int) method.Invoke(

                null,

                new object[]
                {

                    clip

                }

            );



            return samples;

        }



        public static int GetChannelCount(AudioClip clip)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "GetChannelCount",

                BindingFlags.Static | BindingFlags.Public

            );



            int channels = (int) method.Invoke(

                null,

                new object[]
                {

                    clip

                }

            );



            return channels;

        }



        public static int GetBitRate(AudioClip clip)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "GetChannelCount",

                BindingFlags.Static | BindingFlags.Public

            );



            int bitRate = (int) method.Invoke(

                null,

                new object[]
                {

                    clip

                }

            );



            return bitRate;

        }



        public static int GetBitsPerSample(AudioClip clip)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "GetBitsPerSample",

                BindingFlags.Static | BindingFlags.Public

            );



            int bits = (int) method.Invoke(

                null,

                new object[]
                {

                    clip

                }

            );



            return bits;

        }



        public static int GetFrequency(AudioClip clip)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "GetFrequency",

                BindingFlags.Static | BindingFlags.Public

            );



            int frequency = (int) method.Invoke(

                null,

                new object[]
                {

                    clip

                }

            );



            return frequency;

        }



        public static int GetSoundSize(AudioClip clip)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "GetSoundSize",

                BindingFlags.Static | BindingFlags.Public

            );



            int size = (int) method.Invoke(

                null,

                new object[]
                {

                    clip

                }

            );



            return size;

        }



        public static Texture2D GetWaveForm(AudioClip clip, int channel, float width, float height)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "GetWaveForm",

                BindingFlags.Static | BindingFlags.Public

            );



            string path = AssetDatabase.GetAssetPath(clip);

            AudioImporter importer = (AudioImporter) AssetImporter.GetAtPath(path);



            Texture2D texture = (Texture2D) method.Invoke(

                null,

                new object[]
                {

                    clip,

                    importer,

                    channel,

                    width,

                    height

                }

            );



            return texture;

        }



        public static Texture2D GetWaveFormFast(AudioClip clip, int channel, int fromSample, int toSample, float width,
            float height)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "GetWaveFormFast",

                BindingFlags.Static | BindingFlags.Public

            );



            Texture2D texture = (Texture2D) method.Invoke(

                null,

                new object[]
                {

                    clip,

                    channel,

                    fromSample,

                    toSample,

                    width,

                    height

                }

            );



            return texture;

        }



        public static void ClearWaveForm(AudioClip clip)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "ClearWaveForm",

                BindingFlags.Static | BindingFlags.Public

            );



            method.Invoke(

                null,

                new object[]
                {

                    clip

                }

            );

        }



        public static bool HasPreview(AudioClip clip)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "GetSoundSize",

                BindingFlags.Static | BindingFlags.Public

            );



            bool hasPreview = (bool) method.Invoke(

                null,

                new object[]
                {

                    clip

                }

            );



            return hasPreview;

        }



        public static bool IsCompressed(AudioClip clip)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "IsCompressed",

                BindingFlags.Static | BindingFlags.Public

            );



            bool isCompressed = (bool) method.Invoke(

                null,

                new object[]
                {

                    clip

                }

            );



            return isCompressed;

        }



        public static bool IsStramed(AudioClip clip)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "IsStramed",

                BindingFlags.Static | BindingFlags.Public

            );



            bool isStreamed = (bool) method.Invoke(

                null,

                new object[]
                {

                    clip

                }

            );



            return isStreamed;

        }



        public static double GetDuration(AudioClip clip)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "GetDuration",

                BindingFlags.Static | BindingFlags.Public

            );



            double duration = (double) method.Invoke(

                null,

                new object[]
                {

                    clip

                }

            );



            return duration;

        }



        public static int GetFMODMemoryAllocated()
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "GetFMODMemoryAllocated",

                BindingFlags.Static | BindingFlags.Public

            );



            int memoryAllocated = (int) method.Invoke(

                null,

                null

            );



            return memoryAllocated;

        }



        public static float GetFMODCPUUsage()
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "GetFMODCPUUsage",

                BindingFlags.Static | BindingFlags.Public

            );



            float cpuUsage = (float) method.Invoke(

                null,

                null

            );



            return cpuUsage;

        }



        public static bool Is3D(AudioClip clip)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "Is3D",

                BindingFlags.Static | BindingFlags.Public

            );



            bool is3D = (bool) method.Invoke(

                null,

                new object[]
                {

                    clip

                }

            );



            return is3D;

        }



        public static bool IsMovieAudio(AudioClip clip)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "IsMovieAudio",

                BindingFlags.Static | BindingFlags.Public

            );



            bool isMovieAudio = (bool) method.Invoke(

                null,

                new object[]
                {

                    clip

                }

            );



            return isMovieAudio;

        }



        public static bool IsMOD(AudioClip clip)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "IsMOD",

                BindingFlags.Static | BindingFlags.Public

            );



            bool isMOD = (bool) method.Invoke(

                null,

                new object[]
                {

                    clip

                }

            );



            return isMOD;

        }



        public static int GetMODChannelCount()
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "GetMODChannelCount",

                BindingFlags.Static | BindingFlags.Public

            );



            int channels = (int) method.Invoke(

                null,

                null

            );



            return channels;

        }



        public static AnimationCurve GetLowpassCurve(AudioLowPassFilter lowPassFilter)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "GetLowpassCurve",

                BindingFlags.Static | BindingFlags.Public

            );



            AnimationCurve curve = (AnimationCurve) method.Invoke(

                null,

                new object[]
                {

                    lowPassFilter

                }

            );



            return curve;

        }



        public static Vector3 GetListenerPos()
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "GetListenerPos",

                BindingFlags.Static | BindingFlags.Public

            );



            Vector3 position = (Vector3) method.Invoke(

                null,

                null

            );



            return position;

        }



        public static void UpdateAudio()
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "UpdateAudio",

                BindingFlags.Static | BindingFlags.Public

            );



            method.Invoke(

                null,

                null

            );

        }



        public static void SetListenerTransform(Transform t)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "SetListenerTransform",

                BindingFlags.Static | BindingFlags.Public

            );



            method.Invoke(

                null,

                new object[]
                {

                    t

                }

            );

        }



        public static AudioType GetClipType(AudioClip clip)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "GetClipType",

                BindingFlags.Static | BindingFlags.Public

            );



            AudioType type = (AudioType) method.Invoke(

                null,

                new object[]
                {

                    clip

                }

            );



            return type;

        }



        public static AudioType GetPlatformConversionType(AudioType inType, BuildTargetGroup targetGroup,
            AudioCompressionFormat format)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "GetPlatformConversionType",

                BindingFlags.Static | BindingFlags.Public

            );



            AudioType type = (AudioType) method.Invoke(

                null,

                new object[]
                {

                    inType,

                    targetGroup,

                    format

                }

            );



            return type;

        }



        public static bool HaveAudioCallback(MonoBehaviour behaviour)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "HaveAudioCallback",

                BindingFlags.Static | BindingFlags.Public

            );



            bool hasCallback = (bool) method.Invoke(

                null,

                new object[]
                {

                    behaviour

                }

            );



            return hasCallback;

        }



        public static int GetCustomFilterChannelCount(MonoBehaviour behaviour)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "GetCustomFilterChannelCount",

                BindingFlags.Static | BindingFlags.Public

            );



            int channels = (int) method.Invoke(

                null,

                new object[]
                {

                    behaviour

                }

            );



            return channels;

        }



        public static int GetCustomFilterProcessTime(MonoBehaviour behaviour)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "GetCustomFilterProcessTime",

                BindingFlags.Static | BindingFlags.Public

            );



            int processTime = (int) method.Invoke(

                null,

                new object[]
                {

                    behaviour

                }

            );



            return processTime;

        }



        public static float GetCustomFilterMaxIn(MonoBehaviour behaviour, int channel)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "GetCustomFilterMaxIn",

                BindingFlags.Static | BindingFlags.Public

            );



            float maxIn = (float) method.Invoke(

                null,

                new object[]
                {

                    behaviour,

                    channel

                }

            );



            return maxIn;

        }



        public static float GetCustomFilterMaxOut(MonoBehaviour behaviour, int channel)
        {

            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(

                "GetCustomFilterMaxOut",

                BindingFlags.Static | BindingFlags.Public

            );



            float maxOut = (float) method.Invoke(

                null,

                new object[]
                {

                    behaviour,

                    channel

                }

            );



            return maxOut;

        }

    }

    /// <summary>
    /// clsMci 的摘要说明。
    /// </summary>
    public class clsMCI
    {
        public clsMCI()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        //定义API函数使用的字符串变量 
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        private string Name = "";

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        private string durLength = "";

        [MarshalAs(UnmanagedType.LPTStr, SizeConst = 128)]
        private string TemStr = "";

        //定义播放状态枚举变量
        public enum State
        {
            mPlaying = 1,
            mPuase = 2,
            mStop = 3
        };

        //结构变量
        public struct structMCI
        {
            public bool bMut;
            public int iDur;
            public int iPos;
            public int iVol;
            public int iBal;
            public string iName;
            public State state;
        };

        public structMCI mc = new structMCI();

        //取得播放文件属性
        public string FileName
        {
            get { return mc.iName; }
            set
            {
                //ASCIIEncoding asc = new ASCIIEncoding(); 
                try
                {
                    TemStr = "";
                    TemStr = TemStr.PadLeft(127, Convert.ToChar(" "));
                    Name = Name.PadLeft(260, Convert.ToChar(" "));
                    mc.iName = value;
                    APIClass.GetShortPathName(mc.iName, Name, Name.Length);
                    Name = GetCurrPath(Name);
                    //Name = "open " + Convert.ToChar(34) + Name + Convert.ToChar(34) + " alias media";
                    Name = "open " + Convert.ToChar(34) + Name + Convert.ToChar(34) + " alias media";
                    APIClass.mciSendString("close all", TemStr, TemStr.Length, 0);
                    APIClass.mciSendString(Name, TemStr, TemStr.Length, 0);
                    APIClass.mciSendString("set media time format milliseconds", TemStr, TemStr.Length, 0);
                    mc.state = State.mStop;
                }
                catch
                {

                }
            }
        }

        //播放
        public void play()
        {
            APIClass.mciSendString("play media", null, 0, 0);
            mc.state = State.mPlaying;
        }

        public static void QuickPlay(string name)
        {
            APIClass.GetShortPathName(name, name, name.Length);
            name = GetCurrPath(name);
            name = "open " + Convert.ToChar(34) + name + Convert.ToChar(34) + " alias media";
            APIClass.mciSendString(name, null, 0, 0);
            APIClass.mciSendString("play media", null, 0, 0);
        }

        //停止
        public void StopT()
        {
            TemStr = "";
            TemStr = TemStr.PadLeft(128, Convert.ToChar(" "));
            APIClass.mciSendString("close media", TemStr, 128, 0);
            APIClass.mciSendString("close all", TemStr, 128, 0);
            mc.state = State.mStop;
        }

        public void Puase()
        {
            TemStr = "";
            TemStr = TemStr.PadLeft(128, Convert.ToChar(" "));
            APIClass.mciSendString("pause media", TemStr, TemStr.Length, 0);
            mc.state = State.mPuase;
        }

        private static string GetCurrPath(string name)
        {
            if (name.Length < 1) return "";
            name = name.Trim();
            name = name.Substring(0, name.Length - 1);
            return name;
        }

        //总时间
        public int Duration
        {
            get
            {
                durLength = "";
                durLength = durLength.PadLeft(128, Convert.ToChar(" "));
                APIClass.mciSendString("status media length", durLength, durLength.Length, 0);
                durLength = durLength.Trim();
                if (durLength == "") return 0;
                return (int) (Convert.ToDouble(durLength) / 1000f);
            }
        }

        //当前时间
        public int CurrentPosition
        {
            get
            {
                durLength = "";
                durLength = durLength.PadLeft(128, Convert.ToChar(" "));
                APIClass.mciSendString("status media position", durLength, durLength.Length, 0);
                durLength = durLength.Trim();
                if (durLength == "") return 0;
                mc.iPos = (int) (Convert.ToDouble(durLength) / 1000f);
                return mc.iPos;
            }
            set
            {
                string Command = String.Format("Seek Media to {0}", value * 1000);
                APIClass.mciSendString(Command, null, 0, 0); //使播放停止  
                mc.state = State.mStop;
                mc.iPos = value * 1000;
            }
        }
    }

    public class APIClass
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetShortPathName(
            string lpszLongPath,
            string shortFile,
            int cchBuffer
        );

        [DllImport("winmm.dll", EntryPoint = "mciSendString", CharSet = CharSet.Auto)]
        public static extern int mciSendString(
            string lpstrCommand,
            string lpstrReturnString,
            int uReturnLength,
            int hwndCallback
        );
    }



}

