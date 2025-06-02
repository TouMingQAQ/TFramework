using System;
using DG.Tweening;
using UnityEngine;

namespace TFramework.Component
{
    public class RotateAnimation : ScriptAnimation
    {
        public Vector3 rotateDirection = new Vector3(0, 1, 0);
        [Range(0,360)]
        public float rotation = 360;
        protected override void CreateTween(DG.Tweening.Sequence sequence,Transform target)
        {
            var endValue = rotateDirection.normalized * rotation * (reverse?-1:1);
            var t = target.DORotate(endValue, GetDuration(), RotateMode.FastBeyond360);
            t.SetEase(curve);
            sequence.Append(t);
        }
    }
}