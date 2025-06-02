using System;
using System.Collections.Generic;
using UnityEngine;

namespace TFramework.Runtime
{
    public partial class UIManager
    {
        private sealed class LevelSystem : BaseSystem
        {
            private Transform sceneMaskLevel;
            private Transform tipLevel;
            private Transform defaultLevel;

            public override void Init()
            {
                Register<ClearEvent>(OnClear);
                if (Manager is UIManager uiManager)
                {
                    defaultLevel = uiManager.DefaultLevel;
                    tipLevel = uiManager.TipLevel;
                    sceneMaskLevel = uiManager.SceneMaskLevel;
                }
            }

            public override void Destroy()
            {
                UnRegister<ClearEvent>(OnClear);
            }
            private void OnClear(ClearEvent clear)
            {
                defaultLevel.ClearChild();
                tipLevel.ClearChild();
                sceneMaskLevel.ClearChild();
            }

           
            public Transform GetLevel(UILevel level) => level switch
            {
                UILevel.Tip => tipLevel,
                UILevel.SceneMask => sceneMaskLevel,
                UILevel.Default => defaultLevel,
                _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
            };
        
           
        }
    }
    
}