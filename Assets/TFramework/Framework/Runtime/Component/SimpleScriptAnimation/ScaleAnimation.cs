using DG.Tweening;
using UnityEngine;

namespace TFramework.Component
{
    public class ScaleAnimation : ScriptAnimation
    {
        public Vector3 startValue = Vector3.one;
        public Vector3 endValue = Vector3.one;
        protected override void CreateTween(Sequence sequence,Transform target)
        {
            target.localScale = startValue;
            var t = target.DOScale(endValue, GetDuration()).SetEase(curve);
            sequence.Append(t);
        }
    }
}