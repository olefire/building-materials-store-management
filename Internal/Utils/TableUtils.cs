using System;
using System.Drawing;
using System.Windows.Forms;
using BuildingMaterialsStoreManagement.Internal.Database.Abstract;
using BuildingMaterialsStoreManagement.Internal.FormModels;

namespace BuildingMaterialsStoreManagement.Internal.Utils
{
    public static class TableUtils
    {
        public class FormField
        {
            public string LabelText { get; set; }
            public Control Control { get; set; }
        }

        public static TableLayoutPanel CreateTablePanel(Point location, IFormModel fields, PairRepository pairRepository = null)
        {
            var tablePanel = new TableLayoutPanel
            {
                Dock = DockStyle.None,
                AutoSize = true,
                Anchor = AnchorStyles.None,
                Location = location
            };

            var row = 0;
            var formFields = fields.CreateFormFieldList(pairRepository);
            foreach (var formField in formFields)
            {
                tablePanel.Controls.Add(new Label { Text = formField.LabelText, Font = new Font("Arial", 12, FontStyle.Bold)}, 0, row);
                tablePanel.Controls.Add(formField.Control, 1, row);
                row++;
            }

            return tablePanel;
        }
        
        public static TableLayoutPanel CreateTablePanelSingleColumn(Point location, IFormModel fields, PairRepository pairRepository = null, ItemRepository itemRepository = null)
        {
            var tablePanel = new TableLayoutPanel
            {
                AutoSize = true,
                Location = location
            };

            var formFields = fields.CreateFormFieldList(pairRepository, itemRepository);
            foreach (var formField in formFields)
            {
                var flowPanel = new FlowLayoutPanel
                {
                    AutoSize = true,
                    FlowDirection = FlowDirection.TopDown
                };

                var label = new Label { Text = formField.LabelText, Font = new Font("Arial", 12, FontStyle.Bold), AutoSize = true};
                label.Size = new Size(125, 30);

                if (formField.LabelText == "Id")
                {
                    formField.Control.Size = new Size(30, 30); // Меньший размер для Id
                }
                else
                {
                    formField.Control.Size = new Size(120, 30); // Больший размер для остальных элементов
                }

                flowPanel.Controls.Add(label);
                flowPanel.Controls.Add(formField.Control);

                tablePanel.Controls.Add(flowPanel, tablePanel.ColumnCount, 0);
                tablePanel.ColumnCount++;
            }

            return tablePanel;
        }
        
        public static DataGridView CreateSmallDataGridView(Size clientSize, Point location, EventHandler selectionChangedHandler)
        {
            var dataGridView = new DataGridView();
            dataGridView.Width = clientSize.Width * 3; 
            dataGridView.Height = clientSize.Height; 
            dataGridView.Location = location;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.SelectionChanged += selectionChangedHandler;

            return dataGridView;
        }
        
        public static DataGridView CreateDataGridView(Size clientSize, EventHandler selectionChangedHandler)
        {
            var dataGridView = new DataGridView();
            dataGridView.Anchor = AnchorStyles.None;
            dataGridView.Width = clientSize.Width * 4;
            dataGridView.Height = clientSize.Height * 3 / 2;
            dataGridView.Dock = DockStyle.None;
            dataGridView.Location = new Point(
                clientSize.Width / 2 - dataGridView.Width / 2,
                clientSize.Height * 3 / 2 - dataGridView.Height * 3 / 2
            );
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.SelectionChanged += selectionChangedHandler;

            return dataGridView;
        }
        
        public static void AddTitleLabel(DataGridView dataGridView, string title)
        {
            var titleLabel = new Label
            {
                Text = title,
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(dataGridView.Location.X - 120, dataGridView.Location.Y)
            };

            dataGridView.Parent.Controls.Add(titleLabel);
        }
    }
}