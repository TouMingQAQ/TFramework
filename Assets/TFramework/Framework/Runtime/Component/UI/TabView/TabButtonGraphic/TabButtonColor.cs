using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TFramework.Component.UI
{
    public class TabButtonColor : MonoBehaviour,ITabButtonGraphic
    {
        public Image targetImages;
        public Color hideColor;
        public Color showColor;

        public void OnInit()
        {
            
        }

        public void OnShow()
        {
            targetImages.color = showColor;
        }

        public void OnHide()
        {
            targetImages.color = hideColor;
        }
    }
}