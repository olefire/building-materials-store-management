using System.Windows.Forms;
using BuildingMaterialsStoreManagement.Internal.Database;
using BuildingMaterialsStoreManagement.Internal.Database.Abstract;
using BuildingMaterialsStoreManagement.Internal.Exporters;
using BuildingMaterialsStoreManagement.Internal.Models;
using BuildingMaterialsStoreManagement.Internal.Utils;
using Npgsql;

namespace BuildingMaterialsStoreManagement.Internal.Views
{
    public partial class SaleForm : Form
    {
        private readonly FormNavigator _formNavigator;
        private readonly SaleRepository _saleRepository;
        private readonly RepositoryWithItems<Sale> _repository;
        private readonly WordExporter _wordExporter;
        public SaleForm(NpgsqlConnection connection, FormNavigator formNavigator)
        {
            _saleRepository = new SaleRepository(connection);
            _repository = new RepositoryWithItems<Sale>(_saleRepository);
            _formNavigator = formNavigator;
            var reportRepo = new SaleReportRepository(connection);
            _wordExporter = new WordExporter(reportRepo, "Sale");
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }
        
        private void LoadData(object sender, System.EventArgs e)
        {
            saleData.DataSource = _repository.LoadData();
        }
        
        private void LoadItems(int id)
        {
            itemsData.DataSource = _repository.LoadItems(id);
            if (saleData.SelectedRows.Count > 0)
            {
                var selectedRow = saleData.SelectedRows[0];
                var depName = selectedRow.Cells["Отдел"].Value.ToString();
                var depId = _saleRepository.GetDepartmentIdByName(depName);
                _itemFields.LoadProductData(_saleRepository, depId);
            }
        }
        
        private void SelectSale(object sender, System.EventArgs e)
        {
            _saleFields.SelectionChanged(saleData, _saleRepository);
            var id = ViewUtils.GetSelectedRowId(saleData);
            LoadItems(id);
        }
        
        private void AddData(object sender, System.EventArgs e)
        {
            if (ViewUtils.CheckSelectedRows(saleData) == false) return;
            var sale = _saleFields.GetSale();
            _repository.ValidateAndAdd(sale);
            LoadData(sender, e);
        }
        
        private void UpdateData(object sender, System.EventArgs e)
        {
            if (ViewUtils.CheckRecordNotSelected(saleData) == false) return;
            var sale = _saleFields.GetSale();
            _repository.ValidateAndUpdate(sale);
            LoadData(sender, e);
        }
        
        private void DeleteData(object sender, System.EventArgs e)
        {
            if (ViewUtils.CheckRecordNotSelected(saleData) == false) return;
            var id = ViewUtils.GetSelectedRowId(saleData);
            _repository.Delete(id);
            LoadData(sender, e);
        }
        
        
        private void LoadItemsOnStart(object sender, System.EventArgs e)
        {
            LoadItems(0);
        }
        
        private void AddItem(object sender, System.EventArgs e)
        {
            if (ViewUtils.CheckRecordNotSelected(saleData) == false) return;
            if (ViewUtils.CheckSelectedRows(itemsData) == false) return;
            var id = ViewUtils.GetSelectedRowId(saleData);
            var item = _itemFields.GetItem(id);
            if (item == null) return;
            var status = _saleRepository.GetSaleStatusById(id);
            if (status == "Чек сформирован")
            {
                MessageBox.Show("Чек уже сформирован");
                return;
            }
            if (_itemFields.SubQuantity(_saleRepository) == false) return;
            _repository.ValidateAndAddItem(item);
            LoadItems(id);
            ViewUtils.UpdateTotalSum(itemsData, saleData.SelectedRows[0]);
        }
        
        private void UpdateItem(object sender, System.EventArgs e)
        {
            if (ViewUtils.CheckRecordNotSelected(saleData) == false) return;
            if (ViewUtils.CheckRecordNotSelected(itemsData) == false) return;
            var id = ViewUtils.GetSelectedRowId(saleData);
            var item = _itemFields.GetItem(id);
            if (item == null) return;
            var status = _saleRepository.GetSaleStatusById(id);
            if (status == "Чек сформирован")
            {
                MessageBox.Show("Чек уже сформирован");
                return;
            }
            if (_itemFields.UpdateWarehouseQuantity(itemsData, _saleRepository, -1) == false) return;
            _repository.ValidateAndUpdateItem(item);
            LoadItems(id);
            ViewUtils.UpdateTotalSum(itemsData, saleData.SelectedRows[0]);
        }
        
        private void DeleteItem(object sender, System.EventArgs e)
        {
            if (ViewUtils.CheckRecordNotSelected(saleData) == false) return;
            if (ViewUtils.CheckRecordNotSelected(itemsData) == false) return;
            var id = ViewUtils.GetSelectedRowId(saleData);
            var itemId = ViewUtils.GetSelectedRowId(itemsData);
            var status = _saleRepository.GetSaleStatusById(id);
            if (status == "Чек сформирован")
            {
                MessageBox.Show("Чек уже сформирован");
                return;
            }
            if (_itemFields.DeleteWarehouseQuantity(itemsData, _saleRepository, 1) == false) return;
            _repository.DeleteItem(itemId);
            LoadItems(id);
            ViewUtils.UpdateTotalSum(itemsData, saleData.SelectedRows[0]);
        }
        
        private void ExportData(object sender, System.EventArgs e)
        {
            if (ViewUtils.CheckRecordNotSelected(saleData) == false) return;
            var id = ViewUtils.GetSelectedRowId(saleData);
            _wordExporter.ExportToWord(id);
            MessageBox.Show($"Экспорт чека {id} завершен");
        }
        
    }
}