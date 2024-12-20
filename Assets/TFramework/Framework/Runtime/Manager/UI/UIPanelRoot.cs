using System;
using System.Collections.Generic;
using UnityEngine;

namespace TFramework.Runtime
{
    /// <summary>
    /// Panel根节点
    /// </summary>
    public class UIPanelRoot : MonoBehaviour
    {
        public UIManager.UIPanel lastPanel;
        public T OpenPanel<T>() where T : UIManager.UIPanel
        {
            T panel = null;
            if (lastPanel == null)
                panel = UIManager.UIPanel.OpenPanel<T>(this);
            else
                panel = lastPanel.Open<T>();
            lastPanel = panel;
            return panel;
        }
        /// <summary>
        /// 回退
        /// </summary>
        public void Back()
        {
            lastPanel.Back();
        }
    }
}