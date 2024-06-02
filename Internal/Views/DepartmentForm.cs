using System;
using System.Windows.Forms;
using BuildingMaterialsStoreManagement.Internal.Database;
using BuildingMaterialsStoreManagement.Internal.Database.Abstract;
using BuildingMaterialsStoreManagement.Internal.Models;
using BuildingMaterialsStoreManagement.Internal.Utils;

namespace BuildingMaterialsStoreManagement.Internal.Views
{
    public partial class DepartmentForm : Form
    {
        private readonly DepartmentRepository _departmentRepository;
        private readonly RepositoryWithItems<Department> _repository;
        private readonly FormNavigator _formNavigator;

        public DepartmentForm(Npgsql.NpgsqlConnection connection, FormNavigator formNavigator)
        {
            _departmentRepository = new DepartmentRepository(connection);
            _repository = new RepositoryWithItems<Department>(_departmentRepository);
            _formNavigator = formNavigator;
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void LoadData(object sender, EventArgs e)
        {
            departmentData.DataSource = _repository.LoadData();
        }

        private void LoadItems(int id)
        {
            itemsData.DataSource = _repository.LoadItems(id);
            _itemFields.LoadProductData(_departmentRepository, id);
        }

        private void SelectDepartment(object sender, EventArgs e)
        {
            _departmentFields.SelectionChanged(departmentData);
            var id = ViewUtils.GetSelectedRowId(departmentData);
            LoadItems(id);
        }

        private void AddData(object sender, EventArgs e)
        {
            if (ViewUtils.CheckSelectedRows(departmentData) == false) return;
            var department = _departmentFields.GetDepartment();
            if (department == null) return;
            _repository.ValidateAndAdd(department);
            LoadData(sender, e);
        }
        
        private void UpdateData(object sender, EventArgs e)
        {
            if (ViewUtils.CheckRecordNotSelected(departmentData) == false) return;
            var department = _departmentFields.GetDepartment();
            if (department == null) return;
            _repository.ValidateAndUpdate(department);
            LoadData(sender, e);
        }
        
        private void DeleteData(object sender, EventArgs e)
        {
            if (ViewUtils.CheckRecordNotSelected(departmentData) == false) return;
            var id = ViewUtils.GetSelectedRowId(departmentData);
            _repository.Delete(id);
            LoadData(sender, e);
        }
        
        private void LoadItemsOnStart(object sender, EventArgs e)
        {
            LoadItems(0);
        }
        
        private void AddItem(object sender, EventArgs e)
        {
            if (ViewUtils.CheckRecordNotSelected(departmentData) == false) return;
            if (ViewUtils.CheckSelectedRows(itemsData) == false) return;
            var id = ViewUtils.GetSelectedRowId(departmentData);
            var item = _itemFields.GetItem(id);
            if (item == null) return;
            _repository.ValidateAndAddItem(item);
            SelectDepartment(sender, e);
        }
        
        private void UpdateItem(object sender, EventArgs e)
        {
            if (ViewUtils.CheckRecordNotSelected(departmentData) == false) return;
            if (ViewUtils.CheckRecordNotSelected(itemsData) == false) return;
            var id = ViewUtils.GetSelectedRowId(departmentData);
            var item = _itemFields.GetItem(id);
            if (item == null) return;
            _repository.ValidateAndUpdateItem(item);
            SelectDepartment(sender, e);
        }
        
        private void DeleteItem(object sender, EventArgs e)
        {
            if (ViewUtils.CheckRecordNotSelected(departmentData) == false) return;
            if (ViewUtils.CheckRecordNotSelected(itemsData) == false) return;
            var id = ViewUtils.GetSelectedRowId(itemsData);
            _repository.DeleteItem(id);
            SelectDepartment(sender, e);
        }
        
    }
}