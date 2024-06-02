using System;
using System.Windows.Forms;
using Npgsql;

namespace BuildingMaterialsStoreManagement.Internal.Views
{
    public partial class HomeForm : Form
    {
        private readonly NpgsqlConnection _connection;
        private readonly FormNavigator _formNavigator;

        public HomeForm(NpgsqlConnection connection)
        {
            _connection = connection;
            _formNavigator = new FormNavigator(connection);
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            ClientSize = new System.Drawing.Size(800, 400);
        }

        private void DepartmentFormButton_Click(object sender, EventArgs e)
        {
            var departmentForm = new DepartmentForm(_connection, _formNavigator);
            departmentForm.Show();
            this.Hide();
        }

        private void ProductFormButton_Click(object sender, EventArgs e)
        {
            var productForm = new ProductForm(_connection, _formNavigator);
            productForm.Show();
            this.Hide();
        }


        private void ArrivalFormButton_Click(object sender, EventArgs e)
        {
            var arrivalForm = new ArrivalForm(_connection, _formNavigator);
            arrivalForm.Show();
            this.Hide();
        }
        
        
        private void SaleFormButton_Click(object sender, EventArgs e)
        {
        var saleForm = new SaleForm(_connection, _formNavigator);
        saleForm.Show();
        this.Hide();
        }
        
        private void ReportFormButton_Click(object sender, EventArgs e)
        {
            var reportForm = new ReportForm(_connection, _formNavigator);
            reportForm.Show();
            this.Hide();
        }
    }
}