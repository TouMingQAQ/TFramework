using System;
using System.Collections.Generic;
using UnityEngine;

namespace TFramework.Runtime
{
    public partial class UIManager
    {
        private sealed class LevelSystem : BaseSystem
        {
            private Transform topLevel;
            private Transform centerLevel;
            private Transform bottomLevel;
            private Dictionary<string,UIPanelRoot> topLevelRoots = new();
            private Dictionary<string,UIPanelRoot> centerLevelRoots = new();
            private Dictionary<string,UIPanelRoot> bottomLevelRoots = new();
            public override void Init()
            {
                Register<ClearEvent>(OnClear);
                if (Manager is UIManager uiManager)
                {
                    topLevel = uiManager.TopLevel;
                    centerLevel = uiManager.CenterLevel;
                    bottomLevel = uiManager.BottomLevel;
                }
            }

            public override void Destroy()
            {
                UnRegister<ClearEvent>(OnClear);
            }
            private void OnClear(ClearEvent clear)
            {
                topLevel.ClearChild();
                centerLevel.ClearChild();
                bottomLevel.ClearChild();
                topLevelRoots.Clear();
                centerLevelRoots.Clear();
                bottomLevelRoots.Clear();
            }

            public UIPanelRoot GetRoot(UILevel level, string rootName)
            {
                var rootMap = level switch
                {
                    UILevel.Top => topLevelRoots,
                    UILevel.Center => centerLevelRoots,
                    UILevel.Bottom => bottomLevelRoots,
                    _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
                };
                if (rootMap.TryGetValue(rootName, out var root))
                {
                    //存在，返回
                    return root;
                }
                else
                {
                    //不存在，创建
                    var rootObj = new GameObject
                    {
                        name = rootName
                    };
                    root = rootObj.AddComponent<UIPanelRoot>();
                    root.transform.SetParent(GetLevel(level));
                    root.transform.position = Vector3.zero;
                    rootMap[rootName] = root;
                    return root;
                }
            }
            public Transform GetLevel(UILevel level) => level switch
            {
                UILevel.Top => topLevel,
                UILevel.Center => centerLevel,
                UILevel.Bottom => bottomLevel,
                _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
            };
        
           
        }
    }
    
}