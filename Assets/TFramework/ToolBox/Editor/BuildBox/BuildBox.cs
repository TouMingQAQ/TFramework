#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TFramework.ToolBox.Editor
{
    public class BuildWindow : EditorWindow
    {
        [MenuItem("TFramework/ToolBox/Build")]
        public static void OpenConsoleWindow()
        {
            EditorWindow wnd = EditorWindow.GetWindow<BuildWindow>();
            wnd.titleContent = new GUIContent("打包工具");
        }

        private void CreateGUI()
        {
            rootVisualElement.Add(new BuildBox());
        }
    }
    public partial class BuildBox : BaseToolBox
    {
        public override string TabName => "打包工具";
        public override string VisualTreeAssetPath => "BuildBox";
        public override int PreLoadIndex => 1;
        public override bool Closeable => false;

        public override bool PreLoad => true;
        public BuildBoxSO boxSo;

        private ScrollView _rightScrollView;
        private VisualElement _rightInfo;
        private ScrollView _leftScrollView;
        
        private ToolbarButton _refreshButton;
        private ToolbarButton _touchSoButton;
        private ToolbarButton _buildButton;
        private ToolbarButton _buildAllButton;
        private ProgressBar _buildProgress;
        public BuildBox() : base()
        {
            boxSo = Resources.Load<BuildBoxSO>("BuildBoxSO");
            var left = this.Q<VisualElement>("LeftPane");
            var right = this.Q<VisualElement>("RightPane");
            _buildButton = this.Q<ToolbarButton>("BuildButton");
            _buildProgress = this.Q<ProgressBar>("BuildProgress");
            _refreshButton = this.Q<ToolbarButton>("Refresh");
            _touchSoButton = this.Q<ToolbarButton>("BoxSO");
            _buildAllButton = this.Q<ToolbarButton>("BuildAllButton");
            _refreshButton.clickable.clicked += Refresh;
            _touchSoButton.clickable.clicked += TouchSo;
            _buildAllButton.clickable.clicked += BuildAll;
            _buildButton.clickable.clicked += BuildSelect;

            _buildProgress.visible = false;
            _buildProgress.title = string.Empty;
            _buildProgress.value = 0;
            _rightScrollView = new ScrollView();
            _leftScrollView = new ScrollView();
            _rightInfo = new VisualElement
            {
                style =
                {
                    flexShrink = 0
                }
            };
            right.Add(_rightInfo);
            Foldout foldout = new Foldout
            {
                text = "Profiles",
                value = true
            };
            foldout.Add(_rightScrollView);
            right.Add(foldout);
            left.Add(_leftScrollView);
            
            LoadData(boxSo);
        }

        public void LoadData(BuildBoxSO so)
        {
            _leftScrollView.Clear();
            foreach(var profile in so.Profiles)
            {
                var button = new Button();
                button.text = profile.BuildBoxProFileInfo.Name;
                button.clickable.clicked += () => Show(profile);
                _leftScrollView.Add(button);
            }
        }
        private void Refresh()
        {
            LoadData(boxSo);
        }
        private void TouchSo()
        {
            Selection.activeObject = boxSo;
        }
        private BuildBoxSO.ProFileInfo _currentProfile = null;
        public void Show(BuildBoxSO.ProFileInfo info)
        {
            _rightInfo.Clear();
            _currentProfile = info;
            var title = new Label(info.BuildBoxProFileInfo.Name)
            {
                style =
                {
                    fontSize = 30,
                }
            };
            
            _rightInfo.Add(title);
            Foldout foldout = new Foldout
            {
                text = "打包配置",
                value = true
            };
            
            
            var toggle = new Toggle("自动构建")
            {
                value = info.BuildBoxProFileInfo.AutoBuild
            };
            toggle.RegisterCallback(new EventCallback<ChangeEvent<bool>>(evt =>
            {
                info.BuildBoxProFileInfo.AutoBuild = evt.newValue;
            }));
            foldout.Add(toggle);
            
            var textfield = new TextField("输出路径")
            {
                value = info.BuildBoxProFileInfo.LocationPathName
            };
            textfield.RegisterCallback(new EventCallback<ChangeEvent<string>>(evt =>
            {
                info.BuildBoxProFileInfo.LocationPathName = evt.newValue;
            }));
            var textfield2 = new TextField("AB包路径")
            {
                value = info.BuildBoxProFileInfo.AssetBundleManifestPath
            };
            textfield2.RegisterCallback(new EventCallback<ChangeEvent<string>>(evt =>
            {
                info.BuildBoxProFileInfo.AssetBundleManifestPath = evt.newValue;
            }));
            
            foldout.Add(textfield);
            foldout.Add(textfield2);
            _rightInfo.Add(foldout);
            _rightScrollView.Clear();
            var _inspector = new InspectorElement(info.BuildProfile);
            _rightScrollView.Add(_inspector);
        }
        public void EndProgress()
        {
            _buildProgress.value = 0;
            _buildProgress.visible = false;
        }
        public void Progress(string state,float value)
        {
            _buildProgress.title = state;
            _buildProgress.visible = true;
            _buildProgress.value = value;
        }

        public void BuildSelect()
        {
            if(_currentProfile == null)
            {
                Debug.LogError("请选择一个Profile");
                return;
            }
            Build(_currentProfile);
        }
        public void Build(BuildBoxSO.ProFileInfo profile)
        {
            var info = profile.BuildBoxProFileInfo;
            if (string.IsNullOrEmpty(info.LocationPathName))
            {
                Debug.LogError($"[{info.Name}]输出路径不能为空");
                return;
            }
            var options = profile.GetBuildPlayerOptions();
            var report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"Build succeeded: {summary.outputPath}");
            }
        }
        public void BuildAll()
        {
            foreach (var profile in boxSo.Profiles)
            {
                var info = profile.BuildBoxProFileInfo;
                if (!info.AutoBuild)
                    continue;
                Build(profile);
            }
            EndProgress();
        }
        
    }

}
#endif