using System;
using TFramework.Runtime;
using UnityEngine;

public class UISamplePanel : UIManager.UIPanel
{
    private UIManager.UISystem _uiSystem;
    private void Awake()
    {
        _uiSystem = Framework.Instance.GetManager<UIManager>().GetSystem<UIManager.UISystem>();
    }

    public override void OnBeforeAnimationShow()
    {
        Framework.LogInfo("SamplePanel","OnBeforeAnimationShow",Color.cyan);
        _uiSystem.RegisterExit(this);
    }

    public override void OnAfterAnimationHide()
    {
        Framework.LogInfo("SamplePanel","OnAfterAnimationHide",Color.cyan);
        _uiSystem.UnRegisterExit(this);
    }

    public void OpenNextPanel()
    {
        _uiSystem.OpenPanel<UISamplePanel>();
    }
}
