using System;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace TFramework.Runtime
{
    public partial class SceneManager : BaseManager
    {
        private void Awake()
        {
            framework.AddManager(this);
            AddModule<AddressableModule>();
            AddModule<EventModule>();
        }
        
        public async UniTask LoadSceneAsync(string mainPath,string[] subPaths = null,Action<float> loadPercent = null)
        {
            if (string.IsNullOrEmpty(mainPath))
            {
                Framework.LogInfo("SceneLoad","Main path is empty",Color.red);
                return;
            }
            var addressableModule = GetModule<AddressableModule>();
            //显示加载界面
            float totalPercent = 0;
            float totalCount = subPaths == null ? 1 : subPaths.Length + 1;
            loadPercent?.Invoke(totalPercent);
            
            
            StringBuilder sb = new StringBuilder(mainPath);
            if (subPaths != null)
            {
                foreach (var subPath in subPaths)
                {
                    sb.AppendLine(subPath);
                }
            }
            
            Framework.LogInfo("SceneLoad","加载场景:" + sb,Color.cyan);
            
            
            
            Framework.LogInfo("SceneLoad","加载主场景:" + mainPath,Color.cyan);
            await addressableModule.LoadSceneAsync(mainPath, UnityEngine.SceneManagement.LoadSceneMode.Single,
                (percent) =>
                {
                    totalPercent += percent / totalCount;
                    loadPercent?.Invoke(totalPercent);
                });
            Framework.LogInfo("SceneLoad","————加载完毕————",Color.green);
            
            
            if (subPaths != null)
            {
                //先加载其他场景
                foreach (var subPath in subPaths)
                {
                    Framework.LogInfo("SceneLoad","加载附属场景:" + subPath,Color.cyan);
                    await addressableModule.LoadSceneAsync(subPath, UnityEngine.SceneManagement.LoadSceneMode.Additive,
                        (percent) =>
                        {
                            totalPercent += percent / totalCount;
                            loadPercent?.Invoke(totalPercent);
                        });
                }
            }
      
           

            //加载完成
            totalPercent = 1;
            loadPercent?.Invoke(totalPercent);
            await UniTask.Delay(1000);
          

        }
        
    }
}