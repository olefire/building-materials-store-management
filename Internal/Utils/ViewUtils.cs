using System;
using System.Drawing;
using System.Windows.Forms;
using BuildingMaterialsStoreManagement.Internal.Database.Abstract;

namespace BuildingMaterialsStoreManagement.Internal.Utils
{
    public static class ViewUtils
    {
        public static void LoadPairsToComboBox(ComboBox comboBox, PairRepository pairRepository)
        {
            var pairs = pairRepository.GetIdAndNamePairs();
            comboBox.ValueMember = "Item1";
            comboBox.DisplayMember = "Item2";
            comboBox.DataSource = pairs;
        }

        public static void LoadProductIdsAndNamesToComboBox(ComboBox comboBox, ItemRepository itemRepository)
        {
            var productIdsAndNames = itemRepository.GetProductIdsAndNames();
            comboBox.ValueMember = "Item1";
            comboBox.DisplayMember = "Item2";
            comboBox.DataSource = productIdsAndNames;
        }
        
        public static void LoadDepartmentsToComboBox(ComboBox comboBox, ItemRepository itemRepository)
        {
            var departments = itemRepository.GetDepartmentIdsAndNames();
            comboBox.ValueMember = "Item1";
            comboBox.DisplayMember = "Item2";
            comboBox.DataSource = departments;
        }
        
        
        public static Label CreateFormLabel(string text)
        {
            return new Label
            {
                Anchor = AnchorStyles.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = text,
                Font = new Font("Arial", 18, FontStyle.Bold),
                Size = new Size(500, 50)
            };
        }

        public static bool IsSingleRowWithIdSelected(DataGridView dataGridView)
        {
            return dataGridView.SelectedRows.Count == 1
                   && dataGridView.SelectedRows[0].Cells[0] != null
                   && dataGridView.SelectedRows[0].Cells[0].Value != DBNull.Value;
        }

        public static int GetSelectedRowId(DataGridView dataGridView)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView.SelectedRows[0];
                var id = Convert.ToInt32(selectedRow.Cells[0].Value);
                return id;
            }

            return 0;
        }


        public static bool CheckSelectedRows(DataGridView dataGridView)
        {
            if (dataGridView.SelectedRows.Count != 0)
            {
                MessageBox.Show("Выбрана строка: невозможно добавить данные.");
                return false;
            }

            return true;
        }
        public static bool CheckRecordNotSelected(DataGridView dataGridView)
        {
            if (!IsSingleRowWithIdSelected(dataGridView))
            {
                MessageBox.Show("Выберите запись");
                return false;
            }

            return true;
        }
        
        public static void UpdateTotalSum(DataGridView itemsData, DataGridViewRow totalDataRow)
        {
            decimal totalSum = 0;

            foreach (DataGridViewRow row in itemsData.Rows)
            {
                if (row.Cells["Цена"].Value != null && row.Cells["Количество"].Value != null)
                {
                    decimal price;
                    int quantity;
                    if (decimal.TryParse(row.Cells["Цена"].Value.ToString(), out price) &&
                        int.TryParse(row.Cells["Количество"].Value.ToString(), out quantity))
                    {
                        totalSum += price * quantity;
                    }
                }
            }

            if (totalDataRow.Cells["Итоговая сумма"].Value != null)
            {
                totalDataRow.Cells["Итоговая сумма"].Value = totalSum;
            }
        }
    }
}