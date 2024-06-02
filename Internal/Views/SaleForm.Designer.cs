using System.ComponentModel;
using System.Windows.Forms;
using BuildingMaterialsStoreManagement.Internal.FormModels;
using BuildingMaterialsStoreManagement.Internal.Utils;
using Microsoft.Office.Interop.Word;
using Point = System.Drawing.Point;

namespace BuildingMaterialsStoreManagement.Internal.Views
{
    partial class SaleForm
    {
        private IContainer components = null;
        private DataGridView saleData;
        private DataGridView itemsData;
        private SaleFields _saleFields = new SaleFields();
        private ItemFields _itemFields = new ItemFields();
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
            var formLabel = ViewUtils.CreateFormLabel("Форма Продаж");
            var backButton = ButtonUtils.CreateBackButton(_formNavigator, this);
            
            saleData = TableUtils.CreateSmallDataGridView(this.ClientSize, new Point(ClientSize.Width, 50), SelectSale);
            Controls.Add(saleData);
            var panel = TableUtils.CreateTablePanelSingleColumn(new Point(this.saleData.Location.X, this.saleData.Bottom + 5), _saleFields, null, _saleRepository);
            var buttons = new RightButtons(AddData, UpdateData, DeleteData,"Экспорт продаж в Word", ExportData);
            TableUtils.AddTitleLabel(saleData, "Продажи");
            
            itemsData = TableUtils.CreateSmallDataGridView(this.ClientSize,
                new Point(ClientSize.Width, panel.Bottom + 5), 
                (sender, e) => _itemFields.SelectionChanged(itemsData, _saleRepository));
            Controls.Add(itemsData);
            var panelItems = TableUtils.CreateTablePanelSingleColumn(new Point(this.itemsData.Location.X, this.itemsData.Bottom + 5), _itemFields, null, _saleRepository);
            var buttonsItems = new RightButtons(AddItem, UpdateItem, DeleteItem);
            TableUtils.AddTitleLabel(itemsData, "Товары");
            Controls.Add(formLabel);
            Controls.Add(backButton);
            Controls.Add(panel);
            Controls.Add(panelItems);
            buttons.AddPanelToForm(this, saleData);
            buttonsItems.AddPanelToForm(this, itemsData);
            this.Load += new System.EventHandler(this.LoadData);
            this.Load += new System.EventHandler(this.LoadItemsOnStart);
            this.components = new System.ComponentModel.Container();
        }
    }
}