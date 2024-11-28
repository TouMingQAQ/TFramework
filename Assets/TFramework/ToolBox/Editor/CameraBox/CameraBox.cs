using TFramework.ToolBox.UIToolkitKit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TFramework.ToolBox.Editor
{
    public class CameraBox : BaseToolBox
    {
        public override string TabName => "摄像机";
        public override bool PreLoad => true;
        public override string VisualTreeAssetPath => "CameraBox";

        
        
        private ScrollView _cameraListView;
        private ScrollView _cameraInfoView;
        private CameraView _cameraView;
        public CameraBox() : base()
        {
            _cameraListView = this.Q<ScrollView>("CameraList");
            _cameraInfoView = this.Q<ScrollView>("CameraInfo");
            _cameraView = this.Q<CameraView>();
            this.Q<Button>("Refresh").clickable.clicked += Refresh;
            this.Q<Button>("ClearCamera").clickable.clicked += ClearCamera;
            this.Q<Button>("CollectCamera").clickable.clicked += CollectCamera;
            CollectCamera();
        }


        public void ClearCamera()
        {
            _cameraListView.Clear();
            _cameraView.ClearCapture();
            _cameraInfoView.Clear();
        }

        public void Refresh()
        {
            ClearCamera();
            CollectCamera();
            _cameraView.CaptureMainCamera();
        }
        public void CollectCamera()
        {
            _cameraListView.Clear();
            var cameras = UnityEngine.Camera.allCameras;
            foreach (var camera in cameras)
            {
                var cameraItem = new Button
                {
                    text = camera.name
                };
                _cameraListView.Add(cameraItem);
                cameraItem.clickable.clicked += () =>
                {
                    _cameraView.CaptureCamera(camera);
                    ShowCameraInfo(camera);
                };
            }
        }

        public void ShowCameraInfo(Camera camera)
        {
            if(camera == null)
                return;
            // 场景中选择相机
            Selection.activeObject = camera;
            _cameraInfoView.Clear();
            InspectorElement inspectorElement = new InspectorElement(camera);
            _cameraInfoView.Add(inspectorElement);
        }
    }
}
