using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace TFramework.ToolBox
{
    public class ButtonBox : BaseToolBox
    {
        public override bool PreLoad => true;
        public override bool Closeable => false;

        public override string TabName => "快捷按钮";

        public override string VisualTreeAssetPath => "ButtonBox";

        private ListView _buttonView;
        private List<ButtonInfo> _infos = new();
        public ButtonBox() : base()
        {
            _buttonView = this.Q<ListView>("ButtonView");
            _buttonView.makeItem = MakeButton;
            _buttonView.itemsSource = _infos;
            _buttonView.bindItem = BindItem;
            _buttonView.unbindItem = BindItem;
            CollectButton();
            _buttonView.Rebuild();
        }

        private void BindItem(VisualElement element, int index)
        {
            var button = (Button) element;
            button.text = _infos[index].Name;
            button.clickable.clicked += _infos[index].ToDo;
        }
        private Button MakeButton()
        {
            var button = new Button();
            return button;
        }
        private void CollectButton()
        {
            _infos.Clear();
            // 扫描程序集中带有ToolButton特性的静态函数
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x=>x.FullName.Contains("Assembly-CSharp"));
            List<Type> types = new();
            foreach (var assembly in assemblies)
            {
                types.AddRange(assembly.GetTypes());
            }
            foreach (var type in types)
            {
                var methods = type.GetMethods();
                foreach (var method in methods)
                {
                    var attr = method.GetCustomAttributes(typeof(ToolButtonAttribute), false);
                    if (attr.Length <= 0) 
                        continue;
                    var button = new ButtonInfo
                    {
                        Name = ((ToolButtonAttribute) attr[0]).Name,
                        ToDo = (Action) Delegate.CreateDelegate(typeof(Action), method)
                    };
                    if(string.IsNullOrEmpty(button.Name))
                        button.Name = method.Name;
                    _infos.Add(button);
                }
            }
        }
        [Serializable]
        public struct ButtonInfo
        {
            public string Name;
            public Action ToDo;
        }
        
        
    }

}
