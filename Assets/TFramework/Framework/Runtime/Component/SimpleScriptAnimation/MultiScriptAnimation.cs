using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using VInspector;

namespace TFramework.Component
{
    [Serializable]
    public struct ScriptAnimationContainer
    {
        public float insertTime;
        public ScriptAnimation scriptAnimation;
    }
    public class MultiScriptAnimation : ScriptAnimation
    {
        public List<ScriptAnimationContainer> animationList = new();
        protected override void CreateTween(Sequence sequence, Transform target)
        {
            foreach (var scriptAnimationContainer in animationList)
            {
                scriptAnimationContainer.scriptAnimation.Rebuild();
                sequence.Insert(scriptAnimationContainer.insertTime,scriptAnimationContainer.scriptAnimation.tween);
            }
        }
    }
}