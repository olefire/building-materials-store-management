using System;
using System.Drawing;
using System.Windows.Forms;
using BuildingMaterialsStoreManagement.Internal.Utils;

namespace BuildingMaterialsStoreManagement.Internal.FormModels
{
    public class StandardButtons
    {
        private FlowLayoutPanel Panel { get; set; }
        
        public StandardButtons(EventHandler addClick, EventHandler editClick, EventHandler deleteClick, Size clientSize)
        {
            Panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Anchor = AnchorStyles.Bottom,
                Padding = new Padding(0, 10, 0, 10)
            };

            Panel.Controls.Add(ButtonUtils.CreateButton("Добавить", addClick));
            Panel.Controls.Add(ButtonUtils.CreateButton("Изменить", editClick));
            Panel.Controls.Add(ButtonUtils.CreateButton("Удалить", deleteClick));

            var paddingLeftRight = (clientSize.Width - Panel.PreferredSize.Width) / 2;
            Panel.Margin = new Padding(paddingLeftRight, 0, paddingLeftRight, 0);
        }
        
        public void AddPanelToForm(Form form)
        {
            Panel.Location = new Point((form.ClientSize.Width - Panel.Width) / 2, form.ClientSize.Height - Panel.Height - 10); 
            form.Controls.Add(Panel);
            form.Resize += (sender, e) => 
            {
                Panel.Location = new Point((form.ClientSize.Width - Panel.Width) / 2, form.ClientSize.Height - Panel.Height - 10);
            };
        }
    }
}