using System.Collections.Generic;
using TFramework.Console;
using TFrameworkKit.Console.Command;
using UnityEditor;
using UnityEngine;

namespace Console.Editor
{
    [CreateAssetMenu(fileName = "ConsoleEditor",menuName = "TFramework/Console/ConsoleEditor")]
    public class ConsoleEditor : ScriptableObject
    {
        
    }
    [CustomEditor(typeof(ConsoleEditor))]
    public class ConsoleEditorView : UnityEditor.Editor
    {
        private string commandValue;
        private string commandValueCache;
        private HashSet<CommandTip> commandList = new HashSet<CommandTip>();

        private Vector2 scrollViewPos;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
         
            
            
            GUILayout.Label("输入指令");
            GUILayout.BeginHorizontal();
            commandValue = GUILayout.TextField(commandValue);
            var send = DrawRichTextButton("<color=#66ccff>Send</color>",GUILayout.Width(80));
            GUILayout.EndHorizontal();
            if (commandValueCache != commandValue)
            {
                ConsoleControl.CommandTipList(commandValue, in commandList);
                commandValueCache = commandValue;
            }
            scrollViewPos = GUILayout.BeginScrollView(scrollViewPos, false, false);
            foreach (var commandTip in commandList)
            {
                var inputStr = commandTip.InputStr;
                var showStr = commandTip.ShowStr;
                var click = DrawRichTextButton(showStr,GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth));
                if (click)
                {
                    commandValue = inputStr;
                    Repaint();
                }
            }
            GUILayout.EndScrollView();
            
            if (send)
            {
                if(string.IsNullOrEmpty(commandValue))
                    return;
                ConsoleControl.ExecuteCommand(commandValue);
                commandValue = string.Empty;
                commandValueCache = string.Empty;
                commandList.Clear();
                Repaint();
            }
            // SO面板必备：修改数据后自动保存
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
        // ✨ 封装好的【富文本按钮核心方法】，复制到你的编辑器类里，直接调用！
        private bool DrawRichTextButton(string richText, params GUILayoutOption[] options)
        {
            // 核心：用Label绘制富文本，模拟按钮的点击+悬浮效果
            Rect rect = GUILayoutUtility.GetRect(new GUIContent(richText), EditorStyles.label, options);
            EditorGUI.LabelField(rect, richText, new GUIStyle(EditorStyles.label)
            {
                richText = true, // 关键：开启富文本解析
                alignment = TextAnchor.MiddleCenter, // 文本居中（按钮标配）
                fontSize = 12 // 可选：统一字体大小
            });
            // 模拟按钮的点击事件 + 鼠标悬浮高亮效果
            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                GUI.FocusControl(null);
                Event.current.Use();
                return true;
            }
            // 鼠标悬浮时，绘制按钮高亮背景
            if (rect.Contains(Event.current.mousePosition))
            {
                EditorGUI.DrawRect(rect, new Color(0.6f, 0.8f, 1f, 0.3f));
            }
            return false;
        }
    }

}
