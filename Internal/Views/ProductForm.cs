using System.Windows.Forms;
using BuildingMaterialsStoreManagement.Internal.Database;
using BuildingMaterialsStoreManagement.Internal.Database.Abstract;
using BuildingMaterialsStoreManagement.Internal.Models;
using BuildingMaterialsStoreManagement.Internal.Utils;

namespace BuildingMaterialsStoreManagement.Internal.Views
{
    public partial class ProductForm : Form
    {
        private readonly FormNavigator _formNavigator;
        private readonly ProductRepository _productRepository;
        private readonly Repository<Product> _repository;
        public ProductForm(Npgsql.NpgsqlConnection connection, FormNavigator formNavigator)
        {
            _formNavigator = formNavigator;
            _productRepository = new ProductRepository(connection);
            _repository = new Repository<Product>(_productRepository);
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }
        
        private void LoadData(object sender, System.EventArgs e)
        {
            var products = _repository.LoadData();
            productData.DataSource = products;
        }
        
        private void AddData(object sender, System.EventArgs e)
        {
            if (ViewUtils.CheckSelectedRows(productData) == false) return;
            var product = productFields.GetProduct();
            if (product == null) return;
            _repository.ValidateAndAdd(product);
            LoadData(sender, e);
        }
        
        private void UpdateData(object sender, System.EventArgs e)
        {
            if (ViewUtils.CheckRecordNotSelected(productData) == false) return;
            var product = productFields.GetProduct();
            if (product == null) return;
            _repository.ValidateAndUpdate(product);
            LoadData(sender, e);
        }
        
        private void DeleteData(object sender, System.EventArgs e)
        {
            if (ViewUtils.CheckRecordNotSelected(productData) == false) return;
            var id = ViewUtils.GetSelectedRowId(productData);
            _repository.Delete(id);
            LoadData(sender, e);
        }
    }
}