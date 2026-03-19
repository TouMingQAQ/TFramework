using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TFrameworkKit.Console.Command;
using UnityEditor;
using UnityEngine;

namespace TFramework.DebugCore
{
    [Command("Debug")]
    public static class DebugCommand
    {
        [CommandMethod]
        public static void TagList()
        {
            foreach (var (tagKey,tagInfo) in Debug.tagMap)
            {
                UnityEngine.Debug.Log($"[{tagInfo.Tag.TagStr}] [{(tagInfo.IsShow ? "ON" : "OFF")}]");
            }
        }
        [CommandMethod]
        public static void EnableTag([CommandParameter("Tag")]string tagStr)
        {
            foreach (var (tagKey,tagInfo) in Debug.tagMap)
            {
                if (tagInfo.Tag.TagStr == tagStr)
                {
                    var newTag = tagInfo;
                    newTag.IsShow = true;
                    Debug.tagMap[tagKey] = newTag;
                    UnityEngine.Debug.Log($"Enable [{tagInfo.Tag.TagStr}]");
                }
            }
        }
        [CommandMethod]
        public static void EnableAllTag()
        {
            foreach (var (tagKey,tagInfo) in Debug.tagMap)
            {
                var newTag = tagInfo;
                newTag.IsShow = true;
                Debug.tagMap[tagKey] = newTag;
                UnityEngine.Debug.Log($"Enable [{tagInfo.Tag.TagStr}]");
            }
        }
        [CommandMethod]
        public static void DisableTag([CommandParameter("Tag")]string tagStr)
        {
            foreach (var (tagKey,tagInfo) in Debug.tagMap)
            {
                if (tagInfo.Tag.TagStr == tagStr)
                {
                    var newTag = tagInfo;
                    newTag.IsShow = false;
                    Debug.tagMap[tagKey] = newTag;
                    UnityEngine.Debug.Log($"Disable [{tagInfo.Tag.TagStr}]");
                }
            }
        }
        [CommandMethod]
        public static void DisableAllTag()
        {
            foreach (var (tagKey,tagInfo) in Debug.tagMap)
            {
                var newTag = tagInfo;
                newTag.IsShow = false;
                Debug.tagMap[tagKey] = newTag;
                UnityEngine.Debug.Log($"Disable [{tagInfo.Tag.TagStr}]");
            }
        }
        
    }
    public interface ITag
    {
        /// <summary>
        /// 默认显示
        /// </summary>
        public virtual bool DefaultShow => true;
        public string TagStr { get; }
        public virtual Color TagColor => Color.cyan;
        public virtual string OnLog(string message) => message;
        public virtual string OnLogWarning(string message) => message;
        public virtual string OnLogError(string message) => message;

        public string GetMessage(string message)
        {
            return $"[<color=#{UnityEngine.ColorUtility.ToHtmlStringRGB(TagColor)}>{TagStr}</color>] {message}";
        }
    }

    public struct DefaultTag : ITag
    {
        public bool DefaultShow => true;
        public string TagStr => "Default";
        public Color TagColor => Color.black;
        public string OnLog(string message) => message;
        public string OnLogWarning(string message) => message;
        public string OnLogError(string message) => message;
    }

    
    public static class Debug
    {
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod]
#endif
        static void Init()
        {
            tagMap.Clear();
            MakeSure<DefaultTag>();
        }
        internal static Dictionary<Type, TagInfo> tagMap = new();
        internal struct TagInfo
        {
            public ITag Tag;
            public bool IsShow;
        }
        public static void Log(object message)
        {
            Log<DefaultTag>(message);
        }
        public static void LogWarning(object message)
        {
            LogWarning<DefaultTag>(message);
        }
        public static void LogError(object message)
        {
            LogError<DefaultTag>(message);
        }

        public static void Log<T>(object message) where T : ITag
        {
            MakeSure<T>();
            var type = typeof(T);
            var tag = tagMap[type];
            if(!tag.IsShow)
                return;
            var tagMessage = tag.Tag.OnLog(message.ToString());
            UnityEngine.Debug.Log(tag.Tag.GetMessage(tagMessage));
        }
        public static void LogWarning<T>(object message) where T : ITag
        {
            MakeSure<T>();
            var type = typeof(T);
            var tag = tagMap[type];
            if(!tag.IsShow)
                return;
            var tagMessage = tag.Tag.OnLogWarning(message.ToString());
            UnityEngine.Debug.LogWarning(tag.Tag.GetMessage(tagMessage));
        }
        public static void LogError<T>(object message) where T : ITag
        {
            MakeSure<T>();
            var type = typeof(T);
            var tag = tagMap[type];
            if(!tag.IsShow)
                return;
            var tagMessage = tag.Tag.OnLogError(message.ToString());
            UnityEngine.Debug.LogError(tag.Tag.GetMessage(tagMessage));
        }

        internal static void MakeSure<T>() where T : ITag
        {
            var type = typeof(T);
            if(tagMap.ContainsKey(type))
                return;
            var tag = default(T);
            tagMap[type] = new TagInfo()
            {
                Tag = tag,
                IsShow = tag.DefaultShow
            };
        }
    }
}

