using System;
using System.Drawing;
using System.Windows.Forms;
using BuildingMaterialsStoreManagement.Internal.Utils;

namespace BuildingMaterialsStoreManagement.Internal.FormModels
{
    public class RightButtons
    {
        private FlowLayoutPanel Panel { get; set; }

        public RightButtons(EventHandler addClick, EventHandler editClick, EventHandler deleteClick, string specialButtonName = null, EventHandler specialButtonClick = null)
        {
            Panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                Anchor = AnchorStyles.Right,
            };

            Panel.Controls.Add(ButtonUtils.CreateSmallButton("Добавить", addClick));
            Panel.Controls.Add(ButtonUtils.CreateSmallButton("Изменить", editClick));
            Panel.Controls.Add(ButtonUtils.CreateSmallButton("Удалить", deleteClick));

            if (specialButtonName != null)
            {
                Panel.Controls.Add(ButtonUtils.CreateSmallButton(specialButtonName, specialButtonClick));
            }
        }

        public void SetLocationRelativeTo(DataGridView dataGridView)
        {
            Panel.Location = new Point(dataGridView.Right + 10, dataGridView.Top);
        }

        public void AddPanelToForm(Form form, DataGridView dataGridView)
        {
            Panel.Location = new Point(dataGridView.Right + 10, dataGridView.Top);
            form.Controls.Add(Panel);
            form.Resize += (sender, e) => 
            {
                Panel.Location = new Point(dataGridView.Right + 10, dataGridView.Top);
            };
        }
    }
}