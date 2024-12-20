using System;
using Animancer;
using UnityEngine;

namespace TFramework.Runtime
{
    [RequireComponent(typeof(AnimancerComponent))]
    public class UIAnimancer : MonoBehaviour,UIManager.IUIPanelAnimation
    {
        public AnimancerComponent animancer;
        public float fadeDuration = 0.2f;
        public ClipTransition show;
        public ClipTransition hide;

        private void Awake()
        {
            show.Events.OnEnd = DisableAnimator;
            hide.Events.OnEnd = DisableAnimator;
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

        public void FadeColor(Color color, float duration)
        {
        }
        public void DisableAnimator()
        {
            animancer.Animator.enabled = false;
        }
        public void EnableAnimator()
        {
            animancer.Animator.enabled = true;
        }
    }
}