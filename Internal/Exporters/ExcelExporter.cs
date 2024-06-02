using System;
using System.IO;
using System.Runtime.InteropServices;
using BuildingMaterialsStoreManagement.Internal.Database;
using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;

namespace BuildingMaterialsStoreManagement.Internal.Exporters
{
    public class ExcelExporter
    {
        private readonly ProductReportRepository _repository;
        private readonly Application _excel;
        private readonly string _directoryPath;
        
        public ExcelExporter(ProductReportRepository repository)
        {
            _repository = repository;
            _excel = new Application();
            _directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "reports");
            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }
        }
        
        ~ExcelExporter()
        {
            if (_excel != null)
            {
                _excel.Quit();
                Marshal.ReleaseComObject(_excel);
            }
        }

        public void ExportToExcel(DateTime startDate, DateTime endDate, int[] productTypeIds)
        {
            Workbook workbook = null;
            Worksheet worksheet = null;
            try
            {
                var data = _repository.GetReport(startDate, endDate, productTypeIds);

                var fileName = $"Report_{startDate:yyyyMMdd}_to_{endDate:yyyyMMdd}.xlsx";
                var filePath = Path.Combine(_directoryPath, fileName);

                workbook = _excel.Workbooks.Add();
                worksheet = (Worksheet)workbook.ActiveSheet;

                worksheet.Cells[1, 1] = $"Отчет за период с {startDate.ToShortDateString()} по {endDate.ToShortDateString()}";

                // Adding column headers
                for (int j = 0; j < data.Columns.Count; j++)
                {
                    worksheet.Cells[2, j + 1] = data.Columns[j].ColumnName;
                }

                // Adding data rows
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    for (int j = 0; j < data.Columns.Count; j++)
                    {
                        worksheet.Cells[i + 3, j + 1] = data.Rows[i][j];
                    }
                }

                // Adding the "Total" row
                int totalRowIndex = data.Rows.Count + 3;
                worksheet.Cells[totalRowIndex, 1] = "Общая сумма";

                for (int j = 1; j < data.Columns.Count; j++)
                {
                    if (data.Columns[j].DataType == typeof(decimal) || data.Columns[j].DataType == typeof(double) || data.Columns[j].DataType == typeof(int))
                    {
                        double columnSum = 0;
                        for (int i = 0; i < data.Rows.Count; i++)
                        {
                            columnSum += Convert.ToDouble(data.Rows[i][j]);
                        }
                        worksheet.Cells[totalRowIndex, j + 1] = columnSum;
                    }
                }

                workbook.SaveAs(filePath);
            }
            finally
            {
                if (workbook != null)
                {
                    workbook.Close();
                    Marshal.ReleaseComObject(workbook);
                }

                if (worksheet != null)
                {
                    Marshal.ReleaseComObject(worksheet);
                }
            }
        }
    }
}
