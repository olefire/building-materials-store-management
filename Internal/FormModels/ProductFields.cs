using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BuildingMaterialsStoreManagement.Internal.Database.Abstract;
using BuildingMaterialsStoreManagement.Internal.Models;
using BuildingMaterialsStoreManagement.Internal.Utils;

namespace BuildingMaterialsStoreManagement.Internal.FormModels
{
    public class ProductFields : IFormModel
    {
        private Label IdLabel { get; set; } = new Label { Font = new Font("Arial", 12) };
        private TextBox NameTextBox { get; set; } = new TextBox();
        private ComboBox DepComboBox { get; set; } = new ComboBox();
        private TextBox PriceTextBox { get; set; } = new TextBox();

        public List<TableUtils.FormField> CreateFormFieldList(PairRepository pairRepository,
            ItemRepository itemRepository = null)
        {
            ViewUtils.LoadPairsToComboBox(DepComboBox, pairRepository);
            return new List<TableUtils.FormField>
            {
                new TableUtils.FormField { LabelText = "Id", Control = IdLabel },
                new TableUtils.FormField { LabelText = "Название товара", Control = NameTextBox },
                new TableUtils.FormField { LabelText = "Отдел", Control = DepComboBox },
                new TableUtils.FormField { LabelText = "Цена", Control = PriceTextBox },
            };
        }

        public Product GetProduct()
        {
            var price = ParseUtils.TryParseDecimal(PriceTextBox);
            if (price == -1) return null;
            if (string.IsNullOrEmpty(NameTextBox.Text))
            {
                MessageBox.Show("Имя не может быть пустым");
                return null;
            }

            if (DepComboBox.SelectedValue == null)
            {
                MessageBox.Show("Отдел не может быть пустым");
                return null;
            }
            return new Product
            {
                Id = string.IsNullOrEmpty(IdLabel.Text) ? 0 : Convert.ToInt32(IdLabel.Text),
                Name = NameTextBox.Text,
                Price = price,
                Department = new Department { Id = (int)DepComboBox.SelectedValue }
            };
        }


        public void SelectionChanged(DataGridView dataGrid, PairRepository pairRepository)
        {
            if (ViewUtils.IsSingleRowWithIdSelected(dataGrid))
            {
                var selectedRow = dataGrid.SelectedRows[0];
                var id = selectedRow.Cells["Id"].Value.ToString();
                IdLabel.Text = id;
                NameTextBox.Text = selectedRow.Cells["Название товара"].Value.ToString();
                PriceTextBox.Text = selectedRow.Cells["Цена"].Value.ToString();
                var productTypeName = selectedRow.Cells["Отдел"].Value.ToString();
                var typeId = pairRepository.GetIdByName(productTypeName);
                DepComboBox.SelectedValue = typeId;
            }
            else
            {
                IdLabel.Text = string.Empty;

                NameTextBox.Text = string.Empty;
                PriceTextBox.Text = string.Empty;
                DepComboBox.SelectedIndex = -1;
            }
        }
    }
}