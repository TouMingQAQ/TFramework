using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TFramework.Runtime
{
    public partial class UIManager
    {
        public class UISystem : BaseSystem
        {
            public override void Init()
            {
                Register<ExitEvent>(OnExit);
                AddModule<EventModule>();
            }
        
            public T GetPanelPrefab<T>() where T : UIPanel
            {
                var loadSystem = GetSystem<LoadSystem>();
                return loadSystem.GetPanel<T>();
            }
            /// <summary>
            /// UI界面实例化核心函数
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public T OpenPanel<T>(UILevel uiLevel = UILevel.Default) where T : UIPanel
            {
                var loadSystem = GetSystem<LoadSystem>();
                var levelSystem = GetSystem<LevelSystem>();
                var panel = loadSystem.GetPanel<T>();
                var root = levelSystem.GetLevel(uiLevel);
                
                panel = Instantiate(panel, root.transform);
                panel.gameObject.SetActive(true);
                panel.IsShow = true;
                return panel;
            }

         
            

            private Stack<UIPanel> exitStack = new();
            void OnExit(ExitEvent exitEvent)
            {
                Back();
            }

            public void RegisterExit(UIPanel uiPanel)
            {
                if (exitStack.TryPeek(out var panel) && panel == uiPanel)
                {
                    Framework.LogInfo("UISystem",$"注册Exit错误[{uiPanel.name}]，名称重复",Color.red);
                    return;
                }
                exitStack.Push(uiPanel);
            }

            public void UnRegisterExit(UIPanel uiPanel)
            {
                if(!exitStack.TryPeek(out var panel))
                    return;
                if (panel != uiPanel)
                {
                    Framework.LogInfo("UISystem",$"注销Exit错误[{uiPanel.name}]，名称对不上",Color.red);
                    return;
                }

                exitStack.Pop();
            }
            public void Back()
            {
                if (!exitStack.TryPeek(out var panel))
                {
                    return;
                }
                panel.Close();
            }
        }
    }
    
}