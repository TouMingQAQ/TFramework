using System;
using System.Collections.Generic;
using UnityEngine;

namespace TFramework.Runtime
{
    public partial class UIManager
    {
        private class LoadSystem : BaseSystem
        {
            private Transform _preLoad;
            private Dictionary<Type, UIPanel> _panelMap = new();
            public override void Init()
            {
                Register<ClearEvent>(OnClear);
                if (Manager is UIManager uiManager)
                    _preLoad = uiManager.PreLoad;
            }

            public override void Destroy()
            {
                UnRegister<ClearEvent>(OnClear);
            }
            private void OnClear(ClearEvent clear)
            {
                _preLoad.ClearChild();
            }
            /// <summary>
            /// 预加载
            /// </summary>
            /// <typeparam name="T"></typeparam>
            public void PreLoad<T>()
            {
                //Todo:需要AssetsSystem进行资源加载
            }

            public T GetPanel<T>() where T : UIPanel
            {
                var type = typeof(T);
                if (_panelMap.TryGetValue(type, out var panel))
                    return panel as T;
                else
                    Debug.LogError($"没有找到UIPanel:{type},请使用PreLoad预载它");
                return default;
            }
         
        }
    }

    
}