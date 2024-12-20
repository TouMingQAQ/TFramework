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
            public void FadeColor(Color color, float duration);
        }
        [RequireComponent(typeof(CanvasGroup))]
        public abstract partial class UIPanel : MonoBehaviour
        {
            public IUIPanelAnimation uiAnimation;
            public UIPanelRoot panelRoot;
            protected CanvasGroup _canvasGroup;
            [SerializeField]
            private UIPanel _nextPanel;

            /// <summary>
            /// 下一个界面
            /// </summary>
            public UIPanel NextPanel
            {
                get=>_nextPanel; 
                set=>_nextPanel = value;
            }
            [SerializeField]
            private UIPanel _lastPanel;

            /// <summary>
            /// 上一个界面
            /// </summary>
            public UIPanel LastPanel
            {
                get=>_lastPanel;
                set=>_lastPanel = value;
            }
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
                        uiAnimation?.Show();
                        OnShow();
                    }
                    else
                    {
                        uiAnimation?.Hide();
                        OnHide();
                    }
                    _canvasGroup.interactable = value;
                    _canvasGroup.blocksRaycasts = value;
                    _isShow = value;
                }
            }
            //暂时用不上
            // protected virtual void OnInit(){}
            // protected virtual void OnRelease(){}
            protected virtual void OnShow(){}
            protected virtual void OnHide(){}

            /// <summary>
            /// 获得尾部的UIPanel
            /// </summary>
            /// <returns></returns>
            public UIPanel GetBottomPanel()
            {
                if ((object)_nextPanel == null)
                    return this;
                var next = _nextPanel;
                while ((object)next._nextPanel != null)
                {
                    next = next._nextPanel;
                }

                return next;
            }

            /// <summary>
            /// 获得顶部的UIPanel
            /// </summary>
            /// <returns></returns>
            public UIPanel GetTopPanel()
            {
                if ((object)_lastPanel == null)
                    return this;
                var last = _lastPanel;
                while ((object)last._lastPanel != null)
                {
                    last = last._lastPanel;
                }

                return last;
            }

            /// <summary>
            /// UI界面实例化核心函数
            /// </summary>
            /// <param name="root"></param>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public static T OpenPanel<T>(UIPanelRoot root) where T : UIPanel
            {
                var uiManager = Framework.Instance.GetManager<UIManager>();
                var loadSystem = uiManager.GetSystem<LoadSystem>();
                var panel = loadSystem.GetPanel<T>();
                panel = Instantiate(panel, root.transform);
                panel.transform.SetAsLastSibling();
                panel.gameObject.SetActive(true);
                panel.Show();
                panel.panelRoot = root;
                return panel;
            }
            /// <summary>
            /// 打开一个UI
            /// 默认放在同层级最后面
            /// </summary>
            /// <typeparam name="T"></typeparam>
            public T Open<T>() where T : UIPanel
            {
                var panel = OpenPanel<T>(panelRoot);
                var next = GetBottomPanel();
                panel._lastPanel = next;
                next._nextPanel = panel;
                next.Hide();
                panelRoot.lastPanel = panel;
                return panel;
            }
            
            /// <summary>
            /// 打开一个UI，带数据
            /// </summary>
            /// <param name="data"></param>
            /// <typeparam name="T"></typeparam>
            /// <typeparam name="TData"></typeparam>
            public UIPanel Open<T, TData>(TData data) where T : UIPanel where TData : struct
            {
                var panel = Open<T>();
                //赋予数据
                return panel;
            }
         
            /// <summary>
            /// 显示
            /// </summary>
            public void Show()
            {
                IsShow = true;
            }
            /// <summary>
            /// 隐藏
            /// </summary>
            public void Hide()
            {
                IsShow = false;
            }
            
            /// <summary>
            /// 关闭自己和所有子界面
            /// </summary>
            public void Close()
            {
                if(NextPanel != null)
                    NextPanel.Close();
                panelRoot.lastPanel = LastPanel;
                DestroyImmediate(gameObject);
            }
            /// <summary>
            /// 回退
            /// </summary>
            public void Back()
            {
                //通过递归，找到最后一个界面进行关闭
                if(NextPanel != null)
                    NextPanel.Back();
                else
                    Close();
            }
            /// <summary>
            /// 移动到下一层
            /// </summary>
            public void MoveNext()
            {
                if(NextPanel == null)
                    return;
                ExChangeUIPanel(this,NextPanel);
            }
            /// <summary>
            /// 移动到上一层
            /// </summary>
            public void MoveLast()
            {
                if(LastPanel == null)
                    return;
                ExChangeUIPanel(this,LastPanel);
            }
            /// <summary>
            /// 移动到顶层
            /// </summary>
            public void MoveTop()
            {
                var topPanel = GetTopPanel();
                if(topPanel == this)
                    return;
                ExChangeUIPanel(this,topPanel);
            }
            /// <summary>
            /// 移动到底层
            /// </summary>
            public void MoveBottom()
            {
                var bottomPanel = GetBottomPanel();
                if(bottomPanel == this)
                    return;
                ExChangeUIPanel(this,bottomPanel);
            }

            private void Reset()
            {
                TryGetComponent(out _canvasGroup);
            }
        }
    }
    
}