using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TFramework.ToolBox
{
    public class PathBoxWindow : EditorWindow
    {
        [MenuItem("TFramework/ToolBox/Path")]
        public static void ShowExample()
        {
            var window = GetWindow<PathBoxWindow>();
            window.titleContent = new GUIContent("路径");
            window.minSize = new Vector2(200, 400);
        }

        private void CreateGUI()
        {
            rootVisualElement.Add(new PathBox());

        }
    }
    public class PathBox : BaseToolBox
    {
        public override string TabName => "路径";
        public override bool PreLoad => true;
        public override string VisualTreeAssetPath => "PathBox";

        private ScrollView _scrollView;

        public PathBox() : base()
        {
            _scrollView = this.Q<ScrollView>();
            AddPath(Application.dataPath);
            AddPath(Application.persistentDataPath);
            AddPath(Application.streamingAssetsPath);
            AddPath(Path.GetTempPath());
        }
        void AddPath(string path)
        {
            path = path.Replace("\\", "/");
            if (!path.EndsWith("/"))
                path += "/";
            var havePath = Directory.Exists(path);
            var root = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };
            var openButton = new Button
            {
                text = "打开",
                style =
                {
                    display = havePath ? DisplayStyle.Flex : DisplayStyle.None
                }
            };
            openButton.clickable.clicked += () => OpenInFinder(path);
            var createButton = new Button()
            {
                text = "创建",
                style =
                {
                    display = !havePath ? DisplayStyle.Flex : DisplayStyle.None
                }
            };
            createButton.clickable.clicked += () =>
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception e)
                {
                    Debug.LogWarning("创建失败" + e);
                }
                openButton.style.display = DisplayStyle.Flex;
                createButton.style.display = DisplayStyle.None;
            };
            var pathLabel = new Label(path);
            root.Add(openButton);
            root.Add(createButton);
            root.Add(pathLabel);
            _scrollView.Add(root);
        }
        public void OpenInFinder(string path)
        {
            EditorUtility.RevealInFinder(path);
        }

        
    }

}
