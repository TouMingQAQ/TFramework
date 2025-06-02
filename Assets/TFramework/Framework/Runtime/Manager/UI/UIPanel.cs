using System;
using UnityEngine;

namespace TFramework.Runtime
{
    public partial class UIManager
    {
        public interface IUIPanelAnimation
        {
            public void Show();
            public void Hide();
        }
        [RequireComponent(typeof(CanvasGroup))]
        public abstract partial class UIPanel : MonoBehaviour
        {
            public IUIPanelAnimation uiAnimation;
            [SerializeField]
            protected CanvasGroup _canvasGroup;
            
            [SerializeField]
            private bool _isShow;
            /// <summary>
            /// 是否显示
            /// </summary>
            public virtual bool IsShow
            {
                get => _isShow;
                set
                {
                    if(_isShow.Equals(value))
                        return;
                    if (value)
                    {
                        OnBeforeAnimationShow();
                        uiAnimation?.Show();
                    }
                    else
                    {
                        if(uiAnimation == null)
                            OnAfterAnimationHide();
                        else
                            uiAnimation.Hide();
                    }
                    _canvasGroup.interactable = value;
                    _canvasGroup.blocksRaycasts = value;
                    _isShow = value;
                }
            }
            [ContextMenu("Show")]
            public void Show()
            {
                IsShow = true;
            }
            [ContextMenu("Hide")]
            public void Hide()
            {
                IsShow = false;
            }
            //暂时用不上
            // protected virtual void OnInit(){}
            // protected virtual void OnRelease(){}
            public virtual void OnBeforeAnimationShow(){}
            public virtual void OnAfterAnimationHide(){}
            
            /// <summary>
            /// 关闭自己和所有子界面
            /// </summary>
            public virtual void Close()
            {
               Destroy(this.gameObject);
            }
            

            protected virtual void Reset()
            {
                TryGetComponent(out _canvasGroup);
            }
        }
    }
    
}