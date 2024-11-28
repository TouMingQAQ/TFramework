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
        public partial class UIPanel : MonoBehaviour
        {
            public IUIPanelAnimation uiAnimation;
            [SerializeField]
            private UIPanel _nextPanel;
            /// <summary>
            /// 下一个界面
            /// </summary>
            public UIPanel NextPanel => _nextPanel;
            [SerializeField]
            private UIPanel _lastPanel;
            /// <summary>
            /// 上一个界面
            /// </summary>
            public UIPanel LastPanel => _lastPanel;
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
                    _isShow = value;
                }
            }
            protected virtual void OnInit(){}
            protected virtual void OnRelease(){}
            protected virtual void OnShow(){}
            protected virtual void OnHide(){}

            /// <summary>
            /// 打开一个UI
            /// </summary>
            /// <typeparam name="T"></typeparam>
            public void Open<T>() where T : UIPanel
            {
                
            }
        
            /// <summary>
            /// 打开一个UI，带数据
            /// </summary>
            /// <param name="data"></param>
            /// <typeparam name="T"></typeparam>
            /// <typeparam name="TData"></typeparam>
            public void Open<T, TData>(TData data) where T : UIPanel where TData : struct
            {
                
            }
            /// <summary>
            /// 关闭自己极其所有子界面
            /// </summary>
            public void Close()
            {
                if(NextPanel != null)
                    NextPanel.Close();
                IsShow = false;
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
                transform.ExChangeSibling(NextPanel.transform);
            }
            /// <summary>
            /// 移动到上一层
            /// </summary>
            public void MoveLast()
            {
                
            }
            /// <summary>
            /// 移动到顶层
            /// </summary>
            public void Pop()
            {
                
            }
        }
    }
    
}