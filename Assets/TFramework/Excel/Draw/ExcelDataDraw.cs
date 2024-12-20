#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TFramework.Excel
{
    [CustomEditor(typeof(ExcelData))]
    public class ExcelDataDraw : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var excelData = target as ExcelData;
            if(excelData == null)
                return;
            if(GUILayout.Button("Select Excel"))
            {
                string lastSelectPath = EditorPrefs.GetString("SelectExcelPath", Application.dataPath);
                var path = EditorUtility.OpenFilePanel("Select Excel", lastSelectPath,"xlsx");
                if(!string.IsNullOrEmpty(path))
                {
                    FileInfo info = new FileInfo(path);
                    if(info.Exists)
                        EditorPrefs.SetString("SelectExcelPath", info.DirectoryName);
                    excelData.excelPath = path;
                    EditorUtility.SetDirty(excelData);
                }
            }

            if (GUILayout.Button("Read Excel"))
            {
                excelData.ReadExcel();
            }

            if(GUILayout.Button("Open Excel"))
            {
                if (string.IsNullOrEmpty(excelData.excelPath))
                    return;
                System.Diagnostics.Process.Start(excelData.excelPath);
            }
            if(GUILayout.Button("Clear"))
            {
               

                var objs = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(excelData));
                foreach (var data in objs)
                {
                    AssetDatabase.RemoveObjectFromAsset(data);
                }
                excelData.SheetDataList.Clear();
                EditorUtility.SetDirty(excelData);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}
#endif
