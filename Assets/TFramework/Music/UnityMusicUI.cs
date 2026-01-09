using System;
using TFramework.Music;
using UnityEngine;
using UnityEngine.UI;

public class UnityMusicUI : MonoBehaviour
{
    public Image image;
    public UnityMusic Music;

    private void Update()
    {
        image.sprite = Music.MusicCover;
    }
}
