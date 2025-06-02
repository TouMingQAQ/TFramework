using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.Universal;

namespace TFramework.Runtime
{
    public sealed partial class UIManager : BaseManager
    {
        [SerializeField] private Camera _uiCamera;
        public Camera UICamera => _uiCamera;
        [SerializeField] private Canvas _root;
        public Canvas Root => _root;
        [SerializeField] private Transform _preLoad;
        public Transform PreLoad => _preLoad;
        [SerializeField] private Transform _defaultLevel;
        public Transform DefaultLevel => _defaultLevel;
   

        [SerializeField]  private Transform _tipLevel;
        public Transform TipLevel => _tipLevel;
        [SerializeField]  private Transform _sceneMaskLevel;
        public Transform SceneMaskLevel => _sceneMaskLevel;

        private void Awake()
        {
            framework.AddManager(this);
            AddSystem<UISystem>();
            AddSystem<LoadSystem>();
            AddSystem<LevelSystem>();
        }

        public async UniTask PreLoadPanelAsync(AssetReference[] reference,Action<UIPanel> callBack)
        {
            var loadSystem = GetSystem<LoadSystem>();
            await loadSystem.PreLoadPanelAsync(reference,callBack);
        }
        public void ApplyUICameraToMainCamera(Camera mainCamera)
        {
            var data = mainCamera.GetComponent<UniversalAdditionalCameraData>();
            data.cameraStack.Add(_uiCamera);
        }
        public void Clear()
        {
            Call<ClearEvent>();
        }
        
        public enum UILevel
        {
            /// <summary>
            /// 过场层级
            /// </summary>
            SceneMask,
            /// <summary>
            /// 提示层级
            /// </summary>
            Tip,
            /// <summary>
            /// 默认层级
            /// </summary>
            Default
        }
        public struct ClearEvent { }
        public struct ExitEvent{}
        
    }
}