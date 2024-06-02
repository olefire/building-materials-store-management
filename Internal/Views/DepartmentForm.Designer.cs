using System;
using System.Drawing;
using System.Windows.Forms;
using BuildingMaterialsStoreManagement.Internal.FormModels;
using BuildingMaterialsStoreManagement.Internal.Utils;

namespace BuildingMaterialsStoreManagement.Internal.Views
{
    partial class DepartmentForm
    {
        private System.ComponentModel.IContainer components = null;
        
        private DataGridView departmentData;
        private DataGridView itemsData;
        private DepartmentFields _departmentFields = new DepartmentFields();
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
            this.Text = "Форма Отделов";
            
            var formLabel = ViewUtils.CreateFormLabel("Форма Отделов");
            var backButton = ButtonUtils.CreateBackButton(_formNavigator, this);

            departmentData = TableUtils.CreateSmallDataGridView(this.ClientSize, new Point(ClientSize.Width, 50), SelectDepartment);
            Controls.Add(departmentData);
            var panel = TableUtils.CreateTablePanelSingleColumn(new Point(this.departmentData.Location.X, this.departmentData.Bottom + 5), _departmentFields);
            var buttons = new RightButtons(AddData, UpdateData, DeleteData);
            TableUtils.AddTitleLabel(departmentData, "Отделы");
            
            itemsData = TableUtils.CreateSmallDataGridView(this.ClientSize,
                new Point(ClientSize.Width, panel.Bottom + 5), 
                (sender, e) => _itemFields.SelectionChanged(itemsData, _departmentRepository));
            Controls.Add(itemsData);
            var panelItems = TableUtils.CreateTablePanelSingleColumn(new Point(this.itemsData.Location.X, this.itemsData.Bottom + 5), _itemFields, null, _departmentRepository);
            var buttonsItems = new RightButtons(AddItem, UpdateItem, DeleteItem);
            TableUtils.AddTitleLabel(itemsData, "Склад");
            
            Controls.Add(formLabel);
            Controls.Add(backButton);
            Controls.Add(panel);
            Controls.Add(panelItems);
            buttons.AddPanelToForm(this, departmentData);
            buttonsItems.AddPanelToForm(this, itemsData);
            this.Load += new System.EventHandler(this.LoadData);
            this.Load += new System.EventHandler(this.LoadItemsOnStart);
            this.components = new System.ComponentModel.Container();
        }
    }
}