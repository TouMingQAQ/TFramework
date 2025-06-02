using System;
using UnityEditor;
using UnityEngine;

namespace TFramework.Runtime
{
    public class GameManager : BaseManager
    {
        private void Awake()
        {
            framework.AddManager(this);
        }

        public void ExitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            return;
#endif
            Application.Quit();
        }
    }
}