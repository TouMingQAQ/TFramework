#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace TFramework.UIEffect
{
    public class UIEffectEditor : ShaderGUI
    {
        private MaterialProperty _MoveSpeed = null;
        private MaterialProperty _MoveDirection = null;
        private MaterialProperty _IsUVFlow = null;
        private bool openUVFlowEffect = false;
        
        private MaterialProperty _ShinyTex = null;
        private MaterialProperty _ShinyRotate = null;
        private MaterialProperty _ShinySpeed = null;
        private MaterialProperty _ShinyDirection = null;
        private MaterialProperty _ShinyDurationTime = null;
        private MaterialProperty _ShinyColor = null;
        private MaterialProperty _IsShiny = null;
        private bool openShinyEffect = false;

        
        private MaterialProperty _ClipArea = null;
        private MaterialProperty _IsClipArea = null;
        private bool openClipEffect = false;

        
        
        private Material _material;
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            // base.OnGUI(materialEditor,properties);
            _material = materialEditor.target as Material;
            if(_material == null)
                return;
            
            
            _MoveSpeed = FindProperty("_MoveSpeed", properties);
            _MoveDirection = FindProperty("_MoveDirection", properties); 
            _IsUVFlow = FindProperty("_IsUVFlow", properties);
            
            _ShinyTex = FindProperty("_ShinyTex", properties);
            _ShinyRotate = FindProperty("_ShinyRotate", properties);
            _ShinySpeed = FindProperty("_ShinySpeed", properties);
            _ShinyDirection = FindProperty("_ShinyDirection", properties);
            _ShinyDurationTime = FindProperty("_ShinyDurationTime", properties);
            _ShinyColor = FindProperty("_ShinyColor", properties);
            _IsShiny = FindProperty("_IsShiny", properties);
            
            _ClipArea = FindProperty("_ClipArea", properties);
            _IsClipArea = FindProperty("_IsClipArea", properties);
            
            
            
            
            var toggleShinyEffect = _IsShiny.floatValue > 0;
            Foldout( "闪光",ref openUVFlowEffect,ref toggleShinyEffect);
            if (openUVFlowEffect && toggleShinyEffect)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                materialEditor.ShaderProperty(_ShinyTex, "闪光贴图");
                materialEditor.ShaderProperty(_ShinyRotate, "旋转方向");
                materialEditor.ShaderProperty(_ShinySpeed, "速度");
                materialEditor.ShaderProperty(_ShinyDirection, "方向");
                materialEditor.ShaderProperty(_ShinyDurationTime, "间隔时间");
                materialEditor.ShaderProperty(_ShinyColor, "颜色");
                EditorGUILayout.EndVertical();
            }
            _IsShiny.floatValue = toggleShinyEffect ? 1 : 0;
            if(toggleShinyEffect)
                _material.EnableKeyword("SHINY_ON");
            else
                _material.DisableKeyword("SHINY_ON");
            
            
            var toggleUIFlowEffect = _IsUVFlow.floatValue > 0;
            Foldout( "UV流动",ref openShinyEffect,ref toggleUIFlowEffect);
            if (openShinyEffect && toggleUIFlowEffect)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                materialEditor.ShaderProperty(_MoveSpeed, "流动速度");
                materialEditor.ShaderProperty(_MoveDirection, "流动方向");
                EditorGUILayout.EndVertical();
            }
            _IsUVFlow.floatValue = toggleUIFlowEffect ? 1 : 0;
            if(toggleUIFlowEffect)
                _material.EnableKeyword("UVFLOW_ON");
            else
                _material.DisableKeyword("UVFLOW_ON");
        }

        static void Foldout(string title,ref bool openFoldout,ref bool openToggle)
        {
            var style = new GUIStyle("ShurikenModuleTitle")
            {
                font = new GUIStyle(EditorStyles.boldLabel).font,
                border = new RectOffset(15, 7, 4, 4),
                fixedHeight = 22,
                contentOffset = new Vector2(20f, -2f)
            };

            var rect = GUILayoutUtility.GetRect(16f, 22f,style);
            GUI.Box(rect,title,style);
            var e = Event.current;

            var foldoutRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            var toggleRect = new Rect(rect.width - 10, rect.y + 2f, 13f, 13f);
            if (e.type == EventType.Repaint)
            {
                EditorStyles.foldout.Draw(foldoutRect, false, false, openFoldout && openToggle, false);
                EditorStyles.toggle.Draw(toggleRect, false, false, openToggle, false);
            }

            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                if (toggleRect.Contains(e.mousePosition))
                {
                    openToggle = !openToggle;
                }
                else
                {
                    openFoldout = !openFoldout;
                }
                e.Use();
            }
        }
    }

}
#endif