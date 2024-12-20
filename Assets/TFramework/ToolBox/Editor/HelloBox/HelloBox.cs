
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.EditorTools;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.UIElements;

namespace TFramework.ToolBox
{
    // 自定义一个HelloBox类，可以在UI Builder中找到
    public class HelloBox : BaseToolBox
    {
        public sealed override bool PreLoad => EditorUserSettings.GetConfigValue("HelloBox") != "true";
        public override bool Closeable => true;

        public override string TabName => "Ciallo\uff5e(\u2220・ω< )\u2312\u2606";

        public override int PreLoadIndex => 0;

        public override string VisualTreeAssetPath => "HelloBox";

        public HelloBox() : base()
        {
            var toggle = this.Q<Toggle>();
            toggle.value = !PreLoad;
            toggle.RegisterValueChangedCallback(OpenOnInit);
        }
        void OpenOnInit(ChangeEvent<bool> evt)
        {
            EditorUserSettings.SetConfigValue("HelloBox", evt.newValue ? "true" : "false");
        }
    
        
    }
    
}