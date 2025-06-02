using System.Collections.Generic;
using TFramework.Runtime;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.Universal;

public class SceneStart : MonoBehaviour
{
    public Camera mainCamera;
    public List<AssetReference> references;
    
    async void Start()
    {
        await Framework.WaitFrameworkInit();
        LoadUI();
        BindUICamera();
    }

    public async void LoadUI()
    {
        await Framework.Instance.GetManager<UIManager>().PreLoadPanelAsync(references.ToArray(),null);
        Framework.Instance.GetManager<UIManager>().GetSystem<UIManager.UISystem>().OpenPanel<UISamplePanel>();
    }

    public void BindUICamera()
    {
        var add =  mainCamera.GetComponent<UniversalAdditionalCameraData>();
        add.cameraStack.Clear();
        var uiCamera = Framework.Instance.GetManager<UIManager>().UICamera;
        add.cameraStack.Add(uiCamera);

    }
   
}
