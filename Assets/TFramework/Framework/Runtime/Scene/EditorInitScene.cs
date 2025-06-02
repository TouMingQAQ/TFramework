#if UNITY_EDITOR
using System;
using TFramework.Runtime;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

public class EditorInitScene : MonoBehaviour
{
    public EditorInitSceneSO InitSceneSo;
    private async void Start()
    {
        EditorSceneManager.playModeStartScene = null;
        await Framework.WaitFrameworkInit();
        if(string.IsNullOrEmpty(InitSceneSo.preloadScenePath))
            return;
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        await EditorSceneManager.LoadSceneAsyncInPlayMode(InitSceneSo.preloadScenePath,
            new LoadSceneParameters(LoadSceneMode.Single));
        InitSceneSo.preloadScenePath = string.Empty;
    }
}
#endif