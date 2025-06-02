#if UNITY_EDITOR
using System;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
namespace TFramework.Framework.Editor
{
    [InitializeOnLoad]
    public static class EditorToolbar
    {
        private static readonly Type kToolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
        private static ScriptableObject sCurrentToolbar;


        static EditorToolbar()
        {
            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            if (sCurrentToolbar == null)
            {
                UnityEngine.Object[] toolbars = Resources.FindObjectsOfTypeAll(kToolbarType);
                sCurrentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
                if (sCurrentToolbar != null)
                {
                    FieldInfo root = sCurrentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                    VisualElement concreteRoot = root.GetValue(sCurrentToolbar) as VisualElement;

                    VisualElement toolbarZone = concreteRoot.Q("ToolbarZoneRightAlign");
                    VisualElement parent = new VisualElement()
                    {
                        style = {
                            flexGrow = 1,
                            flexDirection = FlexDirection.Row,
                        }
                    };
                    IMGUIContainer container = new IMGUIContainer();
                    container.onGUIHandler += OnGuiBody;
                    parent.Add(container);
                    toolbarZone.Add(parent);
                }
            }
        }

        private static void OnGuiBody()
        {
            //自定义按钮加在此处
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("PreInitPlay", EditorGUIUtility.FindTexture("PlayButton"))))
            {
                PreInitPlay();
            }
            GUILayout.EndHorizontal();
        }

        static async void PreInitPlay()
        {
            var initScenePath = "Assets/TFramework/Framework/Runtime/Scene/EditorInitScene.unity";
            var currenScenePath = SceneManager.GetActiveScene().path;
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(initScenePath);
            var initSceneSO =
                AssetDatabase.LoadAssetAtPath<EditorInitSceneSO>(
                    "Assets/TFramework/Framework/Runtime/Scene/EditorInitSceneSO.asset");
            initSceneSO.preloadScenePath = currenScenePath;
            EditorSceneManager.playModeStartScene = sceneAsset;
            EditorApplication.isPlaying = true;
        }
    
    }
}
#endif