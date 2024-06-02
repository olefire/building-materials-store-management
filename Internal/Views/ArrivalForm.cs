using System.Windows.Forms;
using BuildingMaterialsStoreManagement.Internal.Database;
using BuildingMaterialsStoreManagement.Internal.Database.Abstract;
using BuildingMaterialsStoreManagement.Internal.Exporters;
using BuildingMaterialsStoreManagement.Internal.Models;
using BuildingMaterialsStoreManagement.Internal.Utils;

namespace BuildingMaterialsStoreManagement.Internal.Views
{
    public partial class ArrivalForm : Form
    {
        private readonly FormNavigator _formNavigator;
        private readonly ArrivalRepository _arrivalRepository;
        private readonly RepositoryWithItems<Arrival> _repository;
        private readonly WordExporter _wordExporter;
        public ArrivalForm(Npgsql.NpgsqlConnection connection, FormNavigator formNavigator)
        {
            _arrivalRepository = new ArrivalRepository(connection);
            _repository = new RepositoryWithItems<Arrival>(_arrivalRepository);
            _formNavigator = formNavigator;
            var reportRepo = new ArrivalReportRepository(connection);
            _wordExporter = new WordExporter(reportRepo, "Arrival");
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }
        
        private void LoadData(object sender, System.EventArgs e)
        {
            arrivalData.DataSource = _repository.LoadData();
        }
        
        private void LoadItems(int id)
        {
            itemsData.DataSource = _repository.LoadItems(id);

            if (arrivalData.SelectedRows.Count > 0)
            {
                var selectedRow = arrivalData.SelectedRows[0];
                var depName = selectedRow.Cells["Отдел"].Value.ToString();
                var depId = _arrivalRepository.GetDepartmentIdByName(depName);
                _itemFields.LoadProductData(_arrivalRepository, depId);
            }
        }
        
        private void SelectArrival(object sender, System.EventArgs e)
        {
            _arrivalFields.SelectionChanged(arrivalData);
            var id = ViewUtils.GetSelectedRowId(arrivalData);
            LoadItems(id);
        }
        
        private void AddData(object sender, System.EventArgs e)
        {
            if (ViewUtils.CheckSelectedRows(arrivalData) == false) return;
            var arrival = _arrivalFields.GetArrival();
            if (arrival == null) return;
            _repository.ValidateAndAdd(arrival);
            LoadData(sender, e);
        }
        
        private void UpdateData(object sender, System.EventArgs e)
        {
            if (ViewUtils.CheckRecordNotSelected(arrivalData) == false) return;
            var arrival = _arrivalFields.GetArrival();
            if (arrival == null) return;
            _repository.ValidateAndUpdate(arrival);
            LoadData(sender, e);
        }
        
        private void DeleteData(object sender, System.EventArgs e)
        {
            if (ViewUtils.CheckRecordNotSelected(arrivalData) == false) return;
            var id = ViewUtils.GetSelectedRowId(arrivalData);
            _repository.Delete(id);
            LoadData(sender, e);
        }
        
        private void AddItem(object sender, System.EventArgs e)
        {
            if (ViewUtils.CheckRecordNotSelected(arrivalData) == false) return;
            if (ViewUtils.CheckSelectedRows(itemsData) == false) return;
            var arrivalId = ViewUtils.GetSelectedRowId(arrivalData);
            var item = _itemFields.GetItem(arrivalId);
            if (item == null) return;
            var status = _arrivalRepository.GetArrivalStatusById(arrivalId);
            if (status == "Поступление принято")
            {
                MessageBox.Show("Поступление уже принято");
                return;
            }
            _itemFields.AddQuantity(_arrivalRepository);
            _repository.ValidateAndAddItem(item);
            LoadItems(arrivalId);
            ViewUtils.UpdateTotalSum(itemsData, arrivalData.SelectedRows[0]);
        }
        
        private void UpdateItem(object sender, System.EventArgs e)
        {
            if (ViewUtils.CheckRecordNotSelected(arrivalData) == false) return;
            if (ViewUtils.CheckRecordNotSelected(itemsData) == false) return;
            var arrivalId = ViewUtils.GetSelectedRowId(arrivalData);
            var item = _itemFields.GetItem(arrivalId);
            if (item == null) return;
            var status = _arrivalRepository.GetArrivalStatusById(arrivalId);
            if (status == "Поступление принято")
            {
                MessageBox.Show("Поступление уже принято");
                return;
            }
            if (_itemFields.UpdateWarehouseQuantity(itemsData, _arrivalRepository, 1) == false) return;
            _repository.ValidateAndUpdateItem(item);
            LoadItems(arrivalId);
            ViewUtils.UpdateTotalSum(itemsData, arrivalData.SelectedRows[0]);
        }
        
        private void DeleteItem(object sender, System.EventArgs e)
        {
            if (ViewUtils.CheckRecordNotSelected(arrivalData) == false) return;
            if (ViewUtils.CheckRecordNotSelected(itemsData) == false) return;
            var itemId = ViewUtils.GetSelectedRowId(itemsData);
            var arrivalId = ViewUtils.GetSelectedRowId(arrivalData);
            var status = _arrivalRepository.GetArrivalStatusById(arrivalId);
            if (status == "Поступление принято")
            {
                MessageBox.Show("Поступление уже принято");
                return;
            }
            if (_itemFields.DeleteWarehouseQuantity(itemsData, _arrivalRepository, -1) == false) return;
            _repository.DeleteItem(itemId);
            LoadItems(arrivalId);
            ViewUtils.UpdateTotalSum(itemsData, arrivalData.SelectedRows[0]);
        }
        
        private void LoadItemsOnStart(object sender, System.EventArgs e)
        {
            LoadItems(0);
        }
        
        private void ExportData(object sender, System.EventArgs e)
        {
            if (ViewUtils.CheckRecordNotSelected(arrivalData) == false) return;
            var id = ViewUtils.GetSelectedRowId(arrivalData);
            _wordExporter.ExportToWord(id);
            MessageBox.Show($"Экспорт поступления {id} завершен");
        }
    }
}