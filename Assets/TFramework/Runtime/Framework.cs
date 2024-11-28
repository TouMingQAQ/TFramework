using System;
using System.Collections.Generic;
using UnityEngine;

namespace TFramework.Runtime
{
    public partial class Framework : MonoBehaviour
    {
        public static Framework Instance;
        protected Dictionary<Type, BaseManager> _systemMap;
        
        public T GetManager<T>() where T : BaseManager
        {
            var type = typeof(T);
            if (_systemMap.TryGetValue(type, out var value))
                return value as T;
            else
                Debug.LogError($"<color=red>[{GetType()}]</color> can`t find system:{type}");
            return default(T);
        }
    }
}
