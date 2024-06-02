using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BuildingMaterialsStoreManagement.Internal.FormModels;
using BuildingMaterialsStoreManagement.Internal.Utils;

namespace BuildingMaterialsStoreManagement.Internal.Views
{
    partial class ProductForm
    {
        private IContainer components = null;
        private DataGridView productData;
        private ProductFields productFields = new ProductFields();
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }


        private void InitializeComponent()
        {
            var label = ViewUtils.CreateFormLabel("Продуктовая форма");
            var backButton = ButtonUtils.CreateBackButton(_formNavigator, this);
            
            productData = TableUtils.CreateDataGridView(this.ClientSize, (sender, e) => productFields.SelectionChanged(productData, _productRepository.PairRepository));
            var tablePanel = TableUtils.CreateTablePanel(new Point(this.productData.Location.X, this.productData.Bottom + 10), productFields, _productRepository.PairRepository);
            var buttonPanel = new StandardButtons(AddData, UpdateData, DeleteData, ClientSize);
            buttonPanel.AddPanelToForm(this);
            
            this.Controls.Add(productData);
            this.Controls.Add(tablePanel);
            this.Controls.Add(label);
            this.Controls.Add(backButton);
            
            this.Text = "ProductForm";
            this.components = new System.ComponentModel.Container();
            this.Load += new System.EventHandler(this.LoadData);
        }

    }
}