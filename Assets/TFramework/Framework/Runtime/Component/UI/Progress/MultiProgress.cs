using System;
using System.Collections.Generic;
using UnityEngine;
namespace TFramework.Component.UI
{
    [ExecuteAlways]
    public class MultiProgress : MonoBehaviour
    {
        [Range(0,1),SerializeField]
        public float progressValue;
        
        public RectTransform parentRect;
        public float height;
        public float width;
        public List<MultiProgressRect> targetList = new();
        private void Reset()
        {
            parentRect = GetComponent<RectTransform>();
        }
        private void LateUpdate()
        {
        
            if (parentRect is null)
            {
                height = -1;
                width = -1;
                return;
            }
            UpdateUI();
        }
    
        private void UpdateUI()
        {
            var rect = parentRect.rect;
            height = rect.height;
            width = rect.width;
            foreach (var targetRectContainer in targetList)
            {
                var Direction = targetRectContainer.Direction;
                var targetRect = targetRectContainer.targetRect;
                var followSpeed = targetRectContainer.followSpeed;
                switch (Direction)
                {
                    case ProgressDirection.HorizontalDown:
                        targetRect.pivot = new Vector2(0.5f, 0);
                        break;
                    case ProgressDirection.HorizontalTop:
                        targetRect.pivot = new Vector2(0.5f, 1);
                        break;
                    case ProgressDirection.VerticalLeft:
                        targetRect.pivot = new Vector2(0, 0.5f);
                        break;
                    case ProgressDirection.VerticalRight:
                        targetRect.pivot = new Vector2(1, 0.5f);
                        break;
                }
                Vector2 v  = Direction switch
                {
                    ProgressDirection.HorizontalTop => new Vector2(0,-(1-progressValue)*height),
                    ProgressDirection.HorizontalDown => new Vector2(0,-(1-progressValue)*height),
                    ProgressDirection.VerticalRight => new Vector2(-(1-progressValue)*width,0),

                    ProgressDirection.VerticalLeft => new Vector2(-(1-progressValue)*width,0),
                    _=>Vector2.zero
                };
                targetRect.sizeDelta = Vector2.Lerp(targetRect.sizeDelta, v, followSpeed);
            }
           
        }
    
        public void SetValue(float value)
        {
            if (value <= 0)
                value = 0;
            else if (value >= 1)
                value = 1;
            progressValue = value;
        }

        public float GetValue()
        {
            return progressValue;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }
    [Serializable]
    public struct MultiProgressRect
    {
        [Range(0.01f,1)]
        public float followSpeed;
        public RectTransform targetRect;
        public ProgressDirection Direction;
    }
}