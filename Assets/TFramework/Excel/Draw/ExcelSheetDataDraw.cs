#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace TFramework.Excel
{
    [CustomEditor(typeof(ExcelSheetData))]
    public class ExcelSheetDataDraw : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var excelSheetData = target as ExcelSheetData;
            if(excelSheetData == null)
                return;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("CopyFrom"))
            {
                if(excelSheetData.sourceSheet != null)
                    excelSheetData.CopyFrom(excelSheetData.sourceSheet);
            }
            excelSheetData.sourceSheet = EditorGUILayout.ObjectField(excelSheetData.sourceSheet,typeof(ExcelSheetData),false) as ExcelSheetData;
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif