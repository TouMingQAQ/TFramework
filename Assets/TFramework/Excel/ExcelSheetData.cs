using System.Collections.Generic;
using UnityEngine;
using OfficeOpenXml;
namespace TFramework.Excel
{
    [CreateAssetMenu(menuName = "TFramework/Data/ExcelSheetData",fileName = "NewExcelSheetData")]
    public class ExcelSheetData : ScriptableObject
    {
        [TextArea]
        public string SheetName;
        public Vector2Int Size;
        public List<string> sheetValueList = new();
        public ExcelSheetData sourceSheet;
        public void SetSheet(ExcelWorksheet sheet)
        {
            // Set sheet data
            sheetValueList.Clear();
            if(sheet.Dimension == null)
                return;
            Size = new Vector2Int(sheet.Dimension.Rows, sheet.Dimension.Columns);
            for (int x = 1; x <= sheet.Dimension.Rows; x++)
            {
                for (int y = 1; y <= sheet.Dimension.Columns; y++)
                {
                    sheetValueList.Add(sheet.Cells[y, x].GetValue<string>());
                }
            }
        }
        public bool TryGetValue(Vector2Int index, out string value)
        {
            if (index.x < 1 || index.x > Size.x || index.y < 1 || index.y > Size.y)
            {
                value = string.Empty;
                return false;
            }
            value = sheetValueList[(index.x - 1) * Size.y + index.y - 1];
            return true;
        }
        public void CopyFrom(ExcelSheetData data)
        {
            SheetName = data.SheetName;
            Size = data.Size;
            sheetValueList = new List<string>(data.sheetValueList);
        }
    }
}