#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Profile;
using UnityEngine;
using UnityEngine.Serialization;

namespace TFramework.ToolBox.Editor
{
    [CreateAssetMenu (menuName = "TFramework/BuildBoxSO", fileName = "BuildBoxSO")]
    public class BuildBoxSO : ScriptableObject
    {
        public List<ProFileInfo> Profiles = new();
        
        [Serializable]
        public class ProFileInfo
        {
            public BuildBoxProFileInfo BuildBoxProFileInfo;
            public BuildProfile BuildProfile;
            
            
            public BuildPlayerWithProfileOptions GetBuildPlayerOptions()
            {
                // 通过反射找到BuildProfile 中的PlayerSetting属性
                var option = new BuildPlayerWithProfileOptions
                {
                    locationPathName = BuildBoxProFileInfo.LocationPathName,
                    assetBundleManifestPath = BuildBoxProFileInfo.AssetBundleManifestPath,
                    buildProfile = BuildProfile
                };
                return option;
            }
        }
        [Serializable]
        public class BuildBoxProFileInfo
        {
            public string Name;
            //是否参与Build
            [FormerlySerializedAs("Build")] public bool AutoBuild;
            //输出路径
            public string LocationPathName;
            //AB包路径
            public string AssetBundleManifestPath;
        }
    }
}
#endif