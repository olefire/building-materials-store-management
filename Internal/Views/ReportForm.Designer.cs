using System.ComponentModel;
using System.Windows.Forms;
using BuildingMaterialsStoreManagement.Internal.Utils;

namespace BuildingMaterialsStoreManagement.Internal.Views
{
    partial class ReportForm
    {
        private IContainer components = null;
        private DateTimePicker startDate;
        private DateTimePicker endDate;
        private ListBox productTypes;
        private Button reportButton;
        
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }


        private void InitializeComponent()
        {
            var formLabel = ViewUtils.CreateFormLabel("Форма Отчетов");
            var backButton = ButtonUtils.CreateBackButton(_formNavigator, this);
            

            startDate = new DateTimePicker
            {
                Width = 200,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd.MM.yyyy",
            };

            endDate = new DateTimePicker
            {
                Width = 200,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd.MM.yyyy",
            };

            productTypes = new ListBox
            {
                Width = 200,
                Height = 200,
                SelectionMode = SelectionMode.MultiExtended
            };
            
            reportButton = ButtonUtils.CreateButton("Сформировать отчет", GenerateReport);
            
            Controls.Add(formLabel);
            Controls.Add(backButton);
            Controls.Add(startDate);
            Controls.Add(endDate);
            Controls.Add(productTypes);
            Controls.Add(reportButton);
            
            this.components = new System.ComponentModel.Container();
            this.Text = "Отчет";
            this.Load += new System.EventHandler(FillProductTypes);
        }

    }
}