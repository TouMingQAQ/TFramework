using System;
using TFramework.Component.UI;
using TMPro;
using UnityEngine;

public class SampleButton : MonoBehaviour
{
    public NormalButton button;

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
        button.onMultiClick.AddListener(OnMultiClick);
        button.onLeftClick.AddListener(OnLeftClick);
        button.onRightClick.AddListener(OnRightClick);
        button.onPressStart.AddListener(OnPressStart);
        button.onPressHold.AddListener(OnPressHold);
        button.onPressEnd.AddListener(OnPressEnd);
        
    }

    void OnClick()
    {
        Debug.Log("[<color=#66ccff>SampleButton</color>]:OnClick");
    }

    void OnMultiClick()
    {
        Debug.Log("[<color=#66ccff>SampleButton</color>]:OnMultiClick");
    }

    void OnLeftClick()
    {
        Debug.Log("[<color=#66ccff>SampleButton</color>]:OnLeftClick");
    }

    void OnRightClick()
    {
        Debug.Log("[<color=#66ccff>SampleButton</color>]:OnRightClick");
    }

    void OnPressStart()
    {
        Debug.Log("[<color=#66ccff>SampleButton</color>]:OnPressStart");
    }

    void OnPressHold(float duration)
    {
        Debug.Log($"[<color=#66ccff>SampleButton</color>]:OnPressHold[<color=yellow>{duration}</color>]");
    }

    void OnPressEnd()
    {
        Debug.Log("[<color=#66ccff>SampleButton</color>]:OnPressEnd");
    }
}



