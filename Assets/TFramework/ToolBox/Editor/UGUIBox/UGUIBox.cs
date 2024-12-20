using System;
using System.Collections.Generic;
using Codice.CM.Common;
using TFramework.ToolBox.UIToolKit;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using TwoPaneSplitView = TFramework.ToolBox.UIToolKit.TwoPaneSplitView;

namespace TFramework.ToolBox
{
    public class UGUIBox : BaseToolBox
    {
        public override bool Closeable => false;
        public override string TabName => "UGUI";
        public override bool PreLoad => true;
        public override string VisualTreeAssetPath => "UGUIBox";
        private ScrollView _scrollView;
        private TabView _tabView;
        private ImageView _uiView;
        public UGUICapture Capture;
        private List<UGUITool> toolList = new();
        public UGUIBox() : base()
        {
            _scrollView = this.Q<ScrollView>();
            _tabView = this.Q<TabView>();
            _tabView.Q<VisualElement>(TabView.contentContainerUssClassName).style.flexGrow = 1;
            _uiView = this.Q<ImageView>();
            Capture = Resources.Load<UGUICapture>("UGUICapture");
            // 监听Unity editor选择对象变化
            Selection.selectionChanged += OnSelectionChanged;
            AddTool<UGUINormalInfo>();
            _tabView.selectedTabIndex = 0;
        }
        

        private void OnSelectionChanged()
        {
            if(!selected)
                return;
            var objs = Selection.gameObjects;
            foreach (var o in objs)
            {
                if (o.TryGetComponent(out RectTransform rectTransform))
                {
                    Show(rectTransform);
                    foreach (var tool in toolList)
                    {
                        tool.ChangedUI(rectTransform);
                    }
                    return;
                }
            }
        }
        void Show(RectTransform rectTransform)
        {
            var capture = Object.Instantiate(Capture);
            try
            {
                var texture = capture.Capture(rectTransform);
                _uiView.Texture = texture;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            Object.DestroyImmediate(capture.gameObject);
        }

        void AddTool<T>() where T : UGUITool,new()
        {
            var tool = new T();
            Tab tab = new Tab(tool.TabName);
            tab.Q<VisualElement>(Tab.contentUssClassName).style.flexGrow = 1;
            tab.Add(tool);
            tool.Init();
            tab.selected += tool.Selected;
            tab.closed += tool.Closed;
            _tabView.Add(tab);
            toolList.Add(tool);
        }

        public abstract class UGUITool : VisualElement
        {
            public virtual string TabName => $"{this.GetType().Name}";
            protected bool selected = false;
            protected RectTransform selectRectTransform;
            public abstract void Init();
            public virtual void Selected(Tab tab)
            {
                selected = true;
            }
            public virtual void Closed(Tab tab)
            {
                selected = false;
            }
            public void ChangedUI(RectTransform rectTransform)
            {
                selectRectTransform = rectTransform;
                OnSelectionChanged(rectTransform);
            }
            protected abstract void OnSelectionChanged(RectTransform rectTransform);
        }
    }
}