using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TFramework.ToolBox
{
    public abstract class BaseToolBox : VisualElement
    {
        protected static int BasePreLoadIndex = 9999;

        /// <summary>
        /// 是否预加载到ToolBox
        /// </summary>
        public virtual bool PreLoad => false;

        /// <summary>
        /// TabName
        /// </summary>
        public virtual string TabName => "ToolBoxTab";

        /// <summary>
        /// UI资源路径
        /// </summary>
        public virtual string VisualTreeAssetPath => $"{this.GetType().Name}";

        /// <summary>
        /// 提示文本
        /// </summary>
        public virtual int PreLoadIndex => BasePreLoadIndex;

        /// <summary>
        /// 是否可以关闭
        /// </summary>
        public virtual bool Closeable => true;

        public BaseToolBox()
        {
            if (string.IsNullOrEmpty(VisualTreeAssetPath))
                throw new Exception("VisualTreeAssetPath is null");
            var visualTree = Resources.Load<VisualTreeAsset>(VisualTreeAssetPath);
            var root = visualTree.GetRealRoot("Root");
            this.CopySheet(root);
            style.flexGrow = 1;
            Add(root);
        }

        public virtual void OnSelected(Tab tab)
        {
        }

        public virtual void OnClosed(Tab tab)
        {
        }
    }
#if UNITY_EDITOR
    public class ToolBoxWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;
     

        [MenuItem("TFramework/ToolBox/Window _F1")]
        public static void ShowExample()
        {
            ToolBoxWindow wnd = GetWindow<ToolBoxWindow>();
            wnd.titleContent = new GUIContent("工具箱");
            wnd.minSize = new Vector2(600, 400);
            wnd.Show();
        }
        public void CreateGUI()
        {
            rootVisualElement.Add(new ToolBox());
        }
    }
#endif
    public partial class ToolBox : VisualElement
    {
        private TabView m_TabView;
        public ToolBox()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("ToolBoxWindow");
            var root = visualTree.GetRealRoot("Root");
            this.CopySheet(root);
            style.flexGrow = 1;
            m_TabView = root.Q<TabView>();
            ReLoadAllTabs();
            Add(root);
        }
        public void ClearTab()
        {
            m_TabView.Clear();
        }

        public Tab AddTab(BaseToolBox element, string tabName = "", bool closeable = true)
        {
            if (tabName == "")
                tabName = element.name;
            Tab tab = new Tab(tabName)
            {
                closeable = closeable
            };
            tab.selected += element.OnSelected;
            tab.closed += element.OnClosed;
            tab.Add(element);
            m_TabView.Add(tab);
            return tab;
        }

        public Tab OpenTab(BaseToolBox element, string tabName = "", bool closeable = true)
        {
            var tab = AddTab(element, tabName, closeable);
            m_TabView.activeTab = tab;
            return tab;
        }

        private void ReLoadAllTabs()
        {
            ClearTab();
            // 扫描程序集中所有继承了ToolBox的类
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<BaseToolBox> boxList = new();
            List<Type> types = new();
            foreach (var assembly in assemblies)
            {
                types.AddRange(assembly.GetTypes());
            }

            foreach (var type in types)
            {
                if (type.IsAbstract || type.IsInterface)
                    continue;
                if (typeof(BaseToolBox).IsAssignableFrom(type))
                {
                    var toolBox = Activator.CreateInstance(type) as BaseToolBox;
                    if (toolBox is { PreLoad: true })
                    {
                        boxList.Add(toolBox);
                    }
                }
            }

            boxList.Sort((a, b) => a.PreLoadIndex.CompareTo(b.PreLoadIndex));
            foreach (var toolBox in boxList)
            {
                Debug.Log($"ToolBox PreLoad: {toolBox.TabName}");
                AddTab(toolBox, toolBox.TabName, toolBox.Closeable);
            }

        }

    }
}