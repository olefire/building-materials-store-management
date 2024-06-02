using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BuildingMaterialsStoreManagement.Internal.Database;
using BuildingMaterialsStoreManagement.Internal.Exporters;
using Npgsql;

namespace BuildingMaterialsStoreManagement.Internal.Views
{
    public partial class ReportForm : Form
    {
        private readonly FormNavigator _formNavigator;
        private readonly ProductReportRepository _repository;
        private readonly ExcelExporter _excelExporter;
        public ReportForm(NpgsqlConnection connection,FormNavigator formNavigator)
        {
            _formNavigator = formNavigator;
            _repository = new ProductReportRepository(connection);
            _excelExporter = new ExcelExporter(_repository);
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            ClientSize = new System.Drawing.Size(800, 600);
        }
        
        private void GenerateReport(object sender, System.EventArgs e)
        {
            var startDateValue = startDate.Value;
            var endDateValue = endDate.Value;
            var selectedItems = productTypes.SelectedItems;
            var productTypeValues = new List<int>();

            foreach (Tuple<int, string> item in selectedItems)
            {
                productTypeValues.Add(item.Item1);
            }

            var productTypeValuesArray = productTypeValues.ToArray();
            _excelExporter.ExportToExcel(startDateValue, endDateValue, productTypeValuesArray);
            MessageBox.Show("Отчет успешно сгенерирован");
        }
        
        private void FillProductTypes(object sender, System.EventArgs e)
        {
            var productTypesData = _repository.GetAllProductTypes();

            productTypes.Items.Clear();

            productTypes.DataSource = productTypesData;
            productTypes.ValueMember = "Item1";
            productTypes.DisplayMember = "Item2";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            int formCenterX = this.ClientSize.Width / 2;

            var startDateLabel = new Label { Text = "Начальная дата", Location = new Point(formCenterX - startDate.Width - 60, 54) };
            var endDateLabel = new Label { Text = "Конечная дата:", Location = new Point(formCenterX - endDate.Width - 60, 104) };
            var productTypesLabel = new Label { Text = "Типы продуктов:", Location = new Point(formCenterX - productTypes.Width - 600, 154) };
            
            this.Controls.Add(startDateLabel);
            this.Controls.Add(endDateLabel);
            this.Controls.Add(productTypesLabel);
            
            startDate.Location = new System.Drawing.Point(formCenterX - startDate.Width / 2, 50);
            endDate.Location = new System.Drawing.Point(formCenterX - endDate.Width / 2, 100);
            productTypes.Location = new System.Drawing.Point(formCenterX - productTypes.Width / 2, 150);
            reportButton.Location = new System.Drawing.Point(formCenterX - reportButton.Width / 2, productTypes.Bottom + 10);
        }
    }
}