using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using VInspector;

namespace TFramework.Component
{
    public abstract class ScriptAnimation : MonoBehaviour
    {
        [Tab("Component")]
        public Transform target;
        [Tab("Value")]
        public bool playOnAwake = false;
        public bool ignoredTimeScale = false;
        public float duration = 1;
        public int loopCount = 1;
        public bool reverse = false;
        public AnimationCurve curve = AnimationCurve.Linear(0,0,1,1);
        [Tab("Event")]
        public UnityEvent onPlay;
        public UnityEvent onStart;
        public UnityEvent onPause;
        public UnityEvent onComplete;
        public UnityEvent onStepComplete;
        public Tween tween => _tween;
        protected Sequence _tween;


        protected virtual void Awake()
        {
            Rebuild();
            if(playOnAwake)
                ReStart();
        }

        public void Rebuild()
        {
            if (_tween == null)
            {
                _tween = DOTween.Sequence();
                CreateTween(_tween,target);
                _tween.SetLoops(loopCount);
                _tween.SetAutoKill(false);
                _tween.SetEase(curve).SetSpeedBased(true);
                _tween.OnComplete(OnComplete)
                    .OnStepComplete(OnStepComplete)
                    .OnStart(OnStart)
                    .OnPlay(OnPlay)
                    .OnPause(OnPause);
            }
        }
        
        protected virtual void OnPause()
        {
            onPause?.Invoke();
        }
        protected virtual void OnPlay()
        {
            onPlay?.Invoke();
        }
        protected virtual void OnStart()
        {
            onStart?.Invoke();
        }
        protected virtual void OnComplete()
        {
            onComplete?.Invoke();
        }

        protected virtual void OnStepComplete()
        {
            onStepComplete?.Invoke();
        }
        [Tab("Value"),Button]
        public void SetTimeScale(float timeScale)
        {
            if(!ignoredTimeScale)
                return;
            _tween.timeScale = timeScale;
        }

        private void LateUpdate()
        {
            if (!ignoredTimeScale)
                _tween.timeScale = Time.timeScale;
        }
        [Tab("Value"),Button]
        public virtual void ReStart()
        {
            _tween.Restart();
        }
        [Tab("Value"),Button]
        public virtual void PlayForward()
        {
            _tween.PlayForward();
        }
        [Tab("Value"),Button]
        public virtual void PlayBackwards()
        {
            _tween.PlayBackwards();
        }

        [Tab("Value"),Button]
        public virtual void Pause()
        {
            _tween.Pause();
        }
        [Tab("Value"),Button]
        public virtual void Play()
        {
            _tween.Play();
        }
#if UNITY_EDITOR
        [Tab("Editor")]
        public bool playOnRebuild = false;
        [Tab("Editor"),Button("Rebuild")]
        [ContextMenu("Rebuild")]
        void RebuildAnimation()
        {
            if (_tween != null)
            {
                _tween.Rewind();
                _tween.Kill();
            }
            _tween = DOTween.Sequence();
            CreateTween(_tween,target);
            _tween.SetEase(curve).SetSpeedBased(true);
            if(playOnRebuild)
                ReStart();
        }
#endif
        protected abstract void CreateTween( Sequence sequence,Transform target);
        public virtual float GetDuration() => duration;

        private void Reset()
        {
            TryGetComponent(out target);
        }
    }
}