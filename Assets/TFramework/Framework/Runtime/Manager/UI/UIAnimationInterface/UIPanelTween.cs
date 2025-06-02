using System;
using DG.Tweening;
using TFramework.Component;
using UnityEngine;
using VInspector;

namespace TFramework.Runtime
{
    public class UIPanelTween : MonoBehaviour,UIManager.IUIPanelAnimation
    {
        public UIManager.UIPanel uiPanel;
        public ScriptAnimation animation;

        private void Awake()
        {
            uiPanel.uiAnimation = this;
        }
        
        [Button]
        public void Show()
        {
            animation.onStepComplete.RemoveListener(OnHideEnd);
            animation.ReStart();
        }
        [Button]
        public void Hide()
        {
            animation.onStepComplete.AddListener(OnHideEnd);
            animation.PlayBackwards();
        }
        
        public void OnHideEnd()
        {
            uiPanel.OnAfterAnimationHide();
        }
        private void Reset()
        {
            TryGetComponent(out uiPanel);
        }
    }


}