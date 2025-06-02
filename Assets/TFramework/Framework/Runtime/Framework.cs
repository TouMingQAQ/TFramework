using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TFramework.Runtime
{
    public partial class Framework : MonoBehaviour
    {
        public static Framework Instance;
        protected Dictionary<Type, BaseManager> _systemMap = new();
        
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        public void AddManager<T>(T manager) where T : BaseManager
        {
            var type = typeof(T);
            _systemMap[type] = manager;
        }
        public T GetManager<T>() where T : BaseManager
        {
            var type = typeof(T);
            if (_systemMap.TryGetValue(type, out var value))
                return value as T;
            else
                Debug.LogError($"<color=red>[{GetType()}]</color> can`t find system:{type}");
            return default(T);
        }

        public static async UniTask WaitFrameworkInit()
        {
            LogInfo("FrameInit","Start wait",Color.yellow);
            await UniTask.WaitUntil(() => Framework.Instance != null);
            LogInfo("FrameInit","Frame init finish",Color.yellow);
            await Instance.WaitManager<UIManager>();
            LogInfo("FrameInit","UIManager init finish",Color.yellow);
            await Instance.WaitManager<SceneManager>();
            LogInfo("FrameInit","SceneManager init finish",Color.yellow);
        }
        public async UniTask WaitManager<T>() where T : BaseManager
        {
            var type = typeof(T);
            await UniTask.WaitUntil(()=>_systemMap.ContainsKey(type));
        }

        public static void LogInfo(string tag,string message,Color tagColor)
        {
            string color = ColorUtility.ToHtmlStringRGB(tagColor);
            string log = $"[<color=#{color}>{tag}</color>]:{message}";
            Debug.Log(log);
        }
    }
}
