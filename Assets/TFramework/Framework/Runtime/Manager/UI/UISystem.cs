using System;
using UnityEngine;

namespace TFramework.Runtime
{
    public partial class UIManager
    {
        public class UISystem : BaseSystem
        {
            public override void Init()
            {
                AddModule<EventModule>();
            }
            public void PreLoad<T>() where T : UIPanel
            {
                var loadSystem = GetSystem<LoadSystem>();
                loadSystem.PreLoad<T>();
            }
            public T GetPanelPrefab<T>() where T : UIPanel
            {
                var loadSystem = GetSystem<LoadSystem>();
                return loadSystem.GetPanel<T>();
            }
            public T OpenPanel<T>(UILevel level,string rootName) where T : UIPanel
            {
                var levelSystem = GetSystem<LevelSystem>();
                var root = levelSystem.GetRoot(level, rootName);
                return root.OpenPanel<T>();
            }

            public void ShowPanelWithData<T, TData>(TData data)
            {
                
            }
        }
    }
    
}