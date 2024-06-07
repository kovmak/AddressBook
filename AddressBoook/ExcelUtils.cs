using Microsoft.Win32;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Reflection;

namespace AddressBoook
{
    class ExcelUtils
    {
        public static void WriteExcel(Collection<Address> addresses, string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Sheet1");

                IRow row = excelSheet.CreateRow(0);
                string[] columns = new string[] { "Fio", "TelephoneNumber", "Home", "Gmail" };
                int columnIndex = 0;

                foreach (string column in columns)
                {
                    row.CreateCell(columnIndex).SetCellValue(column);
                    columnIndex++;
                }

                int rowIndex = 1;
                foreach (Address address in addresses)
                {
                    row = excelSheet.CreateRow(rowIndex);
                    int cellIndex = 0;
                    foreach (String col in columns)
                    {
                        Type objectType = address.GetType();
                        PropertyInfo property = objectType.GetProperty(col);
                        row.CreateCell(cellIndex).SetCellValue((string)property.GetValue(address, null));
                        cellIndex++;
                    }

                    rowIndex++;
                }

                for (int i = 0; i < columns.Length; i++)
                {
                    excelSheet.AutoSizeColumn(i);
                }

                workbook.Write(fs, false);
            }
        }
    }
}