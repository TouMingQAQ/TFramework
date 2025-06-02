using System;
using UnityEngine;

namespace TFramework.Component.UI
{
    public class TabButtonActive : MonoBehaviour,ITabButtonGraphic
    {
        public void OnInit()
        {
            
        }

        public void OnShow()
        {
            gameObject.SetActive(true);
        }

        public void OnHide()
        {
            gameObject.SetActive(false);
        }

        
    }
}