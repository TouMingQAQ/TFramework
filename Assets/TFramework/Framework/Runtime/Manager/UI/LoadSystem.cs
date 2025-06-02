using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
                AddModule<AddressableModule>();
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

            public async UniTask PreLoadPanelAsync(AssetReference[] referenceList,Action<UIPanel> callBack)
            {
                foreach (var reference in referenceList)
                {
                    var handle = reference.LoadAssetAsync<GameObject>();
                    await handle;
                    var panelObj = handle.Result;
                    if (!panelObj.TryGetComponent<UIPanel>(out var panel))
                    {
                        Framework.LogInfo("PreloadPanel",$"Can`t find uiPanel {reference.RuntimeKey}",Color.red);
                        continue;
                    }

                    panelObj = Instantiate(panelObj,_preLoad);
                    panel = panelObj.GetComponent<UIPanel>();
                    var panelType = panel.GetType();
                    
                    if (!_panelMap.TryAdd(panelType, panel))
                    {
                        Debug.LogError($"已经存在UIPanel:{panelType}");
                        continue;
                    }
                    
                    panel.gameObject.SetActive(false);
                    callBack?.Invoke(panel);
                }
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