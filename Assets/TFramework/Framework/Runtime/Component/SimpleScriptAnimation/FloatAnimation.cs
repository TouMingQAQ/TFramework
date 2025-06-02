using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using VInspector;

namespace TFramework.Component
{
    public class FloatAnimation : ScriptAnimation
    {
        public List<Vector3> offsetPointList = new();
        protected override void CreateTween(DG.Tweening.Sequence sequence,Transform target)
        {
            var t = target.DOLocalPath(offsetPointList.ToArray(), GetDuration()).SetEase(curve);
            sequence.Append(t);

        }
        #if UNITY_EDITOR
        [Tab("Editor"),Button]
        public void AddPoint()
        {
            var localOffset = transform.localPosition;
            offsetPointList.Add(localOffset);
        }
        #endif
    }

}
