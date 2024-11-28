using System;
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