using System;
using Animancer;
using UnityEngine;

namespace TFramework.Runtime
{
    [RequireComponent(typeof(AnimancerComponent))]
    public class UIPanelAnimancer : MonoBehaviour,UIManager.IUIPanelAnimation
    {
        public AnimancerComponent animancer;
        public UIManager.UIPanel uiPanel;
        public float fadeDuration = 0.2f;
        public ClipTransition show;
        public ClipTransition hide;

        private void Awake()
        {
            show.Events.OnEnd = DisableAnimator;
            hide.Events.OnEnd = OnHideEnd;
            uiPanel.uiAnimation = this;
        }

        public void Show()
        {
            EnableAnimator();
            animancer.Play(show, fadeDuration);
        }

        public void Hide()
        {
            animancer.Play(hide, fadeDuration);
        }

        void OnHideEnd()
        {
            uiPanel.OnAfterAnimationHide();
            DisableAnimator();
        }
        public void DisableAnimator()
        {
            animancer.Animator.enabled = false;
        }
        public void EnableAnimator()
        {
            animancer.Animator.enabled = true;
        }

        private void Reset()
        {
            TryGetComponent(out animancer);
            TryGetComponent(out uiPanel);
        }
    }
}