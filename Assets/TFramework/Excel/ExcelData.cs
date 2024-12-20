using System.Collections.Generic;
using OfficeOpenXml;
using Unity.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace TFramework.Excel
{
    [CreateAssetMenu(menuName = "TFramework/Data/ExcelData",fileName = "NewExcelData")]
    public class ExcelData : ScriptableObject
    {
        [TextArea]
        public string excelPath = Application.dataPath;
        public List<ExcelSheetData> SheetDataList = new();
#if UNITY_EDITOR

        public void ReadExcel()
        {
            if(string.IsNullOrEmpty(excelPath))
                return;
            using var package = new ExcelPackage(new System.IO.FileInfo(excelPath));
            if(!package.File.Exists)
                return;
            foreach (var excelWorksheet in package.Workbook.Worksheets)
            {
                Debug.Log($"ReadExcel:{excelWorksheet.Name}");
                AddOrUpdateSheetData(excelWorksheet);
            }
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        public void AddOrUpdateSheetData(ExcelWorksheet worksheet)
        {
            var sheetData = SheetDataList.Find(data => data.SheetName == worksheet.Name);
            if (sheetData == null)
            {
                sheetData = ScriptableObject.CreateInstance<ExcelSheetData>();
                sheetData.SheetName = worksheet.Name;
                sheetData.name = worksheet.Name;
                SheetDataList.Add(sheetData);
                AssetDatabase.AddObjectToAsset(sheetData, this);
            }
            sheetData.SetSheet(worksheet);
        }
#endif
    }
}