using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace TFramework.Runtime
{
    public class AddressableModule : BaseModule
    {
        public List<string> key;
        
        public async UniTask<T> LoadAssetAsync<T>(string loadPath)
        {
           var handle = Addressables.LoadAssetAsync<T>(loadPath);
           await handle;
           var value = handle.Result;
           return value;
        }
        public async UniTask<IList<T>> LoadAssetsAsync<T>(Action<T> callBack)
        {
            var handle = Addressables.LoadAssetsAsync<T>(key, callBack);
            await handle;
            var value = handle.Result;
            return value;
        }
        public async UniTask<SceneInstance> LoadSceneAsync(string loadPath, LoadSceneMode loadSceneMode,Action<float> loadPercent)
        {
            var handle = Addressables.LoadSceneAsync(loadPath, loadSceneMode);
            while ( handle.PercentComplete < 0.9f)
            {
                loadPercent?.Invoke(handle.PercentComplete);
                await UniTask.DelayFrame(1);
            }
            await handle;
            return handle.Result;
        }
        
        
    }
}
