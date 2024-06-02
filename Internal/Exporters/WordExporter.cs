using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BuildingMaterialsStoreManagement.Internal.Database;
using Microsoft.Office.Interop.Word;
using Application = Microsoft.Office.Interop.Word.Application;

namespace BuildingMaterialsStoreManagement.Internal.Exporters
{
    public class WordExporter
    {
        private readonly IReportRepository _repository;
        private readonly Application _word;
        private readonly string _directoryPath;

        public WordExporter(IReportRepository repository, string type)
        {
            _repository = repository;
            _word = new Application();
            _directoryPath = Path.Combine(Directory.GetCurrentDirectory(), type);
            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }
        }

        ~WordExporter()
        {
            if (_word != null)
            {
                _word.Quit();
                Marshal.ReleaseComObject(_word);
            }
        }

        public void ExportToWord(int id)
        {
            Document doc = null;
            object missing = Type.Missing;
            object falseValue = false;
            try
            {
                var data = _repository.Get(id);
                var items = _repository.GetItems(id);

                doc = _word.Documents.Add(ref missing, ref missing, ref missing, ref missing);
                Range range = doc.Range(ref missing, ref missing);

                InsertHeader(range, "Учет");

                InsertTable(doc, data);

                range = doc.Content.Paragraphs.Add(ref missing).Range;
                InsertHeader(range, "Продукты");
                InsertTable(doc, items);

                var fileName = $"Учет_{id}.docx";
                var filePath = Path.Combine(_directoryPath, fileName);
                doc.SaveAs2(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                MessageBox.Show("Не закрывайте Word(");
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close(ref falseValue, ref missing, ref missing);
                    Marshal.ReleaseComObject(doc);
                }
            }
        }

        private void InsertHeader(Range range, string headerText)
        {
            range.Text = headerText + "\n";
            range.Font.Size = 14;
            range.Font.Bold = 1;
            range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            range.InsertParagraphAfter();
        }

        private void InsertTable(Document doc, System.Data.DataTable tableData)
        {
            object missing = Type.Missing;
            Range range = doc.Content.Paragraphs.Add(ref missing).Range;
            Table table = doc.Tables.Add(range, tableData.Rows.Count + 1, tableData.Columns.Count, ref missing, ref missing);
            table.Borders.Enable = 1;
            table.Range.Font.Size = 12;

            for (int i = 0; i < tableData.Columns.Count; i++)
            {
                table.Cell(1, i + 1).Range.Text = tableData.Columns[i].ColumnName;
                table.Cell(1, i + 1).Range.Bold = 1;
                table.Cell(1, i + 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            }

            // Данные
            for (int i = 0; i < tableData.Rows.Count; i++)
            {
                for (int j = 0; j < tableData.Columns.Count; j++)
                {
                    table.Cell(i + 2, j + 1).Range.Text = tableData.Rows[i][j].ToString();
                }
            }

            range.InsertParagraphAfter();
        }
    }
}
