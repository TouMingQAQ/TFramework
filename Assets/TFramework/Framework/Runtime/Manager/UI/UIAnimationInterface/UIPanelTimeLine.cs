using UnityEngine;
using UnityEngine.Playables;

namespace TFramework.Runtime
{
    [RequireComponent(typeof(PlayableDirector))]
    public class UIPanelTimeLine : MonoBehaviour,UIManager.IUIPanelAnimation
    {
        public PlayableDirector playableDirector;
        public UIManager.UIPanel uiPanel;
        public PlayableAsset showPlayableAsset;
        public PlayableAsset hidePlayableAsset;
        public void Show()
        {
            playableDirector.Play(showPlayableAsset);
        }

        public void Hide()
        {
            playableDirector.Play(hidePlayableAsset);
        }
        public void OnHideEnd()
        {
            uiPanel.OnAfterAnimationHide();
        }
        private void Reset()
        {
            TryGetComponent(out playableDirector);
            TryGetComponent(out uiPanel);
        }
    }
}