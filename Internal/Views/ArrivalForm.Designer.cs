using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BuildingMaterialsStoreManagement.Internal.FormModels;
using BuildingMaterialsStoreManagement.Internal.Utils;

namespace BuildingMaterialsStoreManagement.Internal.Views
{
    partial class ArrivalForm
    {
        private IContainer components = null;
        private DataGridView arrivalData;
        private DataGridView itemsData;
        private ArrivalFields _arrivalFields = new ArrivalFields();
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
            var formLabel = ViewUtils.CreateFormLabel("Форма Поступлений");
            var backButton = ButtonUtils.CreateBackButton(_formNavigator, this);
            
            arrivalData = TableUtils.CreateSmallDataGridView(this.ClientSize, new Point(ClientSize.Width, 50), SelectArrival);
            Controls.Add(arrivalData);
            var panel = TableUtils.CreateTablePanelSingleColumn(new Point(this.arrivalData.Location.X, this.arrivalData.Bottom + 5), _arrivalFields, null, _arrivalRepository);
            var buttons = new RightButtons(AddData, UpdateData, DeleteData, "Экспорт поступления в Word", ExportData);
            TableUtils.AddTitleLabel(arrivalData, "Поступления");
            
            itemsData = TableUtils.CreateSmallDataGridView(this.ClientSize,
                new Point(ClientSize.Width, panel.Bottom + 5), 
                (sender, e) => _itemFields.SelectionChanged(itemsData, _arrivalRepository));
            Controls.Add(itemsData);
            var panelItems = TableUtils.CreateTablePanelSingleColumn(new Point(this.itemsData.Location.X, this.itemsData.Bottom + 5), _itemFields, null, _arrivalRepository);
            var buttonsItems = new RightButtons(AddItem, UpdateItem, DeleteItem);
            TableUtils.AddTitleLabel(itemsData, "Товары");
            
            Controls.Add(formLabel);
            Controls.Add(backButton);
            Controls.Add(panel);
            Controls.Add(panelItems);
            buttons.AddPanelToForm(this, arrivalData);
            buttonsItems.AddPanelToForm(this, itemsData);
            this.Load += new System.EventHandler(this.LoadData);
            this.Load += new System.EventHandler(this.LoadItemsOnStart);
            
            this.components = new System.ComponentModel.Container();
            this.Text = "Форма Поступлений";
        }
    }
}