using System;
using UnityEngine;

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

        public void Clear()
        {
            Call(new ClearEvent());
        }
        public enum UILevel
        {
            Top,Center,Bottom
        }
        public struct ClearEvent { }
        
    }
}