using UnityEngine;

namespace TFramework.Framework.Editor
{
    [CreateAssetMenu(menuName = "TFramework/Editor/EditorToolbar",fileName = "EditorToolbarSO")]
    public class EditorToolbarSO : ScriptableObject
    {
        public string preloadScenePath;
    }
}
