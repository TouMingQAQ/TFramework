using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using VInspector;

namespace TFramework.Component.UI
{
    
    public class ButtonAnimation : MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        public ScriptAnimation clickAnimation;
        public ScriptAnimation coverAnimation;
        
        
        public void OnPointerDown(PointerEventData eventData)
        {
            clickAnimation?.PlayForward();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            clickAnimation?.PlayBackwards();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            coverAnimation?.PlayForward();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            coverAnimation?.PlayBackwards();
        }
    }
}