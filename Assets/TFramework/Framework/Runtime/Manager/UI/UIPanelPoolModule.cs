using UnityEngine;

namespace TFramework.Runtime
{
    public partial class UIManager
    {
        private class UIPanelPoolModule : GameObjectPoolModule<UIPanel>
        {
            public UIPanelPoolModule(UIPanel prefab, Transform showRoot, Transform hideRoot) : base(prefab, showRoot, hideRoot)
            {
            }
        }
    }
    
}