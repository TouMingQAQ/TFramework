using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VInspector;

namespace TFramework.Component.UI
{
    [Serializable]
    public struct ButtonStyleValue<T>
    {
        public bool Enable;
        public T Value;
    }
    
    public class ButtonStyle : MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        [Tab("Component")]
        public Image target;
        [Tab("Toggle")]
        
        public bool overrideSprite = true;

        [Tab("Color")] 
        [SerializeField,ReadOnly]
        private Color baseColor;
        public ButtonStyleValue<Color> coverColor;
        public ButtonStyleValue<Color> downColor;
        [Tab("Sprite")] [SerializeField,ReadOnly]
        private Sprite baseSprite;
        public ButtonStyleValue<Sprite> coverSprite;
        public ButtonStyleValue<Sprite> downSprite;

        private void Awake()
        {
            baseColor = target.color;
            baseSprite = target.sprite;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
           DownColor();
           DownSprite();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(coverColor.Enable)
                EnterColor();
            else
                UpColor();
            if(coverSprite.Enable)
                EnterSprite();
            else
                UpSprite();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
           EnterColor();
           EnterSprite();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ExitColor();
            ExitSprite();
        }
        

        void DownSprite()
        {
            if (downSprite.Enable)
            {
                if (overrideSprite)
                    target.overrideSprite = downSprite.Value;
                else
                    target.sprite = downSprite.Value;
            }
        }

        void DownColor()
        {
            if (downColor.Enable)
                target.color = downColor.Value;
        }

        void UpColor()
        {
            if (downColor.Enable)
                target.color = baseColor;
        }

        void UpSprite()
        {
            if (downSprite.Enable)
            {
                if (overrideSprite)
                    target.overrideSprite = null;
                else
                    target.sprite = baseSprite;
            }
        }

        void EnterSprite()
        {
        
            if (coverSprite.Enable)
            {
                if (overrideSprite)
                    target.overrideSprite = coverSprite.Value;
                else
                    target.sprite = coverSprite.Value;
            }
        }

        void EnterColor()
        {
            if (coverColor.Enable)
                target.color = coverColor.Value;
        }

        void ExitColor()
        {
            if (coverColor.Enable)
                target.color = baseColor;
        }
        
        void ExitSprite()
        {
            
            if (coverSprite.Enable)
            {
                if (overrideSprite)
                    target.overrideSprite = null;
                else
                    target.sprite = baseSprite;
            }
        }
    }
}