using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BuildingMaterialsStoreManagement.Internal.Database.Abstract;
using BuildingMaterialsStoreManagement.Internal.Models;
using BuildingMaterialsStoreManagement.Internal.Utils;

namespace BuildingMaterialsStoreManagement.Internal.FormModels
{
    public class SaleFields: IFormModel
    {
        private Label IdLabel { get; set; } = new Label { Font = new Font("Arial", 12) };
        private ComboBox DepComboBox { get; set; } = new ComboBox();

        private DateTimePicker SaleData { get; set; } = new DateTimePicker
        {
            Format = DateTimePickerFormat.Short,
            Value = DateTime.Now,
            MinDate = new DateTime(2024, 1, 1)
        };
        private ComboBox StatusComboBox { get; set; } = new ComboBox();

        public List<TableUtils.FormField> CreateFormFieldList(PairRepository pairRepository = null, ItemRepository itemRepository = null)
        {
            ViewUtils.LoadDepartmentsToComboBox(DepComboBox, itemRepository);

            StatusComboBox.Items.Add("Чек формируется");
            StatusComboBox.Items.Add("Чек сформирован");

            return new List<TableUtils.FormField>
            {
                new TableUtils.FormField { LabelText = "Id", Control = IdLabel },
                new TableUtils.FormField { LabelText = "Дата продажи", Control = SaleData },
                new TableUtils.FormField { LabelText = "Отдел", Control = DepComboBox },
                new TableUtils.FormField { LabelText = "Статус", Control = StatusComboBox }
            };
        }
        public Sale GetSale()
        {
            if (DepComboBox.SelectedValue == null)
            {
                MessageBox.Show("Отдел не выбран");
                return null;
            }
            return new Sale
            {
                Id = string.IsNullOrEmpty(IdLabel.Text) ? 0 : Convert.ToInt32(IdLabel.Text),
                SaleDate = SaleData.Value,
                Department = new Department { Id = Convert.ToInt32(DepComboBox.SelectedValue) },
                Status = StatusComboBox.Text
            };
        }
        
        public void SelectionChanged(DataGridView dataGrid, ItemRepository itemRepository)
        {
            if (ViewUtils.IsSingleRowWithIdSelected(dataGrid))
            {
                var selectedRow = dataGrid.SelectedRows[0];
                var id = selectedRow.Cells["Id"].Value.ToString();
                IdLabel.Text = id;
                SaleData.Value = (DateTime) selectedRow.Cells["Дата продажи"].Value;
                DepComboBox.Visible = false;
                StatusComboBox.Visible = true;
                StatusComboBox.Text = selectedRow.Cells["Статус"].Value.ToString();
            }
            else
            {
                IdLabel.Text = string.Empty;
                SaleData.Value = DateTime.Now;
                DepComboBox.Visible = true;
                StatusComboBox.Visible = false;
            }
        }
    }
}