using System;
using UnityEngine;

namespace TFramework.Runtime
{
    [RequireComponent(typeof(Animator))]
    public class UIAnimator : MonoBehaviour,UIManager.IUIPanelAnimation
    {
        private static readonly int IsShow = Animator.StringToHash("IsShow");
        public Animator animator;
        public UIManager.UIPanel uiPanel;

        private void Awake()
        {
            uiPanel.uiAnimation = this;
        }

        public void Show()
        {
            EnableAnimator();
            animator.SetBool(IsShow,true);
        }

        public void Hide()
        {
            EnableAnimator();
            animator.SetBool(IsShow,false);
        }

        public void FadeColor(Color color, float duration)
        {
            
        }
        public void DisableAnimator()
        {
            animator.enabled = false;
        }
        public void EnableAnimator()
        {
            animator.enabled = true;
        }

        private void Reset()
        {
            TryGetComponent(out animator);
            TryGetComponent(out uiPanel);
        }
    }
}