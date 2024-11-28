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
            
            public void ShowPanel<T>(UILevel level) where T : UIPanel
            {
                var levelModule = GetSystem<LevelSystem>();
                var loadModule = GetSystem<LoadSystem>();
                var root = levelModule.GetLevel(level);
                var panel = loadModule.GetPanel<T>();
                panel = Instantiate(panel, root);
                panel.gameObject.SetActive(true);
            }

            public void ShowPanelWithData<T, TData>(TData data)
            {
                
            }
        }
    }
    
}