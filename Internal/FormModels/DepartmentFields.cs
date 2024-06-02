using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BuildingMaterialsStoreManagement.Internal.Database.Abstract;
using BuildingMaterialsStoreManagement.Internal.Models;
using BuildingMaterialsStoreManagement.Internal.Utils;

namespace BuildingMaterialsStoreManagement.Internal.FormModels
{
    public class DepartmentFields: IFormModel
    {
        private Label IdLabel { get; set; } = new Label { Font = new Font("Arial", 12) };
        private TextBox AddressTextBox { get; set; } = new TextBox();
        
        public List<TableUtils.FormField> CreateFormFieldList(PairRepository pairRepository = null, ItemRepository itemRepository = null)
        {
            return new List<TableUtils.FormField>
            {
                new TableUtils.FormField { LabelText = "Id", Control = IdLabel },
                new TableUtils.FormField { LabelText = "Отдел", Control = AddressTextBox }
            };
        }
        
        public Department GetDepartment()
        {
            if (string.IsNullOrEmpty(AddressTextBox.Text))
            {
                MessageBox.Show("Поле 'Отдел' не может быть пустым.");
                return null;
            }
            return new Department
            {
                Id = string.IsNullOrEmpty(IdLabel.Text) ? 0 : Convert.ToInt32(IdLabel.Text),
                Name = AddressTextBox.Text
            };
        }
        
        
        public void SelectionChanged(DataGridView dataGrid)
        {
            if (ViewUtils.IsSingleRowWithIdSelected(dataGrid))
            {
                var selectedRow = dataGrid.SelectedRows[0];
                var id = selectedRow.Cells["Id"].Value.ToString();
                IdLabel.Text = id;
                AddressTextBox.Text = selectedRow.Cells["Отдел"].Value.ToString();
            }
            else
            {
                IdLabel.Text = string.Empty;

                AddressTextBox.Text = string.Empty;
            }
        }
    }
}