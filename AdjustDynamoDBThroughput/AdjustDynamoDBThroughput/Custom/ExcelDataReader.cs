using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using Excel;

namespace AdjustDynamoDBThroughput.Custom
{
    class ExcelDataReader
    {
        public string[] ReadFromExcel(string fileName, int serialOfColumn)
        {
            //var fileName = @"C:\Users\kollol.nag\Documents\Visual Studio 2015\Projects\DynamoDBThroughputAdjustmentTool\FileValidationWatcher\bin\Debug\Throughputs.xls";

            FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read);

            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            string[] columnName = new string[41];
            int i = 0;

            while (excelReader.Read())
            {
                columnName[i] = excelReader.GetString(serialOfColumn);
                i++;
            }

            excelReader.Close();

            return columnName;
        }
    }
}
