using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BuildingMaterialsStoreManagement.Internal.Database.Abstract;
using BuildingMaterialsStoreManagement.Internal.Models;
using BuildingMaterialsStoreManagement.Internal.Utils;

namespace BuildingMaterialsStoreManagement.Internal.FormModels
{
    public class ArrivalFields: IFormModel
    {
        private Label IdLabel { get; set; } = new Label { Font = new Font("Arial", 12) };

        private DateTimePicker ArrivalDate { get; set; } = new DateTimePicker
        {
            Format = DateTimePickerFormat.Short,
            Value = DateTime.Now,
            MinDate = new DateTime(2024, 1, 1)
        };
        private ComboBox DepComboBox { get; set; } = new ComboBox();
        private ComboBox StatusComboBox { get; set; } = new ComboBox(); 
        
        public List<TableUtils.FormField> CreateFormFieldList(PairRepository pairRepository, ItemRepository itemRepository)
        {
            ViewUtils.LoadDepartmentsToComboBox(DepComboBox, itemRepository);

            StatusComboBox.Items.Add("Поступление формируется");
            StatusComboBox.Items.Add("Поступление принято");

            return new List<TableUtils.FormField>
            {
                new TableUtils.FormField { LabelText = "Id", Control = IdLabel },
                new TableUtils.FormField { LabelText = "Дата поступления", Control = ArrivalDate },
                new TableUtils.FormField { LabelText = "Отдел", Control = DepComboBox },
                new TableUtils.FormField { LabelText = "Статус", Control = StatusComboBox }
            };
        }
        
        
        public Arrival GetArrival()
        {
            if (DepComboBox.SelectedValue == null)
            {
                MessageBox.Show("Отдел не выбран");
                return null;
            }

            return new Arrival
            {
                Id = string.IsNullOrEmpty(IdLabel.Text) ? 0 : Convert.ToInt32(IdLabel.Text),
                ArrivalDate = ArrivalDate.Value,
                Department = new Department { Id = (int)DepComboBox.SelectedValue },
                Status = StatusComboBox.Text
            };
        }
        
        public void SelectionChanged(DataGridView dataGrid)
        {
            if (ViewUtils.IsSingleRowWithIdSelected(dataGrid))
            {
                var selectedRow = dataGrid.SelectedRows[0];
                var id = selectedRow.Cells["Id"].Value.ToString();
                IdLabel.Text = id;
                ArrivalDate.Value = (DateTime) selectedRow.Cells["Дата поступления"].Value;
                DepComboBox.Visible = false;
                StatusComboBox.Visible = true;
                StatusComboBox.Text = selectedRow.Cells["Статус"].Value.ToString();
            }
            else
            {
                IdLabel.Text = string.Empty;
                ArrivalDate.Value = ArrivalDate.MinDate;
                DepComboBox.Visible = true;
                StatusComboBox.Visible = false;
            }
        }
        
    }
}