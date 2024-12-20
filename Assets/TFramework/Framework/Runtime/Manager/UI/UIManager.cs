using System;
using UnityEngine;
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
        [SerializeField] private Transform _topLevel;
        public Transform TopLevel => _topLevel;
        [SerializeField] private Transform _centerLevel;
        public Transform CenterLevel => _centerLevel;
        [SerializeField] private Transform _bottomLevel;
        public Transform BottomLevel => _bottomLevel;
        private void Awake()
        {
            AddSystem<UISystem>();
            AddSystem<LoadSystem>();
            AddSystem<LevelSystem>();
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
        /// <summary>
        /// 交换UIPanel
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void ExChangeUIPanel(UIPanel from,UIPanel to)
        {
            if (from == null || to == null)
                return;
            if(from.panelRoot != to.panelRoot)
                return;
            if(from == to)
                return;
            var root = from.panelRoot;
            
            //交换Panel
            var fromLast = from.LastPanel;
            var fromNext = from.NextPanel;
            var toNext = to.NextPanel;
            var toLast = to.LastPanel;

            from.NextPanel = toNext == from ? to : toNext;
            from.LastPanel = toLast == from ? to : toLast;
            to.LastPanel = fromLast == to ? from : fromLast;
            to.NextPanel = fromNext == to ? from : fromNext;
            //交换坐标
            from.transform.ExChangeSibling(to.transform);
            
            if(root.lastPanel == from || root.lastPanel == to)
                root.lastPanel = from == root.lastPanel ? to : from;
        }
        public enum UILevel
        {
            Top,Center,Bottom
        }
        public struct ClearEvent { }
        
    }
}