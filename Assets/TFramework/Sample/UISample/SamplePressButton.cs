using System;
using TFramework.Component.UI;
using UnityEngine;

public class SamplePressButton : MonoBehaviour
{
    public Progress progress;
    public NormalButton button;

    private void Awake()
    {
        button.onPressHold.AddListener(OnPressHold);
        button.onPressStart.AddListener(OnPressStart);
        button.onPressEnd.AddListener(OnPressEnd);
    }

    void OnPressHold(float value)
    {
        progress.SetValue(value);
    }

    void OnPressStart()
    {
        progress.SetValue(0);
    }

    void OnPressEnd()
    {
        progress.SetValue(0);
    }
}
