using System.Windows.Forms;
using BuildingMaterialsStoreManagement.Internal.Utils;

namespace BuildingMaterialsStoreManagement.Internal.Views
{
    partial class HomeForm
    {

        private void InitializeComponent()
        {
            var tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.ColumnCount = 3;
            tableLayoutPanel.RowCount = 2;
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            var productFormButton = ButtonUtils.CreateButton("Открыть продуктовую форму", ProductFormButton_Click);
            var departmentFormButton = ButtonUtils.CreateButton("Открыть форму отделов", DepartmentFormButton_Click);
            var arrivalFormButton = ButtonUtils.CreateButton("Открыть форму поступлений", ArrivalFormButton_Click);
            var saleFormButton = ButtonUtils.CreateButton("Открыть форму продаж", SaleFormButton_Click);
            var reportFormButton = ButtonUtils.CreateButton("Открыть форму отчетов", ReportFormButton_Click);
            
            tableLayoutPanel.Controls.Add(productFormButton, 0, 0);
            tableLayoutPanel.Controls.Add(departmentFormButton, 1, 0);
            tableLayoutPanel.Controls.Add(arrivalFormButton, 2, 0);
            tableLayoutPanel.Controls.Add(saleFormButton, 0, 1);
            tableLayoutPanel.Controls.Add(reportFormButton, 1, 1);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(tableLayoutPanel);
            this.Name = "HomeForm";
            this.Text = "Домашняя форма";
            this.ResumeLayout(false);
        }
    }
}