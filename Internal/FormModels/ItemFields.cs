using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BuildingMaterialsStoreManagement.Internal.Database;
using BuildingMaterialsStoreManagement.Internal.Database.Abstract;
using BuildingMaterialsStoreManagement.Internal.Models;
using BuildingMaterialsStoreManagement.Internal.Utils;

namespace BuildingMaterialsStoreManagement.Internal.FormModels
{
    public class ItemFields : IFormModel
    {
        private Label IdLabel { get; set; } = new Label { Font = new Font("Arial", 12) };
        private ComboBox ProductComboBox { get; set; } = new ComboBox();
        private Label PriceLabel { get; set; } = new Label { Font = new Font("Arial", 12) };
        private TextBox QuantityTextBox { get; set; } = new TextBox();
        private Label WarehouseQuantityLabel { get; set; } = new Label { Font = new Font("Arial", 12) };

        public List<TableUtils.FormField> CreateFormFieldList(PairRepository pairRepository,
            ItemRepository itemRepository)
        {
            ProductComboBox.SelectedIndexChanged += (sender, e) =>
            {
                if (ProductComboBox.SelectedIndex != -1)
                {
                    var productId = Convert.ToInt32(ProductComboBox.SelectedValue);
                    var product = itemRepository.GetProductDataById(productId);
                    if (product != null)
                    {
                        PriceLabel.Text = product.Price.ToString(new CultureInfo("ru-RU"));
                        WarehouseQuantityLabel.Text = itemRepository.GetProductQuantityById(productId).ToString();
                    }
                }
                else
                {
                    PriceLabel.Text = string.Empty;
                    WarehouseQuantityLabel.Text = "-";
                }
            };
            return new List<TableUtils.FormField>
            {
                new TableUtils.FormField { LabelText = "Id", Control = IdLabel },
                new TableUtils.FormField { LabelText = "Продукт", Control = ProductComboBox },
                new TableUtils.FormField { LabelText = "Цена", Control = PriceLabel },
                new TableUtils.FormField { LabelText = "Количество", Control = QuantityTextBox },
                new TableUtils.FormField { LabelText = "На складе", Control = WarehouseQuantityLabel }
            };
        }

        public Item GetItem(int id)
        {
            if (ProductComboBox.SelectedValue == null)
            {
                MessageBox.Show("Продукт не выбран");
                return null;
            }

            if (string.IsNullOrEmpty(QuantityTextBox.Text))
            {
                MessageBox.Show("Количество не введено.");
                return null;
            }

            return new Item
            {
                Id = string.IsNullOrEmpty(IdLabel.Text) ? 0 : Convert.ToInt32(IdLabel.Text),
                RefId = id,
                Product = new Product { Id = (int)ProductComboBox.SelectedValue },
                Quantity = int.Parse(QuantityTextBox.Text)
            };
        }

        public void LoadProductData(ItemRepository itemRepository, int departmentId)
        {
            var productIdsAndNames = itemRepository.GetProductIdsAndNamesByDepartment(departmentId);
            ProductComboBox.ValueMember = "Item1";
            ProductComboBox.DisplayMember = "Item2";
            ProductComboBox.DataSource = productIdsAndNames;
            ProductComboBox.SelectedIndex = -1;
        }

        public void SelectionChanged(DataGridView dataGrid, ItemRepository itemRepository)
        {
            if (ViewUtils.IsSingleRowWithIdSelected(dataGrid))
            {
                var selectedRow = dataGrid.SelectedRows[0];
                var id = selectedRow.Cells["Id"].Value.ToString();
                IdLabel.Text = id;
                PriceLabel.Text = selectedRow.Cells["Цена"].Value.ToString();
                QuantityTextBox.Text = selectedRow.Cells["Количество"].Value.ToString();
                var productName = selectedRow.Cells["Продукт"].Value.ToString();
                var productId = itemRepository.GetProductIdByName(productName);
                ProductComboBox.SelectedValue = productId;
            }
            else
            {
                IdLabel.Text = string.Empty;
                QuantityTextBox.Text = string.Empty;
                PriceLabel.Text = "-";
                ProductComboBox.SelectedIndex = -1;
                WarehouseQuantityLabel.Text = "-";
            }
        }
        
        public void AddQuantity(ItemRepository itemRepository)
        {
            var productId = (int)ProductComboBox.SelectedValue;
            var currQuantity = int.Parse(QuantityTextBox.Text);
            itemRepository.AddProductQuantityById(productId, currQuantity);
        }

        public bool SubQuantity(ItemRepository itemRepository)
        {
            var productId = (int)ProductComboBox.SelectedValue;
            var warehouseQuantity = Convert.ToInt32(WarehouseQuantityLabel.Text);
            var currQuantity = int.Parse(QuantityTextBox.Text);
            if (!(warehouseQuantity - currQuantity > 0))
            {
                MessageBox.Show("Количество товара не может быть меньше 0!");
                return false;
            }

            itemRepository.AddProductQuantityById(productId, currQuantity * -1);
            return true;
        }

        public bool UpdateWarehouseQuantity(DataGridView itemsData, ItemRepository itemRepository, int sign)
        {
            if (ViewUtils.IsSingleRowWithIdSelected(itemsData))
            {
                var selectedRow = itemsData.SelectedRows[0];
                var quantity = Convert.ToInt32(selectedRow.Cells["Количество"].Value.ToString());
                var warehouseQuantity = Convert.ToInt32(WarehouseQuantityLabel.Text);
                var currQuantity = int.Parse(QuantityTextBox.Text);
                if (!(warehouseQuantity + (currQuantity - quantity) * sign > 0))
                {
                    MessageBox.Show("Количество товара не может быть меньше 0!");
                    return false;
                }
                itemRepository.AddProductQuantityById((int)ProductComboBox.SelectedValue,   (currQuantity - quantity) * sign);
                return true;
            }
            MessageBox.Show("Выберите строку для изменения количества товара.");
            return false;
        }

        public bool DeleteWarehouseQuantity(DataGridView itemsData, ItemRepository itemRepository, int sign)
        {
            if (ViewUtils.IsSingleRowWithIdSelected(itemsData))
            {
                var warehouseQuantity = Convert.ToInt32(WarehouseQuantityLabel.Text);
                var currQuantity = int.Parse(QuantityTextBox.Text);
                if (!(warehouseQuantity + currQuantity * sign > 0))
                {
                    MessageBox.Show("Количество товара не может быть меньше 0!");
                    return false;
                }
                itemRepository.AddProductQuantityById((int)ProductComboBox.SelectedValue,   currQuantity * sign);
                return true;
            }
            MessageBox.Show("Выберите строку для изменения количества товара.");
            return false;
        }
    }
}