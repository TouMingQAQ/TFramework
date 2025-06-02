using System;
using System.Collections.Generic;
using TFramework.Component.UI;
using UnityEngine;
using UnityEngine.UI;

public class BindSlider2Progress : MonoBehaviour
{
    public Slider slider;
    public MultiProgress multiProgress;
    public Progress progress;

    private void LateUpdate()
    {
        multiProgress.SetValue(slider.value);
        progress.SetValue(slider.value);
    }
}
